using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bootstrap.Components.Mobiles.Android.Models.Constants;

namespace Bootstrap.Components.Mobiles.Android.Models.Exceptions
{
    public class AdbInvalidExitCodeException : AdbException
    {
        public int ExitCode { get; init; }

        public AdbInvalidExitCodeException(int exitCode, string message = null) : base(AdbExceptionCode.InvalidExitCode,
            $"exit code: {exitCode}, {message}")
        {
            ExitCode = exitCode;
        }
    }
}