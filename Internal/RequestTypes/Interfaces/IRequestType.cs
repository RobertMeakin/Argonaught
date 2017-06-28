using Argonaught.Internal.RequestTypes.RequestFormat.Interfaces;
using Argonaught.Internal.RequestTypes.RequestOrigin.Interfaces;
using Argonaught.Internal.RequestTypes.RequestResponse.Interfaces;

namespace Argonaught.Internal.RequestTypes.Interfaces
{
    internal interface IRequestType
    {
        IRequestTypeFormat Format { get; }

        IRequestTypeOrigin Origin { get; }

        IRequestTypeReponse Response { get; }
    }
    
}