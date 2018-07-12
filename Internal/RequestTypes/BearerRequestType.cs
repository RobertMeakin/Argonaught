using Argonaut.Internal.RequestTypes.Interfaces;
using Argonaut.Internal.RequestTypes.RequestFormat;
using Argonaut.Internal.RequestTypes.RequestFormat.Interfaces;
using Argonaut.Internal.RequestTypes.RequestOrigin;
using Argonaut.Internal.RequestTypes.RequestOrigin.Interfaces;
using Argonaut.Internal.RequestTypes.RequestResponse;
using Argonaut.Internal.RequestTypes.RequestResponse.Interfaces;

namespace Argonaut.Internal.RequestTypes
{
    internal class BearerRequestType : IRequestType
    {
        public IRequestTypeFormat Format => new BearerRequestFormat();

        public IRequestTypeOrigin Origin => new BearerRequestOrigin();

        public IRequestTypeReponse Response => new BearerRequestResponse();

    }
}