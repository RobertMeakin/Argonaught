using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Argonaut.Internal.RequestTypes.RequestResponse.Interfaces
{
    internal interface IRequestTypeReponse
    {
        Task Execute(RequestDelegate next, HttpContext context, ArgonautOptions options);
    }
}