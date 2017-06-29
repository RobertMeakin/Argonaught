using Argonaught.Internal.RequestTypes.RequestFormat.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Argonaught.Internal.RequestTypes.RequestFormat
{
    internal class UnknownRequestFormat : IRequestTypeFormat
    {
        public string FindErrors(HttpContext context, ArgonautOptions options)
        {
            //Check to see if the user is attemping to request an access token by checking if the 'grant_type' parameter exists, even though the request type could not be found.
            if (!context.Request.HasFormContentType)
                return "";
            
            if (context.Request.Form.ContainsKey("grant_type"))
                return "The request is not properly formatted.";
            //End check
            
            return "";
        }
        

    }
}