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
using Be.Stateless.BizTalk.Dummies.ServiceModel.Security;
using Be.Stateless.Extensions;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.ServiceModel.Configuration
{
	public class WebHttpAuthorizationBehaviorExtensionFixture
	{
		[Fact]
		public void DeserializeFromXml()
		{
			var xml = $"<{nameof(WebHttpAuthorizationBehaviorExtension).ToCamelCase()} "
				+ $"{nameof(WebHttpAuthorizationBehaviorExtension.AuthorizationTokenServiceMiddlewareType).ToCamelCase()}=\"{typeof(AuthorizationTokenServiceMiddleware).AssemblyQualifiedName}\" "
				+ $"{nameof(WebHttpAuthorizationBehaviorExtension.AuthorizationTokenServiceUri).ToCamelCase()}=\"https://localhost/sts/token\" />";
			var sut = new WebHttpAuthorizationBehaviorExtensionSpy();
			sut.DeserializeFromXml(xml);
			sut.AuthorizationTokenServiceMiddlewareType.Should().Be(typeof(AuthorizationTokenServiceMiddleware));
			sut.AuthorizationTokenServiceUri.Should().Be("https://localhost/sts/token");
		}

		[Fact]
		public void SerializeToXml()
		{
			var xml = $"<{nameof(WebHttpAuthorizationBehaviorExtension).ToCamelCase()} "
				+ $"{nameof(WebHttpAuthorizationBehaviorExtension.AuthorizationTokenServiceMiddlewareType).ToCamelCase()}=\"{typeof(AuthorizationTokenServiceMiddleware).AssemblyQualifiedName}\" "
				+ $"{nameof(WebHttpAuthorizationBehaviorExtension.AuthorizationTokenServiceUri).ToCamelCase()}=\"https://localhost/sts/token\" />";
			var sut = new WebHttpAuthorizationBehaviorExtensionSpy {
				AuthorizationTokenServiceMiddlewareType = typeof(AuthorizationTokenServiceMiddleware),
				AuthorizationTokenServiceUri = "https://localhost/sts/token"
			};
			XDocument.Parse(sut.SerializeToXml()).Should().BeEquivalentTo(XDocument.Parse(xml));
		}

		[SuppressMessage("ReSharper", "ArgumentsStyleLiteral")]
		private class WebHttpAuthorizationBehaviorExtensionSpy : WebHttpAuthorizationBehaviorExtension
		{
			public void DeserializeFromXml(string xml)
			{
				using (var reader = XmlReader.Create(new StringReader(xml)))
				{
					reader.MoveToContent();
					base.DeserializeElement(reader, serializeCollectionKey: false);
				}
			}

			public string SerializeToXml()
			{
				var builder = new StringBuilder();
				using (var writer = XmlWriter.Create(builder, new XmlWriterSettings { CloseOutput = true, Indent = false, OmitXmlDeclaration = true }))
				{
					writer.WriteStartElement(nameof(WebHttpAuthorizationBehaviorExtension).ToCamelCase());
					base.SerializeElement(writer, serializeCollectionKey: false);
					writer.WriteEndElement();
				}
				return builder.ToString();
			}
		}
	}
}
