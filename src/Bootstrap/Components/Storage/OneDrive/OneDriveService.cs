using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Bootstrap.Components.Storage.OneDrive
{
    public class OneDriveService
    {
        public static string DefaultHomePath => RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? Environment.GetEnvironmentVariable("OneDriveConsumer")
            : Path.Combine(Environment.GetEnvironmentVariable("Home"), "OneDrive");
    }
}