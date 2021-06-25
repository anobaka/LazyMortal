﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bootstrap.Components.Communication.HttpClient;
using Bootstrap.Components.IM.DingTalk.Models.ResponseModels;
using Bootstrap.Models.RequestModels;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Bootstrap.Components.IM.DingTalk
{
    public class DingTalkClient : ServiceHttpClient<DingTalkClientOptions>
    {
        private const string AccessTokenCacheKey = "DingTalkAccessToken";
        private readonly IDistributedCache _cache;
        private readonly List<int> _invalidAccessTokenErrCodes = new List<int> {40001, 40014, 41001, 42001, 43007};

        public DingTalkClient(IOptions<DingTalkClientOptions> options, IDistributedCache cache) : base(options)
        {
            _cache = cache;
        }

        protected async Task<string> GetAccessToken()
        {
            var rsp = await base.InvokeAsync<DingTalkAccessTokenGetResponseModel>(new ServiceHttpClientRequestModel
            {
                Method = HttpMethod.Get,
                QueryParameters = new Dictionary<string, List<object>>
                {
                    {"appkey", new List<object> {Options.Value.AppKey}},
                    {"appsecret", new List<object> {Options.Value.AppSecret}}
                },
                RelativeUri = "gettoken"
            });
            if (rsp.ErrCode == 0)
            {
                await _cache.SetStringAsync(AccessTokenCacheKey, rsp.AccessToken, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(90)
                });
                return rsp.AccessToken;
            }
            else
            {
                throw new Exception(
                    $"Unable to get ding talk access token, response: {JsonConvert.SerializeObject(rsp)}");
            }
        }

        public async Task<DingTalkUserInfoGetResponseModel> GetUserInfo(string code)
        {
            return await InvokeAsync<DingTalkUserInfoGetResponseModel>(new ServiceHttpClientRequestModel
            {
                Method = HttpMethod.Get,
                QueryParameters = new Dictionary<string, List<object>>
                {
                    {nameof(code), new List<object> {code}}
                },
                RelativeUri = "user/getuserinfo"
            });
        }

        public async Task<DingTalkCorpConversationMessageSendResponseModel> SendCorpConversationMessage(CorpConversationMessageSendRequestModel model)
        {
            model.AgentId = Options.Value.AgentId;
            return await InvokeAsync<DingTalkCorpConversationMessageSendResponseModel>(new ServiceHttpClientRequestModel
            {
                Method = HttpMethod.Post,
                Body = model,
                RelativeUri = "topapi/message/corpconversation/asyncsend_v2"
            });
        }

        protected override async Task<T> InvokeAsync<T>(ServiceHttpClientRequestModel request,
            CancellationToken? cancellationToken = null)
        {
            var accessToken = await _cache.GetStringAsync(AccessTokenCacheKey);
            if (string.IsNullOrEmpty(accessToken))
            {
                accessToken = await GetAccessToken();
            }

            if (request.QueryParameters == null)
            {
                request.QueryParameters = new Dictionary<string, List<object>>();
            }

            request.QueryParameters["access_token"] = new List<object> {accessToken};
            var rsp = await base.InvokeAsync<T>(request, cancellationToken);
            if (rsp is DingTalkResponseModel dsp)
            {
                if (_invalidAccessTokenErrCodes.Contains(dsp.ErrCode))
                {
                    // Again
                    accessToken = await GetAccessToken();
                    request.QueryParameters["access_token"] = new List<object> {accessToken};
                    rsp = await base.InvokeAsync<T>(request, cancellationToken);
                }
            }

            return rsp;
        }
    }
}