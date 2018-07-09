using System;
using Argonaught.Internal;
using Argonaught.Internal.Middleware;
using Argonaught.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Argonaught {
    public static class ArgonaughtServicesExtensions {
        public static void AddArgonaught(this IServiceCollection services, IAudience audience) {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (audience == null)
                throw new ArgumentNullException(nameof(audience));

            var jwtBuilder = new Argonaught.Internal.JWTBuilder(audience);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => options.TokenValidationParameters = jwtBuilder.validationParameters.SecretAndExpirationDate());

        }
    }
}