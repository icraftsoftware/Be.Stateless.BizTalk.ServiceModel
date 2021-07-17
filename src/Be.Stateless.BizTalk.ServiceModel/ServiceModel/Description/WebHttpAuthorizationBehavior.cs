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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Be.Stateless.BizTalk.ServiceModel.Dispatcher;
using Be.Stateless.BizTalk.ServiceModel.Security;

namespace Be.Stateless.BizTalk.ServiceModel.Description
{
	public class WebHttpAuthorizationBehavior : IEndpointBehavior
	{
		public WebHttpAuthorizationBehavior(Type authorizationTokenServiceMiddlewareType, Uri authorizationTokenServiceUri)
		{
			if (authorizationTokenServiceMiddlewareType == null) throw new ArgumentNullException(nameof(authorizationTokenServiceMiddlewareType));
			_authorizationTokenServiceMiddleware = (IAuthorizationTokenServiceMiddleware) Activator.CreateInstance(authorizationTokenServiceMiddlewareType);
			_authorizationTokenServiceUri = authorizationTokenServiceUri ?? throw new ArgumentNullException(nameof(authorizationTokenServiceUri));
		}

		#region IEndpointBehavior Members

		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			if (clientRuntime == null) throw new ArgumentNullException(nameof(clientRuntime));
			// TODO ?? should ensure endpoint.Binding is WebHttpBinding ??
			var proxyUri = (endpoint.Binding as WebHttpBinding)?.ProxyAddress;
			clientRuntime.MessageInspectors.Add(new WebHttpAuthorizationProvider(_authorizationTokenServiceMiddleware, _authorizationTokenServiceUri, proxyUri));
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }

		public void Validate(ServiceEndpoint endpoint) { }

		#endregion

		private readonly IAuthorizationTokenServiceMiddleware _authorizationTokenServiceMiddleware;
		private readonly Uri _authorizationTokenServiceUri;
	}
}
