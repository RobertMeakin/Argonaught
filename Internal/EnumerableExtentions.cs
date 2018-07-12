using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System;
using Argonaut.Model;

namespace Argonaut.Internal
{
    internal static class EnumerableExtentions
    {
        /// <summary>
        /// Excludes the claims that will be found in an existing access token that are not needed when a new access token is generated as these claims will be added afresh by default. What remains are the user's unique claims.
        /// </summary>
        public static IEnumerable<Claim> ExcludeDefaultAccessTokenClaims(this IEnumerable<Claim> sequence)
        {
            var excluded = new List<string> { "sub", "nbf", "exp", "iss", "aud", "jti" };
            return sequence.Where(l => !excluded.Contains(l.Type));
        }
    }
}