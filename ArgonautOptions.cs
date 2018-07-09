using System;
using System.Collections.Generic;
using Argonaught.Model;

namespace Argonaught {
    public class ArgonautOptions {
        public ArgonautOptions(IEnumerable<IAudience> audiences, Func<string, string, string, ArgonaughtUser> validateUser, Func<string, RefreshResponse> refreshAccessToken, Action<IRefreshToken> refreshTokenGenerated) {
            var myName = nameof(ArgonautOptions);

            if (audiences == null)
                throw new ArgumentNullException(myName);

            if (validateUser == null)
                throw new ArgumentNullException(myName + "." + nameof(validateUser));

            if (refreshAccessToken == null)
                throw new ArgumentNullException(myName + "." + nameof(refreshAccessToken));

            if (refreshTokenGenerated == null)
                throw new ArgumentNullException(myName + "." + nameof(refreshTokenGenerated));

            this.Audiences = audiences;
            this.ValidateUser = validateUser;
            this.RefreshAccessToken = refreshAccessToken;
            this.RefreshTokenGenerated = refreshTokenGenerated;
            this.IncludeAuthGeneration = true;
        }

        public ArgonautOptions(IEnumerable<IAudience> audiences) {
            var myName = nameof(ArgonautOptions);

            if (audiences == null)
                throw new ArgumentNullException(myName);

            this.Audiences = audiences;
            this.IncludeAuthGeneration = false;
        }

        public ArgonautOptions() =>
            throw new ArgumentNullException("Parameterless constructor not allowed for " + nameof(ArgonautOptions) + ". Exists only to allow class to be used as options.");

        public IEnumerable<IAudience> Audiences { get; private set; }

        /// <summary>
        /// Called by Argonaught when a user attempts to login by passing a username, password and audience.
        /// This function should check with the database that the user is allowed access to the requested audience and pass back the user's claims, along with the full audience object.
        /// </summary>
        public Func<string, string, string, ArgonaughtUser> ValidateUser { get; private set; }

        /// <summary>
        /// This function is called when the user requests a new access token using a refresh token.
        /// It receives a refreshTokenId. Use it to find the full refresh token and the associated audience object in the db and pass these back in a RefreshReponse object.
        /// Argonaught will then take care to validate that it hasn't expired and generate a new access token for the user. 
        /// If this function returns null it is assumed a refresh token couldn't be found and the user will receive an Unauthorized 401 response.
        /// </summary>
        /// /// <returns>RefreshResponse</returns>
        public Func<string, RefreshResponse> RefreshAccessToken { get; private set; }

        /// <summary>
        /// Called whenever a refesh token has been generated. Use to store in the database for later validation.
        /// It is recommended to delete any existing relevant tokens for the user before saving the new one. 
        /// </summary>
        public Action<IRefreshToken> RefreshTokenGenerated { get; private set; }

        /// <summary>
        /// Defaults to five minutes.
        /// </summary>
        public int AccessTokenLifetimeMinutes { get; set; } = 5;

        /// <summary>
        /// Defaults to false. If true, Argonaught will create endpoints for creating access and refresh tokens, Otherwise it will only handle authorising existing access tokens.
        /// </summary>
        public bool IncludeAuthGeneration { get; private set; }

        /// <summary>
        /// Defaults to '[root]/token'. The path used to access the api token end point. 
        /// </summary>
        public string TokenPath { get; set; } = "/token";

        /// <summary>
        /// Defaults to 'api/'. This value needs to be found in the url in order for preflight get requests to succeed. Only change if your api will not include 'api/' in the url.
        /// </summary>
        public string APIPath { get; set; } = "api/";

        /// <summary>
        /// Defaults to false. Set to true if the access token should include a plain English representation of the claims. Useful for debugging.
        /// </summary>
        public bool VisualiseClaims { get; set; } = false;

    }
}