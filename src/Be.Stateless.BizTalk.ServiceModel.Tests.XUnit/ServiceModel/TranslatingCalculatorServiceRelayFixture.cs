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
using System.ServiceModel;
using System.ServiceModel.Description;
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

namespace Be.Stateless.BizTalk.ServiceModel
{
	public class TranslatingCalculatorServiceRelayFixture : IClassFixture<TranslatingCalculatorServiceHostActivator>, IClassFixture<SoapStubHostActivator>
	{
		#region Setup/Teardown

		[SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Necessary for typed xUnit injection.")]
		public TranslatingCalculatorServiceRelayFixture(TranslatingCalculatorServiceHostActivator calculatorServiceHostActivator, SoapStubHostActivator soapStubHostActivator)
		{
			_calculatorServiceHost = calculatorServiceHostActivator.Host;
			_soapStub = (SoapStub) soapStubHostActivator.Host.SingletonInstance;
			_soapStub.ClearSetups();
		}

		#endregion

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
		public void RelayAsyncMessageThroughXLangTransform()
		{
			_soapStub.As<ITranslatingCalculatorService>()
				.Setup(
					s => s.BeginDivide(
						It.IsAny<XLangCalculatorRequest>(),
						It.IsAny<AsyncCallback>(),
						It.IsAny<object>()))
				.Returns(new StringStream(string.Format(CALCULATOR_RESPONSE_XML, 2)));

			ITranslatingCalculatorService client = null;
			try
			{
				client = SoapClient<ITranslatingCalculatorService>.For(_calculatorServiceHost.Endpoint);
				var calculatorResult = Task<XLangCalculatorResponse>.Factory
					.FromAsync(client.BeginDivide, client.EndDivide, new XLangCalculatorRequest(CALCULATOR_REQUEST_XML), null)
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
		public void RelayAsyncMessageThroughXslt()
		{
			_soapStub.As<ITranslatingCalculatorService>()
				.Setup(
					s => s.BeginDivide(
						It.IsAny<XLangCalculatorRequest>(),
						It.IsAny<AsyncCallback>(),
						It.IsAny<object>()))
				.Returns(new StringStream(string.Format(CALCULATOR_RESPONSE_XML, 2)));

			ITranslatingCalculatorService client = null;
			try
			{
				client = SoapClient<ITranslatingCalculatorService>.For(_calculatorServiceHost.Endpoint);
				var calculatorResult = Task<XLangCalculatorResponse>.Factory
					.FromAsync(client.BeginDivide, client.EndDivide, new XLangCalculatorRequest(CALCULATOR_REQUEST_XML), null)
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
		public void RelaySyncMessageThroughXLangTransform()
		{
			_soapStub.As<ITranslatingCalculatorService>()
				.Setup(s => s.Subtract(It.IsAny<XLangCalculatorRequest>()))
				.Returns(new StringStream(string.Format(CALCULATOR_RESPONSE_XML, 1)));

			ITranslatingCalculatorService client = null;
			try
			{
				client = SoapClient<ITranslatingCalculatorService>.For(_calculatorServiceHost.Endpoint);
				var calculatorResult = client.Subtract(new(CALCULATOR_REQUEST_XML));
				calculatorResult.RawXmlBody.Should().Be(string.Format(CALCULATOR_RESPONSE_XML, 1));
				client.Close();
			}
			catch (Exception)
			{
				client?.Abort();
				throw;
			}
		}

		[Fact]
		public void RelaySyncMessageThroughXslt()
		{
			_soapStub.As<ITranslatingCalculatorService>()
				.Setup(s => s.Subtract(It.IsAny<XLangCalculatorRequest>()))
				.Returns(new StringStream(string.Format(CALCULATOR_RESPONSE_XML, 3)));

			ITranslatingCalculatorService client = null;
			try
			{
				client = SoapClient<ITranslatingCalculatorService>.For(_calculatorServiceHost.Endpoint);
				var calculatorResult = client.Subtract(new(CALCULATOR_REQUEST_XML));
				calculatorResult.RawXmlBody.Should().Be(string.Format(CALCULATOR_RESPONSE_XML, 3));
				client.Close();
			}
			catch (Exception)
			{
				client?.Abort();
				throw;
			}
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
