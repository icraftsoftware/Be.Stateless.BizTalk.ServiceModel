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
using System.Net;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using Be.Stateless.BizTalk.ServiceModel.Channels.Extensions;

namespace Be.Stateless.BizTalk.ServiceModel.Dispatcher
{
	// https://stackoverflow.com/a/6351134/1789441
	public class WebHttpErrorInspector : IClientMessageInspector
	{
		#region IClientMessageInspector Members

		public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel)
		{
			return default;
		}

		[SuppressMessage("ReSharper", "InvertIf")]
		public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
		{
			if (reply == null) throw new ArgumentNullException(nameof(reply));
			if (reply.IsFault) return;

			var httpResponse = reply.GetHttpResponseMessage();
			// TODO what about other error StatusCodes, 400 and above, defined or not, i.e. Enum.IsDefined(typeof(HttpStatusCode), httpResponse.StatusCode)
			if (httpResponse.StatusCode == HttpStatusCode.InternalServerError)
			{
				var errorMessage = $"{(int) httpResponse.StatusCode} {httpResponse.StatusDescription}.";
				if (reply.IsEmpty) throw new CommunicationException(errorMessage);

				errorMessage += Environment.NewLine + reply.GetWebContentFormat() switch {
					WebContentFormat.Json => ReadBodyAsJsonString(reply),
					_ => ReadBodyAsXmlString(reply)
				};
				throw new CommunicationException(errorMessage);
			}
		}

		#endregion

		private string ReadBodyAsJsonString(System.ServiceModel.Channels.Message message)
		{
			using (var buffer = message.CreateBufferedCopy(int.MaxValue))
			using (var clonedMessage = buffer.CreateMessage())
			using (var stream = new MemoryStream())
			using (var writer = JsonReaderWriterFactory.CreateJsonWriter(stream))
			{
				clonedMessage.WriteMessage(writer);
				writer.Flush();
				return Encoding.UTF8.GetString(stream.ToArray());
			}
		}

		private string ReadBodyAsXmlString(System.ServiceModel.Channels.Message message)
		{
			using (var buffer = message.CreateBufferedCopy(int.MaxValue))
			using (var clonedMessage = buffer.CreateMessage())
			using (var reader = clonedMessage.GetXmlReaderAtWebBodyContents())
			{
				return reader.ReadOuterXml();
			}
		}
	}
}
