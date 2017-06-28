using Argonaught.Internal.RequestTypes.RequestFormat.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Argonaught.Internal.RequestTypes.RequestFormat
{
    internal class PasswordRequestFormat : IRequestTypeFormat
    {
        public string FindErrors(HttpContext context, ArgonautOptions options)
        {

            if (!context.Request.Method.Equals("POST"))
                return "The request method must be POST.";

            if (context.Request.Form == null)
                return "The request content type must be application/x-www-form-urlencoded.";

            if (!context.Request.HasFormContentType)
                return "The request content type must be application/x-www-form-urlencoded.";

            if (!context.Request.Form.ContainsKey("username"))
                return "A username form field is required.";

            if (!context.Request.Form.ContainsKey("password"))
                return "A password form field is required.";

            if (!context.Request.Form.ContainsKey("audience"))
                return "An audience form field is required.";

            return "";
        }
    }
}