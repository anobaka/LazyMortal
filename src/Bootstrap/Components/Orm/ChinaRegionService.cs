using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Bootstrap.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Bootstrap.Components.Orm
{
    /// <summary>
    /// todo: redesign
    /// </summary>
    public class
        ChinaRegionService<TDbContext, TRegion, TKey> : ActiveMultilevelResourceService<TDbContext, TRegion, TKey>
        where TDbContext : DbContext
        where TRegion : ActiveMultilevelResource<TRegion>, new()
    {
        private readonly HttpClient _client = new HttpClient();

        private const string ChinaRegionJsonUrl =
            "https://raw.githubusercontent.com/small-dream/China_Province_City/master/2019%E5%B9%B411%E6%9C%88%E4%B8%AD%E5%8D%8E%E4%BA%BA%E6%B0%91%E5%85%B1%E5%92%8C%E5%9B%BD%E5%8E%BF%E4%BB%A5%E4%B8%8A%E8%A1%8C%E6%94%BF%E5%8C%BA%E5%88%92%E4%BB%A3%E7%A0%81.json";

        public ChinaRegionService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public class ProvinceResponseModel
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public List<CityResponseModel> CityList { get; set; }

            public class CityResponseModel
            {
                public string Code { get; set; }
                public string Name { get; set; }
                public List<AreaResponseModel> AreaList { get; set; }

                public class AreaResponseModel
                {
                    public string Code { get; set; }
                    public string Name { get; set; }
                }
            }
        }

        protected async Task<List<ProvinceResponseModel>> GetRegionData()
        {
            var json = await _client.GetStringAsync(ChinaRegionJsonUrl);
            var data = JsonConvert.DeserializeObject<List<ProvinceResponseModel>>(json);
            return data;
        }
    }
}