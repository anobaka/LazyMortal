using Newtonsoft.Json;

namespace Bootstrap.Components.Map.Baidu.Models.ResponseModels
{
    public class BaiduMapLocation
    {
        [JsonProperty("lng")]
        public decimal Longitude { get; set; }
        [JsonProperty("lat")]
        public decimal Latitude { get; set; }
    }
}
