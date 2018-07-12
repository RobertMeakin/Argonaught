using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Argonaut.Authentication.Authorization {

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeClaimAttribute : TypeFilterAttribute {
        public AuthorizeClaimAttribute(string claimType, string claimValue) : base(typeof(AuthorizeClaimFilter)) {
            Arguments = new object[] { new Claim(claimType, claimValue) };
        }
    }

    internal class AuthorizeClaimFilter : IAuthorizationFilter {
        readonly Claim _claim;

        public AuthorizeClaimFilter(Claim claim) {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context) {
            var authenticated = context.HttpContext.User.Identities.FirstOrDefault(x => x.IsAuthenticated == true) != null ? true : false;
            if (!authenticated) {
                context.Result = new UnauthorizedResult();
                return;
            }

            var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == _claim.Type && c.Value == _claim.Value);
            if (!hasClaim) {
                context.Result = new ForbidResult();
                return;
            }
        }
    }

}