namespace Argonaught.Model
{
    public interface IAudience
    {
        /// <summary>
        /// The name of the audience. The access token will need to match this name to pass validation.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The name of the issuer. The access token will need to match this name to pass validation.
        /// </summary>
        string Issuer { get; }

        /// <summary>
        /// Used to authenticate the access token to prevent tampering. Keep this as hidden as possible.
        /// </summary>
        string Secret { get; }

        /// <summary>
        /// The refresh token can be used to generate another access token without the user needing to login again. The duration of this feature can be set here.
        /// </summary>
        bool Active { get;  }

        /// <summary>
        /// The refresh token can be used to generate another access token without the user needing to login again. The duration of this feature can be set here.
        /// </summary>
        int RefreshTokenLifetimeMinutes { get; }

        /// <summary>
        ///  Comma separated. Use an asterisk (*) to allow any origin (which may be required for an API where the end consumer cannot be predicted). Otherwise, only the specified origins will receive the Access-Control-Allow-Origin header.
        /// </summary>
        string AllowedOrigins { get; }
    }
}