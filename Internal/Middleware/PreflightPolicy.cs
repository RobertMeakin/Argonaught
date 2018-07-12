using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Argonaut.Internal.Middleware
{
    internal class PreflightPolicy
    {
        private readonly RequestDelegate _next;
        private readonly string _preflightPath;

        public PreflightPolicy(RequestDelegate next, string preflightPath)
        {
            _next = next;
            _preflightPath = preflightPath;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Value.Contains(_preflightPath) && context.Request.Method == "OPTIONS")
            {
                context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { context.Request.Headers["Origin"].ToString() });
                context.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "Authorization, Origin, X-Requested-With, Content-Type, Accept" });
                context.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "GET, POST, PUT, PATCH, DELETE, OPTIONS" });
                context.Response.Headers.Add("Access-Control-Allow-Credentials", new[] { "true" });
                return;
            }

            await _next(context);
        }
    }
}