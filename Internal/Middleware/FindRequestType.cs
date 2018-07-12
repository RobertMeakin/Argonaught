using System;
using System.Threading.Tasks;
using Argonaut.Internal.RequestTypes.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Argonaut.Internal.Middleware
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
            context.Items["RequestType"] = context.GetArgonautRequestType(_options.APIPath, _options.IncludeAuthGeneration);
            await _next(context);
        }



    }
}