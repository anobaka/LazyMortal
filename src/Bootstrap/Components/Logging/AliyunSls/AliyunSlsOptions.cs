namespace Bootstrap.Components.Logging.AliyunSls
{
    public class AliyunSlsOptions
    {
        public string Project { set; get; }
        public string LogStore { set; get; }
        public string EndPoint { set; get; }
        public string AccessId { set; get; }
        public string AccessKey { set; get; }
        public LoggerOptions[] Loggers { set; get; } = {new LoggerOptions()};
    }
}