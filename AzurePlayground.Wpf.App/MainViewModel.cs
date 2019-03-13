using AzurePlayground.Service.Shared;
using AzurePlayground.Wpf.App.Infrastructure;
using Dasein.Core.Lite.Shared;
using IdentityModel.OidcClient;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace AzurePlayground.Wpf.App
{
    public class MainViewModel : ViewModelBase
    {

        private OidcClient _oidcClient;
        private ObservableCollection<TradeViewModel> _trades;
        private ObservableCollection<string> _events;
        private LoginResult _token;
        private ITradeService _tradeService;

        private object _locker = new object();

        private CompositeDisposable _cleanup = new CompositeDisposable();

        private ISignalRService<Price, PriceRequest> _priceEventService;
        private ISignalRService<Trade, TradeEventRequest> _tradeEventService;


        public ICommand Login { get; private set; }
        public ICommand Logout { get; private set; }
        public ICommand MakeTrade { get; private set; }

        public bool IsLogin
        {
            get
            {
                return Token != null;
            }
        }

        public LoginResult Token
        {
            get
            {
                return _token;
            }
            set
            {
                _token = value;
                OnPropertyChanged(nameof(Token));
                OnPropertyChanged(nameof(IsLogin));
            }
        }

        public ObservableCollection<TradeViewModel> Trades
        {
            get
            {
                return _trades;
            }
            set
            {
                _trades = value;
                OnPropertyChanged(nameof(Trades));
            }
        }

        public ObservableCollection<String> Events
        {
            get
            {
                return _events;
            }
            set
            {
                _events = value;
                OnPropertyChanged(nameof(Events));
            }
        }

        public void Notify(string @event)
        {
            Application.Current.Dispatcher.Invoke(() => Events.Add(@event));
        }

        private String MakeEvent(String name, String sender, object subject, object arg)
        {
            return $"[{name}] - [{sender}] - [{subject} - {arg}]";
        }


        public MainViewModel()
        {
            Events = new ObservableCollection<string>();

            Trades = new ObservableCollection<TradeViewModel>();

            var configuration = AppCore.Instance.Get<AppConfiguration>();


            var refitSettings = new RefitSettings()
            {
                JsonSerializerSettings = AppCore.Instance.Get<JsonSerializerSettings>(),
                HttpMessageHandlerFactory = () => new HttpRetryForeverMessageHandler(5000)
            };

            _tradeService = ApiServiceBuilder<ITradeService>.Build(configuration.Gateway)
                                                .AddAuthorizationHeader(() =>
                                                {
                                                    return _token.AccessToken;
                                                })
                                                .Create(refitSettings: refitSettings);



            MakeTrade = new Command(async () =>
            {
                var asset = TradeServiceReferential.Assets.Random();
                var counterparty = TradeServiceReferential.Counterparties.Random();

                var rand = new Random();

                var request = new TradeCreationRequest()
                {
                    Asset = asset.Name,
                    Volume = rand.Next(1, 50),
                    Way = TradeWay.Buy
                };

                var tradeEvent = await _tradeService.CreateTrade(request);

                var trade = await _tradeService.GetTradeById(tradeEvent.TradeId);

                Notify($"{_token.User.Identity.Name} created trade {trade.EntityId} on desktop UI");

            });

            var options = new OidcClientOptions()
            {
                Authority =configuration.Identity,
                ClientId = AzurePlaygroundConstants.Auth.ClientOpenIdNative,
                Scope = "openid profile desk trade",
                RedirectUri = "http://127.0.0.1/sample-wpf-app",
                PostLogoutRedirectUri = "http://127.0.0.1/sample-wpf-app",
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.FormPost,
                Flow = OidcClientOptions.AuthenticationFlow.AuthorizationCode,
                Browser = new WpfEmbeddedBrowser()
            };

            _oidcClient = new OidcClient(options);

            Login = new Command(async () =>
            {
             
                try
                {
                    _token = await _oidcClient.LoginAsync(new IdentityModel.OidcClient.LoginRequest());
                }
                catch (Exception ex)
                {
                    Notify(ex.ToString());
                    return;
                }

                if (_token.IsError)
                {
                    var error = _token.Error == "User Cancellled" ? "The sign-in window was closed before authorization was completed." : _token.Error;
                    Notify(error);
                }
                else
                {
                    Token = _token;

                    Notify($"Logged in as { _token.User.Identity.Name}");

                    _priceEventService = SignalRServiceBuilder<Price, PriceRequest>
                                .Create()
                                .Build(new PriceRequest((p) => true), (opts) =>
                                {
                                    opts.AccessTokenProvider = () => Task.FromResult(_token.AccessToken);
                                });

                    var priceServiceConnection = _priceEventService
                              .Connect(Scheduler.Default, 1000)
                              .Subscribe((priceEvent) =>
                              {
                                  lock (_locker)
                                  {
                                      if (null == Application.Current.Dispatcher) return;

                                      foreach (var trade in Trades.Where(trade => trade.Asset == priceEvent.Asset).ToList())
                                      {
                                          Application.Current.Dispatcher.Invoke(() => trade.CurrentPrice = priceEvent.Value);
                                      }

                                  }

                                  var ev = MakeEvent("PRICE", "PRICE", priceEvent.Asset, priceEvent.Value);

                                  this.Notify(ev);
                              });


                    _tradeEventService = SignalRServiceBuilder<Trade, TradeEventRequest>
                                                                .Create()
                                                                .Build(new TradeEventRequest((p) => true), (opts) =>
                                                                 {
                                                                     opts.AccessTokenProvider = () => Task.FromResult(_token.AccessToken);
                                                                 });


                    var tradeEventServiceConnection = _tradeEventService
                        .Connect(Scheduler.Default, 500)
                        .Subscribe((tradeEvent) =>
                        { 

                            lock (_locker)
                            {

                                if (null == Application.Current.Dispatcher) return;

                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    var trade = Trades.FirstOrDefault(t => t.Id == tradeEvent.EntityId);

                                    if (null == trade)
                                    {
                                        Trades.Add(new TradeViewModel(tradeEvent));
                                    }
                                    else
                                    {
                                        trade.ApplyChange(tradeEvent);
                                    }

                                });

                            }

                            this.Notify(MakeEvent("TRADE", _tradeEventService.Current.Endpoint, tradeEvent.EntityId, tradeEvent.EntityId));

                        });



                    _cleanup.Add(priceServiceConnection);
                    //_cleanup.Add(tradeEventServiceConnection);

                    if (null == Application.Current.Dispatcher) return;

                    await Application.Current.Dispatcher.Invoke(async () =>
                    {
                        var trades = await _tradeService.GetAllTrades();

                        foreach (var trade in trades)
                        {
                            var vm = new TradeViewModel(trade);
                            Trades.Add(vm);
                        }

                        Notify($"Fetched {trades.Count()} trade(s)");

                    });

                }

            }, () => Token == null);


            Dispatcher.CurrentDispatcher.ShutdownStarted += (s,e)=>
            {
                _cleanup.Dispose();
            };

            //Logout = new Command(async() =>
            //{
               
            //    try
            //    {
            //        var request = new LogoutRequest
            //        {
            //            IdTokenHint = Token.IdentityToken
            //        };

            //        await _oidcClient.LogoutAsync(request);

            //        Token = null;

            //        Notify($"Logged out");

                 
            //    }
            //    catch (Exception ex)
            //    {
            //        Notify(ex.ToString());
            //        return;
            //    }
            //});



        }

    
    }
}
