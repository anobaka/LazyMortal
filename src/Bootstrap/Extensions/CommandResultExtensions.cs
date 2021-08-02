using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bootstrap.Models;

namespace Bootstrap.Extensions
{
    public static class CommandResultExtensions
    {
        public static CommandResult ToCommandResult(this CliWrap.CommandResult cr, string error = null)
        {
            return new CommandResult
            {
                ExitCode = cr.ExitCode,
                StartTime = cr.StartTime,
                ExitTime = cr.ExitTime,
                Error = error
            };
        }
    }
}
