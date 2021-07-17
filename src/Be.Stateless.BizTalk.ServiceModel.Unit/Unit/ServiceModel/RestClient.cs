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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Be.Stateless.BizTalk.Unit.ServiceModel.Channels;
using Be.Stateless.BizTalk.Unit.ServiceModel.Channels.Extensions;
using Be.Stateless.Linq.Extensions;
using Flurl;

namespace Be.Stateless.BizTalk.Unit.ServiceModel
{
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
	public class RestClient
	{
		// e.g. fiddler proxy default uri http://localhost:8888/
		[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "Public API.")]
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
		public static Uri ProxyUri { get; set; } = null;

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
		public static RestClient WithDefaultBehavior() => new();

		public static RestClient WithCustomBehaviors(params IEndpointBehavior[] behaviors) => new(behaviors);

		private RestClient()
		{
			_behaviorExtensions = Enumerable.Empty<IEndpointBehavior>();
		}

		private RestClient(IEnumerable<IEndpointBehavior> behaviors)
		{
			_behaviorExtensions = behaviors;
		}

		public System.ServiceModel.Channels.Message Get(Url url)
		{
			return Invoke(WcfWebRequestFactory.Create(url));
		}

		public System.ServiceModel.Channels.Message Get(Uri uri)
		{
			return Invoke(WcfWebRequestFactory.Create(uri));
		}

		public System.ServiceModel.Channels.Message Post(Url url, System.IO.Stream stream)
		{
			return Invoke(WcfWebRequestFactory.Create(url, stream));
		}

		public System.ServiceModel.Channels.Message Post(Uri uri, System.IO.Stream stream)
		{
			return Invoke(WcfWebRequestFactory.Create(uri, stream));
		}

		public System.ServiceModel.Channels.Message Invoke(System.ServiceModel.Channels.Message request)
		{
			var restClientFactory = new ChannelFactory<IRequestChannel>();
			restClientFactory.Endpoint.Address = request.GetEndpointAddress();
			restClientFactory.Endpoint.Binding = new WebHttpBinding {
				ProxyAddress = ProxyUri,
				UseDefaultWebProxy = ProxyUri == null,
				Security = new WebHttpSecurity {
					Mode = request.Headers.To.Scheme == Uri.UriSchemeHttps ? WebHttpSecurityMode.Transport : WebHttpSecurityMode.None,
					Transport = new HttpTransportSecurity { ClientCredentialType = HttpClientCredentialType.None }
				}
			};
			restClientFactory.Endpoint.EndpointBehaviors.Add(new WebHttpBehavior { FaultExceptionEnabled = true });
			_behaviorExtensions.ForEach(behavior => restClientFactory.Endpoint.EndpointBehaviors.Add(behavior));

			var restClient = restClientFactory.CreateChannel();
			return restClient.Request(request);
		}

		private readonly IEnumerable<IEndpointBehavior> _behaviorExtensions;
	}
}
