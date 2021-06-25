using System.Collections.Generic;
using System.Linq;

namespace Bootstrap.Models.ResponseModels
{
    public class ListResponse<T> : SingletonResponse<List<T>>
    {
        public ListResponse()
        {
        }

        public ListResponse(IEnumerable<T> data) : base(data.ToList())
        {
        }

        public ListResponse(int code) : base(code)
        {
        }

        public ListResponse(int code, string message) : base(code, message)
        {
        }
    }
}
