using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System;

namespace Bootstrap.Tests;

internal record FileSystemTestContext : IDisposable
{
    public FileSystemTestContext()
    {
        // todo: dynamic directories
        var suffix = $"{DateTime.Now:yyyyMMddHHmmssfff}";
        AnotherDriveDir = $@"E:\Bootstrap.Tests.FileSystem.Target.{suffix}";
        Directory.CreateDirectory(AnotherDriveDir);
        SourceDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            $"FileSystem.Source.{suffix}");
        Directory.CreateDirectory(SourceDir);
    }

    public string AnotherDriveDir { get; }
    public string SourceDir { get; }

    public void Dispose()
    {
        Directory.Delete(SourceDir, true);
        Directory.Delete(AnotherDriveDir, true);
    }

    public async Task<byte[]> CreateFile(string path, long byteCount = 5120)
    {
        var dir = Path.GetDirectoryName(path)!;
        Directory.CreateDirectory(dir);
        var rand = new Random();
        var bytes = new byte[byteCount];
        for (var i = 0; i < byteCount; i++)
        {
            bytes[i] = (byte) rand.Next(256);
        }

        await File.WriteAllBytesAsync(path, bytes);
        return bytes;
    }

    public async Task<Dictionary<string, byte[]?>> PrepareFileSystemEntries(string dir)
    {
        var paths = new string[] {"f1", "d1\\f1", "d1\\d2\\f1", "d3\\d4\\d5\\f5", "d6\\d7"};
        return await PrepareFileSystemEntries(dir, paths);
    }

    public async Task<Dictionary<string, byte[]?>> PreparePartialFileSystemEntries(string dir)
    {
        var paths = new string[] {"f2", "d1\\f1", "d1\\d2\\f3"};
        return await PrepareFileSystemEntries(dir, paths);
    }

    protected async Task<Dictionary<string, byte[]?>> PrepareFileSystemEntries(string dir, string[] paths)
    {
        var pathAndSizeMap = new Dictionary<string, byte[]?>();
        foreach (var p in paths)
        {
            var path = Path.Combine(dir, p);
            if (Path.GetFileName(path).StartsWith("d"))
            {
                Directory.CreateDirectory(path);
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                pathAndSizeMap.Add(path, await CreateFile(path));
            }
        }

        foreach (var d in Directory.GetDirectories(dir, "*", SearchOption.AllDirectories))
        {
            pathAndSizeMap[d] = null;
        }

        return pathAndSizeMap;
    }
}