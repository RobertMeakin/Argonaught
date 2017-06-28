using System.Collections.Generic;
using Argonaught.Internal.RequestTypes.Interfaces;

namespace Argonaught.Internal.RequestTypes
{
    internal static class ArgonautRequestTypes
    {
        internal static IEnumerable<IRequestType> GetTypes()
        {
            var types = new List<IRequestType>();
            types.Add(new BearerRequestType());
            types.Add(new PasswordRequestType());
            types.Add(new RefreshTokenRequestType());
            types.Add(new UnknownRequestType());

            return types;
        }
    }
}