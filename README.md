# Microsoft Graph Connect Sample for ASP.NET Core

## Table of contents

* [Prerequisites](#prerequisites)
* [Register the app](#register-the-app)
* [Configure and run the sample](#configure-and-run-the-sample)
* [Key components of the sample](#key-components-of-the-sample)
* [Contributing](#contributing)
* [Questions and comments](#questions-and-comments)
* [Additional resources](#additional-resources)

This ASP.NET Core MVC sample shows how to connect to Microsoft Graph using delegate permissions and the Azure AD v1.0 endpoint to retrieve a user's profile and profile picture and send an email that contains the photo as an attachment.  
It uses OpenID Connect for sign in, [Azure AD Authentication Library for .NET](https://github.com/AzureAD/azure-activedirectory-library-for-dotnet) (ADAL) to obtain an access token using the [client credentials grant](https://tools.ietf.org/html/rfc6749#section-4.4), and the [Microsoft Graph Client Library for .NET](https://github.com/microsoftgraph/msgraph-sdk-dotnet) (SDK) to interact with Microsoft Graph.  
The sample uses only delegate permissions, therefore it does not require admin consent.

## Using the Microsoft Graph Webhooks Sample

The screenshot below shows the app's start page. 
  
![Microsoft Graph Connect Sample for ASP.NET Core screenshot](readme-images/Page1.PNG)

### Prerequisites

To use the Microsoft Graph Connect Sample for ASP.NET Core, you need the following:

- Visual Studio 2017 installed on your development computer. 
- A [work or school account](https://dev.office.com/devprogram). (You don't need to be an administrator of the tenant.)
- The application ID and key from the application that you [register on the Azure Portal](#register-the-app). 

### Register the app

This app uses the Azure AD endpoint, so you'll register it in the [Azure Portal](https://portal.azure.com/).

1. Sign in to the portal using your work or school account.

2. Choose **Azure Active Directory** in the left-hand navigation pane.

3. Choose **App registrations**, and then choose **New application registration**.  

4. Enter a name for the app, and choose **Create application**. 

   a. Enter a friendly name for the application.

   b. Choose 'Web app/API' as the **Application Type**.

   c. Enter `https://localhost:44334/signin-oidc` for the **Sign-on URL**. This is the base callback URL for this sample.
  
   d. Click **Create**.

5. Choose your new application from the list of registered applications.

6. Copy and store the Application ID. This value is shown in the **Essentials** pane or in **Settings** > **Properties**.

7. Optional. To enable multi-tenanted support for the app, open **Settings** > **Properties** and set **Multi-tenanted** to **Yes**.

8. Configure Permissions for your application:  

   a. Choose **Settings** > **Required permissions** > **Add**.
  
   b. Choose **Select an API** > **Microsoft Graph**, and then click **Select**.
  
   c. Choose **Select permissions**. Under **Delegated Permissions**, choose **View users' basic profile**, **View users' email address**, **Sign users in**, **Send mail as a user**, **Read all users' basic profiles** and **Sign in and read user profile**. Then click **Select**.
  
   d. Click **Done**.

9. Choose **Settings** > **Keys**. Enter a description, choose a duration for the key, and then click **Save**.

   >**Important**: Note that in production apps you should always use certificates as your application secrets, but for this sample we will use a simple shared secret password.

10. Copy the key value - this is your app's secret. You won't be able to access this value again after you leave this blade.

You'll use the application ID and secret to configure the app in Visual Studio.

## Configure and run the sample

1. Download or clone the Microsoft Graph Connect Sample for ASP.NET Core.

2. Open the **MicrosoftGraphAspNetCoreConnectSample.sln** sample file in Visual Studio 2017. 

3. In Solution Explorer, open the **appsettings.json** file in the root directory of the project.  
   a. For the **AppId** key, replace `ENTER_YOUR_APP_ID` with the application ID of your registered Azure application.  
   b. For the **AppSecret** key, replace `ENTER_YOUR_SECRET` with the key of your registered Azure application. Note that in production apps you should always use certificates as your application secrets, but for this sample we will use a simple shared secret password.  

4. Press F5 to build and run the sample. This will restore NuGet package dependencies and open the app.

   >If you see any errors while installing packages, make sure the local path where you placed the solution is not too long/deep. Moving the solution closer to the root of your drive resolves this issue.

5. Sign in with your work or school account and grant the requested permissions.

6. You should see your profile picture and your profile data in JSON on the start page.

7. Change the email address in the box to another valid account's email in the same tenant and choose the **Load data** button. When the operation completes, the profile of the choosen user is displayed on the page.

8. Optionally edit the recipient list, and then choose the **Send email** button. When the mail is sent, a Success message is displayed below the button.

## Key components of the sample 
The following files contain code that's related to connecting to Microsoft Graph, loading user data and sending emails.

- [`appsettings.json`](https://github.com/szmaorka/aspnetcore-connect-sample/blob/master/MicrosoftGraphAspNetCoreConnectSample/appsettings.json) Contains values used for authentication and authorization. 
- [`Startup.cs`](https://github.com/szmaorka/aspnetcore-connect-sample/blob/master/MicrosoftGraphAspNetCoreConnectSample/Startup.cs) Configures the app and the services it uses, including authentication.

**Controllers**  
- [`AccountController.cs`](https://github.com/szmaorka/aspnetcore-connect-sample/blob/master/MicrosoftGraphAspNetCoreConnectSample/Controllers/AccountController.cs) Handles sign in and sign out.  
- [`HomeController.cs`](https://github.com/szmaorka/aspnetcore-connect-sample/blob/master/MicrosoftGraphAspNetCoreConnectSample/Controllers/HomeController.cs) Handles the requests from the UI.

**Views**
- [`Index.cshtml`](https://github.com/szmaorka/aspnetcore-connect-sample/blob/master/MicrosoftGraphAspNetCoreConnectSample/Views/Home/Index.cshtml) Contains the sample's UI.

**Helpers**  
- [`GraphAuthProvider.cs`](https://github.com/szmaorka/aspnetcore-connect-sample/blob/master/MicrosoftGraphAspNetCoreConnectSample/Helpers/GraphAuthProvider.cs) Gets an access token using ADAL's **AcquireTokenByAuthorizationCodeAsync** method.
- [`GraphSDKHelper.cs`](https://github.com/szmaorka/aspnetcore-connect-sample/blob/master/MicrosoftGraphAspNetCoreConnectSample/Helpers/GraphSDKHelper.cs) Initiates the SDK client used to interact with Microsoft Graph.
- [`GraphService.cs`](https://github.com/szmaorka/aspnetcore-connect-sample/blob/master/MicrosoftGraphAspNetCoreConnectSample/Helpers/GraphService.cs) Contains methods that use the **GraphServiceClient** to build and send calls to the Microsoft Graph service and to process the response.
   - The **GetUserJson** action gets the user's profile by an email adress and converts it to JSON.
   - The **GetPictureBase64** action gets the user's profile picture and converts it to a base64 string.
   - The **SendEmail** action sends an email on behalf of the current user.

**TokenStorage**
- [`SessionTokenCache.cs`](https://github.com/szmaorka/aspnetcore-connect-sample/blob/master/MicrosoftGraphAspNetCoreConnectSample/Helpers/SessionTokenCache.cs) Sample implementation of an in-memory token cache. Production apps will typically use some method of persistent storage. 

## Contributing

If you'd like to contribute to this sample, see [CONTRIBUTING.MD](/CONTRIBUTING.md).

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Questions and comments

We'd love to get your feedback about the Microsoft Graph Connect Sample for ASP.NET Core. You can send your questions and suggestions to us in the [Issues](https://github.com/szmaorka/aspnetcore-connect-sample/issues) section of this repository.

Questions about Microsoft Graph in general should be posted to [Stack Overflow](https://stackoverflow.com/questions/tagged/MicrosoftGraph). Make sure that your questions or comments are tagged with *[MicrosoftGraph]*.

You can suggest changes for Microsoft Graph on [UserVoice](https://officespdev.uservoice.com/).

## Additional resources

- [Microsoft Graph documentation](https://developer.microsoft.com/graph)
- [Other Microsoft Graph Connect samples](https://github.com/MicrosoftGraph?q=connect)
- [Microsoft Graph Webhooks Sample for ASP.NET Core](https://github.com/microsoftgraph/aspnetcore-apponlytoken-webhooks-sample)
- [Microsoft Graph Connect Sample for ASP.NET 4.6](https://github.com/microsoftgraph/aspnet-connect-sample)

## Copyright
Copyright (c) 2017 Microsoft. All rights reserved.
