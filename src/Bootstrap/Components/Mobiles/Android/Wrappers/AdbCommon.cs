using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bootstrap.Components.Mobiles.Android.Infrastructures;
using Bootstrap.Components.Mobiles.Android.Models.Constants;
using Microsoft.Extensions.Hosting;

namespace Bootstrap.Components.Mobiles.Android.Wrappers
{
    public class AdbCommon : AdbWrapper
    {
        public AdbShell Shell { get; }
        public AdbExecOut ExecOut { get; }

        /// <summary>
        /// For extending.
        /// </summary>
        /// <param name="prev"></param>
        /// <param name="appendArguments"></param>
        public AdbCommon(AdbWrapper prev, params string[] appendArguments) : base(prev,
            appendArguments)
        {
            Shell = new AdbShell(this);
            ExecOut = new AdbExecOut(this);
        }

        public async Task Pull(string pathInDevice, string destPath)
        {
            throw new NotImplementedException();
            await Execute("pull");
        }

        public async Task Push(string localFilename, string devicePath)
        {
            await Execute($"push \"{localFilename}\" \"{devicePath}\"");
        }

        public async Task Install(string apkPath, string arguments = null)
        {
            var output = await Execute($"install {arguments} {apkPath}");
            AdbExtensions.ThrowIfError(output);
        }

        public async Task InstallMultiple(string[] apkPaths, string arguments = null)
        {
            throw new NotImplementedException();
        }

        public async Task InstallMultiPackage(string[] apkPaths, string arguments = null)
        {
            throw new NotImplementedException();
        }

        public async Task Uninstall(string package, string arguments = null)
        {
            var output = await Execute($"uninstall {arguments} {package}");
            AdbExtensions.ThrowIfError(output);
        }
    }
}