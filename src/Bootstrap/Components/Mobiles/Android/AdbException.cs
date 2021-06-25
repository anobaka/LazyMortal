using System;
using Bootstrap.Components.Mobiles.Android.Models.Constants;

namespace Bootstrap.Components.Mobiles.Android
{
    public class AdbException : Exception
    {
        public AdbExceptionCode Code { get; }
        public int AdbExitCode { get; }

        public AdbException(AdbExceptionCode code, string message = null) : base(message)
        {
            Code = code;
        }

        public AdbException(int adbExitCode, string message = null, Exception innerException = null) : base(message,
            innerException)
        {
            AdbExitCode = adbExitCode;
            Code = AdbExceptionCode.AdbError;
        }
    }
}