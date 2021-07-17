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
using System.Net.Http;
using System.ServiceModel.Channels;
using Be.Stateless.BizTalk.ServiceModel.Channels.Extensions;
using Flurl;

namespace Be.Stateless.BizTalk.Unit.ServiceModel.Channels
{
	public static class WcfWebRequestFactory
	{
		public static System.ServiceModel.Channels.Message Create(Url url)
		{
			return Create(url.ToUri());
		}

		public static System.ServiceModel.Channels.Message Create(Uri uri)
		{
			return System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.None, null)
				.EnsureWebAddressing(uri)
				.EnsureWebBodyFormatting()
				.EnsureWebMethod(HttpMethod.Get.Method);
		}

		public static System.ServiceModel.Channels.Message Create(Url url, System.IO.Stream bodyStream)
		{
			return Create(url.ToUri(), bodyStream);
		}

		public static System.ServiceModel.Channels.Message Create(Uri uri, System.IO.Stream bodyStream)
		{
			return System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.None, null, new StreamBodyWriter(bodyStream))
				.EnsureWebAddressing(uri)
				.EnsureWebBodyFormatting()
				.EnsureWebMethod(HttpMethod.Post.Method);
		}

		private static System.ServiceModel.Channels.Message EnsureWebAddressing(this System.ServiceModel.Channels.Message request, Uri url)
		{
			request.Headers.To = url;
			return request;
		}

		private static System.ServiceModel.Channels.Message EnsureWebBodyFormatting(this System.ServiceModel.Channels.Message request)
		{
			request.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Raw));
			return request;
		}

		private static System.ServiceModel.Channels.Message EnsureWebMethod(this System.ServiceModel.Channels.Message request, string verb)
		{
			request.GetHttpRequestMessage().Method = verb;
			return request;
		}
	}
}
