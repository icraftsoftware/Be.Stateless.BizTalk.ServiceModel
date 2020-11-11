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

using System.Diagnostics.CodeAnalysis;
using System.IO;
using Be.Stateless.BizTalk.Dummies.ServiceModel;
using Be.Stateless.BizTalk.Message;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.Unit.ServiceModel.Stub;
using Be.Stateless.IO;
using BTF2Schemas;
using FluentAssertions;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.Unit.ServiceModel
{
	public class SoapClientFixture : IClassFixture<SoapStubHostActivator>
	{
		[Fact]
		public void InvokeSucceeds()
		{
			_soapStub.As<ISolicitResponse>()
				.Setup(s => s.Request(SchemaMetadata.For<btf2_services_header>().DocumentSpec))
				.Returns(new StringStream("<response />"));

			using (var response = SoapClient.Invoke(_soapStubHost.Endpoint, new StringStream(MessageBodyFactory.Create<btf2_services_header>().OuterXml)))
			{
				new StreamReader(response).ReadToEnd().Should().Be("<response />");
			}
		}

		[Fact]
		public void InvokeWithActionSucceeds()
		{
			_soapStub.As<IWork>()
				.Setup(s => s.Perform(It.IsAny<System.ServiceModel.Channels.Message>()))
				.Returns(new StringStream("<response />"));

			using (var response = SoapClient.Invoke(_soapStubHost.Endpoint, "urn:services.stateless.be:unit:work:perform:request", new StringStream("<request />")))
			{
				new StreamReader(response).ReadToEnd().Should().Be("<response />");
			}
		}

		[SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Necessary for typed xUnit injection.")]
		public SoapClientFixture(SoapStubHostActivator soapStubHostActivator)
		{
			_soapStubHost = soapStubHostActivator.Host;
			_soapStub = (SoapStub) _soapStubHost.SingletonInstance;
			_soapStub.ClearSetups();
		}

		private readonly SoapStub _soapStub;
		private readonly SoapServiceHost _soapStubHost;
	}
}
