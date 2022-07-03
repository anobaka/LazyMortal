using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using FileSystem = Microsoft.VisualBasic.FileIO.FileSystem;
using SearchOption = System.IO.SearchOption;

namespace Bootstrap.Components.Storage
{
    public static class DirectoryUtils
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

            // Copy each file into it's new directory.
            foreach (var fi in source.GetFiles())
            {
                var path = Path.Combine(target.ToString(), fi.Name);
                fi.MoveTo(path, overwrite);
            }

            // Copy each sub directory using recursion.
            foreach (var diSourceSubDir in source.GetDirectories())
            {
                var nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                Merge(diSourceSubDir, nextTargetSubDir, overwrite);
            }

            source.Delete();
        }

        public static void Merge(string source, string target, bool overwrite)
        {
            Merge(new DirectoryInfo(source), new DirectoryInfo(target), overwrite);
        }

        public static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            //Now Create all of the directories
            foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (var newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
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

        public static async Task Move(Dictionary<string, string> sourcesAndDestinations, bool overwrite,
            Func<int, Task> onProgressChange, CancellationToken ct)
        {
            var unitPercentage = (decimal) 100 / sourcesAndDestinations.Count;
            var doneCount = 0;
            var percentage = 0;
            foreach (var (source, dest) in sourcesAndDestinations)
            {
                await Move(source, dest, overwrite, async fileProgress =>
                {
                    var newPercentage = (int) (unitPercentage * doneCount + unitPercentage * fileProgress / 100);
                    if (newPercentage != percentage)
                    {
                        await onProgressChange(newPercentage);
                        percentage = newPercentage;
                    }
                }, ct);
                doneCount++;
            }
        }

        public static async Task Move(string sourcePath, string destinationPath, bool overwrite,
            Func<int, Task> onProgressChange, CancellationToken ct)
        {
            string[] files;
            if (Directory.Exists(sourcePath))
            {
                //Now Create all of the directories
                foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
                }

                //Copy all the files & Replaces any files with the same name
                files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
            }
            else
            {
                if (File.Exists(sourcePath))
                {
                    files = new[] {sourcePath};
                }
                else
                {
                    throw new Exception($"{sourcePath} is not found");
                }
            }

            var percentage = 0;
            var singleFilePercentage = 100 / (decimal) files.Length;
            for (var i = 0; i < files.Length; i++)
            {
                var filePath = files[i];
                var dest = filePath.Replace(sourcePath, destinationPath);
                await FileUtils.MoveAsync(filePath, dest, overwrite, async fileProgress =>
                {
                    var newPercentage = (int) (singleFilePercentage * i + singleFilePercentage * fileProgress / 100);
                    if (newPercentage != percentage)
                    {
                        await onProgressChange(newPercentage);
                        percentage = newPercentage;
                    }
                }, ct);
            }
        }
    }
}