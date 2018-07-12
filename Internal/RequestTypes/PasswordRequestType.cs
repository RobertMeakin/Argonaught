using Argonaut.Internal.RequestTypes.Interfaces;
using Argonaut.Internal.RequestTypes.RequestFormat;
using Argonaut.Internal.RequestTypes.RequestFormat.Interfaces;
using Argonaut.Internal.RequestTypes.RequestOrigin;
using Argonaut.Internal.RequestTypes.RequestOrigin.Interfaces;
using Argonaut.Internal.RequestTypes.RequestResponse;
using Argonaut.Internal.RequestTypes.RequestResponse.Interfaces;

namespace Argonaut.Internal.RequestTypes
{
    internal class PasswordRequestType : IRequestType
    {
        public IRequestTypeFormat Format => new PasswordRequestFormat();

        public IRequestTypeOrigin Origin => new PasswordRequestOrigin();

        public IRequestTypeReponse Response => new PasswordRequestResponse();
    }
}