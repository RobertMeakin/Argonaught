using Argonaut.Internal.RequestTypes.RequestOrigin.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Argonaut.Internal.RequestTypes.RequestOrigin
{
    internal class UnknownRequestOrigin : IRequestTypeOrigin
    {
        public void Validate(HttpContext context, ArgonautOptions options) { /*Do nothing*/ }
    }
}