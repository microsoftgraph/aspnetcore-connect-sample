---
page_type: sample
description: "Use o ASP.NET Core 2.1 MVC para conectar-se ao Microsoft Graph usando o fluxo de permissões delegadas."
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
# Exemplo de conexão com o Microsoft Graph para ASP.NET Core 2.1

![Captura de tela do exemplo de conexão com o Microsoft Graph para ASP.NET Core 2.1](readme-images/Page1.PNG)

**Cenário**: Use o ASP.NET Core 2.1 MVC para conectar-se ao Microsoft Graph usando o fluxo de permissões delegadas para recuperar o perfil de um usuário, a foto do ponto de extremidade do Azure AD (v2.0) e, em seguida, envie um email que contenha a foto como anexo.

O exemplo usa o OpenID Connect para entrar, a [Biblioteca de Autenticação da Microsoft (MSAL) para .NET](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet) para obter um token de acesso e a [Biblioteca de Clientes do Microsoft Graph para .NET](https://github.com/microsoftgraph/msgraph-sdk-dotnet) (SDK) para interagir com o Microsoft Graph. O SDK da MSAL fornece recursos para trabalhar com o [ponto de extremidade do Microsoft Azure AD versão 2.0](https://azure.microsoft.com/en-us/documentation/articles/active-directory-appmodel-v2-overview), que permite aos desenvolvedores gravar um único fluxo de código para tratar da autenticação de contas pessoais (Microsoft), corporativas ou de estudantes (Azure Active Directory).
O exemplo usa apenas permissões do representante, portanto, não requer consentimento do administrador.

> A versão anterior deste exemplo que usa a versão ASP.NET Core 1.1 está [aqui](https://github.com/microsoftgraph/aspnetcore-connect-sample/tree/netcore1.1) e a versão ASP.NET Core 2.0 [aqui](https://github.com/microsoftgraph/aspnetcore-connect-sample/tree/netcore2.0).

## Sumário

- [Pré-requisitos](#prerequisites)
- [Registrar o aplicativo](#register-the-app)
- [Configurar e executar o exemplo](#configure-and-run-the-sample)
- [Componentes principais do exemplo](#key-components-of-the-sample)
- [Colaboração](#contributing)
- [Perguntas e comentários](#questions-and-comments)
- [Recursos adicionais](#additional-resources)

## Diferenças entre ADAL e MSAL

ADAL (Azure AD v1.0) e MSAL (Azure AD v2.0) são bibliotecas de autenticação para uma ampla variedade de idiomas, que permitem adquirir tokens do Azure AD, acessar APIs da Web protegidas (APIs da Microsoft ou aplicativos registrados com Azure Active Directory). Os aplicativos ADAL permitem que os usuários entrem com sua conta corporativa e de estudante e precisam ser registrados no [portal do Azure](https://portal.azure.com/), enquanto os aplicativos que usam a nova MSAL (versão prévia) permitem que os usuários entrem com: suas contas corporativas e de estudante ou pessoais e precisam ser registrados no [portal de registro de aplicativos](https://apps.dev.microsoft.com/), a menos que sejam aplicativos do Azure AD B2C. A ADAL e a MSAL têm suas bibliotecas de clientes .NET: [ADAL.NET](https://github.com/AzureAD/azure-activedirectory-library-for-dotnet) e [MSAL.NET](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet), respectivamente. [Saiba mais sobre a migração e as diferenças entre ADAL.NET e MSAL.NET aqui.](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/adal-to-msal)

## Pré-requisitos

Para usar o exemplo de conexão do Microsoft Graph para ASP.NET Core 2.1, você precisará do seguinte:

- Visual Studio 2017 [com .NET Core 2.1 SDK](https://www.microsoft.com/net/download/core) instalado no computador de desenvolvimento.
- Uma [conta pessoal da Microsoft](https://signup.live.com) ou uma [conta corporativa ou de estudante](https://dev.office.com/devprogram). (Você não precisa ser um administrador do locatário.)
- A ID do aplicativo e a chave do aplicativo que você [registra no portal de registro do aplicativo](#register-the-app).

## Registrar o aplicativo

1. Navegue até o [Portal do Azure AD](https://portal.azure.com). Faça logon usando uma **conta pessoal** (também conhecida como: Conta da Microsoft) ou **conta corporativa ou de estudante** com permissões para criar registros de aplicativos.

   > **Observação:** Se você não tiver permissões para criar registros de aplicativos, entre em contato com os administradores de domínio do Azure AD.

2. Clique em **Azure Active Directory** no menu de navegação à esquerda.

3. Clique em **Registros de aplicativo** no painel de navegação atual do blade.

4. Clique em **Novo registro** no painel de navegação atual do blade.

5. Na página **Registrar um aplicativo**, especifique os valores a seguir:

   - **Nome** = [nome do aplicativo desejado]
   - **Tipos de conta com suporte** = [escolha o valor que se aplica às suas necessidades]
   - **URI de redirecionamento**
     - Tipo (lista suspensa) = Web
     - Valor = `https://localhost:44334/signin-oidc`

   > **Observação:** Certifique-se de que o valor do URI de redirecionamento seja exclusivo em seu domínio. Esse valor pode ser alterado posteriormente e não precisa direcionar para um URI hospedado. Se o exemplo de URI acima já estiver em uso, escolha um valor exclusivo.

   1. Em **Configurações avançadas**, defina o valor da **URL de logoff** como `https://localhost:44334/Account/SignOut`
   2. Copie o **URI de redirecionamento**, pois você precisará dele mais tarde.

6. Depois que o aplicativo for criado, copie a **ID do aplicativo (cliente)** e a **ID do diretório (locatário)** da página de visão geral e armazene-as temporariamente, pois você precisará delas mais tarde.

7. Clique em **Certificados e segredos** no painel de navegação atual do blade.

   1. Clique em **Novo segredo do cliente**.
   2. Na caixa de diálogo **Adicionar um segredo do cliente**, especifique os seguintes valores:

      - **Descrição** = MyAppSecret1
      - **Expira em** = 1 ano

   3. Clique em **Adicionar**.

   4. Depois que a tela for atualizada com o segredo do cliente recém-criado, copie o **VALOR** do segredo do cliente e armazene-o temporariamente, pois você precisará dele mais tarde.

      > **Importante:**
      > Essa cadeia de caracteres secreta nunca será exibida novamente, portanto, não deixe de copiá-la agora. Nos aplicativos de produção você sempre deve usar os certificados como segredos do seu aplicativo, mas, neste exemplo, usaremos uma senha secreta simples compartilhada.

8. Clique em **Autenticação** no painel de navegação atual do blade.
   1. Selecione “tokens de ID”
9. Clique em **Permissões de API** no painel de navegação atual do blade.

   1. Clique em **Adicionar uma permissão** no painel de navegação atual do blade.
   2. No painel **Solicitar permissões de API**, selecione **Microsoft Graph**.

   3. Selecione **Permissões delegadas**.
   4. Na caixa de pesquisa “Selecionar permissões”, digite “Usuário”.
   5. Selecione **openid**, **email**, **profile**, **offline_access**, **User.Read**, **User.ReadBasic.All** e **Mail.Send**.

   6. Clique em **Adicionar permissões** na parte inferior do submenu.

   > **Observação:** A Microsoft recomenda que você liste explicitamente todas as permissões delegadas ao registrar seu aplicativo. Embora os recursos de consentimento incremental e dinâmico do ponto de extremidade v2 tornem essa etapa opcional, deixar de fazer isso pode afetar negativamente a autorização do administrador.

## Configurar e executar o exemplo

1. Baixe ou clone o Exemplo de Conexão com o Microsoft Graph para ASP.NET Core.

2. Abra o arquivo de exemplo **MicrosoftGraphAspNetCoreConnectSample.sln** no Visual Studio 2017.

3. No Gerenciador de Soluções, abra o arquivo **appsettings.json** na pasta raiz do projeto.

   a. Na chave **AppId**, substitua `ENTER_YOUR_APP_ID` com a ID do aplicativo de seu aplicativo registrado.

   b. Na chave **AppSecret**, substitua `ENTER_YOUR_SECRET` com a senha de seu aplicativo registrado. Observe que nos aplicativos de produção você sempre deve usar os certificados como segredos do seu aplicativo, mas, neste exemplo, usaremos uma senha de secreta simples compartilhada.

4. Pressione F5 para criar e executar o exemplo. Isso restaurará dependências do pacote NuGet e abrirá o aplicativo.

   > Caso receba mensagens de erro durante a instalação de pacotes, verifique se o caminho para o local onde você colocou a solução não é muito longo ou extenso. Para resolver esse problema, coloque a solução junto à raiz da unidade.

5. Entre com sua conta pessoal (MSA) ou com sua conta comercial ou de estudante e conceda as permissões solicitadas.

6. Você deve ver sua foto do perfil e os dados do seu perfil em JSON na página inicial.

7. Altere o endereço de email na caixa para o email de outra conta válida no mesmo locatário e escolha o botão **Carregar dados**. Quando a operação for concluída, o perfil do usuário escolhido será exibido na página.

8. Como alternativa, edite a lista de destinatários e, em seguida, escolha o botão **Enviar email**. Quando o email for enviado, será exibida uma mensagem de sucesso na parte superior da página.

## Componentes principais do exemplo

Os seguintes arquivos contêm códigos relacionados à conexão com o Microsoft Graph, ao carregamento de dados do usuário e ao envio de emails.

- [`appsettings.json`](MicrosoftGraphAspNetCoreConnectSample/appsettings.json) Contém valores usados para autenticação e autorização.
- [`Startup.cs`](MicrosoftGraphAspNetCoreConnectSample/Startup.cs) Configura o aplicativo e os serviços que ele usa, incluindo autenticação.

### Controladores

- [`AccountController.cs`](MicrosoftGraphAspNetCoreConnectSample/Controllers/AccountController.cs) Lida com a entrada e a saída.
- [`HomeController.cs`](MicrosoftGraphAspNetCoreConnectSample/Controllers/HomeController.cs) Lida com as solicitações da interface do usuário.

### Exibições

- [`Index.cshtml`](MicrosoftGraphAspNetCoreConnectSample/Views/Home/Index.cshtml) Contém a interface de usuário do exemplo.

### Auxiliares

- [`GraphAuthProvider.cs`](MicrosoftGraphAspNetCoreConnectSample/Helpers/GraphAuthProvider.cs) Obtém um token de acesso usando o método **AcquireTokenSilent** da MSAL.
- [`GraphSdkHelper.cs`](MicrosoftGraphAspNetCoreConnectSample/Helpers/GraphSDKHelper.cs) Inicia o cliente SDK usado para interagir com o Microsoft Graph.
- [`GraphService.cs`](MicrosoftGraphAspNetCoreConnectSample/Helpers/GraphService.cs) Contém métodos que usam o **GraphServiceClient** para criar e enviar chamadas para o serviço do Microsoft Graph e processar a resposta.
  - A ação **GetUserJson** obtém o perfil do usuário por um endereço de email e o converte em JSON.
  - A ação **GetPictureBase64** obtém a imagem do perfil do usuário e a converte em uma cadeia de caracteres base64.
  - A ação **SendMail** envia um email em nome do usuário atual.

## Colaboração

Se quiser contribuir para esse exemplo, confira [CONTRIBUTING.MD](/CONTRIBUTING.md).

Este projeto adotou o [Código de Conduta de Código Aberto da Microsoft](https://opensource.microsoft.com/codeofconduct/).  Para saber mais, confira as [Perguntas frequentes sobre o Código de Conduta](https://opensource.microsoft.com/codeofconduct/faq/) ou entre em contato pelo [opencode@microsoft.com](mailto:opencode@microsoft.com) se tiver outras dúvidas ou comentários.

## Perguntas e comentários

Adoraríamos receber seus comentários sobre o exemplo de conexão do Microsoft Graph para ASP.NET Core. Você pode enviar perguntas e sugestões na seção [Problemas](https://github.com/microsoftgraph/aspnetcore-connect-sample/issues) deste repositório.

Em geral, as perguntas sobre o Microsoft Graph devem ser postadas no [Stack Overflow](https://stackoverflow.com/questions/tagged/MicrosoftGraph). Não deixe de marcar as perguntas ou comentários com _[MicrosoftGraph]_.

Você pode sugerir alterações no Microsoft Graph em [UserVoice](https://officespdev.uservoice.com/).

## Recursos adicionais

- [Documentação do Microsoft Graph](https://developer.microsoft.com/graph)
- [Outros exemplos de conexão usando o Microsoft Graph](https://github.com/MicrosoftGraph?q=connect)
- [Exemplo de webhooks do Microsoft Graph para ASP.NET Core](https://github.com/microsoftgraph/aspnetcore-apponlytoken-webhooks-sample)
- [Exemplo de conexão com o Microsoft Graph para ASP.NET 4.6](https://github.com/microsoftgraph/aspnet-connect-sample)

## Direitos autorais

Copyright (c) 2019 Microsoft. Todos os direitos reservados.
