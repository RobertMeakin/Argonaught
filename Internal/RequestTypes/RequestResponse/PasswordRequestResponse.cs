using System;
using System.Threading.Tasks;
using Argonaut.Internal;
using Argonaut.Internal.Interfaces;
using Argonaut.Internal.RequestTypes.RequestResponse.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Argonaut.Internal.RequestTypes.RequestResponse
{
    internal class PasswordRequestResponse : IRequestTypeReponse
    {
        private JWTBuilder _jwtBuilder;
        private IEncryptor _encryptor;

        public PasswordRequestResponse()
        {
            _encryptor = new StringCipher();
        }

        public async Task Execute(RequestDelegate next, HttpContext context, ArgonautOptions options)
        {
            var username = context.Request.Form["username"];
            var password = context.Request.Form["password"];
            var audienceId = context.Request.Form["audience"];

            var userValidation = options.ValidateUser.Invoke(username, password, audienceId); //Passback ArgonautUser, which should include audience.

            if (!userValidation.Validated)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid username, password, or audience.");
                return;
            }

            _jwtBuilder = JWTBuilder.New(userValidation.Audience);
            foreach (var c in userValidation.Claims)
                _jwtBuilder.AddClaim(c);

            await GenerateAccessToken(context, username, options);
        }


        private async Task GenerateAccessToken(HttpContext context, string subject, ArgonautOptions options)
        {

            _jwtBuilder.Subject = subject;
            _jwtBuilder.Expiration = TimeSpan.FromMinutes(options.AccessTokenLifetimeMinutes);
            _jwtBuilder.VisualiseClaims = options.VisualiseClaims;
            _jwtBuilder.Build();

            var response = _jwtBuilder.GetWebResponse();
            var rt = _jwtBuilder.GetRefreshToken();

            rt.EncryptTicket(_encryptor);

            //Call method for server to save refresh token to persistence
            options.RefreshTokenGenerated(
                new Internal.Model.RefreshToken(rt.Id, rt.Subject, rt.AudienceId, rt.IssuedUtc, rt.ExpiresUtc, rt.ProtectedTicket)
                );

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

    }
}