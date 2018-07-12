using System;
using Argonaut.Internal;
using Argonaut.Internal.Middleware;
using Argonaut.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Argonaut {
    public static class ArgonautServicesExtensions {
        public static void AddArgonaut(this IServiceCollection services, IAudience audience) {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (audience == null)
                throw new ArgumentNullException(nameof(audience));

            var jwtBuilder = new Argonaut.Internal.JWTBuilder(audience);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => options.TokenValidationParameters = jwtBuilder.validationParameters.SecretAndExpirationDate());

        }
    }
}