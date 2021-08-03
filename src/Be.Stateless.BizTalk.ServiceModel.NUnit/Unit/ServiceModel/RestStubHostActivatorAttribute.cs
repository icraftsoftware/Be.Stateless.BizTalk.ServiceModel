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
using Be.Stateless.BizTalk.Unit.ServiceModel.Stub;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Be.Stateless.BizTalk.Unit.ServiceModel
{
	public class RestStubHostActivatorAttribute : TestActionAttribute
	{
		public RestStubHostActivatorAttribute()
		{
			RestStubHostActivator = new RestStubHostActivator();
		}

		#region Base Class Member Overrides

		public override void AfterTest(ITest test)
		{
			if (test.IsSuite) RestStubHostActivator.Dispose();
		}

		public override void BeforeTest(ITest test)
		{
			if (test == null) throw new ArgumentNullException(nameof(test));
			if (test.IsSuite && test.Fixture is IRestServiceHostInjection fixture) fixture.InjectRestServiceHost(RestStubHostActivator.Host);
			if (!test.IsSuite) RestStubHostActivator.Host.Reset();
		}

		public override ActionTargets Targets => ActionTargets.Suite | ActionTargets.Test;

		#endregion

		protected RestStubHostActivator RestStubHostActivator { get; }
	}
}
