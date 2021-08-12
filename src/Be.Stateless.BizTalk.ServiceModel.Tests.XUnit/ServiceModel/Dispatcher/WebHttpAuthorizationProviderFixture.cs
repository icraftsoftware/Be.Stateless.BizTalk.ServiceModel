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
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using Be.Stateless.BizTalk.ContextProperties.Subscribable;
using Be.Stateless.BizTalk.Dummies.ServiceModel.Security;
using Be.Stateless.BizTalk.ServiceModel.Channels.Extensions;
using Be.Stateless.BizTalk.ServiceModel.Description;
using Be.Stateless.BizTalk.ServiceModel.Security;
using Be.Stateless.BizTalk.ServiceModel.Security.Tokens;
using Be.Stateless.BizTalk.Stream;
using Be.Stateless.BizTalk.Unit.ServiceModel;
using Be.Stateless.BizTalk.Unit.ServiceModel.Channels;
using Be.Stateless.BizTalk.Unit.ServiceModel.Stub;
using Be.Stateless.IO;
using FluentAssertions;
using Flurl;
using Moq;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;
using Xunit.Abstractions;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.ServiceModel.Dispatcher
{
	public class WebHttpAuthorizationProviderFixture : IClassFixture<RestStubHostActivator>, IDisposable
	{
		#region Setup/Teardown

		public WebHttpAuthorizationProviderFixture(RestStubHostActivator restStubHostActivator, ITestOutputHelper output)
		{
			_restStubHostActivator = restStubHostActivator;
			_restStubHostActivator.Host.Reset();
			_output = output;
		}

		public void Dispose()
		{
			if (Debugger.IsAttached) _output.WriteLine(_restStubHostActivator.LogEntries);
		}

		#endregion

		[Fact]
		public void AuthorizationTokenInjectionFails()
		{
			_restStubHostActivator.Host
				.Given(
					Request.Create()
						.WithPath("/sts/token")
						.WithHeader("Authorization", $"Bearer {SsoSettings.ApiKey}")
						.WithHeader("Accept", "application/json")
						.UsingGet()
				)
				.RespondWith(
					Response.Create()
						.WithStatusCode(HttpStatusCode.InternalServerError)
						.WithHeader("Content-Type", "application/json")
						.WithBody("{\"Message\":\"Authorization header not present\"}")
				);

			var url = RestStubHostActivator.Uri.AppendPathSegment("/api/resource");
			var requestBodyStream = new MultipartFormDataContentStream(new StringStream("dummy"));
			var request = WcfWebRequestFactory.Create(url, requestBodyStream);
			request.SetProperty(BizTalkFactoryProperties.EnvironmentTag, "token-cache-key");
			var httpRequestHeaders = request.GetHttpRequestMessage().Headers;
			httpRequestHeaders.Add(HttpRequestHeader.Accept, "application/xml");
			httpRequestHeaders.Add(HttpRequestHeader.ContentType, requestBodyStream.ContentType);

			Invoking(() => RestClient.WithCustomBehaviors(WebHttpAuthorizationBehavior).Invoke(request))
				.Should().Throw<HttpRequestException>()
				.WithMessage(
					$"Failed to get token from '{AuthorizationTokenServiceUri}'.\r\n"
					+ "The remote server returned an error: (500) Internal Server Error.\r\n"
					+ "Authorization header not present.");
		}

		[Fact]
		public void AuthorizationTokenInjectionSucceeds()
		{
			var authorizationToken = $"cryptographic-client-authorization-token-{Guid.NewGuid():D}";
			var url = RestStubHostActivator.Uri.AppendPathSegment("/api/resource");
			_restStubHostActivator.Host
				.Given(
					Request.Create()
						.WithPath("/sts/token")
						.WithHeader("Authorization", $"Bearer {SsoSettings.ApiKey}")
						.WithHeader("Accept", "application/json")
						.UsingGet()
				)
				.RespondWith(
					Response.Create()
						.WithStatusCode(HttpStatusCode.OK)
						.WithHeader("Content-Type", "application/json")
						.WithBody($"{{\"Token\":\"{authorizationToken}\",\"Expires\":\"{DateTime.UtcNow:O}\"}}")
				);
			_restStubHostActivator.Host
				.Given(
					Request.Create()
						.WithPath(url.Path)
						.WithHeader("Authorization", $"Bearer {authorizationToken}")
						.WithHeader("Accept", "application/xml")
						.WithHeader("Content-Type", new RegexMatcher(Regex.Escape("multipart/form-data; boundary=\"")))
						.UsingPost()
				)
				.RespondWith(
					Response.Create()
						.WithHeader("Content-Type", "application/xml")
						.WithBody("<root>dummy</root>")
						.WithStatusCode(HttpStatusCode.OK)
				);

			var requestBodyStream = new MultipartFormDataContentStream(new StringStream("dummy"));
			var request = WcfWebRequestFactory.Create(url, requestBodyStream);
			request.SetProperty(BizTalkFactoryProperties.EnvironmentTag, "token-cache-key");
			var httpRequestHeaders = request.GetHttpRequestMessage().Headers;
			httpRequestHeaders.Add(HttpRequestHeader.Accept, "application/xml");
			httpRequestHeaders.Add(HttpRequestHeader.ContentType, requestBodyStream.ContentType);

			var response = RestClient.WithCustomBehaviors(WebHttpAuthorizationBehavior).Invoke(request);

			response.Should().NotBeNull();
			var httpResponse = response.GetHttpResponseMessage();
			httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public void InvokeAuthorizationTokenServiceClientThrowsWhenAuthorizationTokenServiceClientReturnsNoAuthorizationToken()
		{
			var url = RestStubHostActivator.Uri.AppendPathSegment("/api/resource");
			var request = WcfWebRequestFactory.Create(url, new StringStream("dummy"));

			var authorizationTokenIdentifierMock = new Mock<IAuthorizationTokenIdentifier>();
			authorizationTokenIdentifierMock.Setup(m => m.Key).Returns("token-cache-key");
			AuthorizationTokenServiceMiddleware.Mock
				.Setup(m => m.CreateAuthorizationTokenIdentifier(It.IsAny<System.ServiceModel.Channels.Message>()))
				.Returns(authorizationTokenIdentifierMock.Object);

			var authorizationTokenServiceClient = new Mock<IAuthorizationTokenServiceClient>();
			authorizationTokenServiceClient
				.Setup(m => m.GetAuthorizationToken(It.IsAny<IAuthorizationTokenIdentifier>()))
				.Returns((IAuthorizationToken) null);
			AuthorizationTokenServiceMiddleware.Mock
				.Setup(m => m.CreateAuthorizationTokenServiceClient(It.IsAny<Uri>(), It.IsAny<Uri>()))
				.Returns(authorizationTokenServiceClient.Object);

			var webHttpAuthorizationBehavior = new WebHttpAuthorizationBehavior(typeof(AuthorizationTokenServiceMiddleware), AuthorizationTokenServiceUri);

			Invoking(() => RestClient.WithCustomBehaviors(webHttpAuthorizationBehavior).Invoke(request))
				.Should().Throw<InvalidOperationException>()
				.WithMessage(
					$"AuthorizationTokenServiceClient '{authorizationTokenServiceClient.Object.GetType().Name}''s {nameof(IAuthorizationTokenServiceClient.GetAuthorizationToken)} returned no {nameof(IAuthorizationToken)}.");
		}

		[Fact]
		public void InvokeAuthorizationTokenServiceClientThrowsWhenAuthorizationTokenServiceMiddlewareReturnsNoAuthorizationTokenIdentifier()
		{
			var url = RestStubHostActivator.Uri.AppendPathSegment("/api/resource");
			var request = WcfWebRequestFactory.Create(url, new StringStream("dummy"));

			AuthorizationTokenServiceMiddleware.Mock
				.Setup(m => m.CreateAuthorizationTokenIdentifier(It.IsAny<System.ServiceModel.Channels.Message>()))
				.Returns((IAuthorizationTokenIdentifier) null);
			var webHttpAuthorizationBehavior = new WebHttpAuthorizationBehavior(typeof(AuthorizationTokenServiceMiddleware), AuthorizationTokenServiceUri);

			Invoking(() => RestClient.WithCustomBehaviors(webHttpAuthorizationBehavior).Invoke(request))
				.Should().Throw<InvalidOperationException>()
				.WithMessage(
					$"AuthorizationTokenServiceMiddleware '{nameof(AuthorizationTokenServiceMiddleware)}''s {nameof(IAuthorizationTokenServiceMiddleware.CreateAuthorizationTokenIdentifier)} returned no {nameof(IAuthorizationTokenIdentifier)}.");
		}

		[Fact]
		public void InvokeAuthorizationTokenServiceClientThrowsWhenAuthorizationTokenServiceMiddlewareReturnsNoAuthorizationTokenServiceClient()
		{
			var url = RestStubHostActivator.Uri.AppendPathSegment("/api/resource");
			var request = WcfWebRequestFactory.Create(url, new StringStream("dummy"));

			var authorizationTokenIdentifierMock = new Mock<IAuthorizationTokenIdentifier>();
			authorizationTokenIdentifierMock.Setup(m => m.Key).Returns("token-cache-key");
			AuthorizationTokenServiceMiddleware.Mock
				.Setup(m => m.CreateAuthorizationTokenIdentifier(It.IsAny<System.ServiceModel.Channels.Message>()))
				.Returns(authorizationTokenIdentifierMock.Object);
			AuthorizationTokenServiceMiddleware.Mock
				.Setup(m => m.CreateAuthorizationTokenServiceClient(It.IsAny<Uri>(), It.IsAny<Uri>()))
				.Returns((IAuthorizationTokenServiceClient) null);

			var webHttpAuthorizationBehavior = new WebHttpAuthorizationBehavior(typeof(AuthorizationTokenServiceMiddleware), AuthorizationTokenServiceUri);

			Invoking(() => RestClient.WithCustomBehaviors(webHttpAuthorizationBehavior).Invoke(request))
				.Should().Throw<InvalidOperationException>()
				.WithMessage(
					$"AuthorizationTokenServiceMiddleware '{nameof(AuthorizationTokenServiceMiddleware)}''s {nameof(IAuthorizationTokenServiceMiddleware.CreateAuthorizationTokenServiceClient)} returned no {nameof(IAuthorizationTokenServiceClient)}.");
		}

		private class AuthorizationTokenServiceMiddleware : IAuthorizationTokenServiceMiddleware
		{
			internal static Mock<IAuthorizationTokenServiceMiddleware> Mock { get; } = new();

			#region IAuthorizationTokenServiceMiddleware Members

			public IAuthorizationTokenIdentifier CreateAuthorizationTokenIdentifier(System.ServiceModel.Channels.Message request)
			{
				return Mock.Object.CreateAuthorizationTokenIdentifier(request);
			}

			public IAuthorizationTokenServiceClient CreateAuthorizationTokenServiceClient(Uri authorizationTokenServiceUri, Uri proxyUri = null)
			{
				return Mock.Object.CreateAuthorizationTokenServiceClient(authorizationTokenServiceUri, proxyUri);
			}

			#endregion
		}

		private Uri AuthorizationTokenServiceUri => RestStubHostActivator.Uri.AppendPathSegment("/sts/token").ToUri();

		private WebHttpAuthorizationBehavior WebHttpAuthorizationBehavior =>
			new(typeof(Dummies.ServiceModel.Security.AuthorizationTokenServiceMiddleware), AuthorizationTokenServiceUri);

		private readonly ITestOutputHelper _output;
		private readonly RestStubHostActivator _restStubHostActivator;
	}
}
