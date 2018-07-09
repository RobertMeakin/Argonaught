using System.Linq;
using Argonaught.Internal.RequestTypes;
using Argonaught.Internal.RequestTypes.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Argonaught.Internal {
    internal static class HttpContextExtentions {
        internal static IRequestType GetArgonaughtRequestType(this HttpContext context, string apiPath, bool includeAuthGeneration) {

            if (context.Request.Path.ToString().ToLower().Contains(apiPath.ToLower()))
                return new BearerRequestType();

            if (!context.Request.HasFormContentType)
                return new UnknownRequestType();

            if (!includeAuthGeneration)
                return new UnknownRequestType();

            IRequestType requestType = null;

            if (context.Request.Form.Keys.Contains("grant_type")) {
                var pascalGrantType =
                    (from x in context.Request.Form["grant_type"].ToString().Split('_') let first_char = char.ToUpper(x[0])
                        let rest_chars = new string(x.Skip(1).Select(c => char.ToLower(c)).ToArray())
                        select first_char + rest_chars).Aggregate((x, y) => x + "" + y);

                requestType = (from at in ArgonautRequestTypes.GetTypes() let t = at.GetType()
                    where t.Name.Substring(0, t.Name.Replace("RequestType", "").Length).Equals(pascalGrantType) select at).FirstOrDefault();
            }

            if (requestType != null)
                return requestType;
            else
                return new UnknownRequestType();

        }

    }
}