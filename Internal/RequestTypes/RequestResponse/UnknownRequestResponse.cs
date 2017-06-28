using System.Threading.Tasks;
using Argonaught.Internal.RequestTypes.RequestResponse.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Argonaught.Internal.RequestTypes.RequestResponse
{
    internal class UnknownRequestResponse : IRequestTypeReponse
    {
        public Task Execute(RequestDelegate next, HttpContext context, ArgonautOptions options)
            => next(context);
    }
}