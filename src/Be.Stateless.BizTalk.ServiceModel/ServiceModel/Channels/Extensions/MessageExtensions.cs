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
using System.IO;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using Be.Stateless.BizTalk.ContextProperties;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.ServiceModel.Channels.Extensions
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
	public static class MessageExtensions
	{
		#region WebHttp Message Extensions

		public static HttpRequestMessageProperty GetHttpRequestMessage(this System.ServiceModel.Channels.Message request)
		{
			if (!request.Properties.ContainsKey(HttpRequestMessageProperty.Name)) request.Properties.Add(HttpRequestMessageProperty.Name, new HttpRequestMessageProperty());
			var httpRequest = (HttpRequestMessageProperty) request.Properties[HttpRequestMessageProperty.Name];
			return httpRequest;
		}

		public static HttpResponseMessageProperty GetHttpResponseMessage(this System.ServiceModel.Channels.Message response)
		{
			var httpResponse = (HttpResponseMessageProperty) response.Properties[HttpResponseMessageProperty.Name];
			return httpResponse;
		}

		public static void SetHttpResponseMessage(this System.ServiceModel.Channels.Message response, HttpResponseMessageProperty httpResponseMessage)
		{
			response.Properties[HttpResponseMessageProperty.Name] = httpResponseMessage;
		}

		public static WebContentFormat GetWebContentFormat(this System.ServiceModel.Channels.Message message)
		{
			// see https://docs.microsoft.com/en-us/archive/blogs/endpoint/wcf-extensibility-message-inspectors, GetMessageContentFormat
			return !message.Properties.ContainsKey(WebBodyFormatMessageProperty.Name)
				? WebContentFormat.Xml // BTS only expects XML payloads
				: ((WebBodyFormatMessageProperty) message.Properties[WebBodyFormatMessageProperty.Name]).Format;
		}

		public static XmlReader GetXmlReaderAtWebBodyContents(this System.ServiceModel.Channels.Message message)
		{
			// see https://blogs.msdn.microsoft.com/endpoint/2011/04/23/wcf-extensibility-message-inspectors/,
			return message.GetWebContentFormat() switch {
				// see https://docs.microsoft.com/en-us/archive/blogs/endpoint/wcf-extensibility-message-inspectors, MessageToString
				WebContentFormat.Default or WebContentFormat.Xml => message.GetReaderAtBodyContents(),
				WebContentFormat.Raw => message.GetXmlReaderAtRawBodyContents(),
				var format => throw new ArgumentException(
					$"Unsupported {nameof(WebContentFormat)}.'{format}' message, only {WebContentFormat.Raw} and {WebContentFormat.Xml} are supported.",
					nameof(message))
			};
		}

		private static XmlReader GetXmlReaderAtRawBodyContents(this System.ServiceModel.Channels.Message message)
		{
			// see https://docs.microsoft.com/en-us/archive/blogs/endpoint/wcf-extensibility-message-inspectors, ReadRawBody
			using (var reader = message.GetReaderAtBodyContents())
			{
				reader.ReadStartElement("Binary");
				var bodyText = Encoding.UTF8.GetString(reader.ReadContentAsBase64());
				return XmlReader.Create(new StringReader(bodyText), new XmlReaderSettings { IgnoreWhitespace = true });
			}
		}

		#endregion

		#region GetProperty Extensions

		public static string GetProperty<T>(this System.ServiceModel.Channels.Message message, MessageContextProperty<T, string> property)
			where T : MessageContextPropertyBase, new()
		{
			return message.TryGetProperty(property, out var value) ? value : default;
		}

		public static TR GetProperty<T, TR>(this System.ServiceModel.Channels.Message message, MessageContextProperty<T, TR> property)
			where T : MessageContextPropertyBase, new()
			where TR : struct
		{
			return message.TryGetProperty(property, out var value) ? value : default;
		}

		#endregion

		#region SetProperty Extensions

		public static void SetProperty<T>(this System.ServiceModel.Channels.Message message, MessageContextProperty<T, string> property, string value)
			where T : MessageContextPropertyBase, new()
		{
			if (message == null) throw new ArgumentNullException(nameof(message));
			message.Properties.SetProperty(property, value);
		}

		public static void SetProperty<T, TV>(this System.ServiceModel.Channels.Message message, MessageContextProperty<T, TV> property, TV value)
			where T : MessageContextPropertyBase, new()
			where TV : struct
		{
			if (message == null) throw new ArgumentNullException(nameof(message));
			message.Properties.SetProperty(property, value);
		}

		#endregion

		#region TryGetProperty Extensions

		public static bool TryGetProperty<T>(this System.ServiceModel.Channels.Message message, MessageContextProperty<T, string> property, out string value)
			where T : MessageContextPropertyBase, new()
		{
			if (message == null) throw new ArgumentNullException(nameof(message));
			return message.Properties.TryGetProperty(property, out value);
		}

		public static bool TryGetProperty<T, TR>(this System.ServiceModel.Channels.Message message, MessageContextProperty<T, TR> property, out TR value)
			where T : MessageContextPropertyBase, new()
			where TR : struct
		{
			if (message == null) throw new ArgumentNullException(nameof(message));
			return message.Properties.TryGetProperty(property, out value);
		}

		#endregion
	}
}
