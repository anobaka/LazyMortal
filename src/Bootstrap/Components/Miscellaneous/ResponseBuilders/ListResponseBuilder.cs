using Bootstrap.Extensions;
using Bootstrap.Models.Constants;
using Bootstrap.Models.ResponseModels;

namespace Bootstrap.Components.Miscellaneous.ResponseBuilders
{
    public static class ListResponseBuilder<T>
    {
        public static ListResponse<T> Ok = Build(ResponseCode.Success);
        public static ListResponse<T> BadRequest = Build(ResponseCode.InvalidPayloadOrOperation);
        public static ListResponse<T> SystemError = Build(ResponseCode.SystemError);
        public static ListResponse<T> Unauthorized = Build(ResponseCode.Unauthorized);
        public static ListResponse<T> NotFound = Build(ResponseCode.NotFound);
        public static ListResponse<T> NotModified = Build(ResponseCode.NotModified);
        public static ListResponse<T> Conflict = Build(ResponseCode.Conflict);
        public static ListResponse<T> Unauthenticated = Build(ResponseCode.Unauthenticated);
        public static ListResponse<T> Timeout = Build(ResponseCode.Timeout);

        public static ListResponse<T> Build(ResponseCode code, string message = null) =>
            new ListResponse<T>((int) code, message ?? code.GetDisplayName());

        public static ListResponse<T> BuildBadRequest(string message) =>
            Build(ResponseCode.InvalidPayloadOrOperation, message);
    }
}