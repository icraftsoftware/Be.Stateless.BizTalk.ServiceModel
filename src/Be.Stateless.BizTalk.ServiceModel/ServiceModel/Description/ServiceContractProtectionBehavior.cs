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

using System.Net.Security;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security;

namespace Be.Stateless.BizTalk.ServiceModel.Description
{
	public class ServiceContractProtectionBehavior : IEndpointBehavior
	{
		public ServiceContractProtectionBehavior(ProtectionLevel protectionLevel)
		{
			_protectionLevel = protectionLevel;
		}

		#region IEndpointBehavior Members

		public void Validate(ServiceEndpoint endpoint) { }

		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
			var requirements = new ChannelProtectionRequirements();

			var signatureProtectionSpecification = new MessagePartSpecification(_protectionLevel != ProtectionLevel.None);
			requirements.IncomingSignatureParts.AddParts(signatureProtectionSpecification, "*");
			requirements.OutgoingSignatureParts.AddParts(signatureProtectionSpecification, "*");

			var encryptionProtectionSpecification = new MessagePartSpecification(_protectionLevel == ProtectionLevel.EncryptAndSign);
			requirements.IncomingEncryptionParts.AddParts(encryptionProtectionSpecification, "*");
			requirements.OutgoingEncryptionParts.AddParts(encryptionProtectionSpecification, "*");

			bindingParameters.Remove<ChannelProtectionRequirements>();
			bindingParameters.Add(requirements);
			endpoint.Contract.ProtectionLevel = _protectionLevel;
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime) { }

		#endregion

		private readonly ProtectionLevel _protectionLevel;
	}
}
