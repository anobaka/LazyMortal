using System.ComponentModel.DataAnnotations;

namespace Bootstrap.Models.Constants
{
    public enum ResponseCode
    {
        [Display(Name = "成功")]
        Success = 0,

        [Display(Name = "数据无任何更新")]
        NotModified = 304,

        [Display(Name = "请求参数有误或操作非法")]
        InvalidPayloadOrOperation = 400,

        [Display(Name = "请登录后再试，正在为您跳转至登录页")]
        Unauthenticated = 401,

        [Display(Name = "权限不足，请联系管理员添加权限并重新登录后再尝试，如果问题依旧存在请联系开发人员")]
        Unauthorized = 403,

        [Display(Name = "数据不存在")]
        NotFound = 404,

        [Display(Name = "数据冲突或已存在")]
        Conflict = 409,

        [Display(Name = "请求异常，请稍后再试")]
        SystemError = 500,

        [Display(Name = "请求超时，请稍后再试")]
        Timeout = 504,

        [Display(Name = "验证码有误或已过期，请重新提交")]
        InvalidCaptcha = 100400
    }
}