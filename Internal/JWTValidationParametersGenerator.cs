using System;
using Argonaught.Model;
using Microsoft.IdentityModel.Tokens;

namespace Argonaught.Internal
{
    internal class JWTValidationParametersGenerator
    {
        IAudience _audience;

        public JWTValidationParametersGenerator(IAudience options)
        {
            _audience = options;
        }

        public TokenValidationParameters SecretAndExpirationDate()
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = SigningKey(),

                ValidateIssuer = true,
                ValidIssuer = _audience.Issuer,

                ValidateAudience = true,
                ValidAudience = _audience.Id,

                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };

            return tokenValidationParameters;
        }


        /// <summary>
        /// Will produce validation parameters that do not validate against expiry date, only issuer, audience and secret.
        /// </summary>
        /// <returns></returns>
        public TokenValidationParameters SecretOnly()
        {

            var tokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = SigningKey(),

                ValidateIssuer = true,
                ValidIssuer = _audience.Issuer,

                ValidateAudience = true,
                ValidAudience = _audience.Id,

                ValidateLifetime = false, //--Difference

                ClockSkew = TimeSpan.Zero
            };

            return tokenValidationParameters;
        }

        private SymmetricSecurityKey SigningKey() =>
            new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(_audience.Secret));


    }
}