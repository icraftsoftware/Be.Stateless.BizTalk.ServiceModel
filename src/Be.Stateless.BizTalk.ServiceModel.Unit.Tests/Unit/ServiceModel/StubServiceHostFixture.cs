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

using System.ServiceModel;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.Unit.ServiceModel
{
	public class StubServiceHostFixture
	{
		[Fact]
		public void DefaultBinding()
		{
			StubServiceHost.DefaultBinding.Should().BeOfType<BasicHttpBinding>();
		}

		[Fact]
		public void DefaultEndpointAddress()
		{
			StubServiceHost.DefaultEndpointAddress.Should().Be(new EndpointAddress("http://localhost:8000/stubservice"));
		}

		[Fact]
		public void DefaultInstanceRecycling()
		{
			StubServiceHost.DefaultInstance.Should().NotBeNull();
			var instance = StubServiceHost.DefaultInstance;
			StubServiceHost.DefaultInstance.Recycle();
			StubServiceHost.DefaultInstance.Should().NotBeSameAs(instance);
		}
	}
}
