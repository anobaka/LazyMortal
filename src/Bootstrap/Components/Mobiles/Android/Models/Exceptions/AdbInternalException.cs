using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bootstrap.Components.Mobiles.Android.Models.Constants;

namespace Bootstrap.Components.Mobiles.Android.Models.Exceptions
{
    public class AdbInternalException : AdbException
    {
        public AdbInternalError AdbError { get; init; }

        public AdbInternalException(AdbInternalError error, string message = null) : base(AdbExceptionCode.Error,
            $"adb error: {error}, {message}")
        {
            AdbError = error;
        }
    }
}