using System;
using System.Threading.Tasks;
using Bootstrap.Components.Logging.LogService.Models.Entities;
using Bootstrap.Components.Orm.Infrastructures;
using Bootstrap.Models.ResponseModels;
using Microsoft.Extensions.Logging;

namespace Bootstrap.Components.Logging.LogService.Services
{
    public class LogService : ResourceService<LogDbContext, Log, int>
    {
        public LogService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task Log(string logger, LogLevel level, object @event, string message)
        {
            await Add(new Log {Logger = logger, Level = level, Event = @event.ToString(), Message = message}, true);
        }

        public async Task<BaseResponse> ReadAll()
        {
            return await UpdateAll(a => !a.Read, a => a.Read = true);
        }

        public async Task<BaseResponse> Read(int id)
        {
            return await UpdateByKey(id, a => a.Read = true);
        }

        public virtual async Task Truncate()
        {
            await RemoveAll(a => true);
        }
    }
}