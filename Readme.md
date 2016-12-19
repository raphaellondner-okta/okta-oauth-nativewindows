# OpenID Connect Windows Native Samples with Okta

The code samples available in this repository demonstrate the use of [Okta OpenID Connect](https://developer.okta.com/docs/api/resources/oidc.html)  as the authentication mechanism for Windows native apps along with [Okta API Access Management](http://developer.okta.com/docs/api/resources/oauth2.html) for authorizing access to a backend API using Okta's Authorization Servers.

One native app is currently demonstrated:
1. A [C# console application using the default browser](./tree/master/Okta.Samples.OpenIDConnect.NativeBrowserConsole) and the Authorization Code Flow (with PKCE)

## Development environment instructions

These code samples were written with Visual Studio 2015 Community Edition Update 3 and we strongly suggest that you use the same development environment (or any other paid-for Edition).


## Samples setup instructions

1. In your Okta org, make sure OpenID Connect has been enabled. If not, please send an email to developers at okta dot com to get it enabled.
2. Next, [create an OpenID Connect Native app](https://help.okta.com/en/prev/Content/Topics/Apps/Apps_App_Integration_Wizard.htm) with a Redirect URI value of ``http://127.0.0.1:[available_port]``, with the ``available_port`` value being a port available on your machine (so that your console app can listen for the browser response on that port. __Important note__: don't forget to assign at least one user to your new OpenID Connect app!
3. Open the ``Okta OpenID Connect Windows Native Examples`` solution in Visual Studio 2015 and in the ``Okta OpenID Connect Console (Code Auth Flow - Native Browser)`` project, edit the __App.config__ file to set the following values:    
     a. __``okta:OrganizationUrl``__: the full url of your Okta org (e.g. ``https://company.okta.com``)     
     a. __``okta:AuthorizationServerUrl``__: the full url of your Okta org (e.g. ``https://company.okta.com``)   
     b. __``okta:ClientId``__: the ``Client ID`` value of your Okta OIDC Native app.      
     c. __``okta:RedirectUri``__: a valid redirect uri as set up in your Okta OIDC Native app. This value should be of the form ``http://127.0.0.1:{any_port}/`` and configured as a redirect uri in your Okta OIDC app. __Important note: Make sure to include the trailing slash!__
     d. __``okta:Scopes``__: the OpendID Connect scopes your application will request from Okta - you can use the default scopes as already configured.    
     e. __``okta:ResponseType``__: the OpenID Connect response type (can currently be ``cpde`` or ``code id_token``)
4. You can test the application with the parameters above and verify that you can sign in with Okta in your browser (or leverage an existing Okta session). You should be able to verify that your console application is able to authenticate you with the same credentials you used in the browser.
5. If you want to test the ability to call an external API (for instance, our [ASP.NET Core Web API sample](https://github.com/raphaellondner-okta/okta-oauth-dotnetcore-rs-simple)), you must have access to Okta's API Access Management product, which is currently in beta version. If you want access to this product, please submit a request on the [Okta beta site](https://oktabeta.zendesk.com) and select ``API Access Management`` in the _Beta Name_ dropdown list.
6. Once you've been granted access to Okta's API Access Management product, navigate to __Security-->API__ in the Admin dashboard of your Okta organization. You should see a page similar to the screenshot below:
     [Authorization Servers Home Page](https://github.com/raphaellondner-okta/okta-oauth-nativewindows/assets/Okta_AuthorizationServersHomePage.png)
7. Press the ``Add Authorization Server`` button and enter a descriptive name (such as _ToDo List API_), a resource Uri (such as _http://todolist.example.com_), as well as an optional description. You should see a page similar to the screenshot below:
        [Todo List API Authorization Server](https://github.com/raphaellondner-okta/okta-oauth-nativewindows/assets/Okta_AuthorizationServersHomePage.png)
8. Take note of the __Issuer__ value on this page and copy/paste it to ``okta:AuthorizationServerUrl`` parameters in the __App.config__ file of this project.
9. In Okta's Admin dashboard, select the _Scopes_ tab and select the ``Add Scope`` button. 
     a. In the window that opens, enter ``todolist.read`` in the __Name__ field and ``Permission to read the Todo List`` in the __Description__ field    
     You should end up with the following _Scopes_ tab:
     [Scopes tab](https://github.com/raphaellondner-okta/okta-oauth-nativewindows/assets/Okta_TodoList.ReadScope.png)
     b. In the __App.config__ file, update the __``okta:Scopes``__ value and append ``todolist.read`` to that list
10. In Okta's Admin dashboard, select the _Access Policies_ tab and press __Add Policy__     
    a. Fill out a name and an (optional) description for your policy.
    b. Select __The following clients__ in the __Assign To__ field and select the OpenID Connect client you previously created. You should now see the following screen:
    [Create Policy window](https://github.com/raphaellondner-okta/okta-oauth-nativewindows/assets/Okta_WindowsClientAccessPolicy.png)
    c. Press __Create Policy__. The following page should appear:
   [OAuth Policy created](https://github.com/raphaellondner-okta/okta-oauth-nativewindows/assets/Okta_WindowsClientAccessPolicyCreated.png)
    d. Press the __Add Rule__ button
    e. In the __Rule Name__ field, enter a string such as _Grant read access to the Todo List_
    f. Uncheck the __Client credentials__ checkbox and check the __Authorization code__ checkbox
    g. In __Grant these scopes__ select __All scopes__ (in reality, you can layer multiples rules on top of each other, but we're doing this configuration for the sake of simplicity)
    h. Leave the other values as default and press __Create Rule__
11. You should now be able to test this command line sample along with a backend API and don't hesitate to send your feedback, comments or suggestions to ``developers AT okta DOT com``!
