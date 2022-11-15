using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MailKit;
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

            if (source.GetFileSystemInfos().Length == 0)
            {
                source.Delete();
            }
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

        public static async Task MergeAsync(Dictionary<string, string> sourcesAndDestinations, bool overwrite,
            Func<int, Task> onProgressChange, CancellationToken ct)
        {
            var unitPercentage = (decimal) 100 / sourcesAndDestinations.Count;
            var doneCount = 0;
            var percentage = 0;
            foreach (var (source, dest) in sourcesAndDestinations)
            {
                await MergeAsync(source, dest, overwrite, async fileProgress =>
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

        public static async Task MoveAsync(string sourcePath, string destinationPath, bool overwrite,
            Func<int, Task> onProgressChange, CancellationToken ct)
        {
            if (Directory.Exists(sourcePath))
            {
                var name = Path.GetFileName(sourcePath);
                destinationPath = Path.Combine(destinationPath, name);
            }

            await MergeAsync(sourcePath, destinationPath, overwrite, onProgressChange, ct);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        /// <param name="overwrite"></param>
        /// <param name="onProgressChange"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task MergeAsync(string sourcePath, string destinationPath, bool overwrite,
            Func<int, Task> onProgressChange, CancellationToken ct)
        {
            if (!Directory.Exists(sourcePath) && !File.Exists(sourcePath))
            {
                throw new Exception($"{sourcePath} is not found");
            }

            string[] files;
            // {key} is removable if {value} is empty.
            var fileEntriesDependencies = new Dictionary<string, HashSet<string>>();
            if (Directory.Exists(sourcePath))
            {
                // Now Create all of the directories
                foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
                    fileEntriesDependencies[dirPath] = Directory.GetFileSystemEntries(dirPath).ToHashSet();
                }

                //Copy all the files & Replaces any files with the same name
                files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);

                Directory.CreateDirectory(sourcePath.Replace(sourcePath, destinationPath));
                fileEntriesDependencies[sourcePath] = Directory.GetFileSystemEntries(sourcePath).ToHashSet();
            }
            else
            {
                files = new[] {sourcePath};
                fileEntriesDependencies[sourcePath] = new HashSet<string> {sourcePath};
            }

            // {key} belongs to {value}
            var fileEntriesReversedDependencies = new Dictionary<string, string>();
            foreach (var (key, value) in fileEntriesDependencies)
            {
                foreach (var v in value)
                {
                    fileEntriesReversedDependencies[v] = key;
                }
            }

            var percentage = 0;
            var singleFilePercentage = files.Length == 0 ? 0 : 100 / (decimal) files.Length;
            var existedFiles = new List<string>();
            var missingFiles = new List<string>();
            for (var i = 0; i < files.Length; i++)
            {
                var filePath = files[i];
                var parent = fileEntriesReversedDependencies[filePath];
                var neighbors = fileEntriesDependencies[parent];
                if (!File.Exists(filePath))
                {
                    missingFiles.Add(filePath);
                    neighbors.Remove(filePath);
                }

                var dest = filePath.Replace(sourcePath, destinationPath);
                if (File.Exists(dest) && !overwrite)
                {
                    existedFiles.Add(filePath);
                }
                else
                {
                    await FileUtils.MoveAsync(filePath, dest, overwrite, async fileProgress =>
                    {
                        var newPercentage =
                            (int) (singleFilePercentage * i + singleFilePercentage * fileProgress / 100);
                        if (newPercentage != percentage)
                        {
                            if (onProgressChange != null)
                            {
                                await onProgressChange(newPercentage);
                                percentage = newPercentage;
                            }
                        }
                    }, ct);
                    neighbors.Remove(filePath);
                }
            }

            foreach (var directory in fileEntriesDependencies.Select(t => t.Key).OrderByDescending(t => t.Length))
            {
                // We've got all sub directories from Directory.GetDirectories,
                // so there is not need to worry about scenarios like sourcePath is /a, and the only sub-directory is /a/b/c/d/e
                if (Directory.Exists(directory) && Directory.GetFileSystemEntries(directory).Length == 0)
                {
                    Directory.Delete(directory);
                    if (directory != sourcePath)
                    {
                        fileEntriesDependencies[fileEntriesReversedDependencies[directory]].Remove(directory);
                    }
                }
            }

            if (existedFiles.Any() || missingFiles.Any())
            {
                const int maxShownFilesCount = 10;
                var sb = new StringBuilder();
                if (existedFiles.Any())
                {
                    sb.Append(@$"Failed to move {existedFiles.Count} files due to files exist.");
                    foreach (var f in existedFiles.Take(maxShownFilesCount))
                    {
                        sb.Append(Environment.NewLine).Append(f);
                    }

                    if (existedFiles.Count > maxShownFilesCount)
                    {
                        sb.Append(Environment.NewLine).Append("...");
                    }

                    sb.Append(Environment.NewLine);
                }

                if (missingFiles.Any())
                {
                    sb.Append(@$"Failed to move {missingFiles.Count} files due to files are not found.");
                    foreach (var f in missingFiles.Take(maxShownFilesCount))
                    {
                        sb.Append(Environment.NewLine).Append(f);
                    }

                    if (missingFiles.Count > maxShownFilesCount)
                    {
                        sb.Append(Environment.NewLine).Append("...");
                    }

                    sb.Append(Environment.NewLine);
                }

                throw new Exception(sb.ToString());
            }
        }
    }
}