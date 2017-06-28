using System;
using System.Threading.Tasks;
using Argonaught.Internal.RequestTypes.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Argonaught.Internal.Middleware
{
    internal class ResponsePolicy
    {
        private readonly RequestDelegate _next;
        private readonly ArgonautOptions _options;

        public ResponsePolicy(RequestDelegate next, IOptions<ArgonautOptions> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(ArgonautOptions));

            _options = options.Value;
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.Equals(_options.TokenPath, StringComparison.OrdinalIgnoreCase))
                return _next(context);

            if (context.Items["RequestType"] == null)
                throw new ArgumentNullException(nameof(IRequestType));

            var requestType = (IRequestType)context.Items["RequestType"];

            return requestType.Response.Execute(_next, context, _options);
        }

    }
}