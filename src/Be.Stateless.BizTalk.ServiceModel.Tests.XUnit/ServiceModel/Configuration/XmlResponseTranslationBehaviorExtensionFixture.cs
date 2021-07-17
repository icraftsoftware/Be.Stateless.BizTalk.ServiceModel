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

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Be.Stateless.Extensions;
using Be.Stateless.Xml.Extensions;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.ServiceModel.Configuration
{
	public class XmlResponseTranslationBehaviorExtensionFixture
	{
		[Fact]
		public void DeserializeFromXml()
		{
			var xml = $"<{nameof(XmlResponseTranslationBehaviorExtension).ToCamelCase()}><translations>"
				+ "<translation matchingPattern=\"\" replacementPattern=\"urn:ns\" url=\"/api/v1/resource\" />"
				+ "<translation matchingPattern=\"urn:old\" replacementPattern=\"urn:new\" url=\"/api/v2/resource\" />"
				+ $"</translations></{nameof(XmlResponseTranslationBehaviorExtension).ToCamelCase()}>";
			var sut = new XmlResponseTranslationBehaviorExtensionSpy();
			sut.DeserializeFromXml(xml);
			sut.XmlResponseTranslations.Should().BeEquivalentTo(
				new XmlResponseTranslationCollection {
					new XmlResponseTranslation { MatchingPattern = string.Empty, ReplacementPattern = "urn:ns", Url = "/api/v1/resource" },
					new XmlResponseTranslation { MatchingPattern = "urn:old", ReplacementPattern = "urn:new", Url = "/api/v2/resource" }
				});
		}

		[Fact]
		public void SerializeToXml()
		{
			var xml = $"<{nameof(XmlResponseTranslationBehaviorExtension).ToCamelCase()}><translations>"
				+ "<translation matchingPattern=\"\" replacementPattern=\"urn:ns\" url=\"/api/v1/resource\" />"
				+ "<translation matchingPattern=\"urn:old\" replacementPattern=\"urn:new\" url=\"/api/v2/resource\" />"
				+ $"</translations></{nameof(XmlResponseTranslationBehaviorExtension).ToCamelCase()}>";
			var sut = new XmlResponseTranslationBehaviorExtensionSpy {
				XmlResponseTranslations = new XmlResponseTranslationCollection {
					new XmlResponseTranslation { MatchingPattern = string.Empty, ReplacementPattern = "urn:ns", Url = "/api/v1/resource" },
					new XmlResponseTranslation { MatchingPattern = "urn:old", ReplacementPattern = "urn:new", Url = "/api/v2/resource" }
				}
			};
			XDocument.Parse(sut.SerializeToXml()).Should().BeEquivalentTo(XDocument.Parse(xml));
		}

		[SuppressMessage("ReSharper", "ArgumentsStyleLiteral")]
		private class XmlResponseTranslationBehaviorExtensionSpy : XmlResponseTranslationBehaviorExtension
		{
			public void DeserializeFromXml(string xml)
			{
				using (var reader = XmlReader.Create(new StringReader(xml)))
				{
					reader.MoveToContent();
					base.DeserializeElement(reader, serializeCollectionKey: false);
					reader.ReadEndElement(nameof(XmlResponseTranslationBehaviorExtension).ToCamelCase());
				}
			}

			public string SerializeToXml()
			{
				var builder = new StringBuilder();
				using (var writer = XmlWriter.Create(builder, new XmlWriterSettings { CloseOutput = true, Indent = false, OmitXmlDeclaration = true }))
				{
					writer.WriteStartElement(nameof(XmlResponseTranslationBehaviorExtension).ToCamelCase());
					base.SerializeElement(writer, serializeCollectionKey: false);
					writer.WriteEndElement();
				}
				return builder.ToString();
			}
		}
	}
}
