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
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;
using Be.Stateless.BizTalk.Dummies.ServiceModel;
using Be.Stateless.BizTalk.Dummies.ServiceModel.Channels;
using Be.Stateless.BizTalk.Unit.ServiceModel;
using Be.Stateless.BizTalk.Unit.ServiceModel.Extensions;
using Be.Stateless.BizTalk.Unit.ServiceModel.Stub;
using Be.Stateless.IO;
using FluentAssertions;
using Moq;
using Xunit;
using static Be.Stateless.Unit.DelegateFactory;

namespace Be.Stateless.BizTalk.ServiceModel
{
	public class CalculatorServiceRelayFixture : IClassFixture<CalculatorServiceHostActivator>, IClassFixture<SoapStubHostActivator>
	{
		[Fact]
		public void GetMetadata()
		{
			// How to: Import Metadata into Service Endpoints, see https://docs.microsoft.com/en-us/dotnet/framework/wcf/feature-details/how-to-import-metadata-into-service-endpoints
			var mexAddress = new EndpointAddress(_calculatorServiceHost.Endpoint.Address.Uri + "/mex");
			var mexClient = new MetadataExchangeClient(mexAddress) { ResolveMetadataReferences = true };
			var metaSet = mexClient.GetMetadata();

			var importer = new WsdlImporter(metaSet);
			var contracts = importer.ImportAllContracts();
			var generator = new ServiceContractGenerator();
			foreach (var contract in contracts)
			{
				generator.GenerateServiceContractType(contract);
			}
			generator.Errors.Should().BeEmpty("There were errors during code compilation.");
		}

		[Fact]
		public void RelayAsyncMessage()
		{
			_soapStub.As<ICalculatorService>()
				.Setup(s => s.BeginMultiply(It.IsAny<XmlCalculatorRequest>(), It.IsAny<AsyncCallback>(), It.IsAny<object>()))
				.Returns(new StringStream(string.Format(CALCULATOR_RESPONSE_XML, 2)));

			ICalculatorService client = null;
			try
			{
				client = SoapClient<ICalculatorService>.For(_calculatorServiceHost.Endpoint);
				var calculatorResult = Task<XmlCalculatorResponse>.Factory
					.FromAsync(client.BeginMultiply, client.EndMultiply, new XmlCalculatorRequest(CALCULATOR_REQUEST_XML), null)
					.Result;
				calculatorResult.RawXmlBody.Should().Be(string.Format(CALCULATOR_RESPONSE_XML, 2));
				client.Close();
			}
			catch (Exception)
			{
				client?.Abort();
				throw;
			}
		}

		[Fact]
		public void RelayAsyncMessageTimesOut()
		{
			_soapStub.As<ICalculatorService>()
				.Setup(s => s.BeginDivide(It.IsAny<XmlCalculatorRequest>(), It.IsAny<AsyncCallback>(), It.IsAny<object>()))
				.Callback(() => Thread.Sleep(3000))
				.Returns(new StringStream(string.Format(CALCULATOR_RESPONSE_XML, 2)));
			var client = SoapClient<ICalculatorService>.For(_calculatorServiceHost.Endpoint);
			Function(
					() => Task<XmlCalculatorResponse>.Factory
						.FromAsync(client.BeginDivide, client.EndDivide, new XmlCalculatorRequest(CALCULATOR_REQUEST_XML), null)
						.Result)
				.Should().Throw<AggregateException>()
				.WithInnerException<FaultException<ExceptionDetail>>()
				.WithMessage("*has exceeded the allotted timeout*");
			client.Close();
		}

		[Fact]
		public void RelayCompositeMessage()
		{
			const string responseXml = "<CalculatorResponse xmlns=\"urn:services.stateless.be:unit:calculator\">" +
				"<s0:Result xmlns:s0=\"urn:services.stateless.be:unit:calculator\">one</s0:Result>" +
				"<s0:Result xmlns:s0=\"urn:services.stateless.be:unit:calculator\">two</s0:Result>" +
				"</CalculatorResponse>";

			_soapStub.As<ICalculatorService>()
				.Setup(s => s.Add(It.IsAny<XmlCalculatorRequest>()))
				.Returns(new StringStream(responseXml));

			ICalculatorService client = null;
			try
			{
				client = SoapClient<ICalculatorService>.For(_calculatorServiceHost.Endpoint);
				var calculatorResult = client.Add(new XmlCalculatorRequest(CALCULATOR_REQUEST_XML));
				calculatorResult.RawXmlBody.Should().Be(responseXml);
				client.Close();
			}
			catch (Exception)
			{
				client?.Abort();
				throw;
			}
		}

		[Fact]
		public void RelaySyncMessage()
		{
			_soapStub.As<ICalculatorService>()
				.Setup(s => s.Add(It.IsAny<XmlCalculatorRequest>()))
				.Returns(new StringStream(string.Format(CALCULATOR_RESPONSE_XML, 3)));

			ICalculatorService client = null;
			try
			{
				client = SoapClient<ICalculatorService>.For(_calculatorServiceHost.Endpoint);
				var calculatorResult = client.Add(new XmlCalculatorRequest(CALCULATOR_REQUEST_XML));
				calculatorResult.RawXmlBody.Should().Be(string.Format(CALCULATOR_RESPONSE_XML, 3));
				client.Close();
			}
			catch (Exception)
			{
				client?.Abort();
				throw;
			}
		}

		[Fact]
		public void RelaySyncMessageTimesOut()
		{
			_soapStub.As<ICalculatorService>()
				.Setup(s => s.Subtract(It.IsAny<XmlCalculatorRequest>()))
				.Callback(() => Thread.Sleep(3000))
				.Returns(new StringStream(string.Format(CALCULATOR_RESPONSE_XML, 1)));

			var client = SoapClient<ICalculatorService>.For(_calculatorServiceHost.Endpoint);
			Action(() => client.Subtract(new XmlCalculatorRequest(CALCULATOR_REQUEST_XML)))
				.Should().Throw<FaultException<ExceptionDetail>>()
				.WithMessage("The request channel timed out while waiting for a reply*");
			client.Close();
		}

		[SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Necessary for typed xUnit injection.")]
		public CalculatorServiceRelayFixture(CalculatorServiceHostActivator calculatorServiceHostActivator, SoapStubHostActivator soapStubHostActivator)
		{
			_calculatorServiceHost = calculatorServiceHostActivator.Host;
			_soapStub = (SoapStub) soapStubHostActivator.Host.SingletonInstance;
			_soapStub.ClearSetups();
		}

		private readonly SoapServiceHost _calculatorServiceHost;
		private readonly SoapStub _soapStub;

		private const string CALCULATOR_REQUEST_XML = "<CalculatorRequest xmlns=\"urn:services.stateless.be:unit:calculator\">" +
			"<s0:Arguments xmlns:s0=\"urn:services.stateless.be:unit:calculator\">" +
			"<s0:Term>2</s0:Term>" +
			"<s0:Term>1</s0:Term>" +
			"</s0:Arguments>" +
			"</CalculatorRequest>";

		private const string CALCULATOR_RESPONSE_XML = "<CalculatorResponse xmlns=\"urn:services.stateless.be:unit:calculator\">" +
			"<s0:Result xmlns:s0=\"urn:services.stateless.be:unit:calculator\">{0}</s0:Result>" +
			"</CalculatorResponse>";
	}
}
