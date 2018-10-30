# Microsoft Graph Connect Sample for ASP.NET Core 2.1

## Table of contents

* [Prerequisites](#prerequisites)
* [Register the app](#register-the-app)
* [Configure and run the sample](#configure-and-run-the-sample)
* [Key components of the sample](#key-components-of-the-sample)
* [Contributing](#contributing)
* [Questions and comments](#questions-and-comments)
* [Additional resources](#additional-resources)

This ASP.NET Core 2.1 MVC sample shows how to connect to Microsoft Graph using delegate permissions and the Azure AD v2.0 endpoint (MSAL) to retrieve a user's profile and profile picture and send an email that contains the photo as an attachment.  
The sample uses OpenID Connect for sign in, [Microsoft Authentication Library (MSAL) for .NET](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet) to obtain an access token, and the [Microsoft Graph Client Library for .NET](https://github.com/microsoftgraph/msgraph-sdk-dotnet) (SDK) to interact with Microsoft Graph. The MSAL SDK provides features for working with the [Azure AD v2.0 endpoint](https://azure.microsoft.com/en-us/documentation/articles/active-directory-appmodel-v2-overview), which enables developers to write a single code flow that handles authentication for both work or school (Azure Active Directory) and personal (Microsoft) accounts.
The sample uses only delegate permissions, therefore it does not require admin consent.

>If you are searching for an earlier version of this sample, you can find the ASP.NET Core 1.1 version [here](https://github.com/microsoftgraph/aspnetcore-connect-sample/tree/netcore1.1) and the ASP.NET Core 2.0 version [here](https://github.com/microsoftgraph/aspnetcore-connect-sample/tree/netcore2.0).

## Using the Microsoft Graph Connect Sample

The screenshot below shows the app's start page.
  
![Microsoft Graph Connect Sample for ASP.NET Core 2.1 screenshot](readme-images/Page1.PNG)

## Important note about the MSAL Preview

This library is suitable for use in a production environment. We provide the same production level support for this library as we do our current production libraries. During the preview we may make changes to the API, internal cache format, and other mechanisms of this library, which you will be required to take along with bug fixes or feature improvements. This may impact your application. For instance, a change to the cache format may impact your users, such as requiring them to sign in again. An API change may require you to update your code. When we provide the General Availability release we will require you to update to the General Availability version within six months, as applications written using a preview version of library may no longer work.

## Prerequisites

To use the Microsoft Graph Connect Sample for ASP.NET Core 2.1, you need the following:

* Visual Studio 2017 [with .NET Core 2.1 SDK](https://www.microsoft.com/net/download/core) installed on your development computer.
* Either a [personal Microsoft account](https://signup.live.com) or a [work or school account](https://dev.office.com/devprogram). (You don't need to be an administrator of the tenant.)
* The application ID and key from the application that you [register on the App Registration Portal](#register-the-app).

## Register the app

This app uses the Azure AD v2.0 endpoint, so you'll register it on the [App Registration Portal](https://apps.dev.microsoft.com/).

1. Sign into the portal using either your personal or work or school account.

2. Choose **Add an app** next to 'Converged applications'.

3. Enter a name for the app, and choose **Create application**. (Don't check the Guided Setup box.)

   a. Enter a friendly name for the application.

   b. Copy the **Application Id**. This is the unique identifier for your app.

   c. Under **Application Secrets**, choose **Generate New Password**. Copy the password from the dialog. You won't be able to access this value again after you leave this dialog.

   >**Important**: Note that in production apps you should always use certificates as your application secrets, but for this sample we will use a simple shared secret password.

   d. Under **Platforms**, choose **Add platform**.

   e. Choose **Web**.

   f. Make sure the **Allow Implicit Flow** check box is selected, and add `https://localhost:44334/signin-oidc` as a **Redirect URL**. This is the base callback URL for this sample.

   >The **Allow Implicit Flow** option enables the hybrid flow. During authentication, this enables the app to receive both sign-in info (the id_token) and artifacts (in this case, an authorization code) that the app can use to obtain an access token.

   g. Enter `https://localhost:44334/Account/SignOut` as the **Logout URL**.
  
   h. Click **Save**.

   >You'll use the application ID and secret to configure the app in Visual Studio.

4. Configure Permissions for your application. **(Optional)**

   >Note that we are not required to add permissions for reading user data and sending emails during the app registration as you would do with the [v1 endpoint (ADAL)](https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-integrating-applications). The [Incremental and dynamic consent](https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-v2-compare#incremental-and-dynamic-consent) capability of the v2 endpoint (MSAL) has made this step optional.

   a. Choose **Microsoft Graph Permissions** > **Delegated Permissions** > **Add**.
  
   b. Select **openid**, **email**, **profile**, **offline_access**, **User.Read**, **User.ReadBasic.All** and **Mail.Send**. Then click **Ok**.
  
   c. Click **Save**.

## Configure and run the sample

1. Download or clone the Microsoft Graph Connect Sample for ASP.NET Core.

2. Open the **MicrosoftGraphAspNetCoreConnectSample.sln** sample file in Visual Studio 2017.

3. In Solution Explorer, open the **appsettings.json** file in the root directory of the project.  

   a. For the **AppId** key, replace `ENTER_YOUR_APP_ID` with the application ID of your registered application.  

   b. For the **AppSecret** key, replace `ENTER_YOUR_SECRET` with the password of your registered application. Note that in production apps you should always use certificates as your application secrets, but for this sample we will use a simple shared secret password.  

4. Press F5 to build and run the sample. This will restore NuGet package dependencies and open the app.

   >If you see any errors while installing packages, make sure the local path where you placed the solution is not too long/deep. Moving the solution closer to the root of your drive resolves this issue.

5. Sign in with your personal (MSA) account or your work or school account and grant the requested permissions.

6. You should see your profile picture and your profile data in JSON on the start page.

7. Change the email address in the box to another valid account's email in the same tenant and choose the **Load data** button. When the operation completes, the profile of the choosen user is displayed on the page.

8. Optionally edit the recipient list, and then choose the **Send email** button. When the mail is sent, a Success message is displayed on the top of the page.

## Key components of the sample

The following files contain code that's related to connecting to Microsoft Graph, loading user data and sending emails.

* [`appsettings.json`](MicrosoftGraphAspNetCoreConnectSample/appsettings.json) Contains values used for authentication and authorization. 
* [`Startup.cs`](MicrosoftGraphAspNetCoreConnectSample/Startup.cs) Configures the app and the services it uses, including authentication.

### Controllers

* [`AccountController.cs`](MicrosoftGraphAspNetCoreConnectSample/Controllers/AccountController.cs) Handles sign in and sign out.  
* [`HomeController.cs`](MicrosoftGraphAspNetCoreConnectSample/Controllers/HomeController.cs) Handles the requests from the UI.

### Views

* [`Index.cshtml`](MicrosoftGraphAspNetCoreConnectSample/Views/Home/Index.cshtml) Contains the sample's UI.

### Helpers

* [`GraphAuthProvider.cs`](MicrosoftGraphAspNetCoreConnectSample/Helpers/GraphAuthProvider.cs) Gets an access token using MSAL's **AcquireTokenSilentAsync** method.
* [`GraphSdkHelper.cs`](MicrosoftGraphAspNetCoreConnectSample/Helpers/GraphSDKHelper.cs) Initiates the SDK client used to interact with Microsoft Graph.
* [`GraphService.cs`](MicrosoftGraphAspNetCoreConnectSample/Helpers/GraphService.cs) Contains methods that use the **GraphServiceClient** to build and send calls to the Microsoft Graph service and to process the response.
  * The **GetUserJson** action gets the user's profile by an email adress and converts it to JSON.
  * The **GetPictureBase64** action gets the user's profile picture and converts it to a base64 string.
  * The **SendEmail** action sends an email on behalf of the current user.

### TokenStorage

* [`SessionTokenCache.cs`](MicrosoftGraphAspNetCoreConnectSample/Helpers/SessionTokenCache.cs) Sample implementation of an in-memory token cache. Production apps will typically use some method of persistent storage.

## Contributing

If you'd like to contribute to this sample, see [CONTRIBUTING.MD](/CONTRIBUTING.md).

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Questions and comments

We'd love to get your feedback about the Microsoft Graph Connect Sample for ASP.NET Core. You can send your questions and suggestions to us in the [Issues](https://github.com/microsoftgraph/aspnetcore-connect-sample/issues) section of this repository.

Questions about Microsoft Graph in general should be posted to [Stack Overflow](https://stackoverflow.com/questions/tagged/MicrosoftGraph). Make sure that your questions or comments are tagged with *[MicrosoftGraph]*.

You can suggest changes for Microsoft Graph on [UserVoice](https://officespdev.uservoice.com/).

## Additional resources

* [Microsoft Graph documentation](https://developer.microsoft.com/graph)
* [Other Microsoft Graph Connect samples](https://github.com/MicrosoftGraph?q=connect)
* [Microsoft Graph Webhooks Sample for ASP.NET Core](https://github.com/microsoftgraph/aspnetcore-apponlytoken-webhooks-sample)
* [Microsoft Graph Connect Sample for ASP.NET 4.6](https://github.com/microsoftgraph/aspnet-connect-sample)

## Copyright

Copyright (c) 2018 Microsoft. All rights reserved.
