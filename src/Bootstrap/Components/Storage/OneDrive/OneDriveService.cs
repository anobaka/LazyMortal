using System;
using System.IO;
using System.Runtime.InteropServices;
using Bootstrap.Extensions;

namespace Bootstrap.Components.Storage.OneDrive
{
    public class OneDriveService
    {
        public static string? DefaultHomePath
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return Environment.GetEnvironmentVariable("OneDriveConsumer");
                }

                var homePath = Environment.GetEnvironmentVariable("Home");
                return homePath.IsNotEmpty() ? Path.Combine(homePath, "OneDrive") : null;
            }
        }
    }
}