#region Copyright & License

// Copyright © 2012 - 2020 François Chabot
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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;
using Be.Stateless.BizTalk.Unit.ServiceModel.Extensions;
using Be.Stateless.BizTalk.Unit.ServiceModel.Stub;
using Be.Stateless.Extensions;
using Be.Stateless.IO;

namespace Be.Stateless.BizTalk.Unit.ServiceModel
{
	public static class SoapClient
	{
		public static Stream Invoke(string address, Stream requestMessageBodyStream)
		{
			return Invoke(address, DEFAULT_ACTION, requestMessageBodyStream);
		}

		public static Stream Invoke(string address, string action, Stream requestMessageBodyStream)
		{
			return Invoke(new EndpointAddress(address), action, requestMessageBodyStream);
		}

		public static Stream Invoke(ServiceEndpoint endpoint, Stream requestMessageBodyStream)
		{
			return Invoke(endpoint, DEFAULT_ACTION, requestMessageBodyStream);
		}

		public static Stream Invoke(ServiceEndpoint endpoint, string action, Stream requestMessageBodyStream)
		{
			if (endpoint == null) throw new ArgumentNullException(nameof(endpoint));
			return Invoke(endpoint.Address, action, requestMessageBodyStream);
		}

		public static Stream Invoke(EndpointAddress address, Stream requestMessageBodyStream)
		{
			return Invoke(address, DEFAULT_ACTION, requestMessageBodyStream);
		}

		[SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
		public static Stream Invoke(EndpointAddress address, string action, Stream requestMessageBodyStream)
		{
			var client = _channelFactory.CreateChannel(address);
			try
			{
				using (var xmlReader = XmlReader.Create(requestMessageBodyStream))
				using (var requestMessage = Message.CreateMessage(MessageVersion.Soap11, action, xmlReader))
				using (var responseMessage = client!.Invoke(requestMessage))
				{
					var responseBody = new StringStream(responseMessage.GetReaderAtBodyContents().ReadOuterXml());
					client.Close();
					return responseBody;
				}
			}
			catch (Exception exception)
			{
				if (exception.IsFatal()) throw;
				if (client is ICommunicationObject communicationObject && communicationObject.State != CommunicationState.Closed) communicationObject.Abort();
				throw;
			}
		}

		private const string DEFAULT_ACTION = "urn:services.stateless.be:unit:soap-client:invoke";
		private static readonly ChannelFactory<IMessageService> _channelFactory = new ChannelFactory<IMessageService>(new BasicHttpBinding());
	}
}
