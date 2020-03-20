---
page_type: sample
description: "使用 ASP.NET Core 2.1 MVC 通过委派权限流连接到 Microsoft Graph。"
products:
- ms-graph
languages:
- aspx
- csharp
extensions:
  contentType: samples
  technologies:
  - Microsoft Graph
  - Microsoft identity platform
  services:
  - Microsoft identity platform
  createdDate: 8/6/2017 5:17:58 AM
---
# 针对 ASP.NET Core 2.1 的 Microsoft Graph 连接示例

![针对 ASP.NET Core 2.1 的 Microsoft Graph 连接示例屏幕截图](readme-images/Page1.PNG)

**应用场景**：使用 ASP.NET Core 2.1 MVC 通过委派权限流连接到 Microsoft Graph，以从 Azure AD (v2.0) 终结点检索用户的个人资料和照片，然后发送包含该照片附件的电子邮件。

该示例使用 OpenID Connect 进行登录，使用[适用于 .NET 的 Microsoft 身份验证库 (MSAL)](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet) 来获取访问令牌，并使用[适用于 .NET 的 Microsoft Graph 客户端库](https://github.com/microsoftgraph/msgraph-sdk-dotnet) (SDK) 与 Microsoft Graph 进行交互。MSAL SDK 提供可使用 [Azure AD v2.0 终结点](https://azure.microsoft.com/en-us/documentation/articles/active-directory-appmodel-v2-overview)的功能，借助该终结点，开发人员可以编写单个代码流来处理对工作或学校 (Azure Active Directory) 帐户或个人 (Microsoft) 帐户的身份验证。
该示例仅使用委派的权限，因此不需要管理员许可。

> 此示例的先前版本：使用 ASP.NET Core 1.1 版本的示例在[此处](https://github.com/microsoftgraph/aspnetcore-connect-sample/tree/netcore1.1)，使用 ASP.NET Core 2.0 版本的示例在[此处](https://github.com/microsoftgraph/aspnetcore-connect-sample/tree/netcore2.0)。

## 目录

- [先决条件](#prerequisites)
- [注册应用](#register-the-app)
- [配置并运行示例](#configure-and-run-the-sample)
- [示例的主要组件](#key-components-of-the-sample)
- [参与](#contributing)
- [问题和意见](#questions-and-comments)
- [其他资源](#additional-resources)

## ADAL 与 MSAL 之间的差异

ADAL（Azure AD 1.0 版）和 MSAL（Azure AD 2.0 版）均是适用于多种语言的身份验证库，它们使你能够从 Azure AD 获取令牌，以便访问受保护的 Web API（Microsoft API 或通过 Azure Active Directory 注册的应用程序）。ADAL 应用程序允许用户使用其工作和学校帐户登录，并且需要在 [Azure 门户](https://portal.azure.com/)中注册，而使用新的（预览版）MSAL 的应用程序允许用户使用其工作和学校帐户或个人帐户登录，并且除非它们是 Azure AD B2C 应用程序，否则就需要在[应用程序注册门户](https://apps.dev.microsoft.com/)中注册。ADAL 和 MSAL 均具有自己的 .NET 客户端库：分别是 [ADAL.NET](https://github.com/AzureAD/azure-activedirectory-library-for-dotnet) 和 [MSAL.NET](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet)。[在此处了解有关 ADAL.NET 与 MSAL.NET 之间的迁移和差异的详细信息。](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/adal-to-msal)

## 先决条件

若要使用针对 ASP.NET Core 2.1 的 Microsoft Graph 连接示例，需满足以下条件：

- 在开发计算机上安装 Visual Studio 2017[（带 .NET Core 2.1 SDK）](https://www.microsoft.com/net/download/core)。
- 一个[个人 Microsoft 帐户](https://signup.live.com)或者一个[工作或学校帐户](https://dev.office.com/devprogram)。（您无需是租户的管理员。）
- [在应用注册门户上注册的](#register-the-app)应用程序的 ID 和密钥。

## 注册应用

1. 导航到 [Azure AD 门户](https://portal.azure.com)。使用具有应用注册创建权限的**个人帐户**（也称为：Microsoft 帐户）或**工作或学校帐户**登录。

   > **注意：**如果你不具备创建应用注册的权限，请联系 Azure AD 域管理员。

2. 从左侧导航菜单中，单击“**Azure Active Directory**”。

3. 从当前边栏选项卡导航窗格中，单击“**应用注册**”。

4. 从当前边栏选项卡内容中，单击“**新建注册**”。

5. 在“**注册应用**”页面上，指定以下值：

   - “**名称**”= [所需的应用名称]
   - “**支持的帐户类型**”= [选择适用于你的需求的值]
   - “**重定向 URI**”
     - 类型（下拉列表）= Web
     - 值 = `https://localhost:44334/signin-oidc`

   > **注意：**确保重定向 URI 值在你的域中是唯一的。此值稍后可以更改，且不需要指向托管的 URI。如果上面的示例 URI 已被使用，请选择一个唯一值。

   1. 在“**高级设置**”下，将“**注销 URL**”的值设置为 `https://localhost:44334/Account/SignOut`
   2. 复制“**重定向 URI**”，以便以后使用。

6. 创建应用后，从概述页面复制“**应用程序(客户端) ID**”和“**Directory (租户) ID**”并暂时保存，以便以后使用。

7. 从当前边栏选项卡导航窗格中，单击“**证书和密码**”。

   1. 单击“**新建客户端密码**”。
   2. 在“**添加客户端密码**”对话框中，指定以下值：

      - “**描述**”= MyAppSecret1
      - “**过期**”= 1 年内

   3. 单击“**添加**”。

   4. 屏幕上更新为新创建的客户端密码后，复制客户端密码的“**值**”并暂时保存，以便以后使用。

      > **重要说明：**
      > 此密码字符串将不再显示，所以请务必现在就复制它。在成品应用中，应始终将证书用作应用程序密码，但对于此示例，我们将使用简单的共享密码。

8. 从当前边栏选项卡导航窗格中，单击“**身份验证**”。
   1. 选择“ID 令牌”
9. 从当前边栏选项卡导航窗格中，单击“**API 权限**”。

   1. 从当前边栏选项卡内容中，单击“**添加权限**”。
   2. 在“**请求 API 权限**”面板中，选择“**Microsoft Graph**”。

   3. 选择“**委派的权限**”。
   4. 在“选择权限”搜索框中键入“用户”。
   5. 选择“**openid**”、“**email**”、“**profile**”、“**offline_access**”、“**User.Read**”、“**User.ReadBasic.All**”和“**Mail.Send**”。

   6. 单击浮出控件底部的“**添加权限**”。

   > **注意：**Microsoft 建议你在注册应用时显式列出所有委派的权限。虽然 v2 终结点的增量和动态许可功能使此步骤成为可选步骤，但如果不执行此操作，可能会对管理员许可产生负面影响。

## 配置并运行示例

1. 下载或克隆针对 ASP.NET Core 的 Microsoft Graph 连接示例。

2. 在 Visual Studio 2017 中打开 **MicrosoftGraphAspNetCoreConnectSample.sln** 示例文件。

3. 在“解决方案资源管理器”中，打开项目根目录下的 **appsettings.json** 文件。

   a.对于 **AppId** 密钥，请将 `ENTER_YOUR_APP_ID` 替换为所注册应用程序的 ID。

   b.对于 **AppSecret** 密钥，请将 `ENTER_YOUR_SECRET` 替换为所注册应用程序的密码。请注意，在成品应用中，应始终将证书用作应用程序密码，但对于此示例，我们将使用简单的共享密码。

4. 按 F5 生成并运行此示例。这将还原 NuGet 包依赖项并打开该应用。

   > 如果在安装包时出现任何错误，请确保你放置该解决方案的本地路径并未太长/太深。将解决方案移动到更接近驱动器根目录的位置可以解决此问题。

5. 使用你的个人帐户 (MSA) 或者工作或学校帐户登录，并授予所请求的权限。

6. 你应该可以在起始页上看到个人资料图片和 JSON 格式的个人资料数据。

7. 将框中的电子邮件地址更改为同一租户中另一个有效帐户的电子邮件地址，然后选择“**加载数据**”按钮。完成此操作后，网页上会显示所选用户的个人资料。

8. 还可以编辑收件人列表，然后选择“**发送电子邮件**”按钮。邮件发送后，页面顶部将显示一条成功消息。

## 示例的主要组件

以下文件包含与 Microsoft Graph 连接、加载用户数据和发送电子邮件相关的代码。

- [`appsettings.json`](MicrosoftGraphAspNetCoreConnectSample/appsettings.json) 包含用于身份验证和授权的值。
- [`Startup.cs`](MicrosoftGraphAspNetCoreConnectSample/Startup.cs) 配置应用及其使用的服务，包括身份验证。

### 控制器

- [`AccountController.cs`](MicrosoftGraphAspNetCoreConnectSample/Controllers/AccountController.cs) 处理登录和注销。
- [`HomeController.cs`](MicrosoftGraphAspNetCoreConnectSample/Controllers/HomeController.cs) 处理来自 UI 的请求。

### 视图

- [`Index. cshtml`](MicrosoftGraphAspNetCoreConnectSample/Views/Home/Index.cshtml) 包含示例的 UI。

### 帮助程序

- [`GraphAuthProvider.cs`](MicrosoftGraphAspNetCoreConnectSample/Helpers/GraphAuthProvider.cs) 使用 MSAL 的 **AcquireTokenSilent** 方法获取访问令牌。
- [`GraphSdkHelper.cs`](MicrosoftGraphAspNetCoreConnectSample/Helpers/GraphSDKHelper.cs) 启动用于与 Microsoft Graph 进行交互的 SDK 客户端。
- [`GraphService.cs`](MicrosoftGraphAspNetCoreConnectSample/Helpers/GraphService.cs) 包含使用 **GraphServiceClient** 来生成并发送对 Microsoft Graph 服务的调用并处理响应的方法。
  - **GetUserJson** 操作将通过电子邮件地址获取用户的个人资料，并将其转换为 JSON 格式。
  - **GetPictureBase64** 操作将获取用户的个人资料图片，并将其转换为 base64 字符串。
  - **SendEmail** 操作将代表当前用户发送一封电子邮件。

## 参与

如果想要参与本示例，请参阅 [CONTRIBUTING.MD](/CONTRIBUTING.md)。

此项目已采用 [Microsoft 开放源代码行为准则](https://opensource.microsoft.com/codeofconduct/)。有关详细信息，请参阅[行为准则 FAQ](https://opensource.microsoft.com/codeofconduct/faq/)。如有其他任何问题或意见，也可联系 [opencode@microsoft.com](mailto:opencode@microsoft.com)。

## 问题和意见

对于针对 ASP.NET Core 的 Microsoft Graph 连接示例，我们非常乐意收到你的相关反馈。你可以在该存储库中的[问题](https://github.com/microsoftgraph/aspnetcore-connect-sample/issues)部分将问题和建议发送给我们。

与 Microsoft Graph 相关的一般问题应发布到 [Stack Overflow](https://stackoverflow.com/questions/tagged/MicrosoftGraph)。请确保你的问题或意见标记有 _[MicrosoftGraph]_。

可在 [UserVoice](https://officespdev.uservoice.com/)上提供有关 Microsoft Graph 的更改意见。

## 其他资源

- [Microsoft Graph 文档](https://developer.microsoft.com/graph)
- [其他 Microsoft Graph 连接示例](https://github.com/MicrosoftGraph?q=connect)
- [针对 ASP.NET Core 的 Microsoft Graph Webhook 示例](https://github.com/microsoftgraph/aspnetcore-apponlytoken-webhooks-sample)
- [针对 ASP.NET 4.6 的 Microsoft Graph 连接示例](https://github.com/microsoftgraph/aspnet-connect-sample)

## 版权信息

版权所有 (c) 2019 Microsoft。保留所有权利。
