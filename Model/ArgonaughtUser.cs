using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Argonaught.Model
{
    public class ArgonaughtUser
    {
        public ArgonaughtUser(bool validated, IAudience audience)
        {
            if (audience == null)
                this.Validated = false;

            this.Validated = validated;
            this.Audience = audience;
        }

        public bool Validated { get; private set; }
        public IAudience Audience { get; private set; }
        public List<Claim> Claims { get; private set; } = new List<Claim>();

    }
}