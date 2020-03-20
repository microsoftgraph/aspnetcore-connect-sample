---
page_type: sample
description: "Utilisez ASP.NET Core 2.1 MVC pour vous connecter à Microsoft Graph à l’aide du flux d’autorisations déléguées."
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
# Exemple de connexion avec Microsoft Graph pour ASP.NET Core 2.1

![Capture d’écran d’exemple de connexion avec Microsoft Graph pour ASP.NET Core 2.1](readme-images/Page1.PNG)

**Scénario** : Utilisez ASP.NET Core 2.1 MVC pour vous connecter à Microsoft Graph à l’aide du flux d’autorisations déléguées et ainsi récupérer le profil d’un utilisateur, sa photo à partir d’un point de terminaison Azure AD (v 2.0), puis envoyez un courrier électronique contenant la photo en pièce jointe.

L’exemple utilise OpenID Connect pour la connexion, [la bibliothèque d’authentification Microsoft Authentication Library (MSAL) pour .NET](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet) pour obtenir un jeton d’accès et la [bibliothèque de client Microsoft Graph pour .NET](https://github.com/microsoftgraph/msgraph-sdk-dotnet) (SDK) pour interagir avec Microsoft Graph. Le kit de développement logiciel (SDK) MSAL offre des fonctionnalités permettant d’utiliser le [point de terminaison Azure AD v2.0](https://azure.microsoft.com/en-us/documentation/articles/active-directory-appmodel-v2-overview), qui permet aux développeurs d’écrire un flux de code unique qui gère l’authentification des comptes professionnels ou scolaires (Azure Active Directory) et personnels (Microsoft).
L’exemple utilise uniquement des autorisations déléguées. Par conséquent, il ne nécessite pas d’accord de l’administrateur.

> La version précédente de cet exemple qui utilise la version ASP.NET Core 1.1 est [ici](https://github.com/microsoftgraph/aspnetcore-connect-sample/tree/netcore1.1) et la version ASP.NET Core 2.0 [ici](https://github.com/microsoftgraph/aspnetcore-connect-sample/tree/netcore2.0).

## Table des matières

- [Conditions préalables](#prerequisites)
- [Inscription de l’application](#register-the-app)
- [Configurer et exécuter l’exemple](#configure-and-run-the-sample)
- [Composants clés de l’exemple](#key-components-of-the-sample)
- [Contribution](#contributing)
- [Questions et commentaires](#questions-and-comments)
- [Ressources supplémentaires](#additional-resources)

## Différences entre ADAL et MSAL

ADAL (Azure AD v 1.0) et MSAL (Azure AD v 2.0) sont toutes deux des bibliothèques d’authentification pour une grande variété de langages, ce qui vous permet d’obtenir des jetons à partir d’Azure AD afin d’accéder aux API Web protégées (API Microsoft ou applications inscrites avec Azure Active Directory). Les applications ADAL permettent aux utilisateurs de se connecter à l’aide de leurs comptes professionnels ou scolaires et doivent être enregistrés dans le [portail Azure](https://portal.azure.com/), tandis que les applications qui utilisent la nouvelle bibliothèque MSAL (dans la version d’évaluation) permettent aux utilisateurs de se connecter avec leur compte professionnel ou scolaire, ou leurs comptes personnels, et doivent être enregistrés dans le [portail d’inscription des applications](https://apps.dev.microsoft.com/), sauf s’il s'agit d’applications AD B2C. Les bibliothèques ADAL et MSAL possèdent toutes deux leurs bibliothèques clientes .NET : [ADAL.NET](https://github.com/AzureAD/azure-activedirectory-library-for-dotnet) et [MSAL.NET](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet), respectivement. [Pour en savoir plus sur la migration et les différences entre ADAL.NET et MSAL.NET, cliquez ici.](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/adal-to-msal)

## Conditions préalables

Pour utiliser l’exemple de connexion Microsoft Graph pour ASP.NET Core 2.1, vous avez besoin des éléments suivants :

- Visual Studio 2017 [avec .NET Core 2.1 SDK](https://www.microsoft.com/net/download/core) installé sur votre ordinateur de développement.
- Soit un [compte Microsoft personnel](https://signup.live.com), soit à un [compte scolaire ou professionnel](https://dev.office.com/devprogram). (Vous n’avez pas besoin d’être administrateur du client).
- ID de l’application et clé de l’application que vous [inscrivez sur le portail d’enregistrement d’application](#register-the-app).

## Inscription de l’application

1. Accédez au portail [Azure AD](https://portal.azure.com). Connectez-vous à l’aide d’un **compte personnel** (à savoir : un compte Microsoft) ou **compte professionnel ou scolaire** avec les autorisations pour créer des inscriptions d’applications.

   > **Remarque :** Si vous n’êtes pas autorisé à créer des inscriptions d’applications, contactez vos administrateurs de domaine Azure AD.

2. Cliquez sur **Azure Active Directory** dans le menu de navigation de gauche.

3. Cliquez sur **Inscriptions des applications** dans le volet de navigation à panneaux actuel.

4. Cliquez sur **Nouvelle inscription** à partir du contenu du panneau actuel.

5. Sur la page **Inscrire une application**, spécifiez les valeurs suivantes :

   - **Nom** = [nom de l’application souhaitée]
   - **Types de compte pris en charge** = [Choisissez la valeur qui s’applique à vos besoins]
   - **URI de redirection**
     - Type (liste déroulante) = Web
     - Valeur = `https://localhost:44334/signin-oidc`

   > **Remarque :** Assurez-vous que la valeur d’URI de redirection est unique au sein de votre domaine. Cette valeur peut être modifiée ultérieurement et ne doit pas nécessairement pointer vers un URI hébergé. Si l’exemple d’URI ci-dessus est déjà utilisé, choisissez une valeur unique.

   1. Sous **Paramètres avancés**, configurez la valeur de l’**URL de déconnexion** sur `https://localhost:44334/Account/SignOut`
   2. Copiez l'**URI de redirection** car vous en aurez besoin plus tard.

6. Une fois l’application créée, copiez l’**ID d’application (client)** et l’**ID de répertoire (client)** à partir de la page de présentation et stockez-le temporairement car vous aurez besoin de ces deux éléments plus tard.

7. Cliquez sur **Certificats et clés secrètes** dans le volet de navigation à panneaux actuel.

   1. Cliquez sur **Nouvelle clé secrète client**.
   2. Dans la boîte de dialogue **Ajouter une clé secrète client**, spécifiez les valeurs suivantes :

      - **Description** = MyAppSecret1
      - **Expire** = dans 1 an

   3. Cliquez sur **Ajouter**.

   4. Une fois l’écran mis à jour avec la nouvelle clé secrète client, copiez la **VALEUR** de la clé secrète client, puis stockez-la temporairement car vous en aurez besoin plus tard.

      > **Important :**
      > Cette chaîne secrète client n’apparaîtra plus jamais ; veillez donc à la copier maintenant. Dans les applications de production, vous devez toujours utiliser des certificats comme secrets d’application. pour cet exemple, nous allons utiliser un mot de passe secret partagé simple.

8. Cliquez sur **Authentification** dans le volet de navigation à panneaux actuel.
   1. Sélectionnez 'jetons d’ID'
9. Cliquez sur **Autorisations d’API** dans le volet de navigation à panneaux actuel.

   1. Cliquez sur **Ajouter une autorisation** à partir du contenu du panneau actuel.
   2. Dans le panneau **Demander des autorisations d’API**, sélectionnez **Microsoft Graph**.

   3. Sélectionnez **Autorisations déléguées**.
   4. Dans la zone de recherche « Sélectionner les autorisations », tapez « utilisateur ».
   5. Sélectionnez **openid**, **email**, **profile**, **offline_access**, **User.Read**, **User.ReadBasic.All** et **Mail.Send**.

   6. Cliquez sur **Ajouter des autorisations** en bas du menu volant.

   > **Remarque :** Microsoft vous recommande de répertorier explicitement toutes les autorisations déléguées lors de l’inscription de votre application. Bien que les fonctionnalités de consentement incrémentiel et dynamique du point de terminaison v 2 rendent cette étape facultative, la non-exécution de celle-ci peut avoir une incidence négative sur le consentement de l’administrateur.

## Configurer et exécuter l’exemple

1. Téléchargez ou clonez l’exemple de connexion Microsoft Graph pour ASP.NET Core.

2. Ouvrez l’exemple de fichier **MicrosoftGraphAspNetCoreConnectSample.sln** dans Visual Studio 2017.

3. Dans l’Explorateur de solutions, ouvrez le fichier **appsettings.json** dans le répertoire racine du projet.

   a. Pour la clé **AppId**, remplacez `ENTER_YOUR_APP_ID` par l’ID d’application de votre application inscrite.

   b. Pour la clé **AppSecret**, remplacez `ENTER_YOUR_SECRET` par le mot de passe de votre application inscrite. Notez que dans les applications de production, vous devez toujours utiliser des certificats comme secrets d’application. pour cet exemple, nous allons utiliser un mot de passe secret partagé simple.

4. Appuyez sur F5 pour créer et exécuter l’exemple. Cela entraîne la restauration des dépendances du package NuGet et l’ouverture de l’application.

   > Si vous constatez des erreurs pendant l’installation des packages, vérifiez que le chemin d’accès local où vous avez sauvegardé la solution n’est pas trop long/profond. Pour résoudre ce problème, il vous suffit de déplacer la solution dans un dossier plus près du répertoire racine de votre lecteur.

5. Connectez-vous à votre compte (MSA) personnel ou à votre compte professionnel ou scolaire, puis accordez les autorisations demandées.

6. Votre image de profil et vos données de profil apparaîtront normalement dans JSON sur la page de démarrage.

7. Modifiez l’adresse électronique dans la zone afin d’utiliser l’adresse électronique d’un autre compte valide dans le même client, puis sélectionnez le bouton **Charger les données**. Une fois l’opération terminée, le profil de l’utilisateur choisi s’affiche dans la page.

8. Vous pouvez également modifier la liste des destinataires, puis cliquer sur le bouton **Envoyer un message électronique**. Lorsque le message est envoyé, un message de réussite s’affiche en haut de la page.

## Composants clés de l’exemple

Les fichiers suivants contiennent du code lié à la connexion à Microsoft Graph, au chargement des données de l’utilisateur et à l’envoi d’e-mails.

- [`appsettings.json`](MicrosoftGraphAspNetCoreConnectSample/appsettings.json) Contient les valeurs utilisées pour l’authentification et l’autorisation.
- [`Startup.cs`](MicrosoftGraphAspNetCoreConnectSample/Startup.cs) Configure l’application et les services qu’elle utilise, y compris l’authentification.

### Contrôleurs

- [`AccountController.cs`](MicrosoftGraphAspNetCoreConnectSample/Controllers/AccountController.cs) Gère la connexion et la déconnexion.
- [`HomeController.cs`](MicrosoftGraphAspNetCoreConnectSample/Controllers/HomeController.cs) Gère les demandes de l’interface utilisateur.

### Affichages

- [`Index.cshtml`](MicrosoftGraphAspNetCoreConnectSample/Views/Home/Index.cshtml) Contient l’interface utilisateur de l’exemple.

### Assistants

- [`GraphAuthProvider.cs`](MicrosoftGraphAspNetCoreConnectSample/Helpers/GraphAuthProvider.cs) Obtient un jeton d’accès à l’aide de la méthode **AcquireTokenSilent** de la bibliothèque MSAL.
- [`GraphSdkHelper.cs`](MicrosoftGraphAspNetCoreConnectSample/Helpers/GraphSDKHelper.cs) Initialise le client du kit de développement logiciel (SDK) utilisé pour interagir avec Microsoft Graph.
- [`GraphService.cs`](MicrosoftGraphAspNetCoreConnectSample/Helpers/GraphService.cs) Contient des méthodes qui utilisent **GraphServiceClient** pour créer et envoyer les appels au service Microsoft Graph et traiter la réponse.
  - L’action **GetUserJson** obtient le profil de l’utilisateur à l’aide d’une adresse de messagerie et la convertit au format JSON.
  - L’action **GetPictureBase64** obtient l’image de profil de l’utilisateur et la convertit en chaîne base64.
  - L’action **SendMail** envoie un e-mail au nom de l’utilisateur actuel.

## Contribution

Si vous souhaitez contribuer à cet exemple, voir [CONTRIBUTING.MD](/CONTRIBUTING.md).

Ce projet a adopté le [code de conduite Open Source de Microsoft](https://opensource.microsoft.com/codeofconduct/). Pour en savoir plus, reportez-vous à la [FAQ relative au code de conduite](https://opensource.microsoft.com/codeofconduct/faq/) ou contactez [opencode@microsoft.com](mailto:opencode@microsoft.com) pour toute question ou tout commentaire.

## Questions et commentaires

Nous serions ravis de connaître votre opinion sur l’exemple de connexion Microsoft Graph pour ASP.NET Core. Vous pouvez nous faire part de vos questions et suggestions dans la rubrique [Problèmes](https://github.com/microsoftgraph/aspnetcore-connect-sample/issues) de ce référentiel.

Les questions générales sur Microsoft Graph doivent être publiées sur la page [Dépassement de capacité de la pile](https://stackoverflow.com/questions/tagged/MicrosoftGraph). Veillez à poser vos questions ou à rédiger vos commentaires en utilisant les tags _[MicrosoftGraph]_.

Vous pouvez suggérer des modifications pour Microsoft Graph sur [UserVoice](https://officespdev.uservoice.com/).

## Ressources supplémentaires

- [Documentation Microsoft Graph](https://developer.microsoft.com/graph)
- [Autres exemples de connexion avec Microsoft Graph](https://github.com/MicrosoftGraph?q=connect)
- [Exemple de Webhooks Microsoft Graph pour ASP.NET Core](https://github.com/microsoftgraph/aspnetcore-apponlytoken-webhooks-sample)
- [Exemple de connexion de Microsoft Graph pour ASP.NET 4.6](https://github.com/microsoftgraph/aspnet-connect-sample)

## Copyright

Copyright (c) 2019 Microsoft. Tous droits réservés.
