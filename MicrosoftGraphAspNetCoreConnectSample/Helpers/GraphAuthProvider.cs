/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using System;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace MicrosoftGraphAspNetCoreConnectSample.Helpers
{
    public class GraphAuthProvider : IGraphAuthProvider
    {
        private readonly IMemoryCache _memoryCache;

        // Properties used to get and manage an access token.
        private readonly string _aadInstance;
        private readonly string _appId;
        private readonly string _appSecret;
        private readonly string _graphResourceId;
        private readonly string _redirectUri;

        public GraphAuthProvider(IMemoryCache memoryCache, IConfiguration configuration)
        {
            _memoryCache = memoryCache;
            
            _redirectUri = configuration.GetValue<string>("Authentication:AzureAd:BaseUrl") + configuration.GetValue<string>("Authentication:AzureAd:CallbackPath");

            _aadInstance = configuration.GetValue<string>("Authentication:AzureAd:AADInstance") + "common";
            _appId = configuration.GetValue<string>("Authentication:AzureAd:ClientId");
            _appSecret = configuration.GetValue<string>("Authentication:AzureAd:ClientSecret");
            _graphResourceId = configuration.GetValue<string>("Authentication:AzureAd:GraphResourceId");
        }

        // Gets an access token. First tries to get the access token from the token cache.
        // Using password (secret) to authenticate. Production apps should use a certificate.
        public async Task<string> GetUserAccessTokenAsync(string userId)
        {
            TokenCache userTokenCache = new SessionTokenCache(userId, _memoryCache).GetCacheInstance();

            try
            {
                AuthenticationContext authContext = new AuthenticationContext(_aadInstance, userTokenCache);
                ClientCredential credential = new ClientCredential(_appId, _appSecret);
                AuthenticationResult result = await authContext.AcquireTokenSilentAsync(_graphResourceId, credential, new UserIdentifier(userId, UserIdentifierType.UniqueId));

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
                ClientCredential credential = new ClientCredential(_appId, _appSecret);
                AuthenticationResult result = await authContext.AcquireTokenByAuthorizationCodeAsync(code, new Uri(_redirectUri), credential, _graphResourceId);

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
