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

using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.ServiceModel.Configuration
{
	public class XmlResponseTranslationCollectionFixture
	{
		[Fact]
		public void GetXmlResponseTranslationByRequestUrl()
		{
			var translation1 = new XmlResponseTranslation { MatchingPattern = string.Empty, ReplacementPattern = "urn:ns", Url = "/api/v1/resource" };
			var translation2 = new XmlResponseTranslation { MatchingPattern = "urn:old", ReplacementPattern = "urn:new", Url = "/api/v2/resource#fragment" };
			var sut = new XmlResponseTranslationCollection { translation1, translation2 };
			sut.GetXmlResponseTranslationByRequestUrl(new("http://localhost/api/v1/resource")).Should().Be(translation1);
			sut.GetXmlResponseTranslationByRequestUrl(new("http://localhost/api/v2/resource")).Should().BeNull();
			sut.GetXmlResponseTranslationByRequestUrl(new("http://localhost/api/v2/resource#fragment")).Should().Be(translation2);
		}
	}
}
