using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bootstrap.Components.Miscellaneous.ResponseBuilders;
using Bootstrap.Models.Constants;
using Bootstrap.Models.ResponseModels;
using Microsoft.Extensions.Localization;

namespace Bootstrap.Extensions
{
    public static class ResponseExtensions
    {
        public static T WithLocalization<T>(this T response, IStringLocalizer localizer, params object[] args)
            where T : BaseResponse
        {
            response.Message = localizer[response.Message, args];
            return response;
        }

        public static bool IsSuccess(this BaseResponse response)
        {
            return response.Code == (int)ResponseCode.Success;
        }
    }
}