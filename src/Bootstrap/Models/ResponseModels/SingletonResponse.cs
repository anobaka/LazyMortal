using JetBrains.Annotations;

namespace Bootstrap.Models.ResponseModels
{
    public class SingletonResponse<T> : BaseResponse
    {
        [CanBeNull] public T Data { get; set; }

        public SingletonResponse()
        {
        }

        public SingletonResponse([CanBeNull] T? data)
        {
            Data = data;
        }

        public SingletonResponse(int code) : base(code)
        {
        }

        public SingletonResponse(int code, string message) : base(code, message)
        {
        }
    }
}
