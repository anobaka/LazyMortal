using System.IO;
using System.Threading.Tasks;
using Bootstrap.Models.ResponseModels;

namespace Bootstrap.Components.Captcha.Generators
{
    public interface IImageCaptchaGenerator
    {
        Task<SingletonResponse<Stream>> Generate(string code, int width = 104, int height = 36);
    }
}
