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
using System.Xml;
using Be.Stateless.BizTalk.Dummies.ServiceModel;
using Be.Stateless.BizTalk.Message;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.Unit.ServiceModel.Extensions;
using Be.Stateless.IO;
using BTF2Schemas;
using FluentAssertions;
using Microsoft.BizTalk.Component.Interop;
using Moq;
using Xunit;
using static Be.Stateless.Unit.DelegateFactory;

namespace Be.Stateless.BizTalk.Unit.ServiceModel.Stub
{
	public class SoapStubFixture : IClassFixture<SoapStubHostActivator>
	{
		[Fact]
		public void CannotSetupExpectationAgainstNonServiceContract()
		{
			Action(() => _soapStub.As<IDisposable>().Setup(s => s.Dispose()).Aborts())
				.Should().Throw<ArgumentException>()
				.WithMessage("TContract type parameter 'IDisposable' is not a service contract.");
		}

		[Fact]
		public void SetupCallbackExpectationAgainstVoidOperation()
		{
			var calledBack = false;
			_soapStub.As<IWork>()
				.Setup(s => s.Execute(It.IsAny<System.ServiceModel.Channels.Message>()))
				.Callback(() => calledBack = true);

			var client = SoapClient<IWork>.For(_soapStubHost.Endpoint);
			Action(
				() => client.Execute(
					System.ServiceModel.Channels.Message.CreateMessage(
						MessageVersion.Soap11,
						"urn:services.stateless.be:unit:work:execute:request",
						XmlReader.Create(new StringReader("<request />"))))).Should().NotThrow();
			client.Close();
			calledBack.Should().BeTrue();
		}

		[Fact]
		public void SetupConsecutiveResponseExpectationsAgainstAction()
		{
			_soapStub.As<IWork>()
				.Setup(s => s.Perform(It.IsAny<System.ServiceModel.Channels.Message>()))
				.Callback(
					() => _soapStub.As<IWork>()
						.Setup(s => s.Perform(It.IsAny<System.ServiceModel.Channels.Message>()))
						.Returns(new StringStream("<response2 />"))
				)
				.Returns(new StringStream("<response1 />"));

			var message1 = System.ServiceModel.Channels.Message.CreateMessage(
				MessageVersion.Soap11,
				"urn:services.stateless.be:unit:work:perform:request",
				XmlReader.Create(new StringReader("<request />")));
			var message2 = System.ServiceModel.Channels.Message.CreateMessage(
				MessageVersion.Soap11,
				"urn:services.stateless.be:unit:work:perform:request",
				XmlReader.Create(new StringReader("<request />")));

			var client = SoapClient<IWork>.For(_soapStubHost.Endpoint);
			try
			{
				var result1 = client.Perform(message1);

				var reader1 = result1.GetReaderAtBodyContents();
				reader1.MoveToContent();
				var outerXml1 = reader1.ReadOuterXml();
				outerXml1.Should().Be("<response1 />");

				var result2 = client.Perform(message2);
				var reader2 = result2.GetReaderAtBodyContents();
				reader2.MoveToContent();
				var outerXml2 = reader2.ReadOuterXml();
				outerXml2.Should().Be("<response2 />");

				client.Close();
			}
			catch (Exception)
			{
				client.Abort();
				throw;
			}
		}

		[Fact]
		public void SetupFailureExpectationAgainstAction()
		{
			_soapStub.As<IWork>()
				.Setup(s => s.Perform(It.IsAny<System.ServiceModel.Channels.Message>()))
				.Aborts();

			var client = SoapClient<IWork>.For(_soapStubHost.Endpoint);
			Action(
					() => client.Perform(
						System.ServiceModel.Channels.Message.CreateMessage(
							MessageVersion.Soap11,
							"urn:services.stateless.be:unit:work:perform:request",
							XmlReader.Create(new StringReader("<request />")))))
				.Should().Throw<CommunicationException>();
			client.Abort();
		}

		[Fact]
		public void SetupFailureExpectationAgainstMessageType()
		{
			_soapStub.As<ISolicitResponse>()
				.Setup(s => s.Request(SchemaMetadata.For<btf2_services_header>().DocumentSpec))
				.Aborts();

			var client = SoapClient<IMessageService>.For(_soapStubHost.Endpoint);
			Action(
					() => client.Invoke(
						System.ServiceModel.Channels.Message.CreateMessage(
							MessageVersion.Soap11,
							"urn:services.stateless.be:unit:work:request",
							XmlReader.Create(new StringReader(MessageBodyFactory.Create<btf2_services_header>().OuterXml)))))
				.Should().Throw<CommunicationException>();
			client.Abort();
		}

		[Fact]
		public void SetupFailureExpectationAgainstVoidOperation()
		{
			_soapStub.As<IWork>()
				.Setup(s => s.Execute(It.IsAny<System.ServiceModel.Channels.Message>()))
				.Aborts();

			var client = SoapClient<IWork>.For(_soapStubHost.Endpoint);
			Action(
					() => client.Execute(
						System.ServiceModel.Channels.Message.CreateMessage(
							MessageVersion.Soap11,
							"urn:services.stateless.be:unit:work:execute:request",
							XmlReader.Create(new StringReader("<request />")))))
				.Should().Throw<CommunicationException>();
			client.Abort();
		}

		[Fact]
		public void SetupResponseExpectationAgainstAction()
		{
			_soapStub.As<IWork>()
				.Setup(s => s.Perform(It.IsAny<System.ServiceModel.Channels.Message>()))
				.Returns(new StringStream("<response />"));

			var client = SoapClient<IWork>.For(_soapStubHost.Endpoint);
			try
			{
				var result = client.Perform(
					System.ServiceModel.Channels.Message.CreateMessage(
						MessageVersion.Soap11,
						"urn:services.stateless.be:unit:work:perform:request",
						XmlReader.Create(new StringReader("<request />"))));

				var reader = result.GetReaderAtBodyContents();
				reader.MoveToContent();
				var outerXml = reader.ReadOuterXml();
				outerXml.Should().Be("<response />");

				client.Close();
			}
			catch (Exception)
			{
				client.Abort();
				throw;
			}
		}

		[Fact]
		public void SetupResponseExpectationAgainstAnyMessageType()
		{
			Action(
					() => _soapStub.As<ISolicitResponse>()
						.Setup(s => s.Request(It.IsAny<DocumentSpec>()))
						.Returns(new StringStream("<response />")))
				.Should().Throw<ArgumentException>()
				.WithMessage("Can only setup a response for a valid and non-null DocumentSpec.");
		}

		[Fact]
		public void SetupResponseExpectationAgainstSpecificMessageType()
		{
			_soapStub.As<ISolicitResponse>()
				.Setup(s => s.Request(SchemaMetadata.For<btf2_services_header>().DocumentSpec))
				.Returns(new StringStream("<response />"));

			var client = SoapClient<IMessageService>.For(_soapStubHost.Endpoint);
			try
			{
				var response = client.Invoke(
					System.ServiceModel.Channels.Message.CreateMessage(
						MessageVersion.Soap11,
						"urn:services.stateless.be:unit:work:request",
						XmlReader.Create(new StringReader(MessageBodyFactory.Create<btf2_services_header>().OuterXml))));

				var reader = response!.GetReaderAtBodyContents();
				reader.MoveToContent();
				var outerXml = reader.ReadOuterXml();
				outerXml.Should().Be("<response />");

				client.Close();
			}
			catch (Exception)
			{
				client.Abort();
				throw;
			}
		}

		[SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Necessary for typed xUnit injection.")]
		public SoapStubFixture(SoapStubHostActivator soapStubHostActivator)
		{
			_soapStubHost = soapStubHostActivator.Host;
			_soapStub = (SoapStub) _soapStubHost.SingletonInstance;
			_soapStub.ClearSetups();
		}

		private readonly SoapStub _soapStub;
		private readonly SoapServiceHost _soapStubHost;

		[SuppressMessage("ReSharper", "UnusedMember.Local")]
		private void EnsureSetupExpressionsCompile()
		{
			_soapStub.As<IWork>()
				.Setup(s => s.Execute(It.IsAny<System.ServiceModel.Channels.Message>()))
				.Callback(null)
				.Aborts();

			_soapStub.As<IWork>()
				.Setup(s => s.Perform(It.IsAny<System.ServiceModel.Channels.Message>()))
				.Callback(null)
				.Returns("file");

			_soapStub.As<ISolicitResponse>()
				.Setup(s => s.Request(new DocumentSpec("s", "a")))
				.Callback(null)
				.Aborts();

			_soapStub.As<ISolicitResponse>()
				.Setup(s => s.Request(new DocumentSpec("s", "a")))
				.Callback(null)
				.Returns("file");
		}
	}
}
