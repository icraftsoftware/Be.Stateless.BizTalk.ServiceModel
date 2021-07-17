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
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using Be.Stateless.BizTalk.ServiceModel.Channels.Extensions;
using Be.Stateless.BizTalk.ServiceModel.Security;
using Be.Stateless.BizTalk.ServiceModel.Security.Tokens;

namespace Be.Stateless.BizTalk.ServiceModel.Dispatcher
{
	// see https://docs.microsoft.com/en-us/biztalk/core/step-3d-enabling-biztalk-server-to-send-and-receive-messages-from-salesforce
	public class WebHttpAuthorizationProvider : IClientMessageInspector
	{
		public WebHttpAuthorizationProvider(IAuthorizationTokenServiceMiddleware authorizationTokenServiceMiddleware, Uri authorizationTokenServiceUri, Uri proxyUri)
		{
			_authorizationTokenServiceMiddleware = authorizationTokenServiceMiddleware ?? throw new ArgumentNullException(nameof(authorizationTokenServiceMiddleware));
			_authorizationTokenServiceUri = authorizationTokenServiceUri ?? throw new ArgumentNullException(nameof(authorizationTokenServiceUri));
			_proxyUri = proxyUri;
		}

		#region IClientMessageInspector Members

		public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));
			var token = GetAuthorizationTokenForRequest(request);
			request.GetHttpRequestMessage().Headers.Add(HttpRequestHeader.Authorization, $"Bearer {token.Body}");
			return default;
		}

		public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState) { }

		#endregion

		private IAuthorizationToken GetAuthorizationTokenForRequest(System.ServiceModel.Channels.Message request)
		{
			var authorizationTokenIdentifier = _authorizationTokenServiceMiddleware.CreateAuthorizationTokenIdentifier(request)
				?? throw new InvalidOperationException(
					$"AuthorizationTokenServiceMiddleware '{_authorizationTokenServiceMiddleware.GetType().Name}''s {nameof(IAuthorizationTokenServiceMiddleware.CreateAuthorizationTokenIdentifier)} returned no {nameof(IAuthorizationTokenIdentifier)}.");
			return AuthorizationTokenCache.Instance.AddOrGetExistingAuthorizationToken(
				authorizationTokenIdentifier.Key,
				() => InvokeAuthorizationTokenServiceClient(authorizationTokenIdentifier));
		}

		private IAuthorizationToken InvokeAuthorizationTokenServiceClient(IAuthorizationTokenIdentifier authorizationTokenIdentifier)
		{
			var authorizationTokenServiceClient = _authorizationTokenServiceMiddleware.CreateAuthorizationTokenServiceClient(_authorizationTokenServiceUri, _proxyUri)
				?? throw new InvalidOperationException(
					$"AuthorizationTokenServiceMiddleware '{_authorizationTokenServiceMiddleware.GetType().Name}''s {nameof(IAuthorizationTokenServiceMiddleware.CreateAuthorizationTokenServiceClient)} returned no {nameof(IAuthorizationTokenServiceClient)}.");
			return authorizationTokenServiceClient.GetAuthorizationToken(authorizationTokenIdentifier)
				?? throw new InvalidOperationException(
					$"AuthorizationTokenServiceClient '{authorizationTokenServiceClient.GetType().Name}''s {nameof(IAuthorizationTokenServiceClient.GetAuthorizationToken)} returned no {nameof(IAuthorizationToken)}.");
		}

		private readonly IAuthorizationTokenServiceMiddleware _authorizationTokenServiceMiddleware;
		private readonly Uri _authorizationTokenServiceUri;
		private readonly Uri _proxyUri;
	}
}
