using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bootstrap.Models
{
    public class CommandResult
    {
        /// <summary>
        /// Exit code set by the underlying process.
        /// </summary>
        public int ExitCode { get; init; }

        /// <summary>
        /// When the command was started.
        /// </summary>
        public DateTimeOffset StartTime { get; init; }

        /// <summary>
        /// When the command finished executing.
        /// </summary>
        public DateTimeOffset ExitTime { get; init; }

        /// <summary>
        /// Total duration of the command execution.
        /// </summary>
        public TimeSpan RunTime => ExitTime - StartTime;

        public string Error { get; init; }
    }

    public class CommandResult<T> : CommandResult
    {
        public T Output { get; set; }
    }
}