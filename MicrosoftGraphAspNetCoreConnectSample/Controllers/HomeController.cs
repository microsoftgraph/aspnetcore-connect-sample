using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicrosoftGraphAspNetCoreConnectSample.Helpers;
using Microsoft.Extensions.Configuration;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Graph;

namespace MicrosoftGraphAspNetCoreConnectSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IGraphSDKHelper _graphSdkHelper;

        public HomeController(IConfiguration configuration, IGraphSDKHelper graphSdkHelper)
        {
            _configuration = configuration;
            _graphSdkHelper = graphSdkHelper;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string email)
        {
            if (User.Identity.IsAuthenticated)
            {
                var identifier = User.FindFirst(Startup.ObjectIdentifierType)?.Value;
                email = email ?? User.Identity.Name;
                ViewData["Email"] = email;

                // Get Graph client
                GraphServiceClient graphClient = _graphSdkHelper.GetAuthenticatedClient(identifier);

                try
                {
                    var user = await graphClient.Users[email].Request().GetAsync();
                    ViewData["Response"] = JsonConvert.SerializeObject(user, Formatting.Indented);
                }
                catch (ServiceException e)
                {
                    switch (e.Error.Code)
                    {
                        case "Request_ResourceNotFound":
                        case "ResourceNotFound":
                        case "ErrorItemNotFound":
                        case "itemNotFound":
                            ViewData["Response"] = JsonConvert.SerializeObject(new { Message = $"User '{email}' not found." }, Formatting.Indented);
                            break;
                        case "ErrorInvalidUser":
                            ViewData["Response"] = JsonConvert.SerializeObject(new { Message = $"The requested user '{email}' is invalid." }, Formatting.Indented);
                            break;
                        default:
                            throw;
                    }
                }

                try
                {
                    Stream pictureStream = await graphClient.Users[email].Photo.Content.Request().GetAsync();
                    MemoryStream pictureMemoryStream = new MemoryStream();
                    await pictureStream.CopyToAsync(pictureMemoryStream);
                    byte[] pictureByteArray = pictureMemoryStream.ToArray();
                    string pictureBase64 = Convert.ToBase64String(pictureByteArray);
                    ViewData["Picture"] = "data:image/jpeg;base64," + pictureBase64;
                }
                catch (ServiceException e)
                {
                    switch (e.Error.Code)
                    {
                        case "Request_ResourceNotFound":
                        case "ResourceNotFound":
                        case "ErrorItemNotFound":
                        case "itemNotFound":
                            ViewData["Picture"] = "data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz4NCjwhRE9DVFlQRSBzdmcgIFBVQkxJQyAnLS8vVzNDLy9EVEQgU1ZHIDEuMS8vRU4nICAnaHR0cDovL3d3dy53My5vcmcvR3JhcGhpY3MvU1ZHLzEuMS9EVEQvc3ZnMTEuZHRkJz4NCjxzdmcgd2lkdGg9IjQwMXB4IiBoZWlnaHQ9IjQwMXB4IiBlbmFibGUtYmFja2dyb3VuZD0ibmV3IDMxMi44MDkgMCA0MDEgNDAxIiB2ZXJzaW9uPSIxLjEiIHZpZXdCb3g9IjMxMi44MDkgMCA0MDEgNDAxIiB4bWw6c3BhY2U9InByZXNlcnZlIiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciPg0KPGcgdHJhbnNmb3JtPSJtYXRyaXgoMS4yMjMgMCAwIDEuMjIzIC00NjcuNSAtODQzLjQ0KSI+DQoJPHJlY3QgeD0iNjAxLjQ1IiB5PSI2NTMuMDciIHdpZHRoPSI0MDEiIGhlaWdodD0iNDAxIiBmaWxsPSIjRTRFNkU3Ii8+DQoJPHBhdGggZD0ibTgwMi4zOCA5MDguMDhjLTg0LjUxNSAwLTE1My41MiA0OC4xODUtMTU3LjM4IDEwOC42MmgzMTQuNzljLTMuODctNjAuNDQtNzIuOS0xMDguNjItMTU3LjQxLTEwOC42MnoiIGZpbGw9IiNBRUI0QjciLz4NCgk8cGF0aCBkPSJtODgxLjM3IDgxOC44NmMwIDQ2Ljc0Ni0zNS4xMDYgODQuNjQxLTc4LjQxIDg0LjY0MXMtNzguNDEtMzcuODk1LTc4LjQxLTg0LjY0MSAzNS4xMDYtODQuNjQxIDc4LjQxLTg0LjY0MWM0My4zMSAwIDc4LjQxIDM3LjkgNzguNDEgODQuNjR6IiBmaWxsPSIjQUVCNEI3Ii8+DQo8L2c+DQo8L3N2Zz4NCg==";
                            break;
                        default:
                            throw;
                    }
                }
            }

            return View();
        }

        [AllowAnonymous]
        public IActionResult About()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Contact()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }
    }
}
