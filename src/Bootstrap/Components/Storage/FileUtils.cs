﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using FileSystem = Microsoft.VisualBasic.FileIO.FileSystem;

namespace Bootstrap.Components.Storage
{
    public class FileUtils
    {
        public static async Task Save(string fullname, Stream stream, FileMode mode = FileMode.Create,
            Encoding encoding = null)
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
            Encoding encoding = null)
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
            Encoding encoding = null)
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

        public static async Task CopyAsync(string sourcePath, string destinationPath)
        {
            await using Stream source = File.OpenRead(sourcePath);
            await using Stream destination = File.Create(destinationPath);
            await source.CopyToAsync(destination);
        }

        public static string GetFullname(string filename)
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
    }
}