using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bootstrap.Models.RequestModels;
using Microsoft.Extensions.Logging;

namespace Bootstrap.Components.Logging.LogService.Models
{
    public record LogSearchRequestModel : SearchRequestModel
    {
        public LogLevel? Level { get; set; }
        public DateTime? StartDt { get; set; }
        public DateTime? EndDt { get; set; }
        public string Logger { get; set; }
        public string Event { get; set; }
        public string Message { get; set; }
    }
}
