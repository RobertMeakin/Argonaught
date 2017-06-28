using System;
using Argonaught.Model;

namespace Argonaught.Internal.Model
{

    internal class RefreshToken : IRefreshToken
    {
        public RefreshToken(string id, string subject, string audienceId, DateTime issuedUtc, DateTime expiresUtc, string protectedTicket)
        {
            this.Id = id;
            this.Subject = subject;
            this.AudienceId = audienceId;
            this.IssuedUtc = issuedUtc;
            this.ExpiresUtc = expiresUtc;
            this.ProtectedTicket = protectedTicket;
        }

        public string Id { get; private set; }

        public string Subject { get; private set; }

        public string AudienceId { get; private set; }

        public DateTime IssuedUtc { get; private set; }

        public DateTime ExpiresUtc { get; private set; }

        public string ProtectedTicket { get; private set; }

    }
}