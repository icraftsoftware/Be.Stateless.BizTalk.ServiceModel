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
using System.ServiceModel.Security;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.ServiceModel.Description
{
	public class ServiceContractProtectionBehaviorFixture
	{
		[Fact]
		public void SignProtectionLevelEntailsMessageNotSignedAndNotEncrypted()
		{
			var serviceEndPoint = new ServiceEndpoint(new ContractDescription("contract"));
			var bindingParameters = new BindingParameterCollection();

			var sut = new ServiceContractProtectionBehavior(ProtectionLevel.None);
			sut.AddBindingParameters(serviceEndPoint, bindingParameters);

			serviceEndPoint.Contract.ProtectionLevel.Should().Be(ProtectionLevel.None);
			var actualRequirements = bindingParameters.Find<ChannelProtectionRequirements>();
			actualRequirements.Should().NotBeNull();

			actualRequirements.IncomingSignatureParts.Actions.Should().BeEquivalentTo("*");
			actualRequirements.IncomingSignatureParts.TryGetParts("*", true, out var messagePartSpecification).Should().BeTrue();
			messagePartSpecification.IsBodyIncluded.Should().BeFalse();
			actualRequirements.OutgoingSignatureParts.Should().BeEquivalentTo(actualRequirements.IncomingSignatureParts);

			actualRequirements.IncomingEncryptionParts.Should().BeEquivalentTo(actualRequirements.IncomingSignatureParts);
			actualRequirements.OutgoingEncryptionParts.Should().BeEquivalentTo(actualRequirements.IncomingSignatureParts);
		}

		[Fact]
		public void SignProtectionLevelEntailsMessageSignedAndEncrypted()
		{
			var behavior = new ServiceContractProtectionBehavior(ProtectionLevel.EncryptAndSign);
			var serviceEndPoint = new ServiceEndpoint(new ContractDescription("contract"));

			var bindingParameters = new BindingParameterCollection();
			behavior.AddBindingParameters(serviceEndPoint, bindingParameters);

			serviceEndPoint.Contract.ProtectionLevel.Should().Be(ProtectionLevel.EncryptAndSign);
			var actualRequirements = bindingParameters.Find<ChannelProtectionRequirements>();
			actualRequirements.Should().NotBeNull();

			actualRequirements.IncomingSignatureParts.Actions.Should().BeEquivalentTo("*");
			actualRequirements.IncomingSignatureParts.TryGetParts("*", true, out var messagePartSpecification).Should().BeTrue();
			messagePartSpecification.IsBodyIncluded.Should().BeTrue();
			actualRequirements.OutgoingSignatureParts.Should().BeEquivalentTo(actualRequirements.IncomingSignatureParts);

			actualRequirements.IncomingEncryptionParts.Should().BeEquivalentTo(actualRequirements.IncomingSignatureParts);
			actualRequirements.OutgoingEncryptionParts.Should().BeEquivalentTo(actualRequirements.IncomingSignatureParts);
		}

		[Fact]
		public void SignProtectionLevelEntailsMessageSignedAndNotEncrypted()
		{
			var behavior = new ServiceContractProtectionBehavior(ProtectionLevel.Sign);
			var serviceEndPoint = new ServiceEndpoint(new ContractDescription("contract"));

			var bindingParameters = new BindingParameterCollection();
			behavior.AddBindingParameters(serviceEndPoint, bindingParameters);

			serviceEndPoint.Contract.ProtectionLevel.Should().Be(ProtectionLevel.Sign);
			var actualRequirements = bindingParameters.Find<ChannelProtectionRequirements>();
			actualRequirements.Should().NotBeNull();

			actualRequirements.IncomingSignatureParts.Actions.Should().BeEquivalentTo("*");
			actualRequirements.IncomingSignatureParts.TryGetParts("*", true, out var signatureProtectionSpecification).Should().BeTrue();
			signatureProtectionSpecification.IsBodyIncluded.Should().BeTrue();
			actualRequirements.OutgoingSignatureParts.Should().BeEquivalentTo(actualRequirements.IncomingSignatureParts);

			actualRequirements.IncomingEncryptionParts.Actions.Should().BeEquivalentTo("*");
			actualRequirements.IncomingEncryptionParts.TryGetParts("*", true, out var encryptionProtectionSpecification).Should().BeTrue();
			encryptionProtectionSpecification.IsBodyIncluded.Should().BeFalse();
			actualRequirements.OutgoingEncryptionParts.Should().BeEquivalentTo(actualRequirements.IncomingEncryptionParts);
		}
	}
}
