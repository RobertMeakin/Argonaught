using System.Threading.Tasks;
using Argonaut.Internal.RequestTypes.RequestResponse.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Argonaut.Internal.RequestTypes.RequestResponse
{
    internal class UnknownRequestResponse : IRequestTypeReponse
    {
        public Task Execute(RequestDelegate next, HttpContext context, ArgonautOptions options)
            => next(context);
    }
}