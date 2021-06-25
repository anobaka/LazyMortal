using Bootstrap.Extensions;
using Bootstrap.Models.Constants;
using Bootstrap.Models.ResponseModels;

namespace Bootstrap.Components.Miscellaneous.ResponseBuilders
{
    public static class SingletonResponseBuilder<T>
    {
        public static SingletonResponse<T> Ok = Build(ResponseCode.Success);
        public static SingletonResponse<T> BadRequest = Build(ResponseCode.InvalidPayloadOrOperation);
        public static SingletonResponse<T> SystemError = Build(ResponseCode.SystemError);
        public static SingletonResponse<T> Unauthorized = Build(ResponseCode.Unauthorized);
        public static SingletonResponse<T> NotFound = Build(ResponseCode.NotFound);
        public static SingletonResponse<T> Conflict = Build(ResponseCode.Conflict);
        public static SingletonResponse<T> Unauthenticated = Build(ResponseCode.Unauthenticated);
        public static SingletonResponse<T> Timeout = Build(ResponseCode.Timeout);

        public static SingletonResponse<T> Build(ResponseCode code, string message = null) =>
            new((int) code, message ?? code.GetDisplayName());

        public static SingletonResponse<T> BuildBadRequest(string message) =>
            Build(ResponseCode.InvalidPayloadOrOperation, message);
    }
}