using Argonaught.Internal.RequestTypes.Interfaces;
using Argonaught.Internal.RequestTypes.RequestFormat;
using Argonaught.Internal.RequestTypes.RequestFormat.Interfaces;
using Argonaught.Internal.RequestTypes.RequestOrigin;
using Argonaught.Internal.RequestTypes.RequestOrigin.Interfaces;
using Argonaught.Internal.RequestTypes.RequestResponse;
using Argonaught.Internal.RequestTypes.RequestResponse.Interfaces;

namespace Argonaught.Internal.RequestTypes
{
    internal class UnknownRequestType : IRequestType
    {
        public IRequestTypeFormat Format => new UnknownRequestFormat();

        public IRequestTypeOrigin Origin => new UnknownRequestOrigin();

        public IRequestTypeReponse Response => new UnknownRequestResponse();
    }
}