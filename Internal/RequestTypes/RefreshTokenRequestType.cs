using Argonaut.Internal.RequestTypes.Interfaces;
using Argonaut.Internal.RequestTypes.RequestFormat;
using Argonaut.Internal.RequestTypes.RequestFormat.Interfaces;
using Argonaut.Internal.RequestTypes.RequestOrigin;
using Argonaut.Internal.RequestTypes.RequestOrigin.Interfaces;
using Argonaut.Internal.RequestTypes.RequestResponse;
using Argonaut.Internal.RequestTypes.RequestResponse.Interfaces;

namespace Argonaut.Internal.RequestTypes
{
    internal class RefreshTokenRequestType : IRequestType
    {
        public IRequestTypeFormat Format => new RefreshTokenRequestFormat();

        public IRequestTypeOrigin Origin => new RefreshTokenRequestOrigin();

        public IRequestTypeReponse Response => new RefreshTokenRequestResponse();

    }
}