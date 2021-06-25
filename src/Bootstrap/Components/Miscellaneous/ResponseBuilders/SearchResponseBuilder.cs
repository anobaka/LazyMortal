using Bootstrap.Extensions;
using Bootstrap.Models.Constants;
using Bootstrap.Models.ResponseModels;

namespace Bootstrap.Components.Miscellaneous.ResponseBuilders
{
    public static class SearchResponseBuilder<T>
    {
        public static SearchResponse<T> Ok = Build(ResponseCode.Success);
        public static SearchResponse<T> BadRequest = Build(ResponseCode.InvalidPayloadOrOperation);
        public static SearchResponse<T> SystemError = Build(ResponseCode.SystemError);
        public static SearchResponse<T> Unauthorized = Build(ResponseCode.Unauthorized);
        public static SearchResponse<T> NotFound = Build(ResponseCode.NotFound);
        public static SearchResponse<T> Conflict = Build(ResponseCode.Conflict);
        public static SearchResponse<T> Unauthenticated = Build(ResponseCode.Unauthenticated);
        public static SearchResponse<T> Timeout = Build(ResponseCode.Timeout);

        public static SearchResponse<T> Build(ResponseCode code, string message = null) =>
            new SearchResponse<T>((int) code, message ?? code.GetDisplayName());

        public static SearchResponse<T> BuildBadRequest(string message) =>
            Build(ResponseCode.InvalidPayloadOrOperation, message);
    }
}