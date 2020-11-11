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
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.Extensions;
using Be.Stateless.Xml.Extensions;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.ServiceModel.Configuration
{
	public class PropertyPropagationBehaviorExtensionFixture
	{
		[Fact]
		public void DeserializeFromXml()
		{
			var xml = $"<{nameof(PropertyPropagationBehaviorExtension).ToCamelCase()}><properties>"
				+ "<property name=\"MessageType\" namespace=\"http://schemas.microsoft.com/BizTalk/2003/system-properties\" mode=\"Promote\" />"
				+ "<property name=\"Operation\" namespace=\"http://schemas.microsoft.com/BizTalk/2003/system-properties\" mode=\"Write\" />"
				+ "<property name=\"MessageDestination\" namespace=\"http://schemas.microsoft.com/BizTalk/2003/system-properties\" />"
				+ $"</properties></{nameof(PropertyPropagationBehaviorExtension).ToCamelCase()}>";
			var sut = new PropertyPropagationBehaviorExtensionSpy();
			sut.DeserializeFromXml(xml);
			sut.PropertyPropagations.Should().BeEquivalentTo(
				new PropertyPropagationCollection {
					new PropertyPropagation { Property = BtsProperties.MessageType, Mode = PropagationMode.Promote },
					new PropertyPropagation { Property = BtsProperties.Operation, Mode = PropagationMode.Write },
					new PropertyPropagation { Property = BtsProperties.MessageDestination, Mode = PropagationMode.Write }
				});
		}

		[Fact]
		public void SerializeToXml()
		{
			var xml = $"<{nameof(PropertyPropagationBehaviorExtension).ToCamelCase()}><properties>"
				+ "<property name=\"MessageType\" namespace=\"http://schemas.microsoft.com/BizTalk/2003/system-properties\" mode=\"Promote\" />"
				+ "<property name=\"Operation\" namespace=\"http://schemas.microsoft.com/BizTalk/2003/system-properties\" mode=\"Write\" />"
				+ "<property name=\"MessageDestination\" namespace=\"http://schemas.microsoft.com/BizTalk/2003/system-properties\" mode=\"Write\" />"
				+ $"</properties></{nameof(PropertyPropagationBehaviorExtension).ToCamelCase()}>";
			var sut = new PropertyPropagationBehaviorExtensionSpy {
				PropertyPropagations = new() {
					new() { Property = BtsProperties.MessageType, Mode = PropagationMode.Promote },
					new() { Property = BtsProperties.Operation, Mode = PropagationMode.Write },
					new() { Property = BtsProperties.MessageDestination }
				}
			};
			XDocument.Parse(sut.SerializeToXml()).Should().BeEquivalentTo(XDocument.Parse(xml));
		}

		[SuppressMessage("ReSharper", "ArgumentsStyleLiteral")]
		private class PropertyPropagationBehaviorExtensionSpy : PropertyPropagationBehaviorExtension
		{
			public void DeserializeFromXml(string xml)
			{
				using (var reader = XmlReader.Create(new StringReader(xml)))
				{
					reader.MoveToContent();
					base.DeserializeElement(reader, serializeCollectionKey: false);
					reader.ReadEndElement(nameof(PropertyPropagationBehaviorExtension).ToCamelCase());
				}
			}

			public string SerializeToXml()
			{
				var builder = new StringBuilder();
				using (var writer = XmlWriter.Create(builder, new() { CloseOutput = true, Indent = false, OmitXmlDeclaration = true }))
				{
					writer.WriteStartElement(nameof(PropertyPropagationBehaviorExtension).ToCamelCase());
					base.SerializeElement(writer, serializeCollectionKey: false);
					writer.WriteEndElement();
				}
				return builder.ToString();
			}
		}
	}
}
