using System;
using ININ.PureCloud.OAuthControl;
using System.Configuration;
using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Okta.Samples.OpenIDConnect
{
    class Program
    {
        private static OpenIdConnectConfiguration _config;
        static string strOktaOrgDomain = string.Empty;
        static string strClientId = string.Empty;
        static string strRedirectUri = string.Empty;
        static string strScopes = string.Empty;
        static string strResponseType = string.Empty;

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                strOktaOrgDomain = ConfigurationManager.AppSettings["okta:OrganizationSubDomain"];
                strClientId = ConfigurationManager.AppSettings["okta:ClientId"];
                strRedirectUri = ConfigurationManager.AppSettings["okta:RedirectUri"];
                strScopes = ConfigurationManager.AppSettings["okta:Scopes"];
                strResponseType = ConfigurationManager.AppSettings["okta:ResponseType"];

                LoadOpenIdConnectConfigurationAsync().Wait();

                Console.WriteLine("Welcome to the Okta OpenID Connect Console sample.");
                Console.WriteLine("Please press Enter to sign in with Okta.");
                Console.ReadLine();

                // Create form
                var form = new OAuthWebBrowserForm("Okta Login");

                // Set settings
                form.oAuthWebBrowser1.Environment = strOktaOrgDomain;
                form.oAuthWebBrowser1.ClientId = strClientId;
                form.oAuthWebBrowser1.RedirectUri = strRedirectUri;
                form.oAuthWebBrowser1.Scopes = strScopes;
                form.oAuthWebBrowser1.ResponseType = strResponseType;
                form.oAuthWebBrowser1.RedirectUriIsFake = true;
                form.oAuthWebBrowser1.OpenIdConfig = _config;
                form.oAuthWebBrowser1.State = KeyGenerator.GetUniqueKey(32);
                form.oAuthWebBrowser1.Nonce = KeyGenerator.GetUniqueKey(32);

                // Open the web form in a window
                var result = form.ShowDialog(true);

                Console.WriteLine($"Result: {result}\r\n");
                Console.WriteLine($"ID Token: {form.oAuthWebBrowser1.IDToken}\r\n");
                Console.WriteLine($"Access Token: {form.oAuthWebBrowser1.AccessToken}\r\n");
                foreach (Claim c in form.oAuthWebBrowser1.IDClaims)
                    Console.WriteLine("{0}: {1}", c.Type, c.Value);
                Console.WriteLine("\r\nPress any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }


        internal static async Task<OpenIdConnectConfiguration> LoadOpenIdConnectConfigurationAsync()
        {
            if (_config == null)
            {
                var discoAddress = "https://" + strOktaOrgDomain + "/.well-known/openid-configuration";

                var manager = new ConfigurationManager<OpenIdConnectConfiguration>(discoAddress);

                _config = await manager.GetConfigurationAsync();
            }
            return _config;
        }
    }
}
