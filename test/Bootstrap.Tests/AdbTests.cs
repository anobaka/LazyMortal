// using System;
// using System.IO;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using Bootstrap.Components.Mobiles.Android;
// using Bootstrap.Components.Mobiles.Android.Infrastructures;
// using Bootstrap.Components.Mobiles.Android.Models.Constants;
// using Bootstrap.Components.Mobiles.Android.Models.Exceptions;
// using Bootstrap.Components.Mobiles.Android.Wrappers;
// using Bootstrap.Components.Storage;
// using CliWrap;
// using Humanizer;
// using JetBrains.Annotations;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Options;
// using Xunit;
// using Xunit.Abstractions;
//
// namespace Bootstrap.Tests
// {
//     public class AdbTests
//     {
//         private readonly IOptions<AdbOptions> _options;
//         private readonly ILogger<AdbTests> _logger;
//         private readonly Adb _adb;
//         private readonly ITestOutputHelper _testOutputHelper;
//
//         public AdbTests(IOptions<AdbOptions> options, ILogger<AdbTests> logger, Adb adb,
//             ITestOutputHelper testOutputHelper)
//         {
//             _options = options;
//             _logger = logger;
//             _adb = adb;
//             _testOutputHelper = testOutputHelper;
//         }
//
//         [Fact]
//         public async Task Devices()
//         {
//             var devices = await _adb.Devices();
//             Assert.NotNull(devices);
//             devices = await _adb.Devices();
//             Assert.NotNull(devices);
//             Assert.Empty(devices.Where(a => a.Description?.Any() != true));
//         }
//
//         [Fact]
//         public async Task Version()
//         {
//             var version = await _adb.Version();
//             _testOutputHelper.WriteLine(version);
//             Assert.NotNull(version);
//         }
//
//         [Fact]
//         public async Task Server()
//         {
//             await _adb.StartServer();
//             await _adb.KillServer();
//         }
//
//         [Theory]
//         [InlineData(@"./libs/b.a.apk", "b.a")]
//         public async Task Installation([NotNull] string apkPath, string package)
//         {
//             if (!Path.IsPathRooted(apkPath))
//             {
//                 apkPath = Path.Combine(Directory.GetCurrentDirectory(), apkPath);
//             }
//
//             await _adb.Install(apkPath, "-r");
//             var ei = await Assert.ThrowsAsync<AdbInternalException>(async () => await _adb.Install(apkPath));
//             Assert.Equal(AdbInternalError.INSTALL_FAILED_ALREADY_EXISTS, ei.AdbError);
//             await _adb.Uninstall(package);
//             var eu = await Assert.ThrowsAsync<AdbInternalException>(async () => await _adb.Uninstall(package));
//             Assert.Equal(AdbInternalError.DELETE_FAILED_INTERNAL_ERROR, eu.AdbError);
//         }
//
//         [Theory]
//         [InlineData(100, 100)]
//         [InlineData(200, 200)]
//         [InlineData(300, 300)]
//         public async Task Touch(int x, int y)
//         {
//             await _adb.Shell.Input.Tap(x, y);
//         }
//
//         [Theory]
//         [InlineData(200, 200, 400, 400)]
//         public async Task Swipe(int x1, int y1, int x2, int y2)
//         {
//             await _adb.Shell.Input.Swipe(x1, y1, x2, y2);
//         }
//
//         [Theory]
//         [InlineData("text")]
//         public async Task Text(string text)
//         {
//             await _adb.Shell.Input.Text(text);
//         }
//
//         [Theory]
//         [InlineData(4)]
//         public async Task KeyEvent(int eventId)
//         {
//             await _adb.Shell.Input.KeyEvent(eventId);
//         }
//     }
// }