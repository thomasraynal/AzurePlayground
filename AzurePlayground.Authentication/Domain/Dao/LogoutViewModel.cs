using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzurePlayground.Authentication
{
    public class LogoutViewModel : LogoutRequest
    {
        public bool ShowLogoutPrompt { get; set; } = true;
    }
}
