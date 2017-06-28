using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Argonaught.Model;
using Microsoft.IdentityModel.Tokens;
using Domain = Argonaught.Internal.DomainObjects;

namespace Argonaught.Internal
{
    //The purpose of this class is to take the required parameters and generate a JSON Web Token (JWT).
    internal class JWTBuilder
    {
        private List<Claim> _standardClaims;

        private List<Claim> _additionalClaims;

        private JwtSecurityToken _jwt;

        private Domain.RefreshToken _refreshToken;

        private IAudience _audience;

        public JWTBuilder(IAudience audience)
        {
            if(audience == null)
                throw new ArgumentNullException(nameof(IAudience));

            _audience = audience;

            this.validationParameters = new JWTValidationParametersGenerator(audience);
            this._standardClaims = new List<Claim>();
            this._additionalClaims = new List<Claim>();
        }

        static public JWTBuilder New (IAudience options)
        {
            var builder = new JWTBuilder(options);
            return builder;
        }

        /// <summary>
        /// The subject of the jwt.  Normally, the username. Required in order to build a jwt.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Defaults to 60 minutes.
        /// </summary>
        public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(60);


        public SigningCredentials GetSigningCredentials()
        {
            var signingKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(_audience.Secret));
            return new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        }

        public void AddClaim(string type, string value)
        {
            _additionalClaims.Add(new Claim(type, value));
        }
        public void AddClaim(Claim claim)
        {
            _additionalClaims.Add(claim);
        }

        public void AddClaims(IEnumerable<Claim> claims)
        {
            _additionalClaims.AddRange(claims);
        }



        public void AddClaim_Role(string roleName)
        {
            _additionalClaims.Add(new Claim(ClaimTypes.Role, roleName));
        }

        public void AddClaim_UserName(string userName)
        {
            _additionalClaims.Add(new Claim(JwtRegisteredClaimNames.Sub, userName));
        }


        public void Build()
        {
            if(this.Subject == null || this.Subject == "")
                throw new ArgumentNullException("Subject must have a value");

            var now = DateTime.UtcNow;
            
            _jwt = new JwtSecurityToken
                (
                    issuer: _audience.Issuer,
                    audience: _audience.Id,
                    notBefore: now,
                    expires: now.Add(this.Expiration),
                    signingCredentials: this.GetSigningCredentials(),
                    claims: this.GetClaims(now)
                );

            //TODO:
            //Encrypt refresh token protected ticket here:
            //
            _refreshToken = Domain.RefreshToken.New(this.Subject, _audience.Id, now, GetAccessTokenString(), _audience.RefreshTokenLifetimeMinutes);
        }

        public string GetAccessTokenString()
        {
            if(_jwt != null)
                return new JwtSecurityTokenHandler().WriteToken(_jwt);
            else
                return "0";
        }

        public Domain.RefreshToken GetRefreshToken()
        {
            return _refreshToken;
        }

        public Dictionary<string, object> GetWebResponse()
        {
            if(_jwt == null)
                throw new ArgumentNullException("Has not been successfully built.");
            
            var accessToken = this.GetAccessTokenString();
            
             var resp = new Dictionary<string, object>();
             resp.Add("access_token", accessToken);
             resp.Add("refresh_token", _refreshToken.GetToken());
             resp.Add("sub", this.Subject);
             resp.Add("expires_seconds", Expiration.TotalSeconds);

             foreach(var r in _additionalClaims)
             {
                 resp.Add(r.Type, r.Value);
             }

            return resp;

        }


        public string NewAccessToken(DateTime now)
        {
            var jwt = new JwtSecurityToken
                (
                    issuer: _audience.Issuer,
                    audience: _audience.Id,
                    notBefore: now,
                    expires: now.Add(this.Expiration),
                    signingCredentials: this.GetSigningCredentials(),
                    claims: this.GetClaims(now)
                );
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public JWTValidationParametersGenerator validationParameters { get; private set; }


        //Private Methods
        private List<Claim> GetStandardClaims(DateTime now)
        {
            return new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //token id - optional
            };
        }

        private List<Claim> GetClaims(DateTime now)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, this.Subject));

            claims.AddRange(this.GetStandardClaims(now));
            claims.AddRange(_additionalClaims);
            
            return claims;
        }
    }
}