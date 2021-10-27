using System.Threading.Tasks;
using Bootstrap.Components.Miscellaneous.ResponseBuilders;
using Bootstrap.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Bootstrap.Components.Miscellaneous
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddCookieWithBootstrapBehavior(this AuthenticationBuilder builder,
            string scheme)
        {
            return builder.AddCookie(scheme, t =>
            {
                t.Cookie.Name = scheme;
                // t.Cookie.SameSite = SameSiteMode.None;
                t.Events.OnValidatePrincipal = context => Task.CompletedTask;
                t.Events.OnRedirectToLogout = async context => { };
                t.Events.OnRedirectToReturnUrl = async context => { };
                t.Events.OnRedirectToLogin = async context =>
                {
                    if (!context.Response.HasStarted && context.Response.ContentType.IsNullOrEmpty())
                    {
                        context.Response.StatusCode = 200;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(
                            JsonConvert.SerializeObject(BaseResponseBuilder.Unauthenticated));
                    }
                };
                t.Events.OnRedirectToAccessDenied = async context =>
                {
                    if (!context.Response.HasStarted && context.Response.ContentType.IsNullOrEmpty())
                    {
                        context.Response.StatusCode = 200;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(
                            JsonConvert.SerializeObject(BaseResponseBuilder.Unauthorized));
#if !NETCOREAPP2_1
                        await context.Response.StartAsync();
#endif
                    }
                };
            });
        }
    }
}