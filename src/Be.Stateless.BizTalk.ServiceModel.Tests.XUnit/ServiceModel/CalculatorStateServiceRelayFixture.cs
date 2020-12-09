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
using Be.Stateless.BizTalk.Dummies.ServiceModel;
using Be.Stateless.BizTalk.Dummies.ServiceModel.Channels;
using Be.Stateless.BizTalk.Unit.ServiceModel;
using Be.Stateless.BizTalk.Unit.ServiceModel.Extensions;
using Be.Stateless.BizTalk.Unit.ServiceModel.Stub;
using FluentAssertions;
using Moq;
using Xunit;
using static Be.Stateless.Unit.DelegateFactory;

namespace Be.Stateless.BizTalk.ServiceModel
{
	public class CalculatorStateServiceRelayFixture : IClassFixture<CalculatorStateServiceHostActivator>, IClassFixture<SoapStubHostActivator>
	{
		[Fact(Skip = "Need to fix relay for void method")]
		public void CleanAsyncFails()
		{
			_soapStub.As<ICalculatorStateServiceSync>()
				.Setup(s => s.Clean(It.IsAny<XLangCalculatorRequest>()))
				.Callback(() => throw new InvalidOperationException("Cannot process this request."));

			var client = SoapClient<ICalculatorStateServiceSync>.For(_calculatorServiceHost.Endpoint);
			Action(() => client.Clean(new XLangCalculatorRequest(CALCULATOR_REQUEST_XML)))
				.Should().Throw<FaultException<ExceptionDetail>>()
				.WithMessage("Cannot process this request.");
			client.Abort();
		}

		[Fact(Skip = "Need to fix relay for void method")]
		public void CleanAsyncSucceeds()
		{
			_soapStub.As<ICalculatorStateServiceSync>()
				.Setup(s => s.Clean(It.IsAny<XLangCalculatorRequest>()));

			var client = SoapClient<ICalculatorStateServiceSync>.For(_calculatorServiceHost.Endpoint);
			Action(() => client.Clean(new XLangCalculatorRequest(CALCULATOR_REQUEST_XML))).Should().NotThrow();
			client.Close();
		}

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

		[Fact(Skip = "Need to fix relay for void method")]
		public void ResetSyncFails()
		{
			_soapStub.As<ICalculatorStateServiceSync>()
				.Setup(s => s.Clean(It.IsAny<XLangCalculatorRequest>()))
				.Callback(() => throw new InvalidOperationException("Cannot process this request."));

			var client = SoapClient<ICalculatorStateService>.For(_calculatorServiceHost.Endpoint);
			Action(() => client.Reset(new XLangCalculatorRequest(CALCULATOR_REQUEST_XML)))
				.Should().Throw<FaultException<ExceptionDetail>>()
				.WithMessage("Cannot process this request.");
			client.Abort();
		}

		[Fact(Skip = "Need to fix relay for void method")]
		public void ResetSyncSucceeds()
		{
			_soapStub.As<ICalculatorStateService>()
				.Setup(s => s.Reset(It.IsAny<XLangCalculatorRequest>()));

			var client = SoapClient<ICalculatorStateService>.For(_calculatorServiceHost.Endpoint);
			Action(() => client.Reset(new XLangCalculatorRequest(CALCULATOR_REQUEST_XML))).Should().NotThrow();
			client.Close();
		}

		[SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Necessary for typed xUnit injection.")]
		public CalculatorStateServiceRelayFixture(CalculatorStateServiceHostActivator calculatorServiceHostActivator, SoapStubHostActivator soapStubHostActivator)
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
	}
}
