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

using Be.Stateless.BizTalk.ContextProperties;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.ServiceModel.Configuration
{
	public class PropertyPropagationCollectionFixture
	{
		[Fact]
		public void GetPropertyNamesToPromote()
		{
			var sut = new PropertyPropagationCollection {
				new PropertyPropagation { Property = BtsProperties.MessageType, Mode = PropagationMode.Promote },
				new PropertyPropagation { Property = BtsProperties.Operation, Mode = PropagationMode.Write }
			};
			sut.GetPropertyNamesToPromote().Should().BeEquivalentTo(BtsProperties.MessageType.QName);
		}

		[Fact]
		public void GetPropertyNamesToWrite()
		{
			var sut = new PropertyPropagationCollection {
				new PropertyPropagation { Property = BtsProperties.MessageType, Mode = PropagationMode.Promote },
				new PropertyPropagation { Property = BtsProperties.Operation, Mode = PropagationMode.Write }
			};
			sut.GetPropertyNamesToWrite().Should().BeEquivalentTo(BtsProperties.Operation.QName);
		}
	}
}
