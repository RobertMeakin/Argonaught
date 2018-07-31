using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Argonaut.Internal.Middleware {
    internal class PreflightPolicy {
        private readonly RequestDelegate _next;
        private readonly ArgonautOptions _options;

        public PreflightPolicy(RequestDelegate next, IOptions<ArgonautOptions> options) {
            if (options == null)
                throw new ArgumentNullException(nameof(ArgonautOptions));

            _next = next;
            _options = options.Value;
        }

        public async Task Invoke(HttpContext context) {
            var path = context.Request.Path.Value;
            if ( (path.Contains(_options.APIPath) || path.Contains(_options.TokenPath)) && context.Request.Method == "OPTIONS") {
                context.Response.Headers.Add("Access-Control-Allow-Origin", new [] { context.Request.Headers["Origin"].ToString() });
                context.Response.Headers.Add("Access-Control-Allow-Headers", new [] { "Authorization, Origin, X-Requested-With, Content-Type, Accept", "Prefer", "Expect" });
                context.Response.Headers.Add("Access-Control-Allow-Methods", new [] { "GET, POST, PUT, PATCH, DELETE, OPTIONS" });
                context.Response.Headers.Add("Access-Control-Allow-Credentials", new [] { "true" });
                return;
            }

            await _next(context);
        }
    }
}