using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Bootstrap.Extensions
{
    public static class CorsExtensions
    {
        private static readonly string[] Origins = new[]
        {
            // next.js
            "http://localhost:3000"
        };

        public static CorsPolicyBuilder WithDevOrigins(this CorsPolicyBuilder builder) =>
            Origins.Aggregate(builder, (current, o) => current.WithOrigins(o));

        public static IApplicationBuilder UseBootstrapCors(this IApplicationBuilder app, Action<CorsPolicyBuilder>? extraConfiguration = null, params string[] origins) =>
            app.UseCors(t =>
            {
                t.AllowAnyHeader().AllowAnyMethod().WithDevOrigins().WithOrigins(origins).AllowCredentials();
                extraConfiguration?.Invoke(t);
            });
    }
}