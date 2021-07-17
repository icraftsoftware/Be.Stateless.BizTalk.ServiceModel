#region Copyright & License

// Copyright © 2012 - 2021 François Chabot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Caching;
using Be.Stateless.BizTalk.ServiceModel.Security.Tokens;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.ServiceModel.Security
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
	public class AuthorizationTokenCache
	{
		public static AuthorizationTokenCache Instance { get; } = new();

		private AuthorizationTokenCache() : this(new MemoryCache(nameof(AuthorizationTokenCache))) { }

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "For testing purposes.")]
		internal AuthorizationTokenCache(MemoryCache cache)
		{
			_cache = cache ?? throw new ArgumentNullException(nameof(cache));
		}

		public IAuthorizationToken AddOrGetExistingAuthorizationToken(string key, Func<IAuthorizationToken> authorizationTokenFactory)
		{
			if (key.IsNullOrEmpty()) throw new ArgumentNullException(nameof(key));
			if (authorizationTokenFactory == null) throw new ArgumentNullException(nameof(authorizationTokenFactory));

			return GetAuthorizationToken(key) ?? AddAuthorizationTokenInternal(key, authorizationTokenFactory) ?? GetAuthorizationToken(key);
		}

		public bool AddAuthorizationToken(string key, IAuthorizationToken token)
		{
			if (key.IsNullOrEmpty()) throw new ArgumentNullException(nameof(key));
			if (token == null) throw new ArgumentNullException(nameof(token));

			return AddAuthorizationTokenInternal(key, () => token) != null;
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "For testing purposes.")]
		internal IAuthorizationToken AddAuthorizationTokenInternal(string key, Func<IAuthorizationToken> authorizationTokenFactory)
		{
			lock (_cache)
			{
				if (_cache.Contains(key)) return null;
				var token = authorizationTokenFactory() ?? throw new InvalidOperationException($"AuthorizationToken factory method returns null for key '{key}'.");
				var cacheItem = new CacheItem(key, token);
				if (!_cache.Add(cacheItem, new CacheItemPolicy { AbsoluteExpiration = token.ExpirationTime }))
					throw new InvalidOperationException($"{nameof(AuthorizationTokenCache)} has a concurrency issue because it should not contain an entry for key '{key}'.");
				return (IAuthorizationToken) cacheItem.Value;
			}
		}

		public bool ContainsAuthorizationToken(string key)
		{
			if (key.IsNullOrEmpty()) throw new ArgumentNullException(nameof(key));

			return _cache.Contains(key);
		}

		public IAuthorizationToken GetAuthorizationToken(string key)
		{
			if (key.IsNullOrEmpty()) throw new ArgumentNullException(nameof(key));

			return _cache.Contains(key) ? (IAuthorizationToken) _cache[key] : null;
		}

		public bool TryGetAuthorizationToken(string key, out IAuthorizationToken token)
		{
			if (key.IsNullOrEmpty()) throw new ArgumentNullException(nameof(key));

			if (_cache.Contains(key))
			{
				token = (IAuthorizationToken) _cache[key];
				return true;
			}
			token = null;
			return false;
		}

		private readonly MemoryCache _cache;
	}
}
