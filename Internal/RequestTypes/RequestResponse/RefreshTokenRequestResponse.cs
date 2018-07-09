using System.Threading.Tasks;
using Argonaught.Internal;
using Argonaught.Internal.Interfaces;
using Argonaught.Internal.RequestTypes.RequestResponse.Interfaces;
using Domain = Argonaught.Internal.DomainObjects;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Argonaught.Internal.RequestTypes.RequestResponse {
    internal class RefreshTokenRequestResponse : IRequestTypeReponse {
        private JWTBuilder _jwtBuilder;
        private IEncryptor _encryptor;

        public RefreshTokenRequestResponse() {
            _encryptor = new StringCipher();
        }
        public async Task Execute(RequestDelegate next, HttpContext context, ArgonautOptions options) {
            var rtFromRequest = context.Request.Form["refresh_token"];
            var hashedRefreshTokenId = Argonaught.Internal.Hashing.GetHash(rtFromRequest);

            var persistenceResponse = options.RefreshAccessToken(hashedRefreshTokenId); //Client returns refresh token model with encrpted ticket.

            if (persistenceResponse == null) {
                await RespondRefreshTokenInvalid(context);
                return;
            }

            if (persistenceResponse.RefreshToken == null) {
                await RespondRefreshTokenInvalid(context);
                return;
            }

            if (persistenceResponse.Audience == null) {
                await RespondRefreshTokenInvalid(context);
                return;
            }

            //Map to domain refresh token
            Domain.RefreshToken rt = Domain.RefreshToken.New(
                persistenceResponse.RefreshToken.Id,
                persistenceResponse.RefreshToken.Subject,
                persistenceResponse.RefreshToken.AudienceId,
                persistenceResponse.RefreshToken.ProtectedTicket,
                persistenceResponse.RefreshToken.IssuedUtc,
                persistenceResponse.RefreshToken.ExpiresUtc
            );

            var nowUtc = DateTime.UtcNow; //TODO: Could do with moving to interface
            if (nowUtc > rt.ExpiresUtc) {
                await RespondRefreshTokenInvalid(context);
                return;
            }

            try {
                rt.DecryptTicket(_encryptor, rtFromRequest.ToString());
            } catch {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Persisted refresh token failed decryption - Log in using username and password.");
                return;
            }

            if (rt.ProtectedTicket == null) {
                await RespondRefreshTokenInvalid(context);
                return;
            }

            var vp = new JWTValidationParametersGenerator(persistenceResponse.Audience).SecretOnly();
            var handler = new JwtSecurityTokenHandler();

            Microsoft.IdentityModel.Tokens.SecurityToken validatedToken = null;
            try {
                handler.ValidateToken(rt.ProtectedTicket, vp, out validatedToken);
            } catch {
                await RespondRefreshTokenInvalid(context);
                return;
            }

            if (validatedToken == null) {
                await RespondRefreshTokenInvalid(context);
                return;
            }

            var jwt = validatedToken as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
            var sub = jwt.Claims.Where(l => l.Type == "sub").FirstOrDefault().Value;
            var claimsToUse = jwt.Claims.ExcludeDefaultAccessTokenClaims();

            _jwtBuilder = JWTBuilder.New(persistenceResponse.Audience);
            _jwtBuilder.AddClaims(claimsToUse);

            await GenerateAccessToken(context, sub, options);
        }

        private async Task GenerateAccessToken(HttpContext context, string subject, ArgonautOptions options) //,User user
        {
            _jwtBuilder.Subject = subject;
            _jwtBuilder.Expiration = TimeSpan.FromMinutes(options.AccessTokenLifetimeMinutes);
            _jwtBuilder.Build();

            var response = _jwtBuilder.GetWebResponse();
            var rt = _jwtBuilder.GetRefreshToken();

            rt.EncryptTicket(_encryptor);

            //Generate event for client to save refresh token to persistence
            options.RefreshTokenGenerated(
                new Internal.Model.RefreshToken(rt.Id, rt.Subject, rt.AudienceId, rt.IssuedUtc, rt.ExpiresUtc, rt.ProtectedTicket)
            );

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        public async Task RespondRefreshTokenInvalid(HttpContext context) {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid refresh Token - Log in using username and password.");
        }
    }
}