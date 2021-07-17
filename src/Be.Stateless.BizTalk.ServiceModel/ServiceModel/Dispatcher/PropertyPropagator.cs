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
using System.ServiceModel.Dispatcher;
using Be.Stateless.BizTalk.ServiceModel.Channels.Extensions;
using Be.Stateless.BizTalk.ServiceModel.Configuration;

namespace Be.Stateless.BizTalk.ServiceModel.Dispatcher
{
	/// <summary>
	/// WCF behavior that either propagates or promotes BizTalk message context properties from request to response.
	/// </summary>
	/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/soap-headers-with-published-wcf-services"/>
	/// <seealso href="https://adventuresinsidethemessagebox.wordpress.com/2015/07/01/sharing-context-between-biztalk-and-wcf-behavior-extensions/"/>
	public class PropertyPropagator : IClientMessageInspector
	{
		public PropertyPropagator(PropertyPropagationCollection propertyPropagations)
		{
			_propertyPropagations = propertyPropagations;
		}

		#region IClientMessageInspector Members

		public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));
			return _propertyPropagations?.Count > 0 ? request.Properties : default;
		}

		public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
		{
			if (reply == null) throw new ArgumentNullException(nameof(reply));
			// TODO validate that there should be nothing to do when empty
			//if (reply.IsEmpty) return;
			// TODO validate that there should be nothing to do when fault
			//if (reply.IsFault) return;
			if (correlationState is not MessageProperties requestMessageProperties) return;
			reply.Properties.AddPropertiesToPromote(requestMessageProperties.GetPropertiesToPropagateByPropertyName(_propertyPropagations.GetPropertyNamesToPromote()));
			reply.Properties.AddPropertiesToWrite(requestMessageProperties.GetPropertiesToPropagateByPropertyName(_propertyPropagations.GetPropertyNamesToWrite()));
		}

		#endregion

		private readonly PropertyPropagationCollection _propertyPropagations;
	}
}
