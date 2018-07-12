using Argonaut.Internal.RequestTypes.RequestFormat.Interfaces;
using Argonaut.Internal.RequestTypes.RequestOrigin.Interfaces;
using Argonaut.Internal.RequestTypes.RequestResponse.Interfaces;

namespace Argonaut.Internal.RequestTypes.Interfaces
{
    internal interface IRequestType
    {
        IRequestTypeFormat Format { get; }

        IRequestTypeOrigin Origin { get; }

        IRequestTypeReponse Response { get; }
    }
    
}