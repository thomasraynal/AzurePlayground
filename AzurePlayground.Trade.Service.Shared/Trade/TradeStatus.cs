using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Shared
{
    public enum TradeStatus
    {
        None,
        Created,
        ComplianceCheck,
        Processed,
        Rejected
    }
}
