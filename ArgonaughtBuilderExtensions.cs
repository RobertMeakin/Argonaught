using System;
using Argonaught.Internal;
using Argonaught.Internal.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace Argonaught
{
    public static class ArgonaughtBuilderExtensions
    {
        public static IApplicationBuilder UseArgonaut(this IApplicationBuilder app, ArgonautOptions argonautOptions)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            if (argonautOptions == null)
                throw new ArgumentNullException(nameof(argonautOptions));

            app.UseMiddleware<PreflightPolicy>(argonautOptions.APIPath);
            app.UseMiddleware<FindRequestType>(Options.Create(argonautOptions));
            app.UseMiddleware<RequestFormatPolicy>(Options.Create(argonautOptions));
            app.UseMiddleware<OriginPolicy>(Options.Create(argonautOptions));
            app.UseMiddleware<ResponsePolicy>(Options.Create(argonautOptions));            
            
            foreach (var aud in argonautOptions.Audiences)
            {
                var jwtHelper = new JWTBuilder(aud);
                var bearerOptions = new JwtBearerOptions
                {
                    AutomaticAuthenticate = true,
                    AutomaticChallenge = true,
                    TokenValidationParameters = jwtHelper.validationParameters.SecretAndExpirationDate()
                };
                app.UseJwtBearerAuthentication(bearerOptions);
            }
            return app;
        }
    }
}