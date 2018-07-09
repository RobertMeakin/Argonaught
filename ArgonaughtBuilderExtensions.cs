using System;
using System.Collections;
using System.Collections.Generic;
using Argonaught.Internal;
using Argonaught.Internal.Middleware;
using Argonaught.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace Argonaught {
    public static class ArgonaughtBuilderExtensions {

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