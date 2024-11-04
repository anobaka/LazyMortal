using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bootstrap.Extensions;
using FluentAssertions;
using MailKit;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using NPOI.SS.Formula.Functions;
using FileSystem = Microsoft.VisualBasic.FileIO.FileSystem;
using SearchOption = System.IO.SearchOption;

namespace Bootstrap.Components.Storage
{
    public class DirectoryUtils
    {
        // public static Dictionary<string, int> CountExtensions(string directory, bool recursively)
        // {
        //     var dir = new DirectoryInfo(directory);
        //
        //     var files = dir.GetFileSystemInfos();
        // }

        public static void Merge(DirectoryInfo source, DirectoryInfo target, bool overwrite)
        {
            if (string.Equals(source.FullName, target.FullName, StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            var sourceIsInSubFileDestinations = false;
            // Copy each file into it's new directory.
            foreach (var fi in source.GetFiles())
            {
                var path = Path.Combine(target.FullName, fi.Name);
                if (path == source.FullName)
                {
                    sourceIsInSubFileDestinations = true;
                }
                else
                {
                    fi.MoveTo(path, overwrite);
                }
            }

            var topLevelDirectories = source.GetDirectories();
            var sourceIsInSubDirectoryDestinations = false;
            // Copy each subdirectory recursively.
            foreach (var diSourceSubDir in topLevelDirectories)
            {
                var nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                if (nextTargetSubDir.FullName == source.FullName)
                {
                    sourceIsInSubDirectoryDestinations = true;
                }

                Merge(diSourceSubDir, nextTargetSubDir, overwrite);
            }

            if (sourceIsInSubFileDestinations)
            {

            }

            var restEntries = source.GetFileSystemInfos();

            if (restEntries.Length == 0)
            {
                if (!sourceIsInSubDirectoryDestinations)
                {
                    source.Delete();
                }
            }
            else
            {
                if (restEntries.Length == 1 && sourceIsInSubFileDestinations)
                {
                    var tempSource = Path.Combine(source.Parent!.FullName,
                        $"{source.Name}_tmp_{DateTime.Now.ToMillisecondTimestamp()}_{Guid.NewGuid().ToString("N")[..6]}");
                    var sameNameFilePath = Path.Combine(tempSource, source.Name);
                    var sameNameFileTargetPath = Path.Combine(target.FullName, source.Name);
                    source.MoveTo(tempSource);
                    File.Move(sameNameFilePath, sameNameFileTargetPath);
                    source.Delete();
                }
            }
        }

        public static void Merge(string source, string target, bool overwrite)
        {
            Merge(new DirectoryInfo(source), new DirectoryInfo(target), overwrite);
        }

        public static void CopyFilesRecursively(string sourcePath, string targetPath, bool overwrite)
        {
            //Now Create all the directories
            foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (var oldPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                var newPath = oldPath.Replace(sourcePath, targetPath);
                if (overwrite || !File.Exists(newPath))
                {
                    File.Copy(oldPath, newPath, overwrite);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sampleDirectory"></param>
        /// <param name="startLayer">Starts from 0. (usually it's drive root)</param>
        /// <returns></returns>
        public static string[] GetSameLayerDirectories(string sampleDirectory, int startLayer = 0)
        {
            var segments = sampleDirectory
                .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Where(a => a.IsNotEmpty()).ToArray();

            var pathRoot = Path.GetPathRoot(sampleDirectory)!;

            var directories = new[]
            {
                startLayer == 0
                    ? pathRoot
                    : Path.Combine(pathRoot, Path.Combine(segments.Skip(1).Take(startLayer).ToArray()))
            };
            for (var i = startLayer + 1; i < segments.Length; i++)
            {
                var nextLevelDirectories = new ConcurrentBag<string>();
                Parallel.ForEach(directories, a =>
                {
                    try
                    {
                        nextLevelDirectories.AddRange(Directory.GetDirectories(a));
                    }
                    catch
                    {
                        // ignored
                    }
                });
                directories = nextLevelDirectories.ToArray();
            }

            return directories;
        }

        public static void Delete(string fullname, bool ignoreError, bool sendToRecycleBin)
        {
            try
            {
                if (sendToRecycleBin)
                {
                    FileSystem.DeleteDirectory(fullname, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }
                else
                {
                    Directory.Delete(fullname, true);
                }
            }
            catch (Exception)
            {
                if (ignoreError)
                {
                    return;
                }

                throw;
            }
        }

        public static bool IsDirectory(string path)
        {
            try
            {
                return File.GetAttributes(path).HasFlag(FileAttributes.Directory);
            }
            catch
            {
                return false;
            }
        }

        public static async Task MoveAsync(string sourcePath, string destinationPath, bool overwrite,
            Func<int, Task> onProgressChange, CancellationToken ct)
        {
            await CopyAsync(sourcePath, destinationPath, overwrite, onProgressChange, ct, true);
        }

        public static async Task CopyAsync(string sourcePath, string destinationPath, bool overwrite,
            Func<int, Task> onProgressChange, CancellationToken ct)
        {
            await CopyAsync(sourcePath, destinationPath, overwrite, onProgressChange, ct, false);
        }

        protected static async Task CopyAsync(string sourcePath, string destinationPath, bool overwrite,
            Func<int, Task> onProgressChange, CancellationToken ct, bool deleteAfter)
        {
            if (File.Exists(sourcePath) || File.Exists(destinationPath))
            {
                throw new Exception($"The {nameof(sourcePath)} or {nameof(destinationPath)} cannot be a file.");
            }

            var sourceDir = new DirectoryInfo(sourcePath!);
            if (!sourceDir.Exists)
            {
                throw new Exception($"{sourcePath} is not found");
            }

            var destinationDir = new DirectoryInfo(destinationPath!);
            if (sourceDir.FullName == destinationDir.FullName)
            {
                await onProgressChange(100);
                return;
            }

            if (destinationDir.FullName.StartsWith(sourceDir.FullName))
            {
                throw new Exception($"{nameof(destinationPath)} can not be a sub path of {nameof(sourcePath)}");
            }

            if (!destinationDir.Exists)
            {
                destinationDir.Create();
            }

            var totalLength = 0L;

            // For large amount of files, this way is more efficient than using Directory.GetFileSystemEntries then detect files or directories.
            var directories = Directory.GetDirectories(sourceDir.FullName, "*", SearchOption.AllDirectories).ToHashSet();
            var files = Directory.GetFiles(sourceDir.FullName, "*", SearchOption.AllDirectories).ToHashSet();
            var entries = files.Concat(directories).OrderBy(x => x, StringComparer.CurrentCultureIgnoreCase).ToList();

            var dirDependencyMap = entries.Select(e => (Path: e, Directory: Path.GetDirectoryName(e)))
                .GroupBy(d => d.Directory).ToDictionary(x => x.Key, x => x.Select(y => y.Path).ToHashSet());
            var reversedDirDependencyMap = dirDependencyMap.SelectMany(x => x.Value.Select(a => (Dir: x.Key, Dep: a)))
                .ToDictionary(d => d.Dep, d => d.Dir);

            var fileLengthMap = new Dictionary<string, long>();
            foreach (var f in files)
            {
                var fi = new FileInfo(f);
                totalLength += fi.Length;
                fileLengthMap[f] = fi.Length;
            }

            var doneLength = 0L;
            var percentage = 0;

            var conflictFiles = new List<string>();
            var missingFiles = new List<string>();

            Directory.CreateDirectory(destinationPath!);

            if (!entries.Any() && deleteAfter)
            {
                sourceDir.Delete();
            }

            foreach (var e in entries)
            {
                var targetPath = Path.Combine(destinationPath!, e.Replace(sourceDir.FullName, destinationDir.FullName));
                var isFileOrEmptyDirectory = !dirDependencyMap.TryGetValue(e, out var tmpDependencies) || !tmpDependencies.Any();
                var isDirectory = directories.Contains(e);
                if (isDirectory)
                {
                    Directory.CreateDirectory(targetPath);
                    if (isFileOrEmptyDirectory && deleteAfter)
                    {
                        Directory.Delete(e);
                    }
                }
                else
                {
                    var fileLength = fileLengthMap[e];
                    var length = doneLength;

                    if (!File.Exists(e))
                    {
                        missingFiles.Add(e);
                        continue;
                    }

                    if (!overwrite && File.Exists(targetPath))
                    {
                        conflictFiles.Add(targetPath);
                        continue;
                    }

                    async Task ProgressChange(int fileProgress)
                    {
                        var newDoneLength = fileLength / 100m * fileProgress;
                        var newPercentage = (int) ((newDoneLength + length) * 100 / totalLength);
                        if (newPercentage != percentage)
                        {
                            if (onProgressChange != null)
                            {
                                await onProgressChange(newPercentage);
                                percentage = newPercentage;
                            }
                        }
                    }

                    if (deleteAfter)
                    {
                        await FileUtils.MoveAsync(e, targetPath, overwrite, ProgressChange, ct);
                    }
                    else
                    {
                        await FileUtils.CopyAsync(e, targetPath, overwrite, ProgressChange, ct);
                    }

                    doneLength += fileLength;
                }

                if (deleteAfter && isFileOrEmptyDirectory)
                {
                    var current = e;
                    while (current != sourceDir.FullName)
                    {
                        var parent = reversedDirDependencyMap[current];
                        var dependencies = dirDependencyMap[parent];
                        dependencies.Remove(current);
                        if (!dependencies.Any())
                        {
                            Directory.Delete(parent);
                            dirDependencyMap.Remove(parent);
                            current = parent;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            if (missingFiles.Any() || conflictFiles.Any())
            {
                throw new DirectoryMovingException
                    {MissingFiles = missingFiles.ToArray(), ConflictFiles = conflictFiles.ToArray()};
            }

            if (deleteAfter)
            {
                dirDependencyMap.Should().BeNullOrEmpty();
            }
        }
    }
}