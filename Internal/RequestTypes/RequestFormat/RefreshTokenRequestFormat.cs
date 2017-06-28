using Argonaught.Internal.RequestTypes.RequestFormat.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Argonaught.Internal.RequestTypes.RequestFormat
{
    internal class RefreshTokenRequestFormat : IRequestTypeFormat
    {
        public string FindErrors(HttpContext context, ArgonautOptions options)
        {
            if (!context.Request.Method.Equals("POST"))
                return "The request method must be POST.";

            if (context.Request.Form == null)
                return "The request content type must be application/x-www-form-urlencoded.";

            if (!context.Request.HasFormContentType)
                return "The request content type must be application/x-www-form-urlencoded.";

            if (!context.Request.Form.ContainsKey("refresh_token"))
                return "A refresh_token form field is required.";

            return "";
        }
    }
}