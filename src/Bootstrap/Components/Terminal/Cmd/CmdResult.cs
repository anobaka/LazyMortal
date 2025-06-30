using JetBrains.Annotations;

namespace Bootstrap.Components.Terminal.Cmd
{
    public class CmdResult
    {
        public string Command { get; }

        public CmdResult(string command, int exitCode, string? output, string? error)
        {
            ExitCode = exitCode;
            Output = output;
            Error = error;
            Command = command;
        }

        public int ExitCode { get; }
        public string? Output { get; }
        public string? Error { get; set; }
    }
}