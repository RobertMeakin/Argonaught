using Argonaught.Internal.RequestTypes.RequestOrigin.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Argonaught.Internal.RequestTypes.RequestOrigin
{
    internal class RefreshTokenRequestOrigin : IRequestTypeOrigin
    {
        /// <summary>
        /// Always allow origin for refresh token requests as the associated audience can only be found by a trip to the database.
        /// Origin still needed for access token and any data.
        /// </summary>
        public void Validate(HttpContext context, ArgonautOptions options)
            => context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { context.Request.Headers["Origin"].ToString() });
    }
    
}