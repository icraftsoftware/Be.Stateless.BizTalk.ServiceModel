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
using System.Net.Http;
using Be.Stateless.BizTalk.Dummies.ServiceModel.Security.Tokens;
using Be.Stateless.BizTalk.ServiceModel.Security;
using Be.Stateless.BizTalk.ServiceModel.Security.Tokens;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Dummies.ServiceModel.Security
{
	internal class AuthorizationTokenServiceClient : IAuthorizationTokenServiceClient
	{
		public AuthorizationTokenServiceClient(Uri authorizationTokenServiceUri, Uri proxyUri = null)
		{
			_authorizationTokenServiceUri = authorizationTokenServiceUri ?? throw new ArgumentNullException(nameof(authorizationTokenServiceUri));
			_proxyUri = proxyUri;
		}

		#region IAuthorizationTokenServiceClient Members

		public IAuthorizationToken GetAuthorizationToken(IAuthorizationTokenIdentifier authorizationTokenIdentifier)
		{
			if (authorizationTokenIdentifier.Key.IsNullOrEmpty()) throw new ArgumentNullException(nameof(AuthorizationTokenIdentifier.Key));
			if (SsoSettings.ApiKey.IsNullOrEmpty()) throw new ArgumentNullException(nameof(SsoSettings.ApiKey));
			using (var wc = new WebClient())
			{
				wc.Headers[HttpRequestHeader.Authorization] = "Bearer " + SsoSettings.ApiKey;
				wc.Headers[HttpRequestHeader.Accept] = "application/json";
				if (_proxyUri != null) wc.Proxy = new WebProxy(_proxyUri);
				// don't bother to fiddle with WebClient's timeout as it defaults to 100 sec
				try
				{
					var serializedToken = wc.DownloadString(_authorizationTokenServiceUri);
					return AuthorizationToken.Deserialize(serializedToken);
				}
				catch (WebException exception) when (exception.Status == WebExceptionStatus.ProtocolError)
				{
					using (var wr = exception.Response)
					using (var rs = wr.GetResponseStream())
					{
						var error = Error.Deserialize(rs);
						throw new HttpRequestException($"Failed to get token from '{wr.ResponseUri}'.\r\n{exception.Message}\r\n{error.Message}.");
					}
				}
			}
		}

		#endregion

		private readonly Uri _authorizationTokenServiceUri;
		private readonly Uri _proxyUri;
	}
}
