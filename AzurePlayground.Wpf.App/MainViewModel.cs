using AzurePlayground.Wpf.App.Infrastructure;
using IdentityModel.OidcClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AzurePlayground.Wpf.App
{
    public class MainViewModel : ViewModelBase
    {
        private string _result;
        private OidcClient _oidcClient;
        private LoginResult _user;

        public MainViewModel()
        {
            var options = new OidcClientOptions()
            {
                //redirect to identity server
                Authority = "http://localhost:5001",
                ClientId = "native.code",
                Scope = "openid profile office",
                //redirect back to app if auth success
                RedirectUri = "http://127.0.0.1/sample-wpf-app",
                PostLogoutRedirectUri = "http://127.0.0.1/sample-wpf-app",
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.FormPost,
                Flow = OidcClientOptions.AuthenticationFlow.AuthorizationCode,
                Browser = new WpfEmbeddedBrowser()
            };

            _oidcClient = new OidcClient(options);

            Login = new Command(async () =>
            {
                LoginResult result;
                try
                {
                    result = await _oidcClient.LoginAsync(new LoginRequest());
                }
                catch (Exception ex)
                {
                    Result = $"Unexpected Error: {ex.Message}";
                    return;
                }

                if (result.IsError)
                {
                    Result = result.Error == "UserCancel" ? "The sign-in window was closed before authorization was completed." : result.Error;
                }
                else
                {
                    User = result;
                    var name = result.User.Identity.Name;
                    Result = $"Hello {name}";
                }

            }, () => User == null);

            Logout = new Command(async() =>
            {
               
                try
                {
                    var request = new LogoutRequest
                    {
                        IdTokenHint = User.IdentityToken
                    };

                    await _oidcClient.LogoutAsync(request);

                    User = null;
                    Result = "Logged out";
                }
                catch (Exception ex)
                {
                    Result = $"Unexpected Error: {ex.Message}";
                    return;
                }
            });
        }


        public String Result
        {
            get
            {
                return _result;
            }
            set
            {
                _result = value;
                OnPropertyChanged(nameof(Result));
            }
        }

        public LoginResult User
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
                OnPropertyChanged(nameof(User));
            }
        }

        public ICommand Login { get; private set; }
        public ICommand Logout { get; private set; }


    }
}
