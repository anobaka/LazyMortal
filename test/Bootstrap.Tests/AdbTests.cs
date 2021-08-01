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
            var devices = await _adb.GetDevices();
            var device = devices[0];
            var r = await device.ExecOut.ScreenCap();
        }
    }
}