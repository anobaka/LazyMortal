using System;

namespace Bootstrap.Components.Storage
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DirectoryAttribute : Attribute
    {
        public string Directory { get; set; }

        public DirectoryAttribute(string directory)
        {
            Directory = directory;
        }
    }
}