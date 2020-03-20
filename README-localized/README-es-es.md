---
page_type: sample
description: "Utilice ASP.NET Core 2.1 MVC para conectarse a Microsoft Graph utilizando el flujo de permisos delegados."
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
# Ejemplo para conectarse a Microsoft Graph para ASP.NET Core 2.1

![Captura de pantalla del ejemplo para conectarse a Microsoft Graph para ASP.NET Core 2.1](readme-images/Page1.PNG)

**Escenario**: Utilice ASP.NET Core 2.1 MVC para conectarse a Microsoft Graph con el flujo de permisos delegados para recuperar el perfil de un usuario, la foto del punto de conexión de Azure AD (v 2.0) y, a continuación, envíe un correo electrónico que contenga la foto como datos adjuntos.

El ejemplo usa OpenID Connect para iniciar sesión, la [Biblioteca de autenticación de Microsoft (MSAL) para .NET](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet) para obtener un token de acceso y la [Biblioteca cliente de Microsoft Graph para .NET](https://github.com/microsoftgraph/msgraph-sdk-dotnet) (SDK) con el fin de interactuar con Microsoft Graph. El SDK de MSAL ofrece características para trabajar con el [punto de conexión Azure AD v2.0](https://azure.microsoft.com/en-us/documentation/articles/active-directory-appmodel-v2-overview), lo que permite a los desarrolladores escribir un flujo de código único que controla la autenticación para las cuentas profesionales, educativas (Azure Active Directory) o las cuentas personales (Microsoft).
El ejemplo usa solo permisos de delegado, por lo que no necesita consentimiento de administrador.

> La versión anterior de este ejemplo que usa la versión ASP.NET Core 1.1 está [aquí](https://github.com/microsoftgraph/aspnetcore-connect-sample/tree/netcore1.1) y la versión ASP.NET Core 2.0 está [aquí](https://github.com/microsoftgraph/aspnetcore-connect-sample/tree/netcore2.0).

## Tabla de contenido

- [Requisitos previos](#prerequisites)
- [Registrar la aplicación](#register-the-app)
- [Configurar y ejecutar el ejemplo](#configure-and-run-the-sample)
- [Componentes clave del ejemplo](#key-components-of-the-sample)
- [Colaboradores](#contributing)
- [Preguntas y comentarios](#questions-and-comments)
- [Recursos adicionales](#additional-resources)

## Diferencias entre ADAL y MSAL

ADAL (Azure AD v 1.0) y MSAL (Azure AD v 2.0) son dos bibliotecas de autenticación para una amplia variedad de idiomas, lo que le permite adquirir tokens de Azure AD para obtener acceso a las API web protegidas (API de Microsoft o aplicaciones registradas con Azure Active Directory). Las aplicaciones de ADAL permiten que los usuarios inicien sesión con su cuenta profesional y educativa y deben registrarse en [Azure Portal](https://portal.azure.com/), mientras que las aplicaciones que usan la nueva (versión preliminar) MSAL, permiten que los usuarios inicien sesión con sus cuentas profesionales o educativas, o con sus cuentas personales y deben registrarse en el [portal de registro de aplicaciones](https://apps.dev.microsoft.com/), a menos que sean aplicaciones Azure AD B2C. Tanto ADAL como MSAL tienen sus bibliotecas de cliente de .NET: [ADAL.NET](https://github.com/AzureAD/azure-activedirectory-library-for-dotnet) y [MSAL.NET](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet) respectivamente. [Obtenga más información sobre la migración y las diferencias entre ADAL.NET y MSAL.NET aquí.](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/adal-to-msal)

## Requisitos previos

Para usar el ejemplo conexión de Microsoft Graph para ASP.NET Core 2.1, necesita lo siguiente:

- Visual Studio 2017[ con .NET Core 2.1 SDK](https://www.microsoft.com/net/download/core) instalado en el equipo de desarrollo.
- Una [cuenta de Microsoft personal](https://signup.live.com) o una [cuenta profesional o educativa](https://dev.office.com/devprogram). (No tiene que ser administrador del espacio empresarial).
- El Id. de la aplicación y la clave que [registró en el portal de registro de aplicaciones](#register-the-app).

## Registrar la aplicación

1. Vaya a [Azure AD Portal](https://portal.azure.com). Inicie sesión con una **cuenta personal** (por ejemplo: cuenta de Microsoft) o **cuenta profesional o educativa** con permisos para crear registros de aplicación.

   > **Nota:** Si no tiene permisos para crear registros de aplicaciones, póngase en contacto con su administrador de dominio de Azure AD.

2. Haga clic en **Azure Active Directory** en el menú de navegación izquierdo.

3. Haga clic en **Registro de aplicaciones** en el panel de navegación de la hoja actual.

4. Haga clic en **Registro nuevo** en el contenido de la hoja actual.

5. En la página **Registrar una aplicación**, especifique los siguientes valores:

   - **Nombre** = [nombre de la aplicación deseada]
   - **Tipos de cuenta compatibles** = [Elija el valor que se aplica a sus necesidades]
   - **URI de redirección**
     - Tipo (lista desplegable) = Web
     - Valor = `https://localhost:44334/signin-oidc`

   > **Nota:** Asegúrese de que el valor de la dirección URI de redirección es único en el dominio. Este valor se puede cambiar posteriormente y no es necesario que apunte a un URI alojado. Si el URI de ejemplo anterior ya está en uso, elija un valor único.

   1. En **opciones avanzadas**, establezca el valor del **URL de cierre de sesión** en `https://localhost:44334/Account/SignOut`
   2. Copie el **URI de redirección**, ya que lo necesitará más adelante.

6. Una vez que se haya creado la aplicación, copie la **Id. de aplicación (cliente)** y la **identificación de directorio (espacio empresarial)** de la página de información general y almacénelas temporalmente, ya que las necesitará más adelante.

7. Haga clic en **Certificados y secretos** en el panel de navegación de la hoja actual.

   1. Haga clic en **Nuevo secreto de cliente**.
   2. En el cuadro de diálogo **agregar un secreto de cliente**, especifique los siguientes valores:

      - **Descripción** = MyAppSecret1
      - **Expira** = en 1 día

   3. Haga clic en **Agregar**.

   4. Después de que la pantalla se haya actualizado con el secreto del cliente recién creado, copie el **VALOR** del secreto del cliente y guárdelo temporalmente, ya que lo necesitará más adelante.

      > **Importante:**
      > El secreto de cliente no se vuelve a mostrar, así que asegúrese de copiarlo en este momento. En las aplicaciones de producción siempre debe usar certificados como secretos de aplicación, pero para este ejemplo usaremos una contraseña secreta compartida simple.

8. Haga clic en **Autenticación** en el panel de navegación de la hoja actual.
   1. Seleccione "Tokens de identificador"
9. Haga clic en **Permisos de API** en el panel de navegación de la hoja actual.

   1. Haga clic en **Agregar un permiso** en el contenido de la hoja actual.
   2. En el panel **Solicitar permisos de API** seleccione **Microsoft Graph**.

   3. Seleccione **Permisos delegados**.
   4. En el cuadro de búsqueda "Seleccionar permisos", escriba "usuario".
   5. Seleccione **OpenID**, **correo electrónico**, **perfil**, **offline_access**, **User.Read**, **User.ReadBasic.All** y **Mail.Send**.

   6. Haga clic en **Agregar permisos** en la parte inferior del control flotante.

   > **Nota:** Microsoft recomienda que enumere explícitamente todos los permisos delegados al registrar la aplicación. Aunque las funciones de consentimiento incremental y dinámico del punto de conexión V2 hacen que este paso sea opcional, no hacerlo puede afectar negativamente el consentimiento del administrador.

## Configurar y ejecutar el ejemplo

1. Descargue o clone el ejemplo para conectarse a Microsoft Graph para ASP.NET Core.

2. Abra el archivo de ejemplo **MicrosoftGraphAspNetCoreConnectSample.sln** en Visual Studio 2017.

3. En el Explorador de soluciones, abra el archivo **appsettings.json** en el directorio raíz del proyecto.

   A. Para la clave **AppID**, reemplace `ENTER_YOUR_APP_ID` con el Id. de la aplicación de la aplicación registrada.

   B. Para la clave **AppSecret**, reemplace `ENTER_YOUR_SECRET` con la contraseña de la aplicación registrada. Tenga en cuenta que en las aplicaciones de producción siempre debe usar certificados como secretos de aplicación, pero para este ejemplo usaremos una contraseña secreta compartida simple.

4. Pulse F5 para compilar y ejecutar el ejemplo. Esto restaurará las dependencias de paquetes de NuGet y abrirá la aplicación.

   > Si observa algún error durante la instalación de los paquetes, asegúrese de que la ruta de acceso local donde colocó la solución no es demasiado larga o profunda. Para resolver este problema, mueva la solución más cerca de la raíz de la unidad.

5. Inicie sesión con su cuenta personal (MSA) o su cuenta profesional o educativa y otorgue los permisos solicitados.

6. Debe ver la imagen de perfil y los datos de perfil en JSON en la página de inicio.

7. Cambie la dirección de correo electrónico en el cuadro al correo electrónico de otra cuenta válida en el mismo espacio empresarial y elija el botón **cargar datos**. Cuando la operación se completa, el perfil del usuario elegido se muestra en la página.

8. Si lo desea, modifique la lista de destinatarios y, después, seleccione el botón **Enviar correo electrónico**. Cuando se envía el correo, se muestra un mensaje de Éxito en la parte superior de la página.

## Componentes clave del ejemplo

Los siguientes archivos contienen código relacionado con la conexión a Microsoft Graph, la carga de datos del usuario y el envío de correos electrónicos.

- [`appsettings.json`](MicrosoftGraphAspNetCoreConnectSample/appsettings.json) contiene los valores utilizados para autenticación y autorización.
- [`Startup.cs`](MicrosoftGraphAspNetCoreConnectSample/Startup.cs) configura la aplicación y los servicios que utiliza, incluida la autenticación.

### Controladores

- [`AccountController.cs`](MicrosoftGraphAspNetCoreConnectSample/Controllers/AccountController.cs) controla el inicio y el cierre de sesión.
- [`HomeController.cs`](MicrosoftGraphAspNetCoreConnectSample/Controllers/HomeController.cs) controla las solicitudes de la interfaz de usuario.

### Vistas

- [`Index.cshtml`](MicrosoftGraphAspNetCoreConnectSample/Views/Home/Index.cshtml) contiene la interfaz de usuario del ejemplo.

### Aplicaciones auxiliares

- [`GraphAuthProvider.cs`](MicrosoftGraphAspNetCoreConnectSample/Helpers/GraphAuthProvider.cs) obtiene un token de acceso utilizando el método **AcquireTokenSilent** de MSAL.
- [`GraphSdkHelper.cs`](MicrosoftGraphAspNetCoreConnectSample/Helpers/GraphSDKHelper.cs) inicia el cliente SDK usado para interactuar con Microsoft Graph.
- [`GraphService.cs`](MicrosoftGraphAspNetCoreConnectSample/Helpers/GraphService.cs) contiene métodos que usan **GraphServiceClient** para generar y enviar llamadas al servicio Microsoft Graph y procesar la respuesta.
  - La acción **GetUserJson** obtiene el perfil de usuario con una dirección de correo electrónico y lo convierte en JSON.
  - La acción **GetPictureBase64** obtiene la imagen de perfil del usuario y la convierte en una cadena base64.
  - La acción **SendEmail** envía un correo electrónico en nombre del usuario actual.

## Colaboradores

Si quiere hacer su aportación a este ejemplo, vea [CONTRIBUTING.MD](/CONTRIBUTING.md).

Este proyecto ha adoptado el [Código de conducta de código abierto de Microsoft](https://opensource.microsoft.com/codeofconduct/). Para obtener más información, vea [Preguntas frecuentes sobre el código de conducta](https://opensource.microsoft.com/codeofconduct/faq/) o póngase en contacto con [opencode@microsoft.com](mailto:opencode@microsoft.com) si tiene otras preguntas o comentarios.

## Preguntas y comentarios

Nos encantaría recibir sus comentarios sobre el ejemplo para conectarse a Microsoft Graph para ASP.NET Core. Puede enviarnos sus preguntas y sugerencias a través de la sección [Problemas](https://github.com/microsoftgraph/aspnetcore-connect-sample/issues) de este repositorio.

Las preguntas sobre Microsoft Graph en general deben publicarse en [desbordamiento de pila](https://stackoverflow.com/questions/tagged/MicrosoftGraph). Asegúrese de que sus preguntas o comentarios estén etiquetados con _[MicrosoftGraph]_.

Puede sugerir cambios para Microsoft Graph en [UserVoice](https://officespdev.uservoice.com/).

## Recursos adicionales

- [Documentación de Microsoft Graph](https://developer.microsoft.com/graph)
- [Otros ejemplos para conectarse a Microsoft Graph](https://github.com/MicrosoftGraph?q=connect)
- [Ejemplo de Webhooks de Microsoft Graph para ASP.NET Core](https://github.com/microsoftgraph/aspnetcore-apponlytoken-webhooks-sample)
- [Ejemplo para conectarse a Microsoft Graph para ASP.NET 4.6](https://github.com/microsoftgraph/aspnet-connect-sample)

## Derechos de autor

Copyright (c) 2019 Microsoft. Todos los derechos reservados.
