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
using System.ServiceModel.Dispatcher;
using System.Xml;
using Be.Stateless.BizTalk.ServiceModel.Channels.Extensions;
using Be.Stateless.BizTalk.ServiceModel.Configuration;
using Be.Stateless.BizTalk.Stream;

namespace Be.Stateless.BizTalk.ServiceModel.Dispatcher
{
	public class XmlResponseTranslator : IClientMessageInspector
	{
		public XmlResponseTranslator(XmlResponseTranslationCollection xmlResponseTranslations)
		{
			_xmlResponseTranslations = xmlResponseTranslations;
		}

		#region IClientMessageInspector Members

		public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));
			return _xmlResponseTranslations?.Count > 0 ? request.Headers.To : default;
		}

		public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
		{
			if (reply == null) throw new ArgumentNullException(nameof(reply));
			if (reply.IsEmpty || reply.IsFault) return;
			if (correlationState is not Uri requestUrl) return;

			var xmlResponseTranslation = _xmlResponseTranslations.GetXmlResponseTranslationByRequestUrl(requestUrl);
			if (xmlResponseTranslation == null) return;

			using (var buffer = reply.CreateBufferedCopy(int.MaxValue))
			using (var message = buffer.CreateMessage())
			using (var reader = message.GetXmlReaderAtWebBodyContents())
			{
				var httpResponseMessage = reply.GetHttpResponseMessage();
				reply = System.ServiceModel.Channels.Message.CreateMessage(
					reply.Version,
					reply.Headers.Action,
					XmlReader.Create(new XmlTranslatorStream(reader, xmlResponseTranslation), new() { CloseInput = true }));
				if (httpResponseMessage != null) reply.SetHttpResponseMessage(httpResponseMessage);
			}
		}

		#endregion

		private readonly XmlResponseTranslationCollection _xmlResponseTranslations;
	}
}
