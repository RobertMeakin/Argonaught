using System;
using System.Threading.Tasks;
using Argonaut.Internal.RequestTypes.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Argonaut.Internal.Middleware
{
    /// <summary>
    /// Checks that the format of the incoming request is allowed. If so, the request continues through the pipeline.
    /// If not, it adds an 'Allowed-Origin' header and returns a 400 Bad Request status code along with an error description. 
    /// 'Allowed-Origin' is added to allow a 400 Bad Request response, otherwise a web browser would show 401, Unauthorised.
    /// </summary>
    internal class RequestFormatPolicy
    {
        private readonly RequestDelegate _next;
        private readonly ArgonautOptions _options;
        private string ErrorMessage { get; set; }

        public RequestFormatPolicy(RequestDelegate next, IOptions<ArgonautOptions> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(ArgonautOptions));


            _options = options.Value;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Items["RequestType"] == null)
                throw new ArgumentNullException(nameof(IRequestType));

            var requestType = (IRequestType)context.Items["RequestType"];
            ErrorMessage = requestType.Format.FindErrors(context, _options);

            if (ErrorMessage != "")
            {
                //Always add allow origin if the request is badly formatted 
                //as the error in the formatting will prevent the audience allowed origins to be checked 
                //and we are only returning an error message. 
                context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { context.Request.Headers["Origin"].ToString() });
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync(ErrorMessage);
                return;
            }

            await _next(context);
        }



    }
}