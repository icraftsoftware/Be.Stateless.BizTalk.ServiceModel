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
using Be.Stateless.BizTalk.Dummies.ServiceModel.Channels;
using Be.Stateless.BizTalk.Unit.ServiceModel.Stub;

namespace Be.Stateless.BizTalk.Dummies.ServiceModel
{
	[ServiceContract(Namespace = "urn:services.stateless.be:unit:calculator", Name = "calculator-state")]
	[XmlSerializerFormat]
	public interface ICalculatorStateService
	{
		[OperationContract(AsyncPattern = true, Action = "Clean", ReplyAction = "Clean/Response")]
		IAsyncResult BeginClean(
			[MessageParameter(Name = "CalculatorRequest")]
			XLangCalculatorRequest request,
			AsyncCallback asyncCallback,
			object asyncState);

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Async contract.")]
		void EndClean(IAsyncResult asyncResult);

		[OperationContract(Action = "Reset", ReplyAction = "Reset/Response")]
		void Reset(
			[MessageParameter(Name = "CalculatorRequest")]
			XLangCalculatorRequest request);
	}

	/// <summary>
	/// Client-side version of the <see cref="ICalculatorStateService"/> contract that allow for easier setup of the <see
	/// cref="SoapStub"/> as all asynchronous operations have been translated into their analogous synchronous counter-part, not
	/// differently from what <c>svcutil.exe</c> would do when generating a service proxy/client code.
	/// </summary>
	[ServiceContract(Namespace = "urn:services.stateless.be:unit:calculator", Name = "PerformingService")]
	[XmlSerializerFormat]
	public interface ICalculatorStateServiceSync
	{
		[OperationContract(Action = "Clean", ReplyAction = "Clean/Response")]
		void Clean(
			[MessageParameter(Name = "CalculatorRequest")]
			XLangCalculatorRequest request);
	}
}
