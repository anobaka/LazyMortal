using System;
using System.Linq;
using System.Threading.Tasks;
using Bootstrap.Extensions;
using Bootstrap.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Bootstrap.Components.Map.Baidu
{
    public class BaiduMapService
    {
        private readonly BaiduMapClient _client;
        protected readonly ILogger<BaiduMapService> Logger;

        public BaiduMapService(BaiduMapClient client, ILogger<BaiduMapService> logger)
        {
            _client = client;
            Logger = logger;
        }

        public async Task<StandardAddress> ParseAddress(string address)
        {
            var coordinateRsp = await _client.ParseAddress(address);
            var location = coordinateRsp.Result?.Location;
            StandardAddress sa = null;
            if (location != null)
            {
                var addressRsp = await _client.ParseCoordinate(location.Longitude, location.Latitude);
                var ac = addressRsp.Result?.AddressComponent;
                if (ac != null)
                {
                    sa = new StandardAddress
                    {
                        Country = ac.Country.ToNullIfEmpty(),
                        Province = ac.Province.ToNullIfEmpty(),
                        City = ac.City.ToNullIfEmpty(),
                        District = ac.District.ToNullIfEmpty(),
                        Town = ac.Town.ToNullIfEmpty(),
                        Street = ac.Street.ToNullIfEmpty()
                    };
                    var trims = new[]
                    {
                        new[] {sa.Country},
                        new[] {sa.Province, sa.Province?.TrimEnd('省')},
                        new[]
                        {
                            sa.City,
                            sa.City?.TrimEnd('市')
                        },
                        new[]
                        {
                            sa.District,
                            sa.District?.TrimEnd('区')
                        },
                        new[]
                        {
                            sa.Town,
                            sa.Town?.TrimEnd('镇'),
                            sa.Town?.TrimEnd('乡'),
                        },
                        new[]
                        {
                            sa.Street,
                            sa.Street?.TrimEnd('路'),
                            sa.Street?.TrimEnd('街')
                        }
                    };
                    var tAddress = address;
                    for (var i = 0; i < trims.Length; i++)
                    {
                        var t = trims[i];
                        var tt = t.Where(a => !a.IsNullOrEmpty()).ToArray();
                        foreach (var r in tt)
                        {
                            var idx = tAddress.IndexOf(r, StringComparison.OrdinalIgnoreCase);
                            if (idx == 0)
                            {
                                tAddress = tAddress.Substring(r.Length);
                                // Remove repeat region name
                                i--;
                                break;
                            }
                        }
                    }

                    sa.DetailedAddress = tAddress;
                }
            }

            Logger.LogInformation($"{address} => {JsonConvert.SerializeObject(sa)}");

            return sa;
        }
    }
}