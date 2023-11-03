using JetBrains.Annotations;

namespace Bootstrap.Components.Terminal.Cmd
{
    public class CmdResult
    {
        public CmdResult(int exitCode, [CanBeNull] string output, [CanBeNull] string error)
        {
            ExitCode = exitCode;
            Output = output;
            Error = error;
        }

        public int ExitCode { get; }
        [CanBeNull] public string Output { get; }
        [CanBeNull] public string Error { get; set; }
    }
}