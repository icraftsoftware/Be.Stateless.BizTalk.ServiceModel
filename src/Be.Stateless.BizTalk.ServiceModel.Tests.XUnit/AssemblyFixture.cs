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

using System.Diagnostics.CodeAnalysis;
using log4net.Config;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Be.Stateless.BizTalk
{
	// see https://stackoverflow.com/a/53143426/1789441
	// see https://github.com/xunit/samples.xunit/tree/main/AssemblyFixtureExample
	[SuppressMessage("ReSharper", "UnusedType.Global")]
	public class AssemblyFixture : XunitTestFramework
	{
		public AssemblyFixture(IMessageSink messageSink) : base(messageSink)
		{
			// log4net console output not shown in output window, see https://github.com/xunit/resharper-xunit/issues/102
			// hence logs will only be captured while debugging tests
			XmlConfigurator.Configure();
		}
	}
}
