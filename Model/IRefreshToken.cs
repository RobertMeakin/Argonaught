using System;

namespace Argonaut.Model
{

    public interface IRefreshToken
    {
        /// <summary>
        /// The primary key. Created by Argonaut. This is the hashed refresh token.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The sub of the access token. This will be the username passed in by the user when logging in and can be used to revoke refresh tokens for users.
        /// </summary>
        string Subject { get; }

        /// <summary>
        /// Foreign key to the Audience object.
        /// </summary>
        string AudienceId { get; }

        /// <summary>
        /// Universal time of issue.
        /// </summary>
        DateTime IssuedUtc { get; }

        /// <summary>
        /// Universal time of expiry.
        /// </summary>
        DateTime ExpiresUtc { get; }

        /// <summary>
        /// The encrypted access token containing the claims for the user. This is decrypted and used to regenerate the claims for the new access token the user is requesting.
        /// </summary>
        string ProtectedTicket { get; }
    }

}