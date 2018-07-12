using System;
using System.Collections;
using System.Collections.Generic;
using Argonaut.Internal;
using Argonaut.Internal.Middleware;
using Argonaut.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace Argonaut {
    public static class ArgonautBuilderExtensions {

        public static IApplicationBuilder UseArgonaut(this IApplicationBuilder app, ArgonautOptions argonautOptions) {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            if (argonautOptions == null)
                throw new ArgumentNullException(nameof(argonautOptions));

            app.UseMiddleware<PreflightPolicy>(argonautOptions.APIPath);
            app.UseMiddleware<FindRequestType>(Options.Create(argonautOptions));
            app.UseMiddleware<RequestFormatPolicy>(Options.Create(argonautOptions));
            app.UseMiddleware<OriginPolicy>(Options.Create(argonautOptions));
            app.UseMiddleware<ResponsePolicy>(Options.Create(argonautOptions));
            app.UseAuthentication();
            
            return app;
        }

    }
}