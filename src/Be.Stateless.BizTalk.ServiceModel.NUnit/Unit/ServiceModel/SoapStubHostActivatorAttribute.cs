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
using Be.Stateless.BizTalk.Unit.ServiceModel.Stub;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Be.Stateless.BizTalk.Unit.ServiceModel
{
	/// <summary>
	/// Triggers the <see cref="SoapStubHostActivator"/>-derived <see cref="ServiceHost"/> activator meant to facilitate
	/// NUnit-based unit testing requiring to interact with the <see cref="SoapStub"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="SoapStubHostActivatorAttribute"/> ensures that the <see cref="ServiceHost"/> meant to be activated by the
	/// <see cref="SoapStubHostActivator"/>-derived <see cref="ServiceHost"/> activator is started before any NUnit-test
	/// depending on it runs.
	/// </para>
	/// <para>
	/// Making use of this attribute therefore assumes and requires that unit tests are written on the basis of the NUnit
	/// testing framework.
	/// </para>
	/// <para>
	/// The in-process <see cref="SoapStubHostActivator"/> will host the <see cref="SoapStub"/> service's endpoint over the <see
	/// cref="BasicHttpBinding"/> binding at the <see cref="SoapStubHostActivator.Uri">SoapStubHostActivator.Uri</see> address,
	/// i.e. <c>http://localhost:8000/soap-stub</c>.
	/// </para>
	/// </remarks>
	/// <seealso cref="SoapServiceHostActivatorAttribute"/>
	/// <seealso cref="SoapStubHostActivator"/>
	/// <seealso cref="SoapServiceHostActivator{THost,TBinding}"/>
	/// <seealso cref="SoapServiceHost{TService,TChannel}"/>
	/// <seealso cref="ITestAction"/>
	/// <seealso cref="TestActionAttribute"/>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
	[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public API.")]
	public class SoapStubHostActivatorAttribute : SoapServiceHostActivatorAttribute
	{
		public SoapStubHostActivatorAttribute() : base(typeof(SoapStubHostActivator)) { }

		#region Base Class Member Overrides

		public override void AfterTest(ITest testDetails)
		{
			if (testDetails == null) throw new ArgumentNullException(nameof(testDetails));
			if (testDetails.IsSuite)
			{
				ServiceHostActivator.Dispose();
			}
		}

		public override void BeforeTest(ITest testDetails)
		{
			base.BeforeTest(testDetails);
			if (testDetails == null) throw new ArgumentNullException(nameof(testDetails));
			if (!testDetails.IsSuite)
			{
				((SoapStub)((SoapStubHostActivator) ServiceHostActivator).Host.SingletonInstance).ClearSetups();
			}
		}

		public override ActionTargets Targets => ActionTargets.Suite | ActionTargets.Test;

		#endregion
	}
}
