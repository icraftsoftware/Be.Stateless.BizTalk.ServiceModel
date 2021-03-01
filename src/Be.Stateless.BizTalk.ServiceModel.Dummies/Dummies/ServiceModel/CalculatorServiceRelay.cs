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
using Be.Stateless.BizTalk.Dummies.ServiceModel.Channels;
using Be.Stateless.BizTalk.Dummies.Transform;
using Be.Stateless.BizTalk.ServiceModel;
using Be.Stateless.BizTalk.ServiceModel.Channels;
using Be.Stateless.BizTalk.Unit.ServiceModel.Stub;
using Be.Stateless.BizTalk.Xml.Xsl;

namespace Be.Stateless.BizTalk.Dummies.ServiceModel
{
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class CalculatorServiceRelay : ServiceRelay, ICalculatorService, ICalculatorStateService, ITranslatingCalculatorService, IValidatingCalculatorService
	{
		public CalculatorServiceRelay() : base(SoapStubHostActivator.Binding, new EndpointAddress(SoapStubHostActivator.Uri)) { }

		#region ICalculatorService Members

		public XmlCalculatorResponse Add(XmlCalculatorRequest request)
		{
			return RelayRequest<XmlCalculatorRequest, XmlCalculatorResponse>(request);
		}

		public XmlCalculatorResponse Subtract(XmlCalculatorRequest request)
		{
			return RelayRequest<XmlCalculatorRequest, XmlCalculatorResponse>(request, TimeSpan.FromMilliseconds(100));
		}

		public IAsyncResult BeginMultiply(XmlCalculatorRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return BeginRelayRequest(request, asyncCallback, asyncState);
		}

		public XmlCalculatorResponse EndMultiply(IAsyncResult asyncResult)
		{
			return EndRelayRequest<XmlCalculatorResponse>(asyncResult);
		}

		public IAsyncResult BeginDivide(XmlCalculatorRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return BeginRelayRequest(request, TimeSpan.FromMilliseconds(100), asyncCallback, asyncState);
		}

		public XmlCalculatorResponse EndDivide(IAsyncResult asyncResult)
		{
			return EndRelayRequest<XmlCalculatorResponse>(asyncResult);
		}

		#endregion

		#region ICalculatorStateService Members

		public void Reset(XLangCalculatorRequest request)
		{
			RelayRequest<XLangCalculatorRequest, EmptyXmlMessage>(request, new XLangTranslator<IdentityTransform, IdentityTransform>());
		}

		public IAsyncResult BeginClean(XLangCalculatorRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return BeginRelayRequest(request, new XLangTranslator<IdentityTransform, IdentityTransform>(), asyncCallback, asyncState);
		}

		public void EndClean(IAsyncResult asyncResult)
		{
			EndRelayRequest<EmptyXmlMessage>(asyncResult);
		}

		#endregion

		#region ITranslatingCalculatorService Members

		public XLangCalculatorResponse Subtract(XLangCalculatorRequest request)
		{
			return RelayRequest<XLangCalculatorRequest, XLangCalculatorResponse>(request, new XLangTranslator<IdentityTransform, IdentityTransform>());
		}

		public IAsyncResult BeginDivide(XLangCalculatorRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return BeginRelayRequest(request, new XLangTranslator<IdentityTransform, IdentityTransform>(), asyncCallback, asyncState);
		}

		XLangCalculatorResponse ITranslatingCalculatorService.EndDivide(IAsyncResult asyncResult)
		{
			return EndRelayRequest<XLangCalculatorResponse>(asyncResult);
		}

		#endregion

		#region IValidatingCalculatorService Members

		public XLangCalculatorResponse Add(XLangCalculatorRequest request)
		{
			return RelayRequest<XLangCalculatorRequest, XLangCalculatorResponse>(request);
		}

		public IAsyncResult BeginMultiply(XLangCalculatorRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return BeginRelayRequest(request, asyncCallback, asyncState);
		}

		XLangCalculatorResponse IValidatingCalculatorService.EndMultiply(IAsyncResult asyncResult)
		{
			return EndRelayRequest<XLangCalculatorResponse>(asyncResult);
		}

		#endregion
	}
}
