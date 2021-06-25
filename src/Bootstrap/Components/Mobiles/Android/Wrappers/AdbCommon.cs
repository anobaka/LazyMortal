using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Bootstrap.Components.Mobiles.Android.Wrappers
{
    public abstract class AdbCommon : AdbWrapper
    {
        private readonly IHostEnvironment _env;
        public AdbShell AdbShell { get; }

        /// <summary>
        /// For initializing.
        /// </summary>
        /// <param name="adb"></param>
        /// <param name="env"></param>
        public AdbCommon(AdbInvoker adb, IHostEnvironment env) : base(adb)
        {
            _env = env;
            AdbShell = new AdbShell(this, "shell");
        }

        /// <summary>
        /// For extending.
        /// </summary>
        /// <param name="prev"></param>
        /// <param name="env"></param>
        /// <param name="appendArguments"></param>
        public AdbCommon(AdbWrapper prev, IHostEnvironment env, params string[] appendArguments) : base(prev,
            appendArguments)
        {
            _env = env;
            AdbShell = new AdbShell(this, "shell");
        }

        public async Task TakeScreenshot(string destPath)
        {
            const string pathInDevice = "/sdcard/screen.png";
            await AdbShell.AdbScreenshot(pathInDevice);
            await AdbPull(pathInDevice, destPath);
        }

        public async Task AdbPull(string pathInDevice, string destPath)
        {
            await Run("pull");
        }

        public async Task<Stream> TakeScreenshot()
        {
            var tmpPath = Path.Combine(_env.ContentRootPath, "adb");
            var tmpFilename = Guid.NewGuid().ToString();
            var tmpFullname = Path.Combine(tmpPath, tmpFilename);
            await TakeScreenshot(tmpFullname);
            var ms = new MemoryStream();
            using (var fs = System.IO.File.OpenRead(tmpFullname))
            {
                await fs.CopyToAsync(ms);
            }

            System.IO.File.Delete(tmpFullname);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
    }
}