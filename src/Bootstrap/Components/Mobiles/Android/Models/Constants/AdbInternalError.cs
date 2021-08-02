using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bootstrap.Components.Mobiles.Android.Models.Constants
{
    public enum AdbInternalError
    {
        /// <summary>
        /// Uncategorized
        /// </summary>
        Error = 1,

        INSTALL_FAILED_ALREADY_EXISTS = 100,
        DELETE_FAILED_INTERNAL_ERROR = 101,

        FailedToConnectDevice = 200
    }
}