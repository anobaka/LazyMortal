using System.Collections.Generic;
using System.Linq;
using Bootstrap.Models.RequestModels;
using Bootstrap.Models.ResponseModels;

namespace Bootstrap.Extensions
{
    public static class SearchRequestModelExtensions
    {
        public static SearchResponse<T> BuildResponse<T>(this SearchRequestModel model, IEnumerable<T> data, int count,
            int code = 0, string? message = null)
        {
            return new SearchResponse<T>
            {
                PageSize = model.PageSize,
                PageIndex = model.PageIndex,
                Data = data.ToList(),
                TotalCount = count,
                Code = code,
                Message = message
            };
        }
    }
}
