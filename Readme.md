# OpenID Connect Windows Native Samples with Okta

The code samples available in this repository demonstrate the use of [Okta OpenID Connect](https://developer.okta.com/docs/api/resources/oidc.html) as the authentication mechanism for Windows native apps.

The first native app to be demonstrated in this repo is a [C# console application](./tree/master/Okta.Samples.OpenIDConnect.Console).

## Setup

These code samples were written with Visual Studio 2015 Community Edition Update 3 and we strongly suggest that you use the same development environment (or Professional/Enterprise Edition).

In order to compile this solution, you must:

1. Clone the [MyPureCloud OAuth Control](https://github.com/raphaellondner-okta/purecloud_api_dotnet_oauth_control/tree/okta-compat) in a ``purecloud_api_dotnet_oauth_control`` folder available at the same level as the ``okta-oauth-nativewindows`` folder (for instance, you can sync this repo to ``C:\GitHub\okta-oauth-nativewindows`` and the [MyPureControl repo](https://github.com/raphaellondner-okta/purecloud_api_dotnet_oauth_control) to ``C:\GitHub\purecloud_api_dotnet_oauth_control``). 
2. Make sure you switch to the ``okta-compat`` branch for the [MyPureControl repo](https://github.com/raphaellondner-okta/purecloud_api_dotnet_oauth_control) and the ``token-validation`` branch for this repo. 
3. In your Okta org, [create an OpenID Connect single page app (SPA)](https://help.okta.com/en/prev/Content/Topics/Apps/Apps_App_Integration_Wizard.htm).
4. In the ``Okta.Samples.OpenIDConnect.Console`` folder, edit the ``App.config`` file and set the following values:    
     a. __``okta:OrganizationSubDomain``__: the sub-domain of your Okta org (i.e. ``company.okta.com`` if the url of your Okta org is ``https://company.okta.com``)     
     b. __``okta:ClientId``__: the ``Client ID`` value of your Okta OIDC SPA.      
     c. __``okta:RedirectUri``__: a valid redirect uri as set up in your Okta OIDC SPA (this value doesn't have to be a valid web site url)     
     d. __``okta:Scopes``__: the OpendID Connect scopes your application will request from Okta - you can use the default scopes as already configured.    
     e. __``okta:ResponseType``__: the OpenID Connect response type (can currently be ``id_token``, ``token`` or ``id_token token``)