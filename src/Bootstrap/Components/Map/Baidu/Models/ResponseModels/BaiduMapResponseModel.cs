namespace Bootstrap.Components.Map.Baidu.Models.ResponseModels
{
    public class BaiduMapResponseModel<T>
    {
        public int Status { get; set; }
        public T Result { get; set; }
    }
}
