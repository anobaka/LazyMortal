﻿using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bootstrap.Components.Tasks;
using Bootstrap.Extensions;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using FileSystem = Microsoft.VisualBasic.FileIO.FileSystem;

namespace Bootstrap.Components.Storage
{
    public class FileUtils
    {
        public static async Task Save(string fullname, Stream stream, FileMode mode = FileMode.Create,
            Encoding? encoding = null)
        {

            if (string.IsNullOrEmpty(fullname))
            {
                throw new ArgumentNullException(nameof(fullname));
            }

            var dir = Path.GetDirectoryName(fullname);
            if (string.IsNullOrEmpty(dir))
            {
                throw new ArgumentException();
            }

            Directory.CreateDirectory(dir);
            await using var fs = new FileStream(fullname, mode, FileAccess.Write, FileShare.ReadWrite);
            await stream.CopyToAsync(fs);
        }

        public static async Task Save(string fullname, byte[] data, FileMode mode = FileMode.Create,
            Encoding? encoding = null)
        {
            if (string.IsNullOrEmpty(fullname))
            {
                throw new ArgumentNullException(nameof(fullname));
            }

            var dir = Path.GetDirectoryName(fullname);
            if (string.IsNullOrEmpty(dir))
            {
                throw new ArgumentException();
            }

            Directory.CreateDirectory(dir);
            await using var fs = new FileStream(fullname, mode, FileAccess.Write, FileShare.ReadWrite);
            await fs.WriteAsync(data);
        }

        public static async Task Save(string fullname, string content, FileMode mode = FileMode.Create,
            Encoding? encoding = null)
        {
            if (string.IsNullOrEmpty(fullname))
            {
                throw new ArgumentNullException(nameof(fullname));
            }

            var dir = Path.GetDirectoryName(fullname);
            if (string.IsNullOrEmpty(dir))
            {
                throw new ArgumentException();
            }

            Directory.CreateDirectory(dir);
            await using var fs = new FileStream(fullname, mode, FileAccess.Write, FileShare.ReadWrite);
            await using var sw = new StreamWriter(fs, encoding ?? Encoding.UTF8);

            await sw.WriteAsync(content);
        }

        public static async Task<string> ReadAsync(string fullname)
        {
            await using var fs = new FileStream(fullname, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs);
            return await sr.ReadToEndAsync();
        }

        public static string Read(string fullname)
        {
            using var fs = new FileStream(fullname, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs);
            return sr.ReadToEnd();
        }

        public static async Task CopyAsync(string sourcePath, string destinationPath)
        {
            await using Stream source = File.OpenRead(sourcePath);
            await using Stream destination = File.Create(destinationPath);
            await source.CopyToAsync(destination);
        }

        public static string? GetFullname(string filename)
        {
            if (File.Exists(filename))
                return Path.GetFullPath(filename);

            var values = Environment.GetEnvironmentVariable("PATH");
            return values?.Split(Path.PathSeparator).Select(path => Path.Combine(path, filename))
                .FirstOrDefault(File.Exists);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sampleFile"></param>
        /// <param name="startLayer">Starts from 0. (usually it's drive root)</param>
        /// <returns></returns>
        public static string[] GetSameLayerFiles(string sampleFile, int startLayer = 0)
        {
            var dir = new FileInfo(sampleFile).Directory!.FullName;
            var sameLayerDirectories = DirectoryUtils.GetSameLayerDirectories(dir, startLayer);
            var files = new ConcurrentBag<string>();
            Parallel.ForEach(sameLayerDirectories, d =>
            {
                try
                {
                    files.AddRange(Directory.GetFiles(d));
                }
                catch
                {
                    // ignored
                }
            });
            return files.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullname"></param>
        /// <param name="ignoreError"></param>
        /// <param name="sendToRecycleBin">
        /// True for using <see cref="Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(string, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin)"/> and false for <see cref="File.Delete"/>
        /// </param>
        public static void Delete(string fullname, bool ignoreError, bool sendToRecycleBin)
        {
            try
            {
                if (sendToRecycleBin)
                {
                    FileSystem.DeleteFile(fullname, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }
                else
                {
                    File.Delete(fullname);
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

        public static bool IsFile(string path)
        {
            try
            {
                return !File.GetAttributes(path).HasFlag(FileAttributes.Directory);
            }
            catch
            {
                return false;
            }
        }


        public static async Task MoveAsync(string sourcePath, string destinationPath, bool overwrite,
            Func<int, Task> onProgressChange, PauseToken pt, CancellationToken ct)
        {
            if (Directory.Exists(sourcePath) || Directory.Exists(destinationPath))
            {
                throw new Exception($"The {nameof(sourcePath)} or {nameof(destinationPath)} cannot be a directory.");
            }

            if (Path.GetPathRoot(sourcePath) == Path.GetPathRoot(destinationPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);
                File.Move(sourcePath, destinationPath, overwrite);
                await onProgressChange(100);
            }
            else
            {
                await CopyAsync(sourcePath, destinationPath, overwrite, onProgressChange, pt, ct);
                File.Delete(sourcePath);
            }
        }

        public static async Task CopyAsync(string sourcePath, string destinationPath, bool overwrite,
            Func<int, Task> onProgressChange, PauseToken pt, CancellationToken ct)
        {
            if (Directory.Exists(sourcePath) || Directory.Exists(destinationPath))
            {
                throw new Exception($"The {nameof(sourcePath)} or {nameof(destinationPath)} cannot be a directory.");
            }

            if (sourcePath == destinationPath)
            {
                await onProgressChange(100);
                return;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);
            await using Stream destination = File.OpenWrite(destinationPath);
            if (destination.Length > 0)
            {
                if (!overwrite)
                {
                    throw new Exception($"{destinationPath} exists and {nameof(overwrite)} is set to false.");
                }
            }

            await using Stream source = File.OpenRead(sourcePath);
            var buffer = new byte[1024 * 1024];
            var totalLength = source.Length;
            var copiedBytesLength = 0L;
            var percentage = 0;
            try
            {
                while (true)
                {
                    await pt.WaitWhilePausedAsync(ct);
                    var readBytesLength = await source.ReadAsync(buffer, 0, buffer.Length, ct);
                    if (readBytesLength == 0)
                    {
                        break;
                    }

                    await destination.WriteAsync(buffer, 0, readBytesLength, ct);
                    copiedBytesLength += readBytesLength;
                    var newPercentage = (int) ((decimal) copiedBytesLength / totalLength * 100);
                    if (newPercentage != percentage)
                    {
                        await onProgressChange(newPercentage);
                        percentage = newPercentage;
                    }
                }

                destination.SetLength(source.Length);
            }
            catch (Exception)
            {
                if (File.Exists(destinationPath))
                {
                    File.Delete(destinationPath);
                }

                throw;
            }
        }
    }
}