/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

using System.Net.Http.Headers;
using Microsoft.Graph;

namespace MicrosoftGraphAspNetCoreConnectSample.Helpers
{
    public class GraphSDKHelper : IGraphSDKHelper
    {
        private readonly IGraphAuthProvider _authProvider;
        private GraphServiceClient _graphClient;

        public GraphSDKHelper(IGraphAuthProvider authProvider)
        {
            _authProvider = authProvider;
        }

        // Get an authenticated Microsoft Graph Service client.
        public GraphServiceClient GetAuthenticatedClient(string userId)
        {
            if (_graphClient != null) return _graphClient;
            _graphClient = new GraphServiceClient(new DelegateAuthenticationProvider(
                async (requestMessage) =>
                {
                    // Passing tenant ID to the sample auth provider to use as a cache key
                    var accessToken = await _authProvider.GetUserAccessTokenAsync(userId);

                    // Append the access token to the request
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    
                    // This header identifies the sample in the Microsoft Graph service. If extracting this code for your project please remove.
                    requestMessage.Headers.Add("SampleID", "aspnetcore-connect-sample");
                }));

            return _graphClient;
        }
    }
    public interface IGraphSDKHelper
    {
        GraphServiceClient GetAuthenticatedClient(string userId);
    }
}
