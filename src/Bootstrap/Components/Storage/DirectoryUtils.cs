﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bootstrap.Extensions;
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
            // Copy each sub directory using recursion.
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
            //Now Create all of the directories
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

        public static async Task MoveAsync(string sourcePath, string destinationDirectory, bool overwrite,
            Func<int, Task> onProgressChange, CancellationToken ct)
        {
            if (Directory.Exists(sourcePath))
            {
                var name = Path.GetFileName(sourcePath);
                destinationDirectory = Path.Combine(destinationDirectory, name);
            }

            await MergeAsync(sourcePath, destinationDirectory, overwrite, onProgressChange, ct);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationDirectory"></param>
        /// <param name="overwrite"></param>
        /// <param name="onProgressChange"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task MergeAsync(string sourcePath, string destinationDirectory, bool overwrite,
            Func<int, Task> onProgressChange, CancellationToken ct)
        {
            var sourceIsDirectory = Directory.Exists(sourcePath);
            if (!sourceIsDirectory && !File.Exists(sourcePath))
            {
                throw new Exception($"{sourcePath} is not found");
            }

            string[] files;
            // {key} is removable if {value} is empty.
            var fileEntriesDependencies = new Dictionary<string, HashSet<string>>();
            if (sourceIsDirectory)
            {
                // Now Create all of the directories
                foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationDirectory));
                    fileEntriesDependencies[dirPath] = Directory.GetFileSystemEntries(dirPath).ToHashSet();
                }

                //Copy all the files & Replaces any files with the same name
                files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);

                Directory.CreateDirectory(sourcePath.Replace(sourcePath, destinationDirectory));
                fileEntriesDependencies[sourcePath] = Directory.GetFileSystemEntries(sourcePath).ToHashSet();
            }
            else
            {
                files = new[] {sourcePath};
                fileEntriesDependencies[Path.GetDirectoryName(sourcePath)!] = new HashSet<string> {sourcePath};
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

                var dest = sourceIsDirectory
                    ? filePath.Replace(sourcePath, destinationDirectory)
                    : Path.Combine(destinationDirectory, Path.GetFileName(filePath));
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

            if (sourceIsDirectory)
            {
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