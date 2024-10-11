using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Bootstrap.Components.Storage;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPOI.SS.Formula.Functions;

namespace Bootstrap.Tests;

[TestClass]

public class FileSystemTests
{
    #region File

    private static List<object[]> MoveOrCopyFileTestData()
    {
        var candidates = new List<List<object>>
        {
            new() {new[] {"t1"}},
            new() {new[] {"t1"}, new[] {"1", "t1"}, new[] {"t2"}, new[] {"2", "t2"}},
            new() {true, false},
            new() {true, false},
            new() {true, false},
            new() {true, false},
            new() {true, false},
        };

        var result = new List<object[]>();

        ExtractTestData([], 0);

        return result;

        void ExtractTestData(List<object> prefix, int idx)
        {
            if (idx < candidates.Count)
            {
                foreach (var cd in candidates[idx])
                {
                    if (idx == candidates.Count - 1)
                    {
                        result.Add(prefix.Concat(new[] {cd}).ToArray());
                    }
                    else
                    {
                        ExtractTestData(prefix.Concat(new[] {cd}).ToList(), idx + 1);
                    }
                }
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(MoveOrCopyFileTestData), DynamicDataSourceType.Method)]
    public async Task MoveOrCopyFile(string[] relativePathsOfSourceFile, string[] relativePathsOfTargetFile,
        bool createConflict, bool overwrite, bool targetFileIsInSourceDrive, bool isCopy, bool targetIsDirectory)
    {
        using var ctx = new FileSystemTestContext();
        var path = Path.Combine([ctx.SourceDir, ..relativePathsOfSourceFile]);
        var sBytes = await ctx.CreateFile(path);
        var toPath = Path.Combine([
            targetFileIsInSourceDrive ? ctx.SourceDir : ctx.AnotherDriveDir, .. relativePathsOfTargetFile
        ]);
        if (createConflict && toPath != path)
        {
            if (targetIsDirectory)
            {
                Directory.CreateDirectory(toPath);
            }
            else
            {
                await ctx.CreateFile(toPath);
            }
        }

        var percentages = new List<int>();

        Exception? e = null;
        try
        {
            if (isCopy)
            {
                await FileUtils.CopyAsync(path, toPath, overwrite, async p => { percentages.Add(p); },
                    new CancellationToken());
            }
            else
            {
                await FileUtils.MoveAsync(path, toPath, overwrite, async p => { percentages.Add(p); },
                    new CancellationToken());
            }
        }
        catch (Exception e1)
        {
            e = e1;
        }

        percentages.Should().OnlyHaveUniqueItems();

        if ((targetIsDirectory || !overwrite) && createConflict && toPath != path)
        {
            e.Should().NotBeNull();
        }
        else
        {
            percentages.Should().HaveCount(x => x > 0);
            var tBytes = await File.ReadAllBytesAsync(toPath);

            sBytes.SequenceEqual(tBytes).Should().BeTrue();
            File.Exists(path).Should().Be(isCopy || toPath == path);
        }
    }

    #endregion

    #region Directory

    [TestMethod]
    [DataRow(true, true)]
    [DataRow(false, true)]
    [DataRow(true, false)]
    public async Task TestMoveOrCopyDirectory_InvalidPath(bool sourceIsFile, bool targetIsFile)
    {
        using var ctx = new FileSystemTestContext();

        var sourcePath = Path.Combine(ctx.SourceDir, "123");
        if (sourceIsFile)
        {
            await ctx.CreateFile(sourcePath);
        }
        else
        {
            Directory.CreateDirectory(sourcePath);
        }

        var targetPath = Path.Combine(ctx.AnotherDriveDir, "123");
        if (targetIsFile)
        {
            await ctx.CreateFile(targetPath);
        }
        else
        {
            Directory.CreateDirectory(targetPath);
        }

        var func = async () =>
            await DirectoryUtils.MoveAsync1(sourcePath, targetPath, true, async p => { }, new CancellationToken());
        await func.Should().ThrowAsync<Exception>();
    }

    private static List<object[]> MoveOrCopyDirectoryTestData() =>
        MoveOrCopyFileTestData().Select(a => a.Take(6).ToArray()).ToList();

    [TestMethod]
    [DynamicData(nameof(MoveOrCopyDirectoryTestData), DynamicDataSourceType.Method)]
    public async Task MoveOrCopyDirectory(string[] relativePathsOfSource, string[] relativePathsOfTarget,
        bool createConflict,
        bool overwrite, bool targetFileIsInSourceDrive, bool isCopy)
    {
        using var ctx = new FileSystemTestContext();
        var sourceDir = Path.Combine([ctx.SourceDir, .. relativePathsOfSource]);
        var sourceFsMap = await ctx.PrepareFileSystemEntries(sourceDir);

        var targetDir = Path.Combine([
            targetFileIsInSourceDrive ? ctx.SourceDir : ctx.AnotherDriveDir, .. relativePathsOfTarget
        ]);

        var targetFsMap = new Dictionary<string, byte[]?>();
        if (createConflict && targetDir != sourceDir)
        {
            targetFsMap = await ctx.PreparePartialFileSystemEntries(targetDir);
        }

        var percentages = new List<int>();

        Exception? e = null;
        try
        {
            if (isCopy)
            {
                await DirectoryUtils.CopyAsync1(sourceDir, targetDir, overwrite, async p => { percentages.Add(p); },
                    new CancellationToken());
            }
            else
            {
                await DirectoryUtils.MoveAsync1(sourceDir, targetDir, overwrite, async p => { percentages.Add(p); },
                    new CancellationToken());
            }
        }
        catch (Exception e1)
        {
            e = e1;
        }

        percentages.Should().OnlyHaveUniqueItems();

        if (targetFsMap.Any(t => t.Value == null && sourceFsMap.ContainsKey(t.Key.Replace(targetDir, sourceDir))) &&
            !overwrite)
        {
            e.Should().NotBeNull();
        }
        else
        {
            e.Should().BeNull();
        }

        var expectedFsMap = sourceFsMap.ToDictionary(x => x.Key.Replace(sourceDir, targetDir), x => x.Value);
        if (targetFsMap.Any())
        {
            foreach (var (k, v) in targetFsMap)
            {
                // add existed files.
                if (!expectedFsMap.TryAdd(k, v))
                {
                    if (v != null)
                    {
                        if (!overwrite)
                        {
                            // use existed file data
                            expectedFsMap[k] = sourceFsMap[k.Replace(targetDir, sourceDir)];
                        }
                    }
                }
            }
        }

        var actualFiles = Directory.GetFiles(targetDir, "*", SearchOption.AllDirectories);
        var actualDirectories = Directory.GetDirectories(targetDir, "*", SearchOption.AllDirectories);

        var actualFsMap = actualDirectories.ToDictionary(d => d, d => (byte[]?) null);
        foreach (var af in actualFiles)
        {
            actualFsMap[af] = await File.ReadAllBytesAsync(af);
        }

        var unknownEntries = actualFsMap.Keys.Except(expectedFsMap.Keys).ToList();
        unknownEntries.Should().BeEmpty();

        var missingEntries = expectedFsMap.Keys.Except(actualFsMap.Keys).ToList();
        missingEntries.Should().BeEmpty();

        foreach (var (k, v) in expectedFsMap)
        {
            v.Should().Equal(actualFsMap[k]);
        }
    }

    #endregion
}