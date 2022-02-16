using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bootstrap.Components.Tasks.Progressor.Abstractions;
using Bootstrap.Extensions;

namespace Bootstrap.Components.Tasks.Progressor.SignalR
{
    public interface ISignalRProgressor : IProgressor
    {
        Type JsonParamType { get; }
        Task<object> ConvertToStartModel(object typedJsonParam);
    }

    public interface ISignalRGenericProgressor<TJsonParam, TStartModel> : ISignalRProgressor where TJsonParam : class
    {
        Type ISignalRProgressor.JsonParamType => SpecificTypeUtils<TJsonParam>.Type;

        Task<TStartModel> ConvertToStartModel(TJsonParam typedJsonParam);

        async Task<object> ISignalRProgressor.ConvertToStartModel(object typedJsonParam)
        {
            return await ConvertToStartModel(typedJsonParam as TJsonParam);
        }
    }

    public interface ISignalRGenericProgressor<TStartModel> : ISignalRGenericProgressor<TStartModel, TStartModel>
        where TStartModel : class
    {
        Task<TStartModel> ISignalRGenericProgressor<TStartModel, TStartModel>.ConvertToStartModel(
            TStartModel typedJsonParam)
        {
            return Task.FromResult(typedJsonParam);
        }
    }
}