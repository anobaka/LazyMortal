using Bootstrap.Extensions;
using Bootstrap.Models.Constants;
using Bootstrap.Models.ResponseModels;

namespace Bootstrap.Components.Miscellaneous.ResponseBuilders
{
    public static class BaseResponseBuilder
    {
        public static BaseResponse Ok = Build(ResponseCode.Success);
        public static BaseResponse BadRequestOrOperation = Build(ResponseCode.InvalidPayloadOrOperation);

        public static BaseResponse SystemError = Build(ResponseCode.SystemError);
        public static BaseResponse Unauthorized = Build(ResponseCode.Unauthorized);
        public static BaseResponse NotFound = Build(ResponseCode.NotFound);
        public static BaseResponse Conflict = Build(ResponseCode.Conflict);
        public static BaseResponse Unauthenticated = Build(ResponseCode.Unauthenticated);
        public static BaseResponse Timeout = Build(ResponseCode.Timeout);
        public static BaseResponse NotModified = Build(ResponseCode.NotModified);

        public static BaseResponse Build(ResponseCode code, string? message = null) =>
            new BaseResponse((int) code, message ?? code.GetDisplayName());

        public static BaseResponse BuildBadRequest(string? message) => Build(ResponseCode.InvalidPayloadOrOperation, message);
        public static BaseResponse BuildUnauthorized(string? message) => Build(ResponseCode.Unauthorized, message);
    }
}