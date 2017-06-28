using System;
using System.Threading.Tasks;
using Argonaught.Internal.RequestTypes.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Argonaught.Internal.Middleware
{
    public class OriginPolicy
    {
        private readonly RequestDelegate _next;
        private readonly ArgonautOptions _options;


        public OriginPolicy(RequestDelegate next, IOptions<ArgonautOptions> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(ArgonautOptions));

            _next = next;
            _options = options.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Items["RequestType"] == null)
                throw new ArgumentNullException(nameof(IRequestType));

            if (!context.Request.Headers.Keys.Contains("Origin"))
            {
                await _next(context);
                return;
            }

            var requestType = (IRequestType)context.Items["RequestType"];
            requestType.Origin.Validate(context, _options);

            await _next(context);
        }

    }
}