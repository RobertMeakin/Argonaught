using Argonaught.Internal.RequestTypes.Interfaces;
using Argonaught.Internal.RequestTypes.RequestFormat;
using Argonaught.Internal.RequestTypes.RequestFormat.Interfaces;
using Argonaught.Internal.RequestTypes.RequestOrigin;
using Argonaught.Internal.RequestTypes.RequestOrigin.Interfaces;
using Argonaught.Internal.RequestTypes.RequestResponse;
using Argonaught.Internal.RequestTypes.RequestResponse.Interfaces;

namespace Argonaught.Internal.RequestTypes
{
    internal class RefreshTokenRequestType : IRequestType
    {
        public IRequestTypeFormat Format => new RefreshTokenRequestFormat();

        public IRequestTypeOrigin Origin => new RefreshTokenRequestOrigin();

        public IRequestTypeReponse Response => new RefreshTokenRequestResponse();

    }
}