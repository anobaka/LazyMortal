using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Bootstrap.Components.Logging.LogService.Models.Entities;
using Bootstrap.Components.Miscellaneous.ResponseBuilders;
using Bootstrap.Components.Orm.Infrastructures;
using Bootstrap.Models.ResponseModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bootstrap.Components.Logging.LogService.Services
{
    public class LogService
    {
        private readonly IServiceProvider _rootServiceProvider;

        protected LogDbContext DbContext => ScopedServiceProvider.Value!.GetRequiredService<LogDbContext>();

        protected AsyncLocal<IServiceProvider> ScopedServiceProvider = new();

        public LogService(IServiceProvider rootServiceProvider)
        {
            _rootServiceProvider = rootServiceProvider;
        }

        protected IServiceScope CreateNewScope()
        {
            var scope = _rootServiceProvider.CreateScope();
            ScopedServiceProvider.Value = scope.ServiceProvider;
            return scope;
        }

        public async Task Log(string logger, LogLevel level, object @event, string message)
        {
            using var scope = CreateNewScope();
            DbContext.Add(new Log {Logger = logger, Level = level, Event = @event.ToString(), Message = message});
            await DbContext.SaveChangesAsync();
        }

        public async Task ReadAll()
        {
            using var scope = CreateNewScope();
            var logs = await DbContext.Logs.Where(t => !t.Read).ToListAsync();
            logs.ForEach(t => t.Read = true);
            await DbContext.SaveChangesAsync();
        }

        public async Task Read(int id)
        {
            using var scope = CreateNewScope();
            var log = await DbContext.Logs.FirstOrDefaultAsync(t => t.Id == id);
            log.Read = true;
            await DbContext.SaveChangesAsync();
        }

        public virtual async Task Truncate()
        {
            using var scope = CreateNewScope();
            var logs = await DbContext.Logs.ToListAsync();
            DbContext.Logs.RemoveRange(logs);
            await DbContext.SaveChangesAsync();
        }

        public virtual async Task<Log[]> GetAll()
        {
            using var scope = CreateNewScope();
            var logs = await DbContext.Logs.ToListAsync();
            return logs.ToArray();
        }

        public virtual async Task<int> Count(Expression<Func<Log, bool>> exp)
        {
            using var scope = CreateNewScope();
            var count = await DbContext.Logs.CountAsync(exp);
            return count;
        }
    }
}