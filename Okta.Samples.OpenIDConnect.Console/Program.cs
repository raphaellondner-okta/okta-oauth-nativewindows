using System;
using ININ.PureCloud.OAuthControl;
using System.Configuration;

namespace Okta.Samples.OpenIDConnect
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                // Create form
                var form = new OAuthWebBrowserForm("Okta Login");

                // Set settings
                form.oAuthWebBrowser1.Environment = ConfigurationManager.AppSettings["okta:OrganizationSubDomain"];
                form.oAuthWebBrowser1.ClientId = ConfigurationManager.AppSettings["okta:ClientId"];
                form.oAuthWebBrowser1.RedirectUri = ConfigurationManager.AppSettings["okta:RedirectUri"];
                form.oAuthWebBrowser1.Scopes = ConfigurationManager.AppSettings["okta:Scopes"]; ;
                form.oAuthWebBrowser1.ResponseType = ConfigurationManager.AppSettings["okta:ResponseType"]; ;
                form.oAuthWebBrowser1.RedirectUriIsFake = true;

                // Open it
                var result = form.ShowDialog(true);

                Console.WriteLine($"Result: {result}");
                Console.WriteLine($"ID Token: {form.oAuthWebBrowser1.IDToken}");
                Console.WriteLine($"Access Token: {form.oAuthWebBrowser1.AccessToken}");

                Console.WriteLine("Application complete.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
