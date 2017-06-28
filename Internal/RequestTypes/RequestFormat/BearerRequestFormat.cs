using Argonaught.Internal.RequestTypes.RequestFormat.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Argonaught.Internal.RequestTypes.RequestFormat
{
    internal class BearerRequestFormat : IRequestTypeFormat
    {
        public string FindErrors(HttpContext context, ArgonautOptions options)
        {
            if (!context.Request.Headers.ContainsKey("authorization"))
                return "An authorization header is required.";

            if (!context.Request.Headers["authorization"].ToString().Contains("Bearer "))
                return "The authorization field needs to be formatted as Bearer [access_token].";

            return "";
        }
    }
}