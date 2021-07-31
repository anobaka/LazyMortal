using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Bootstrap.Components.Mobiles.Android;
using Bootstrap.Components.Mobiles.Android.Infrastructures;
using Bootstrap.Components.Mobiles.Android.Wrappers;
using Bootstrap.Components.Storage;
using CliWrap;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace Bootstrap.Tests
{
    public class AdbTests
    {
        private readonly IOptions<AdbOptions> _options;
        private readonly ILogger<AdbTests> _logger;
        private readonly AdbInvoker _adbInvoker;
        private readonly Adb _adb;

        public AdbTests(IOptions<AdbOptions> options, ILogger<AdbTests> logger, AdbInvoker adbInvoker, Adb adb)
        {
            _options = options;
            _logger = logger;
            _adbInvoker = adbInvoker;
            _adb = adb;
        }

        [Fact]
        public async Task Test1()
        {
            // var fs = File.Create("./1.png");
            // var sb = new StringBuilder();
            // var result = await Cli.Wrap(_options.Value.ExecutablePath).WithArguments("exec-out screencap -p")
            //     .WithStandardOutputPipe(PipeTarget.ToStream(fs))
            //     // .WithWorkingDirectory(Directory.GetCurrentDirectory())
            //     .ExecuteAsync();
            var devices = await _adb.GetDevices();
            var device = devices[0];
            var r = await device.AdbExecOut.ExecuteAndSaveToFile("screencap -p");
            // var bytes = Encoding.Default.GetBytes(r);
            // await FileUtils.Save("1.png", bytes);
            await fs.DisposeAsync();
        }
    }
}