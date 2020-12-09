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
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading.Tasks;
using Be.Stateless.BizTalk.Dummies.ServiceModel;
using Be.Stateless.BizTalk.Dummies.ServiceModel.Channels;
using Be.Stateless.BizTalk.Unit.ServiceModel;
using Be.Stateless.BizTalk.Unit.ServiceModel.Extensions;
using Be.Stateless.BizTalk.Unit.ServiceModel.Stub;
using Be.Stateless.IO;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.ServiceModel
{
	[SoapServiceHostActivator(typeof(ValidatingCalculatorServiceHostActivator))]
	[SoapStubHostActivator]
	[TestFixture]
	public class ValidatingCalculatorServiceRelayFixture : ISoapServiceHostInjection
	{
		[Test]
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
			Assert.That(generator.Errors.Count, Is.EqualTo(0), "There were errors during code compilation.");
		}

		[Test]
		public void RelayAsyncInvalidMessageFails()
		{
			var client = SoapClient<IValidatingCalculatorService>.For(_calculatorServiceHost.Endpoint);
			Assert.That(
				() => Task<XLangCalculatorResponse>.Factory
					.FromAsync(client.BeginMultiply, client.EndMultiply, new XLangCalculatorRequest(INVALID_CALCULATOR_REQUEST_XML), null)
					.Result,
				Throws.TypeOf<AggregateException>()
					.With.InnerException.TypeOf<FaultException<ExceptionDetail>>()
					.With.InnerException.Property("Detail")
					.With.InnerException.InnerException.Message.Contains(
						"The element 'Arguments' in namespace 'urn:services.stateless.be:unit:calculator' has invalid child element 'Operand' in namespace 'urn:services.stateless.be:unit:calculator'. "
						+ "List of possible elements expected: 'Term' in namespace 'urn:services.stateless.be:unit:calculator'"));
			client.Close();
		}

		[Test]
		public void RelayAsyncMessage()
		{
			_soapStub.As<IValidatingCalculatorService>()
				.Setup(
					s => s.BeginMultiply(
						It.IsAny<XLangCalculatorRequest>(),
						It.IsAny<AsyncCallback>(),
						It.IsAny<object>()))
				.Returns(new StringStream(string.Format(CALCULATOR_RESPONSE_XML, 2)));

			IValidatingCalculatorService client = null;
			try
			{
				client = SoapClient<IValidatingCalculatorService>.For(_calculatorServiceHost.Endpoint);
				var calculatorResult = Task<XLangCalculatorResponse>.Factory
					.FromAsync(client.BeginMultiply, client.EndMultiply, new XLangCalculatorRequest(CALCULATOR_REQUEST_XML), null)
					.Result;
				Assert.AreEqual(string.Format(CALCULATOR_RESPONSE_XML, 2), calculatorResult.RawXmlBody);
				client.Close();
			}
			catch (Exception)
			{
				client?.Abort();
				throw;
			}
		}

		[Test]
		public void RelaySyncInvalidMessageFails()
		{
			var client = SoapClient<IValidatingCalculatorService>.For(_calculatorServiceHost.Endpoint);
			Assert.That(
				() => client.Add(new XLangCalculatorRequest(INVALID_CALCULATOR_REQUEST_XML)),
				Throws.TypeOf<FaultException<ExceptionDetail>>()
					.With.Property("Detail")
					.With.InnerException.InnerException.Message.Contains(
						"The element 'Arguments' in namespace 'urn:services.stateless.be:unit:calculator' has invalid child element 'Operand' in namespace 'urn:services.stateless.be:unit:calculator'. "
						+ "List of possible elements expected: 'Term' in namespace 'urn:services.stateless.be:unit:calculator'"));
			client.Close();
		}

		[Test]
		public void RelaySyncMessage()
		{
			_soapStub.As<IValidatingCalculatorService>()
				.Setup(s => s.Add(It.IsAny<XLangCalculatorRequest>()))
				.Returns(new StringStream(string.Format(CALCULATOR_RESPONSE_XML, 3)));

			IValidatingCalculatorService client = null;
			try
			{
				client = SoapClient<IValidatingCalculatorService>.For(_calculatorServiceHost.Endpoint);
				var calculatorResult = client.Add(new XLangCalculatorRequest(CALCULATOR_REQUEST_XML));
				Assert.AreEqual(string.Format(CALCULATOR_RESPONSE_XML, 3), calculatorResult.RawXmlBody);
				client.Close();
			}
			catch (Exception)
			{
				client?.Abort();
				throw;
			}
		}

		public void InjectSoapServiceHost(SoapServiceHost soapServiceHost)
		{
			if (soapServiceHost.ServiceType == typeof(SoapStub))
			{
				_soapStub = (SoapStub) soapServiceHost.SingletonInstance;
			}
			else if (soapServiceHost.ServiceType == typeof(CalculatorServiceRelay))
			{
				_calculatorServiceHost = soapServiceHost;
			}
		}

		private SoapServiceHost _calculatorServiceHost;
		private SoapStub _soapStub;

		private const string CALCULATOR_REQUEST_XML = "<CalculatorRequest xmlns=\"urn:services.stateless.be:unit:calculator\">" +
			"<s0:Arguments xmlns:s0=\"urn:services.stateless.be:unit:calculator\">" +
			"<s0:Term>2</s0:Term>" +
			"<s0:Term>1</s0:Term>" +
			"</s0:Arguments>" +
			"</CalculatorRequest>";

		private const string CALCULATOR_RESPONSE_XML = "<CalculatorResponse xmlns=\"urn:services.stateless.be:unit:calculator\">" +
			"<s0:Result xmlns:s0=\"urn:services.stateless.be:unit:calculator\">{0}</s0:Result>" +
			"</CalculatorResponse>";

		private const string INVALID_CALCULATOR_REQUEST_XML = "<CalculatorRequest xmlns=\"urn:services.stateless.be:unit:calculator\">" +
			"<s0:Arguments xmlns:s0=\"urn:services.stateless.be:unit:calculator\">" +
			"<s0:Operand>2</s0:Operand>" +
			"<s0:Operand>1</s0:Operand>" +
			"</s0:Arguments>" +
			"</CalculatorRequest>";
	}
}
