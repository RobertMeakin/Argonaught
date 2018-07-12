using System;
using System.Security.Cryptography;
using Argonaut.Internal.Interfaces;

namespace Argonaut.Internal.DomainObjects {
    internal class RefreshToken {
        private string _token = "";
        public string Id { get; private set; }
        public string Subject { get; private set; }
        public string AudienceId { get; private set; }
        public DateTime IssuedUtc { get; private set; }
        public DateTime ExpiresUtc { get; private set; }
        public string ProtectedTicket { get; private set; } = "";

        private RefreshToken() { }

        private RefreshToken(string token, string subject, string audienceId, DateTime issuedUtc, DateTime expiresUtc, string protectedTicket) {
            _token = token;
            this.Id = Argonaut.Internal.Hashing.GetHash(token);
            this.Subject = subject;
            this.AudienceId = audienceId;
            this.IssuedUtc = issuedUtc;
            this.ExpiresUtc = expiresUtc;
            this.ProtectedTicket = protectedTicket;
        }

        public override string ToString() {
            return String.Format(@"RefreshToken: Id {0}, Subject {1}, AudienceId {2}, IssuedUtc {3}, ExpiresUtc {4}, 
            ProtectedTicket {5}", Id, Subject, AudienceId, IssuedUtc, ExpiresUtc, ProtectedTicket);
        }

        public static RefreshToken New(string subject, string audienceId, DateTime issuedUtc, string protectedTicket, int lifeTimeMinutes) {
            var token = Guid.NewGuid().ToString("N");
            var expirationUtc = issuedUtc.Add(TimeSpan.FromMinutes(lifeTimeMinutes));
            var refreshToken = new RefreshToken(token, subject, audienceId, issuedUtc, expirationUtc, protectedTicket);
            return refreshToken;
        }

        public static RefreshToken New(string token, string subject, string audienceId, DateTime issuedUtc, DateTime expiresUtc, string protectedTicket) {
            var rt = new RefreshToken();
            rt.SetToken(token);
            rt.Id = Argonaut.Internal.Hashing.GetHash(token);
            rt.Subject = subject;
            rt.AudienceId = audienceId;
            rt.IssuedUtc = issuedUtc;
            rt.ExpiresUtc = expiresUtc;
            rt.ProtectedTicket = protectedTicket;
            return rt;
        }
        public static RefreshToken New(string Id, string subject, string audienceId, string protectedTicket, DateTime issuedUtc, DateTime expiresUtc) {
            var rt = new RefreshToken();
            rt.Id = Id;
            rt.Subject = subject;
            rt.AudienceId = audienceId;
            rt.IssuedUtc = issuedUtc;
            rt.ExpiresUtc = expiresUtc;
            rt.ProtectedTicket = protectedTicket;
            return rt;
        }

        /// <summary>
        /// Uses the refresh token that will only be retained by the client to encrypt the ticket.
        /// </summary>
        internal void EncryptTicket(IEncryptor encryptor) {
            if (this.GetToken() == "")
                return;

            this.ProtectedTicket = encryptor.Encrypt(this.ProtectedTicket, this.GetToken());
        }

        internal void DecryptTicket(IEncryptor encryptor, string originalRefreshToken) {
            if (this.ProtectedTicket == "")
                return;

            this.ProtectedTicket = encryptor.Decrypt(this.ProtectedTicket, originalRefreshToken);
        }
        
        public string GetToken() {
            return _token;
        }

        private void SetToken(string token) {
            _token = token;
        }

    }
}