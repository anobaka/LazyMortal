using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bootstrap.Models.Constants
{
    /// <summary>
    /// https://learn.microsoft.com/en-us/dotnet/core/rid-catalog
    /// </summary>
    public class DotNetRids
    {
        public const string Win64 = "win-x64";
        public const string Win32 = "win-x86";
        public const string WinArm64 = "win-arm86";
        public const string Linux64 = "linux-x64";
        public const string LinuxMusl64 = "linux-musl-x64";
        public const string LinuxArm = "linux-arm";
        public const string LinuxArm64 = "linux-arm64";
        public const string LinuxBionicArm64 = "linux-bionic-arm64";
        public const string Osx64 = "osx-x64";
        public const string OsxArm64 = "osx-arm64";
        public const string IOsArm64 = "ios-arm64";
        public const string AndroidArm64 = "android-arm64";
    }
}