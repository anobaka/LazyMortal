using System;
using System.IO;
using System.Threading.Tasks;
using Bootstrap.Components.Mobiles.Android.Infrastructures;
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
            await Execute("pull");
        }

        public async Task Install(string apkPath)
        {
            await Execute($"install {apkPath}");
        }

        public async Task Uninstall(string package)
        {
            await Execute($"uninstall {package}");
        }
    }
}