using Microsoft.AspNetCore.Http;

namespace Argonaut.Internal.RequestTypes.RequestFormat.Interfaces
{
    internal interface IRequestTypeFormat
    {
        string FindErrors(HttpContext context, ArgonautOptions options);
    }
}