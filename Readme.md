# OpenID Connect Windows Native Samples with Okta

The code samples available in this repository demonstrate the use of [Okta OpenID Connect](https://developer.okta.com/docs/api/resources/oidc.html) as the authentication mechanism for Windows native apps.

Two native apps are demonstrated:
1. A [C# console application using the default browser](./tree/master/Okta.Samples.OpenIDConnect.NativeBrowserConsole) and the Authorization Code Flow (with PKCE)
2. A [C# console application using an embedded browser window](./tree/master/Okta.Samples.OpenIDConnect.Console) and the OAuth 2.0 Implicit Grant Flow.

## Development environment instructions

These code samples were written with Visual Studio 2015 Community Edition Update 3 and we strongly suggest that you use the same development environment (or Professional/Enterprise Edition).

In order to compile this solution, you must:

1. Clone the [MyPureCloud OAuth Control](https://github.com/raphaellondner-okta/purecloud_api_dotnet_oauth_control/tree/okta-compat) in a ``purecloud_api_dotnet_oauth_control`` folder available at the same level as the ``okta-oauth-nativewindows`` folder (for instance, you can sync this repo to ``C:\GitHub\okta-oauth-nativewindows`` and the [MyPureControl repo](https://github.com/raphaellondner-okta/purecloud_api_dotnet_oauth_control) to ``C:\GitHub\purecloud_api_dotnet_oauth_control``). 
2. Make sure you switch to the ``okta-compat`` branch for the [MyPureControl repo](https://github.com/raphaellondner-okta/purecloud_api_dotnet_oauth_control) and the ``token-validation`` branch for this repo. 

## Samples setup instructions

1. In your Okta org, [create an OpenID Connect Native app  app](https://help.okta.com/en/prev/Content/Topics/Apps/Apps_App_Integration_Wizard.htm).
4. In the ``Okta.Samples.OpenIDConnect.NativeBrowserConsole`` folder, edit the ``App.config`` file and set the following values:    
     a. __``okta:OrganizationUrl``__: the full url of your Okta org (e.g. ``https://company.okta.com``)     
     b. __``okta:ClientId``__: the ``Client ID`` value of your Okta OIDC Native app.      
     c. __``okta:RedirectUri``__: a valid redirect uri as set up in your Okta OIDC Native app. This value should be of the form ``http://127.0.0.1:{any_port}/`` and configured as a redirect uri in your Okta OIDC app. __Important note: Make sure to include the trailing slash!__
     d. __``okta:Scopes``__: the OpendID Connect scopes your application will request from Okta - you can use the default scopes as already configured.    
     e. __``okta:ResponseType``__: the OpenID Connect response type (can currently be ``cpde`` or ``code id_token``)
2. In your Okta org, [create an OpenID Connect single page app (SPA)](https://help.okta.com/en/prev/Content/Topics/Apps/Apps_App_Integration_Wizard.htm).
4. In the ``Okta.Samples.OpenIDConnect.Console`` folder, edit the ``App.config`` file and set the following values:    
     a. __``okta:OrganizationSubDomain``__: the sub-domain of your Okta org (i.e. ``company.okta.com`` if the url of your Okta org is ``https://company.okta.com``)     
     b. __``okta:ClientId``__: the ``Client ID`` value of your Okta OIDC SPA.      
     c. __``okta:RedirectUri``__: a valid redirect uri as set up in your Okta OIDC SPA (this value doesn't have to be a valid web site url)     
     d. __``okta:Scopes``__: the OpendID Connect scopes your application will request from Okta - you can use the default scopes as already configured.    
     e. __``okta:ResponseType``__: the OpenID Connect response type (can currently be ``id_token``, ``token`` or ``id_token token``)