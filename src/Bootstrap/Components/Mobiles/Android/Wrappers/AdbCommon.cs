using System;
using System.IO;
using System.Threading.Tasks;
using Bootstrap.Components.Mobiles.Android.Infrastructures;
using Microsoft.Extensions.Hosting;

namespace Bootstrap.Components.Mobiles.Android.Wrappers
{
    public class AdbCommon : AdbWrapper
    {
        public AdbShell AdbShell { get; }
        public AdbExecOut AdbExecOut { get; }

        /// <summary>
        /// For extending.
        /// </summary>
        /// <param name="prev"></param>
        /// <param name="appendArguments"></param>
        public AdbCommon(AdbWrapper prev, params string[] appendArguments) : base(prev,
            appendArguments)
        {
            AdbShell = new AdbShell(this);
            AdbExecOut = new AdbExecOut(this);
        }

        public async Task TakeScreenshot(string destPath)
        {
            const string pathInDevice = "/sdcard/screen.png";
            await AdbShell.AdbScreenshot(pathInDevice);
            await AdbPull(pathInDevice, destPath);
        }

        public async Task AdbPull(string pathInDevice, string destPath)
        {
            await Execute("pull");
        }

        // public async Task<Stream> TakeScreenshot()
        // {
        //     var tmpPath = Path.Combine(_env.ContentRootPath, "adb");
        //     var tmpFilename = Guid.NewGuid().ToString();
        //     var tmpFullname = Path.Combine(tmpPath, tmpFilename);
        //     await TakeScreenshot(tmpFullname);
        //     var ms = new MemoryStream();
        //     using (var fs = System.IO.File.OpenRead(tmpFullname))
        //     {
        //         await fs.CopyToAsync(ms);
        //     }
        //
        //     System.IO.File.Delete(tmpFullname);
        //     ms.Seek(0, SeekOrigin.Begin);
        //     return ms;
        // }
    }
}