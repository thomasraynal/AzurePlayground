﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace AzurePlayground.Persistence.Mongo
{
    public class IdentityUserClaim
    {
        public IdentityUserClaim()
        {
        }

        public IdentityUserClaim(Claim claim)
        {
            Type = claim.Type;
            Value = claim.Value;
        }

        public ClaimsIdentity Subject { get; set; }

        public string OriginalIssuer { get; set; }

        public string Issuer { get; set; }

        public string ValueType { get; }

        public string Type { get; set; }

        public string Value { get; set; }

        public Claim ToSecurityClaim()
        {
            return new Claim(Type, Value);
        }
    }
}
