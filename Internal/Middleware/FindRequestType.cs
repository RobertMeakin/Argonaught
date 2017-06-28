using System;
using System.Threading.Tasks;
using Argonaught.Internal.RequestTypes.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Argonaught.Internal.Middleware
{
    internal class FindRequestType
    {
        private readonly RequestDelegate _next;
        private readonly ArgonautOptions _options;

        public IRequestType RequestType { get; private set; }

        public FindRequestType(RequestDelegate next, IOptions<ArgonautOptions> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(ArgonautOptions));

            _options = options.Value;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Items["RequestType"] = context.GetArgonaughtRequestType(_options.APIPath);
            await _next(context);
        }



    }
}