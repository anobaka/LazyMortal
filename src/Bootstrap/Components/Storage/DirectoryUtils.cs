using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static void Merge(DirectoryInfo source, DirectoryInfo target)
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
                if (File.Exists(path))
                {
                    fi.Delete();
                }
                else
                {
                    fi.MoveTo(path);
                }
            }

            // Copy each sub directory using recursion.
            foreach (var diSourceSubDir in source.GetDirectories())
            {
                var nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                Merge(diSourceSubDir, nextTargetSubDir);
            }

            source.Delete();
        }

        public static void Merge(string source, string target)
        {
            Merge(new DirectoryInfo(source), new DirectoryInfo(target));
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
    }
}