using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Aliyun.OSS;
using Bootstrap.Components.Storage.Infrastructures;
using Bootstrap.Models.ResponseModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bootstrap.Components.Storage.AliyunOss
{
    public class OssFileStorage : IFileStorage
    {
        private readonly OssClient _client;
        private readonly IOptions<OssFileStorageOptions> _options;
        private readonly ILogger<OssFileStorage> _logger;

        public OssFileStorage(IOptions<OssFileStorageOptions> options, ILogger<OssFileStorage> logger)
        {
            _options = options;
            _logger = logger;
            _client = new OssClient(options.Value.Endpoint, options.Value.AccessKeyId, options.Value.AccessKeySecret);
        }

        public Task<SingletonResponse<ObjectMetadata>> GetObjectMetaData(string key)
        {
            return Task.Run(() =>
            {
                try
                {
                    return new SingletonResponse<ObjectMetadata>(_client.GetObjectMetadata(_options.Value.BucketName,
                        key));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return new SingletonResponse<ObjectMetadata>
                    {
                        Code = (int) FileStorageUploadResponseCode.Error,
                        Message = ex.Message
                    };
                }
            });
        }

        public string GetUrl(string ossKey, bool keyIsRelativeToRootPath = false)
        {
            if (keyIsRelativeToRootPath)
            {
                ossKey = (_options.Value.RootPath.TrimEnd('/') + "/" + ossKey.Trim('/')).Trim('/');
            }

            return
                $"{_options.Value.Domain}/{string.Join("/", ossKey.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries).Select(WebUtility.UrlEncode))}";
        }

        public Task<SingletonResponse<string>> Save(string relativeFilename, Stream file)
        {
            return Task.Run(() =>
            {
                try
                {
                    relativeFilename = (_options.Value.RootPath + "/" + relativeFilename).Trim('/');
                    _client.PutObject(_options.Value.BucketName, relativeFilename, file);
                    return new SingletonResponse<string>(GetUrl(relativeFilename));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return new SingletonResponse<string>
                    {
                        Code = (int) FileStorageUploadResponseCode.Error,
                        Message = ex.Message
                    };
                }
            });
        }
    }
}