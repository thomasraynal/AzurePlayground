﻿using IdentityModel.OidcClient.Browser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using mshtml;

namespace AzurePlayground.Wpf.App
{
    //https://github.com/IdentityModel/IdentityModel.OidcClient.Samples/blob/master/Wpf/WpfEmbeddedBrowser.cs
    public class WpfEmbeddedBrowser : IBrowser
    {
        private BrowserOptions _options = null;

        public WpfEmbeddedBrowser()
        {

        }

        public async Task<BrowserResult> InvokeAsync(BrowserOptions options)
        {
            _options = options;

            var window = new Window()
            {
                Width = 900,
                Height = 625,
                Title = "Login"
            };

            // Note: Unfortunately, WebBrowser is very limited and does not give sufficient information for 
            //   robust error handling. The alternative is to use a system browser or third party embedded
            //   library (which tend to balloon the size of your application and are complicated).
            var webBrowser = new WebBrowser();

            var signal = new SemaphoreSlim(0, 1);

            var result = new BrowserResult()
            {
                ResultType = BrowserResultType.UserCancel
            };

            webBrowser.Navigating += (s, e) =>
            {
                if (BrowserIsNavigatingToRedirectUri(e.Uri))
                {
                    e.Cancel = true;

                    var responseData = GetResponseDataFromFormPostPage(webBrowser);

                    result = new BrowserResult()
                    {
                        ResultType = BrowserResultType.Success,
                        Response = responseData
                    };

                    signal.Release();

                    window.Close();
                }
            };

            window.Closing += (s, e) =>
            {
                signal.Release();
            };

            window.Content = webBrowser;
            window.Show();
            webBrowser.Source = new Uri(_options.StartUrl);

            await signal.WaitAsync();

            return result;
        }

        private bool BrowserIsNavigatingToRedirectUri(Uri uri)
        {
            return uri.AbsoluteUri.StartsWith(_options.EndUrl);
        }

        private string GetResponseDataFromFormPostPage(WebBrowser webBrowser)
        {
            var document = (IHTMLDocument3)webBrowser.Document;
            var inputElements = document.getElementsByTagName("INPUT").OfType<IHTMLElement>();
            var resultUrl = "?";

            foreach (var input in inputElements)
            {
                resultUrl += input.getAttribute("name") + "=";
                resultUrl += input.getAttribute("value") + "&";
            }

            resultUrl = resultUrl.TrimEnd('&');

            return resultUrl;
        }
    }
}
