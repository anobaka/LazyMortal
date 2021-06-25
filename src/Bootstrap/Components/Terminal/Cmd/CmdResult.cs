namespace Bootstrap.Components.Terminal.Cmd
{
    public class CmdResult
    {
        public CmdResult(int exitCode, string output)
        {
            ExitCode = exitCode;
            Output = output;
        }

        public int ExitCode { get; }
        public string Output { get; }
    }
}
