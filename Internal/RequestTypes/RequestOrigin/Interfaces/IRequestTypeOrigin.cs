using Microsoft.AspNetCore.Http;

namespace Argonaut.Internal.RequestTypes.RequestOrigin.Interfaces
{
    internal interface IRequestTypeOrigin
    {
        /// <summary>
        /// Confirms the origin is allowed according to the audience and if so, adds an 'Access-Control-Allow-Origin' response header.
        /// </summary>
        void Validate(HttpContext context, ArgonautOptions options);
    }
    
}