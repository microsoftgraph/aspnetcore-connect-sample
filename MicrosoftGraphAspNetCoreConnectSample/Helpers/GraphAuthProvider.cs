/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace MicrosoftGraphAspNetCoreConnectSample.Helpers
{
    public class GraphAuthProvider : IGraphAuthProvider
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ClientCredential _credential;

        // Properties used to get and manage an access token.
        private readonly string _aadInstance;
        private readonly string _appId;
        private readonly string _appSecret;
        private readonly string _graphResourceId;
        private readonly string _redirectUri;

        public GraphAuthProvider(IMemoryCache memoryCache, IConfiguration configuration)
        {
            var azureOptions =  new AzureAdOptions();
            configuration.Bind("AzureAd", azureOptions);

            _redirectUri = azureOptions.BaseUrl + azureOptions.CallbackPath;
            _aadInstance = azureOptions.Instance + "common";
            _appId = azureOptions.ClientId;
            _appSecret = azureOptions.ClientSecret;
            _graphResourceId = azureOptions.GraphResourceId;

            _memoryCache = memoryCache;
            _credential = new ClientCredential(_appId, _appSecret);
        }
        public GraphAuthProvider(IMemoryCache memoryCache, AzureAdOptions azureOptions)
        {
            _redirectUri = azureOptions.BaseUrl + azureOptions.CallbackPath;
            _aadInstance = azureOptions.Instance + "common";
            _appId = azureOptions.ClientId;
            _appSecret = azureOptions.ClientSecret;
            _graphResourceId = azureOptions.GraphResourceId;

            _memoryCache = memoryCache;
            _credential = new ClientCredential(_appId, _appSecret);
        }

        // Gets an access token. First tries to get the access token from the token cache.
        // Using password (secret) to authenticate. Production apps should use a certificate.
        public async Task<string> GetUserAccessTokenAsync(string userId)
        {
            TokenCache userTokenCache = new SessionTokenCache(userId, _memoryCache).GetCacheInstance();

            try
            {
                AuthenticationContext authContext = new AuthenticationContext(_aadInstance, userTokenCache);
                AuthenticationResult result = await authContext.AcquireTokenSilentAsync(_graphResourceId, _credential, new UserIdentifier(userId, UserIdentifierType.UniqueId));

                return result.AccessToken;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Gets a token by Authorization Code.
        // Using password (secret) to authenticate. Production apps should use a certificate.
        public async Task<AuthenticationResult> GetTokenByAuthorizationCodeAsync(string userId, string code)
        {
            TokenCache userTokenCache = new SessionTokenCache(userId, _memoryCache).GetCacheInstance();

            try
            {
                AuthenticationContext authContext = new AuthenticationContext(_aadInstance, userTokenCache);
                AuthenticationResult result = await authContext.AcquireTokenByAuthorizationCodeAsync(code, new Uri(_redirectUri), _credential, _graphResourceId);

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public interface IGraphAuthProvider
    {
        Task<string> GetUserAccessTokenAsync(string userId);
        Task<AuthenticationResult> GetTokenByAuthorizationCodeAsync(string userId, string code);
    }
}
