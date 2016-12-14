/*!
 * Copyright (c) 2016, Okta, Inc. and/or its affiliates. All rights reserved.
 * The Okta software accompanied by this notice is provided pursuant to the Apache License, Version 2.0 (the "License.")
 *
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *
 * See the License for the specific language governing permissions and limitations under the License.
 */

using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Configuration;

using Microsoft.IdentityModel.Protocols;
using IdentityModel.Client;
using System.Security.Claims;
using IdentityModel;

namespace Okta.Samples.OpenIDConnect
{
    class Program
    {
        private static OpenIdConnectConfiguration _config;
        private static string strOktaOrgUrl = string.Empty;
        static string strClientId = string.Empty;
        static string strRedirectUri = string.Empty;
        static string strScopes = string.Empty;
        static string strResponseType = string.Empty;

        static void Main(string[] args)
        {
            Console.WriteLine("+-----------------------+");
            Console.WriteLine("|  Sign in with Okta  |");
            Console.WriteLine("+-----------------------+");
            Console.WriteLine("");
            Console.WriteLine("Press any key to sign in...");
            Console.ReadKey();

            strOktaOrgUrl = ConfigurationManager.AppSettings["okta:OrganizationUrl"];
            strClientId = ConfigurationManager.AppSettings["okta:ClientId"];
            strRedirectUri = ConfigurationManager.AppSettings["okta:RedirectUri"];
            strScopes = ConfigurationManager.AppSettings["okta:Scopes"];
            strResponseType = ConfigurationManager.AppSettings["okta:ResponseType"];

            LoadOpenIdConnectConfigurationAsync().Wait();

            Program p = new Program();
            p.doOAuth();

            Console.ReadKey();
        }


        internal static async Task<OpenIdConnectConfiguration> LoadOpenIdConnectConfigurationAsync()
        {
            if (_config == null)
            {
                var discoAddress = strOktaOrgUrl + "/.well-known/openid-configuration";

                var manager = new ConfigurationManager<OpenIdConnectConfiguration>(discoAddress);

                _config = await manager.GetConfigurationAsync();
            }
            return _config;
        }

        private async void doOAuth()
        {
            // Generates state and PKCE values.
            string state = randomDataBase64url(32);
            string nonce = randomDataBase64url(32);
            string code_verifier = randomDataBase64url(32);
            string code_challenge = base64urlencodeNoPadding(sha256(code_verifier));

            output("redirect URI: " + strRedirectUri);

            // Creates an HttpListener to listen for requests on that redirect URI.
            var http = new HttpListener();
            http.Prefixes.Add(strRedirectUri);
            output("Listening..");
            http.Start();

            // Creates the OAuth 2.0 authorization request

            // Manual way of creating the authorization request
            // Defaulting to using the IdentityModel.AuthorizationRequest class instead
            //string authorizationRequest = $"{_config.AuthorizationEndpoint}?response_type={strResponseType}&scope={strScopes}&redirect_uri={System.Uri.EscapeDataString(redirectURI)}&client_id={strClientId}&state={state}&nonce={nonce}&code_challenge={code_challenge}&code_challenge_method=S256";

            var request = new AuthorizeRequest(_config.AuthorizationEndpoint);
            string authorizationRequest = request.CreateAuthorizeUrl(
                clientId: strClientId,
                responseType: OidcConstants.ResponseTypes.Code,
                scope: strScopes,
                redirectUri: Program.strRedirectUri,
                nonce: nonce,
                state: state,
                codeChallenge: code_challenge,
                codeChallengeMethod: OidcConstants.CodeChallengeMethods.Sha256);

            // Opens request in the browser.
            System.Diagnostics.Process.Start(authorizationRequest);

            // Waits for the OAuth authorization response.
            var context = await http.GetContextAsync();

            // Brings the Console to Focus.
            BringConsoleToFront();

            // Sends an HTTP response to the browser.
            var response = context.Response;
            string responseString = string.Format("<html><head><meta http-equiv='refresh' content='3;url={0}'></head><body>Please return to the console app.</body></html>", strOktaOrgUrl);
            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            Task responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
            {
                responseOutput.Close();
                http.Stop();
                Console.WriteLine("HTTP server stopped.");
            });

            // Checks for errors.
            if (context.Request.QueryString.Get("error") != null)
            {
                output(String.Format("OAuth authorization error: {0}.", context.Request.QueryString.Get("error")));
                return;
            }
            if (context.Request.QueryString.Get("code") == null
                || context.Request.QueryString.Get("state") == null)
            {
                output("Malformed authorization response. " + context.Request.QueryString);
                return;
            }

            // extracts the code
            var code = context.Request.QueryString.Get("code");
            var incoming_state = context.Request.QueryString.Get("state");

            // Compares the receieved state to the expected value, to ensure that
            // this app made the request which resulted in authorization.
            if (incoming_state != state)
            {
                output(String.Format("Received request with invalid state ({0})", incoming_state));
                return;
            }
            output("Authorization code: " + code);

            // Starts the code exchange at the Token Endpoint.
            performCodeExchange(code, code_verifier, strRedirectUri, state, nonce);
        }

        async void performCodeExchange(string code, string code_verifier, string redirectURI, string state, string nonce)
        {
            output("Exchanging code for tokens...");

            // builds the  request
            string tokenRequestURI = _config.TokenEndpoint;

            TokenClient tokenClient = new TokenClient(tokenRequestURI,
                strClientId);

            var tokenResponse = await tokenClient.RequestAuthorizationCodeAsync(code, strRedirectUri, code_verifier);

            output("Making API Call to the UserInfo endpoint...");
            // use the access token to retrieve claims from userinfo
            var userInfoClient = new UserInfoClient(_config.UserInfoEndpoint); 
            var userInfoResponse = await userInfoClient.GetAsync(tokenResponse.AccessToken);

            var id = new ClaimsIdentity();

            foreach(Claim c in userInfoResponse.Claims)
            {
                output(string.Format("Claim {0}:{1}", c.Type, c.Value));
            }

            id.AddClaims(userInfoResponse.Claims);
            id.AddClaim(new Claim("id_token", tokenResponse.IdentityToken));
            id.AddClaim(new Claim("access_token", tokenResponse.AccessToken));
        }


        /// <summary>
        /// Appends the given string to the on-screen log, and the debug console.
        /// </summary>
        /// <param name="output">string to be appended</param>
        public void output(string output)
        {
            Console.WriteLine(output);
        }

        /// <summary>
        /// Returns URI-safe data with a given input length.
        /// </summary>
        /// <param name="length">Input length (nb. output will be longer)</param>
        /// <returns></returns>
        public static string randomDataBase64url(uint length)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);
            return base64urlencodeNoPadding(bytes);
        }

        /// <summary>
        /// Returns the SHA256 hash of the input string.
        /// </summary>
        /// <param name="inputStirng"></param>
        /// <returns></returns>
        public static byte[] sha256(string inputStirng)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(inputStirng);
            SHA256Managed sha256 = new SHA256Managed();
            return sha256.ComputeHash(bytes);
        }

        /// <summary>
        /// Base64url no-padding encodes the given input buffer.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string base64urlencodeNoPadding(byte[] buffer)
        {
            string base64 = Convert.ToBase64String(buffer);

            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");

            return base64;
        }

        // Hack to bring the Console window to front.
        // ref: http://stackoverflow.com/a/12066376

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public void BringConsoleToFront()
        {
            SetForegroundWindow(GetConsoleWindow());
        }
    }
}
