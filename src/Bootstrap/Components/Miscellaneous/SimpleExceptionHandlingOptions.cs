using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Bootstrap.Components.Miscellaneous
{
    public class SimpleExceptionHandlingOptions
    {
        /// <summary>
        /// Default value is true.
        /// </summary>
        public bool NoCache { get; set; } = true;
        public Func<HttpResponse, Exception, Task> ModifyResponse { get; set; }
    }
}
