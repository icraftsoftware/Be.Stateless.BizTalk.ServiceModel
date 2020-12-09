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
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Be.Stateless.BizTalk.Unit.ServiceModel
{
	/// <summary>
	/// Triggers the <see cref="SoapServiceHostActivator{THost,TBinding}"/>-derived <see cref="ServiceHost"/> activator meant to
	/// facilitate the NUnit-based unit testing of WCF services and their contracts.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="SoapServiceHostActivatorAttribute"/> ensures that the <see cref="ServiceHost"/> meant to be activated by the
	/// <see cref="SoapServiceHostActivator{THost,TBinding}"/>-derived <see cref="ServiceHost"/> activator is started before any
	/// NUnit-test depending on it runs.
	/// </para>
	/// <para>
	/// Making use of this attribute therefore assumes and requires that unit tests are written on the basis of the NUnit
	/// testing framework.
	/// </para>
	/// </remarks>
	/// <seealso cref="SoapServiceHostActivator{THost,TBinding}"/>
	/// <seealso cref="SoapServiceHost{TService,TChannel}"/>
	/// <seealso cref="ITestAction"/>
	/// <seealso cref="TestActionAttribute"/>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
	[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "NUnit attribute.")]
	public class SoapServiceHostActivatorAttribute : TestActionAttribute
	{
		/// <summary>
		/// Creates a <see cref="SoapServiceHostActivatorAttribute"/> that will ensure that the
		/// <see cref="SoapServiceHostActivator{THost,TBinding}"/>-derived <see cref="ServiceHost"/> activator
		/// <see
		/// cref="SoapServiceHost{TService,TChannel}"/>-derived <paramref name="serviceHostActivator"/> is started before any NUnit-test
		/// depending on it runs.
		/// </summary>
		/// <param name="serviceHostActivator">
		/// The <see cref="Type"/> of the <see cref="SoapServiceHost{TService,TChannel}"/>-derived host to activate.
		/// </param>
		/// <remarks>
		/// Don't forget to <see
		/// href="https://docs.microsoft.com/en-us/dotnet/framework/wcf/feature-details/configuring-http-and-https">configure
		/// HTTP</see>; e.g.:
		/// <code><![CDATA[netsh http add urlacl url=http://+:8001/calculator user=$env:USERDOMAIN\$env:USERNAME]]>!</code>
		/// </remarks>
		/// <seealso href="https://docs.microsoft.com/en-us/dotnet/framework/wcf/feature-details/configuring-http-and-https"/>
		public SoapServiceHostActivatorAttribute(Type serviceHostActivator)
		{
			if (serviceHostActivator == null) throw new ArgumentNullException(nameof(serviceHostActivator));
			ServiceHostActivator = (ISoapServiceHost) Activator.CreateInstance(serviceHostActivator);
		}

		#region Base Class Member Overrides

		public override void AfterTest(ITest test)
		{
			ServiceHostActivator.Dispose();
		}

		public override void BeforeTest(ITest test)
		{
			if (test == null) throw new ArgumentNullException(nameof(test));
			if (test.IsSuite && test.Fixture is ISoapServiceHostInjection fixture) fixture.InjectSoapServiceHost(ServiceHostActivator.Host);
		}

		public override ActionTargets Targets => ActionTargets.Suite;

		#endregion

		protected ISoapServiceHost ServiceHostActivator { get; }
	}
}
