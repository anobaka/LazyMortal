using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bootstrap.Components.Orm.Infrastructures;
using Bootstrap.Models.ResponseModels;
using Microsoft.EntityFrameworkCore;

namespace Bootstrap.Components.Miscellaneous
{
    public static class MockUtils
    {
        public static async Task<T> GetOrCreate<TDbContext, T, TKey>(ResourceService<TDbContext, T, TKey> service,
            Expression<Func<T, bool>> first,
            object createModel) where T : class where TDbContext : DbContext
        {
            var instance = await service.GetFirst(first);
            if (instance == null)
            {
                var serviceType = service.GetType();
                var methods = serviceType.GetMethods();
                var createModelType = createModel.GetType();
                var candidateMethodKeys = new[] {"Add", "Create"};
                var method = methods.FirstOrDefault(t =>
                {
                    var parameterType = t.GetParameters().FirstOrDefault()?.ParameterType;
                    return candidateMethodKeys.Contains(t.Name) && createModelType == parameterType;
                });
                instance = (await (method.Invoke(service, new[] {createModel}) as Task<SingletonResponse<T>>)).Data;
            }

            return instance;
        }
    }
}