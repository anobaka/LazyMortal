using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bootstrap.Components.Configuration.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bootstrap.Components.Configuration
{
    /// <summary>
    /// todo: file version control
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    public class JsonOptionsManager<TOptions> : AbstractOptionsManager<TOptions> where TOptions : class, new()
    {
        public string FilePath { get; }

        public override TOptions Value
        {
            get
            {
                TOptions? options = null;
                if (File.Exists(FilePath))
                {
                    var json = File.ReadAllText(FilePath, Encoding.UTF8);
                    var jObject = JObject.Parse(json);
                    options = jObject[_key]?.ToObject<TOptions>();
                }

                options ??= new TOptions();

                return options;
            }
        }

        private readonly string _key;
        private readonly SemaphoreSlim _ss;

        public JsonOptionsManager(string filePath, string key)
        {
            FilePath = filePath;
            _key = key;
            _ss = new SemaphoreSlim(1, 1);
        }

        public override void Save(TOptions options)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
            var jo = new JObject
            {
                [_key] = JToken.FromObject(options)
            };
            _ss.Wait();
            try
            {
                File.WriteAllText(FilePath, JsonConvert.SerializeObject(jo, Formatting.Indented), Encoding.UTF8);
            }
            finally
            {
                _ss.Release();
            }
        }

        public override async Task SaveAsync(TOptions options)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
            var jo = new JObject
            {
                [_key] = JToken.FromObject(options)
            };
            await _ss.WaitAsync();
            try
            {
                await File.WriteAllTextAsync(FilePath, JsonConvert.SerializeObject(jo, Formatting.Indented),
                    Encoding.UTF8);
            }
            finally
            {
                _ss.Release();
            }
        }
    }
}