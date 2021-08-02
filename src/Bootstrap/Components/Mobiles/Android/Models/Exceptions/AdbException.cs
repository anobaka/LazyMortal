using System;
using Bootstrap.Components.Mobiles.Android.Models.Constants;

namespace Bootstrap.Components.Mobiles.Android.Models.Exceptions
{
    public class AdbException : Exception
    {
        public AdbExceptionCode Code { get; init; }

        public AdbException(AdbExceptionCode code, string message = null) : base($"{code}, {message}")
        {
            Code = code;
        }
    }
}