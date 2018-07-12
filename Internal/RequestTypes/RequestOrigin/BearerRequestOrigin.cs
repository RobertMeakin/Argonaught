using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Argonaut.Internal.RequestTypes.RequestOrigin.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Argonaut.Internal.RequestTypes.RequestOrigin
{
    internal class BearerRequestOrigin : IRequestTypeOrigin
    {
        public void Validate(HttpContext context, ArgonautOptions options)
        {
            var authHeader = context.Request.Headers.Where(l => l.Key.ToLower() == "authorization").FirstOrDefault();
            var accessToken = authHeader.Value.ToString().Replace("Bearer ", "");
            var audRequested = GetAudienceIdFromAccessToken(accessToken).ToLower();

            var audFound = options.Audiences.Where(l => l.Id.ToLower() == audRequested).FirstOrDefault();

            if (audFound == null)
                return; //OR Return 404 - Audience not found?? Decided against this on the off chance it could be used to fish for allowed audiences.

            var origin = context.Request.Headers["Origin"].ToString().ToLower();

            var allowedOrigins = audFound.AllowedOrigins.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            if (allowedOrigins.Contains("*") || allowedOrigins.Contains(origin))
                context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { context.Request.Headers["Origin"].ToString() });
        }

        private string GetAudienceIdFromAccessToken(string accessToken)
        {
            var handler = new JwtSecurityTokenHandler();

            try
            {
                var token = handler.ReadJwtToken(accessToken);
                var audClaim = token.Claims.Where(l => l.Type == "aud").FirstOrDefault();

                if (audClaim == null)
                    return "";

                var audienceId = audClaim.Value;
                return audienceId.ToString().ToLower();
            }
            catch
            {
                return "";
            }

        }
    }
}