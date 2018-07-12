using System;
using System.Linq;
using Argonaut.Internal.RequestTypes.RequestOrigin.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Argonaut.Internal.RequestTypes.RequestOrigin
{
    internal class PasswordRequestOrigin : IRequestTypeOrigin
    {
        public void Validate(HttpContext context, ArgonautOptions options)
        {
            var origin = context.Request.Headers["Origin"].ToString().ToLower();
            var audienceId = context.Request.Form["audience"].ToString().ToLower();
            var audFound = options.Audiences.Where(l => l.Id.ToLower() == audienceId).FirstOrDefault();

            if (audFound == null)
                return;

            var allowedOrigins = audFound.AllowedOrigins.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();

            if (allowedOrigins.Contains("*") || allowedOrigins.Contains(origin))
                context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { context.Request.Headers["Origin"].ToString() });
        }
    }
}