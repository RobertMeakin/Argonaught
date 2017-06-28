using Microsoft.AspNetCore.Http;

namespace Argonaught.Internal.RequestTypes.RequestFormat.Interfaces
{
    internal interface IRequestTypeFormat
    {
        string FindErrors(HttpContext context, ArgonautOptions options);
    }
}