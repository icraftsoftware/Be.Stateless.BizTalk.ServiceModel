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
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using Be.Stateless.BizTalk.ServiceModel.Security.Tokens;
using FluentAssertions;
using Moq;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.ServiceModel.Security
{
	public class AuthorizationTokenCacheFixture
	{
		#region Setup/Teardown

		public AuthorizationTokenCacheFixture()
		{
			Key = Guid.NewGuid().ToString("N");

			var authorizationTokenMock = new Mock<IAuthorizationToken>();
			authorizationTokenMock.Setup(m => m.ExpirationTime).Returns(DateTime.UtcNow.AddHours(1));
			AuthorizationToken = authorizationTokenMock.Object;

			authorizationTokenMock = new();
			authorizationTokenMock.Setup(m => m.ExpirationTime).Returns(DateTime.UtcNow.AddHours(-1));
			ExpiredAuthorizationToken = authorizationTokenMock.Object;
		}

		#endregion

		[Fact]
		public void AddAuthorizationTokenInternalReturnsAuthorizationToken()
		{
			var cache = new MemoryCache(nameof(AuthorizationTokenCacheFixture));
			var sut = new AuthorizationTokenCache(cache);
			sut.AddAuthorizationTokenInternal(Key, () => AuthorizationToken).Should().BeSameAs(AuthorizationToken);
		}

		[Fact]
		public void AddAuthorizationTokenInternalReturnsNullWhenAccessedConcurrently()
		{
			var cache = new MemoryCache(nameof(AuthorizationTokenCacheFixture));
			var sut = new AuthorizationTokenCache(cache);
			var task = Task.Run(
				// populate cache concurrently
				() => {
					lock (cache)
					{
						// allow AddAuthorizationToken to try acquiring same lock and force it to wait
						Thread.Sleep(500);
						cache.Add(new(Key, AuthorizationToken), new() { AbsoluteExpiration = AuthorizationToken.ExpirationTime });
						Thread.Sleep(500);
					}
					Thread.Sleep(500);
				});
			// ensure task is running and lock is already acquired by it
			do
			{
				Thread.Sleep(1);
			}
			while (task.Status != TaskStatus.Running);
			sut.ContainsAuthorizationToken(Key).Should().BeFalse();
			// no exception has been thrown, i.e. token factory method has not been called because the cache actually contains an
			// entry for the same key despite the previous line and because the ask added it concurrently
			Invoking(() => sut.AddAuthorizationTokenInternal(Key, () => throw new InvalidOperationException()))
				.Should().NotThrow()
				.And.Subject().Should().BeNull();
			// AddAuthorizationToken returned null but the cache nonetheless contains an entry for the same key thanks to the task
			sut.ContainsAuthorizationToken(Key).Should().BeTrue();
			// task is still running and yet AddAuthorizationToken could run because it relinquished its lock
			task.Status.Should().Be(TaskStatus.Running);
			Task.WaitAll(task);
		}

		[Fact]
		public void AddAuthorizationTokenInternalThrowsWhenAuthorizationTokenFactoryReturnsNull()
		{
			var cache = new MemoryCache(nameof(AuthorizationTokenCacheFixture));
			var sut = new AuthorizationTokenCache(cache);
			Invoking(() => sut.AddAuthorizationTokenInternal(Key, () => null))
				.Should().Throw<InvalidOperationException>()
				.WithMessage($"AuthorizationToken factory method returns null for key '{Key}'.");
		}

		[Fact]
		public void AddAuthorizationTokenInternalThrowsWhenConcurrencyIssue()
		{
			var cache = new MemoryCache(nameof(AuthorizationTokenCacheFixture));
			var sut = new AuthorizationTokenCache(cache);

			IAuthorizationToken AuthorizationTokenFactory()
			{
				// fake that authorization token has been added to cache concurrently
				cache.Add(new(Key, AuthorizationToken), new() { AbsoluteExpiration = AuthorizationToken.ExpirationTime });
				return AuthorizationToken;
			}

			Invoking(() => sut.AddAuthorizationTokenInternal(Key, AuthorizationTokenFactory))
				.Should().Throw<InvalidOperationException>()
				.WithMessage($"{nameof(AuthorizationTokenCache)} has a concurrency issue because it should not contain an entry for key '{Key}'.");
		}

		[Fact]
		public void AddAuthorizationTokenReturnsFalseIfEntryExists()
		{
			var cache = new MemoryCache(nameof(AuthorizationTokenCacheFixture)) {
				{ new CacheItem(Key, AuthorizationToken), new CacheItemPolicy { AbsoluteExpiration = AuthorizationToken.ExpirationTime } }
			};
			var sut = new AuthorizationTokenCache(cache);
			sut.AddAuthorizationToken(Key, new Mock<IAuthorizationToken>().Object).Should().BeFalse();
		}

		[Fact]
		public void AddAuthorizationTokenReturnsTrueEvenIfExpired()
		{
			var cache = new MemoryCache(nameof(AuthorizationTokenCacheFixture));
			var sut = new AuthorizationTokenCache(cache);
			sut.AddAuthorizationToken(Key, ExpiredAuthorizationToken).Should().BeTrue();
			sut.GetAuthorizationToken(Key).Should().BeNull();
		}

		[Fact]
		public void AddAuthorizationTokenReturnsTrueIfEntryDoesNotExist()
		{
			var cache = new MemoryCache(nameof(AuthorizationTokenCacheFixture));
			var sut = new AuthorizationTokenCache(cache);
			sut.AddAuthorizationToken(Key, AuthorizationToken).Should().BeTrue();
		}

		[Fact]
		public void AddOrGetExistingAuthorizationTokenAddsAndReturnsAuthorizationToken()
		{
			var cache = new MemoryCache(nameof(AuthorizationTokenCacheFixture));
			var sut = new AuthorizationTokenCache(cache);
			sut.AddOrGetExistingAuthorizationToken(Key, () => AuthorizationToken).Should().BeSameAs(AuthorizationToken);
		}

		[Fact]
		public void AddOrGetExistingAuthorizationTokenReturnsAuthorizationToken()
		{
			var cache = new MemoryCache(nameof(AuthorizationTokenCacheFixture)) {
				{ new CacheItem(Key, AuthorizationToken), new CacheItemPolicy { AbsoluteExpiration = AuthorizationToken.ExpirationTime } }
			};
			var sut = new AuthorizationTokenCache(cache);
			sut.AddOrGetExistingAuthorizationToken(Key, () => new Mock<IAuthorizationToken>().Object).Should().BeSameAs(AuthorizationToken);
		}

		[Fact]
		public void ContainsAuthorizationTokenReturnsFalse()
		{
			var cache = new MemoryCache(nameof(AuthorizationTokenCacheFixture));
			var sut = new AuthorizationTokenCache(cache);
			sut.ContainsAuthorizationToken(Key).Should().BeFalse();
		}

		[Fact]
		public void ContainsAuthorizationTokenReturnsFalseIfExpired()
		{
			var cache = new MemoryCache(nameof(AuthorizationTokenCacheFixture)) {
				{ new CacheItem(Key, ExpiredAuthorizationToken), new CacheItemPolicy { AbsoluteExpiration = ExpiredAuthorizationToken.ExpirationTime } }
			};
			var sut = new AuthorizationTokenCache(cache);
			sut.ContainsAuthorizationToken(Key).Should().BeFalse();
		}

		[Fact]
		public void ContainsAuthorizationTokenReturnsTrue()
		{
			var cache = new MemoryCache(nameof(AuthorizationTokenCacheFixture)) {
				{ new CacheItem(Key, AuthorizationToken), new CacheItemPolicy { AbsoluteExpiration = AuthorizationToken.ExpirationTime } }
			};
			var sut = new AuthorizationTokenCache(cache);
			sut.ContainsAuthorizationToken(Key).Should().BeTrue();
		}

		[Fact]
		public void GetAuthorizationTokenReturnsAuthorizationToken()
		{
			var cache = new MemoryCache(nameof(AuthorizationTokenCacheFixture)) {
				{ new CacheItem(Key, AuthorizationToken), new CacheItemPolicy { AbsoluteExpiration = AuthorizationToken.ExpirationTime } }
			};
			var sut = new AuthorizationTokenCache(cache);
			sut.GetAuthorizationToken(Key).Should().BeSameAs(AuthorizationToken);
		}

		[Fact]
		public void GetAuthorizationTokenReturnsNull()
		{
			var cache = new MemoryCache(nameof(AuthorizationTokenCacheFixture));
			var sut = new AuthorizationTokenCache(cache);
			sut.GetAuthorizationToken(Key).Should().BeNull();
		}

		[Fact]
		public void GetAuthorizationTokenReturnsNullIfExpired()
		{
			var cache = new MemoryCache(nameof(AuthorizationTokenCacheFixture)) {
				{ new CacheItem(Key, ExpiredAuthorizationToken), new CacheItemPolicy { AbsoluteExpiration = ExpiredAuthorizationToken.ExpirationTime } }
			};
			var sut = new AuthorizationTokenCache(cache);
			sut.GetAuthorizationToken(Key).Should().BeNull();
		}

		[Fact]
		public void TryGetAuthorizationTokenReturnsFalseAndNull()
		{
			var cache = new MemoryCache(nameof(AuthorizationTokenCacheFixture));
			var sut = new AuthorizationTokenCache(cache);
			sut.TryGetAuthorizationToken(Key, out var token).Should().BeFalse();
			token.Should().BeNull();
		}

		[Fact]
		public void TryGetAuthorizationTokenReturnsFalseAndNullIfExpired()
		{
			var cache = new MemoryCache(nameof(AuthorizationTokenCacheFixture)) {
				{ new CacheItem(Key, ExpiredAuthorizationToken), new CacheItemPolicy { AbsoluteExpiration = ExpiredAuthorizationToken.ExpirationTime } }
			};
			var sut = new AuthorizationTokenCache(cache);
			sut.TryGetAuthorizationToken(Key, out var token).Should().BeFalse();
			token.Should().BeNull();
		}

		[Fact]
		public void TryGetAuthorizationTokenReturnsTrueAndAuthorizationToken()
		{
			var cache = new MemoryCache(nameof(AuthorizationTokenCacheFixture)) {
				{ new CacheItem(Key, AuthorizationToken), new CacheItemPolicy { AbsoluteExpiration = AuthorizationToken.ExpirationTime } }
			};
			var sut = new AuthorizationTokenCache(cache);
			sut.TryGetAuthorizationToken(Key, out var token).Should().BeTrue();
			token.Should().BeSameAs(AuthorizationToken);
		}

		private string Key { get; }

		private IAuthorizationToken AuthorizationToken { get; }

		private IAuthorizationToken ExpiredAuthorizationToken { get; }
	}
}
