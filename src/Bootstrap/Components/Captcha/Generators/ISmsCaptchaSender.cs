using System.Threading.Tasks;
using Bootstrap.Models.ResponseModels;

namespace Bootstrap.Components.Captcha.Generators
{
    public interface ISmsCaptchaSender
    {
        Task<BaseResponse> Send(string mobile, string code);
    }
}
