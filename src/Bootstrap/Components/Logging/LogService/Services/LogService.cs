using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Bootstrap.Components.Logging.LogService.Models;
using Bootstrap.Components.Logging.LogService.Models.Entities;
using Bootstrap.Components.Miscellaneous.ResponseBuilders;
using Bootstrap.Components.Orm.Infrastructures;
using Bootstrap.Extensions;
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

        public async Task<SearchResponse<Log>> Search(LogSearchRequestModel model)
        {
            using var scope = CreateNewScope();
            Expression<Func<Log, bool>> exp =
                l => (!model.Level.HasValue || model.Level.Value == l.Level)
                     && (!model.StartDt.HasValue || model.StartDt <= l.DateTime)
                     && (!model.EndDt.HasValue || model.EndDt >= l.DateTime)
                     && (string.IsNullOrEmpty(model.Logger) || l.Logger != null && l.Logger.Contains(model.Logger))
                     && (string.IsNullOrEmpty(model.Event) || l.Event != null && l.Event.Contains(model.Event))
                     && (string.IsNullOrEmpty(model.Message) || l.Message != null && l.Message.Contains(model.Message));
            var data = await DbContext.Logs.OrderByDescending(a => a.DateTime).Where(exp).Skip(model.SkipCount)
                .Take(model.PageSize).ToListAsync();
            var count = await DbContext.Logs.CountAsync(exp);

            return model.BuildResponse(data, count);
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