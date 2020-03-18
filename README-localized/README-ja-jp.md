---
page_type: sample
description: "ASP.NET Core 2.1 MVC を使用して、委任されたアクセス許可フローを使用して Microsoft Graph に接続する。"
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
# ASP.NET Core 2.1 用 Microsoft Graph Connect のサンプル

![ASP.NET Core 2.1 用 Microsoft Graph Connect のサンプルのスクリーンショット](readme-images/Page1.PNG)

**シナリオ**:Azure AD (v2.0) エンドポイントからユーザーのプロファイルと写真を取得した後にユーザーの写真が添付ファイルに含まれるメールを送信するために、ASP.NET Core 2.1 MVC を使用して、委任されたアクセス許可フローを使用して Microsoft Graph に接続します。

このサンプルでは、サインインするために OpenID Connect を、アクセス トークンを取得するために [Microsoft Authentication Library (MSAL) for .NET](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet) を、そして Microsoft Graph の操作を行うために [Microsoft Graph Client Library for .NET](https://github.com/microsoftgraph/msgraph-sdk-dotnet) (SDK) を使用します。MSAL SDK では [Azure AD v2 0 エンドポイント](https://azure.microsoft.com/en-us/documentation/articles/active-directory-appmodel-v2-overview)を操作するための機能が提供されており、開発者は職場または学校 (Azure Active Directory) アカウント、および個人用 (Microsoft) アカウントの両方に対する認証を処理する単一のコード フローを記述することができます。
このサンプルでは委任されたアクセス許可のみが使用されるため、管理者の同意は必要ありません。

> ASP.NET Core 1.1 バージョンを使用するこのサンプルの以前のバージョンは[こちらに](https://github.com/microsoftgraph/aspnetcore-connect-sample/tree/netcore1.1)あります。ASP.NET Core 2.0 バージョンは[こちら](https://github.com/microsoftgraph/aspnetcore-connect-sample/tree/netcore2.0)にあります。

## 目次

- [前提条件](#prerequisites)
- [アプリを登録する](#register-the-app)
- [アプリを構成して実行する](#configure-and-run-the-sample)
- [サンプルの主要なコンポーネント](#key-components-of-the-sample)
- [投稿](#contributing)
- [質問とコメント](#questions-and-comments)
- [その他のリソース](#additional-resources)

## ADAL と MSAL の違い

ADAL (Azure AD v1.0) と MSAL (Azure AD v2.0) はともに、さまざまな言語用の認証ライブラリで、Azure AD からのトークンの取得および保護された Web API (Microsoft API または Azure Active Directory に登録されているアプリケーション) へのアクセスを可能にします。ADAL アプリケーションではユーザーは各自の職場および学校のアカウントでサインインすることが可能で、ADAL アプリケーションは [Azure ポータル](https://portal.azure.com/)に登録する必要があります。一方、新しい (プレビュー中の) MSAL を使用するアプリケーションでは、ユーザーは各自の職場および学校のアカウントと個人用アカウントのいずれでもサインインすることが可能で、アプリケーションが Azure AD B2C アプリケーションである場合を除き、[アプリケーション登録ポータル](https://apps.dev.microsoft.com/)に登録する必要があります。ADAL と MSAL のどちらにも、それぞれの .NET クライアント ライブラリがあります。それぞれ、[ADAL.NET](https://github.com/AzureAD/azure-activedirectory-library-for-dotnet) と [MSAL.NET](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet) になります。[ADAL.NET と MSAL.NET での移行と両者の違いの詳細については、こちらを参照してください。](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/adal-to-msal)

## 前提条件

ASP.NET Core 2.1 用 Microsoft Graph Connect サンプルを使用するには、以下が必要です。

- 開発用コンピューターにインストールされている [.NET Core 2.1 SDK](https://www.microsoft.com/net/download/core) を使用した Visual Studio 2017。
- [個人用 Microsoft アカウント](https://signup.live.com)または[職場/学校アカウント](https://dev.office.com/devprogram)。(テナントの管理者である必要はありません。)
- [アプリ登録ポータルに登録](#register-the-app)するアプリケーションのアプリケーション ID とキー。

## アプリを登録する

1. [Azure AD ポータル](https://portal.azure.com)に移動します。アプリ登録を作成するためのアクセス許可が付与されている**個人用アカウント** (別名:Microsoft アカウント) または**職場または学校のアカウント**を使用してログインします。

   > **注:**アプリ登録を作成するためのアクセス許可を持っていない場合は、Azure AD ドメイン管理者に問い合わせてください。

2. 左側のナビゲーション メニューで、[**Azure Active Directory**] を選択します。

3. 現在のブレードのナビゲーション ウィンドウで [**アプリの登録**] をクリックします。

4. 現在のブレードのコンテンツで [**新規登録**] をクリックします。

5. [**アプリケーションの登録**] ページで、次の値を指定します。

   - [**名前**] = 希望するアプリ名
   - [**サポートされているアカウントの種類**] = 該当する値を選択します
   - [**リダイレクト URI**]
     - [種類] (ドロップダウン) = Web
     - [値] = `https://localhost:44334/signin-oidc`

   > **注:**リダイレクト URI 値が、ドメイン内で一意であることを確認します。この値は後から変更することができます。ホストされている URI をポイントする必要はありません。上記の URI の例が既に使用されている場合は、別の一意の値を選択してください。

   1. [**詳細設定**] で、[**ログアウト URL**] の値を "`https://localhost:44334/Account/SignOut`" に設定します。
   2. [**リダイレクト URI**] は後で必要になるため、コピーしておきます。

6. アプリが作成されたら、概要ページから [**アプリケーション (クライアント) ID**] と [**Directory (テナント) ID**] をコピーして一時的に保管します。両方とも後で必要になります。

7. 現在のブレードのナビゲーション ウィンドウで、[**証明書とシークレット**] をクリックします。

   1. [**新しいクライアント シークレット**] をクリックします。
   2. [**クライアント シークレットの追加**] ダイアログで、次の値を指定します。

      - [**説明**] = MyAppSecret1
      - [**有効期限**] = 1 年

   3. [**追加**] をクリックします。

   4. 画面が更新されて新しく作成したクライアント シークレットが表示されたら、クライアント シークレットの [**値**] を一時的にコピーして保管します。後で必要にります。

      > **重要:**
      > このシークレットの文字列は今後表示されないため、今必ずコピーするようにしてください。運用アプリではアプリケーション シークレットとして常に証明書を使用するべきですが、このサンプルでは、簡単な共有シークレット パスワードを使用します。

8. 現在のブレードのナビゲーション ウィンドウで [**認証**] をクリックします。
   1. [ID トークン] を選択します。
9. 現在のブレードのナビゲーション ウィンドウで [**API のアクセス許可**] をクリックします。

   1. 現在のブレードのコンテンツで [**アクセス許可の追加**] をクリックします。
   2. [**API アクセス許可の要求**] パネルで [**Microsoft Graph**] を選択します。

   3. [**委任されたアクセス許可**] を選択します。
   4. [アクセス許可を選択する] 検索ボックスに、「user」と入力します。
   5. [**openid**]、[**email**]、[**profile**]、[**offline_access**]、[**User.Read**]、[**User.ReadBasic.All**]、および [**Mail.Send**] を選択します。

   6. ポップアップの下部で [**アクセス許可の追加**] をクリックします。

   > **注:**Microsoft では、アプリを登録するときにすべての委任されたアクセス許可を明示的に一覧表示することをお勧めしています。V2 エンドポイントには増分および動的な同意機能があるためにこの手順は省略可能ですが、これを行わない場合、管理者の同意に悪影響を与える可能性があります。

## アプリを構成して実行する

1. ASP.NET Core 用 Microsoft Graph Connect のサンプルをダウンロードするか複製します。

2. **MicrosoftGraphAspNetCoreConnectSample.sln** サンプル ファイルを Visual Studio 2017 で開きます。

3. ソリューション エクスプローラーで、プロジェクトのルート ディレクトリにある **appsettings.json** ファイルを開きます。

   a.**AppId** キーは、`ENTER_YOUR_APP_ID` を登録したアプリケーションのアプリケーション ID で置き換えます。

   b.**AppSecret** キーは、`ENTER_YOUR_SECRET` を登録したアプリケーションのパスワードで置き換えます。運用アプリではアプリケーション シークレットとして常に証明書を使用するべきですが、このサンプルでは、簡単な共有シークレット パスワードを使用している点にご留意ください。

4. F5 キーを押して、サンプルをビルドして実行します。これにより、NuGet パッケージの依存関係が復元され、アプリが開きます。

   > パッケージのインストール中にエラーが発生した場合は、ソリューションを保存したローカル パスが長すぎたり深すぎたりしていないかご確認ください。この問題は、ソリューションをドライブのルート近くに移動すると解決します。

5. 個人用アカウント (MSA) あるいは職場または学校アカウントでサインインし、要求されたアクセス許可を付与します。

6. JSON にあるプロファイル画像および プロファイル データがスタート ページに表示されます。

7. ボックス内のメールアドレスを、同じテナント内の別の有効なアカウントのメールに変更し、[**データの読み込み**] ボタンを選択します。操作が完了すると、選択したユーザーのプロファイルがページに表示されます。

8. 必要に応じて受信者一覧を編集し、[**メールの送信**] ボタンを選択します。メールが送信されると、ページ上部に成功メッセージが表示されます。

## サンプルの主要なコンポーネント

次のファイルには、Microsoft Graph への接続、ユーザー データの読み込み、およびメールの送信に関連するコードが含まれています。

- [`appsettings.json`](MicrosoftGraphAspNetCoreConnectSample/appsettings.json) 認証と承認に使用される値が含まれています。
- [`Startup.cs`](MicrosoftGraphAspNetCoreConnectSample/Startup.cs) 認証を含む、使用するアプリとサービスの構成を行います。

### コントローラー

- [`AccountController.cs`](MicrosoftGraphAspNetCoreConnectSample/Controllers/AccountController.cs) サインインとサインアウトを処理します。
- [`HomeController.cs`](MicrosoftGraphAspNetCoreConnectSample/Controllers/HomeController.cs) UI からの要求を処理します。

### ビュー

- [`Index.cshtml`](MicrosoftGraphAspNetCoreConnectSample/Views/Home/Index.cshtml) サンプルの UI が含まれています。

### ヘルパー

- [`GraphAuthProvider.cs`](MicrosoftGraphAspNetCoreConnectSample/Helpers/GraphAuthProvider.cs) MSAL の **AcquireTokenSilent** メソッドを使用してアクセス トークンを取得します。
- [`GraphSdkHelper.cs`](MicrosoftGraphAspNetCoreConnectSample/Helpers/GraphSDKHelper.cs) Microsoft Graph の操作に使用される SDK クライアントを開始します。
- [`GraphService.cs`](MicrosoftGraphAspNetCoreConnectSample/Helpers/GraphService.cs) 呼び出しを構築して Microsoft Graph サービスに送信し、応答を処理するための、**GraphServiceClient** を使用するメソッドが含まれています。
  - **GetUserJson** アクションは、メール アドレスによりユーザーのプロファイルを取得し、JSON に変換します。
  - **GetPictureBase64** アクションは、ユーザーのプロファイル画像を取得し、base64 文字列に変換します。
  - **SendMail** アクションは、現在のユーザーに代わってメールを送信します。

## 投稿

このサンプルに投稿する場合は、[CONTRIBUTING.MD](/CONTRIBUTING.md) を参照してください。

このプロジェクトでは、[Microsoft Open Source Code of Conduct (Microsoft オープン ソース倫理規定)](https://opensource.microsoft.com/codeofconduct/) が採用されています。詳細については、「[Code of Conduct の FAQ (倫理規定の FAQ)](https://opensource.microsoft.com/codeofconduct/faq/)」を参照してください。また、その他の質問やコメントがあれば、[opencode@microsoft.com](mailto:opencode@microsoft.com) までお問い合わせください。

## 質問とコメント

ASP.NET Core用 Microsoft Graph Connect のサンプルに関するフィードバックをお寄せください。質問や提案は、このリポジトリの「[問題](https://github.com/microsoftgraph/aspnetcore-connect-sample/issues)」セクションで送信できます。

Microsoft Graph 全般の質問については、「[Stack Overflow](https://stackoverflow.com/questions/tagged/MicrosoftGraph)」に投稿してください。質問やコメントには、必ず "_MicrosoftGraph_" とタグを付けてください。

Microsoft Graph に関する変更の提案は、[UserVoice](https://officespdev.uservoice.com/) で行うことができます。

## その他のリソース

- [Microsoft Graph ドキュメント](https://developer.microsoft.com/graph)
- [その他の Microsoft Graph Connect のサンプル](https://github.com/MicrosoftGraph?q=connect)
- [ASP.NET 用 Microsoft Graph Webhook のサンプル](https://github.com/microsoftgraph/aspnetcore-apponlytoken-webhooks-sample)
- [ASP.NET 4.6 用 Microsoft Graph Connect のサンプル](https://github.com/microsoftgraph/aspnet-connect-sample)

## 著作権

Copyright (c) 2019 Microsoft.All rights reserved.
