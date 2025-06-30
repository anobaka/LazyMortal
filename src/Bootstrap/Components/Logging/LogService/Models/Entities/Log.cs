using System;
using Microsoft.Extensions.Logging;

namespace Bootstrap.Components.Logging.LogService.Models.Entities
{
    public class Log
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
        public LogLevel Level { get; set; }
        public string? Logger { get; set; }
        public string? Event { get; set; }
        public string? Message { get; set; }
        public bool Read { get; set; }
    }
}