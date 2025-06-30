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
            // ice design 1
            "http://localhost:4444",
            // ice design 2
            "http://localhost:4445",
            // umi 1
            "http://localhost:8000",
            // umi 2
            "http://localhost:8001",
            "http://localhost:1970",
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