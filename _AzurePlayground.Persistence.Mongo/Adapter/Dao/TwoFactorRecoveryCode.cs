using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Persistence.Mongo
{
    public class TwoFactorRecoveryCode
    {
        public string Code { get; set; }

        public bool Redeemed { get; set; }
    }
}
