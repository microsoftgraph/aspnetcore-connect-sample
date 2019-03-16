/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using System.Threading;

namespace MicrosoftGraphAspNetCoreConnectSample.Helpers
{
    // Store the user's token information.
    public class SessionTokenCache
    {
        private static readonly ReaderWriterLockSlim SessionLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private readonly string _cacheId;
        private readonly HttpContext httpContext = null;
        private readonly TokenCache _cache = new TokenCache();

        public SessionTokenCache(string userId, HttpContext httpcontext)
        {
            // not object, we want the SUB
            _cacheId = userId + "_TokenCache";
            httpContext = httpcontext;

            Load();
        }

        public TokenCache GetCacheInstance()
        {
            _cache.SetBeforeAccess(BeforeAccessNotification);
            _cache.SetAfterAccess(AfterAccessNotification);
            Load();

            return _cache;
        }

        public void SaveUserStateValue(string state)
        {
            SessionLock.EnterWriteLock();
            httpContext.Session.SetString(_cacheId + "_state", state);
            SessionLock.ExitWriteLock();
        }

        public string ReadUserStateValue()
        {
            SessionLock.EnterReadLock();
            string state = httpContext.Session.GetString(_cacheId + "_state");
            SessionLock.ExitReadLock();
            return state;
        }

        public void Load()
        {
            SessionLock.EnterReadLock();
            byte[] blob = httpContext.Session.Get(_cacheId);
            if (blob != null)
            {
                _cache.Deserialize(blob);
            }
            SessionLock.ExitReadLock();
        }

        public void Persist()
        {
            SessionLock.EnterWriteLock();

            // Reflect changes in the persistent store
            httpContext.Session.Set(_cacheId, _cache.Serialize());

            SessionLock.ExitWriteLock();
        }

        // Triggered right before MSAL needs to access the cache.
        // Reload the cache from the persistent store in case it changed since the last access.
        void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            Load();
        }

        // Triggered right after MSAL accessed the cache.
        void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (args.HasStateChanged)
            {
                Persist();
            }
        }
    }
}