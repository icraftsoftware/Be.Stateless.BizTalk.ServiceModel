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
using Be.Stateless.BizTalk.ContextProperties.Subscribable;
using Be.Stateless.BizTalk.ServiceModel.Channels.Extensions;
using Be.Stateless.BizTalk.ServiceModel.Security;

namespace Be.Stateless.BizTalk.Dummies.ServiceModel.Security
{
	internal class AuthorizationTokenServiceMiddleware : IAuthorizationTokenServiceMiddleware
	{
		#region IAuthorizationTokenServiceMiddleware Members

		public IAuthorizationTokenIdentifier CreateAuthorizationTokenIdentifier(System.ServiceModel.Channels.Message request)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));
			return new AuthorizationTokenIdentifier(request.GetProperty(BizTalkFactoryProperties.EnvironmentTag));
		}

		public IAuthorizationTokenServiceClient CreateAuthorizationTokenServiceClient(Uri authorizationTokenServiceUri, Uri proxyUri = null)
		{
			if (authorizationTokenServiceUri == null) throw new ArgumentNullException(nameof(authorizationTokenServiceUri));
			return new AuthorizationTokenServiceClient(authorizationTokenServiceUri, proxyUri);
		}

		#endregion
	}
}
