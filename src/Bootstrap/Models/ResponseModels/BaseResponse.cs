using JetBrains.Annotations;
using Microsoft.Extensions.Localization;

namespace Bootstrap.Models.ResponseModels
{
    public class BaseResponse
    {
        public int Code { get; set; }
        public string? Message { get; set; }

        public BaseResponse()
        {
        }

        public BaseResponse(int code)
        {
            Code = code;
        }

        public BaseResponse(int code, string? message)
        {
            Code = code;
            Message = message;
        }

        public SingletonResponse<TData> ToSingletonResponse<TData>() => new(Code, Message);
        public ListResponse<TData> ToListResponse<TData>() => new(Code, Message);
        public SearchResponse<TData> ToSearchResponse<TData>() => new(Code, Message);
    }
}