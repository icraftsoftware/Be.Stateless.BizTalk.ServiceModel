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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel.Channels;
using Be.Stateless.BizTalk.ContextProperties;
using FluentAssertions;
using Xunit;
using PropertyValuePair = System.Collections.Generic.KeyValuePair<System.Xml.XmlQualifiedName, object>;

namespace Be.Stateless.BizTalk.ServiceModel.Channels.Extensions
{
	public class MessagePropertiesExtensionsFixture
	{
		[Fact]
		public void AddPropertiesToPromote()
		{
			var sut = new MessageProperties {
				{
					MessagePropertiesExtensions.PROPERTIES_TO_PROMOTE_KEY, new List<PropertyValuePair>(
						new[] {
							new PropertyValuePair(BtsProperties.MessageType.QName, nameof(BtsProperties.MessageType)),
							new PropertyValuePair(BtsProperties.Operation.QName, nameof(BtsProperties.Operation))
						})
				}
			};
			sut.AddPropertiesToPromote(
				new[] {
					new PropertyValuePair(BtsProperties.MessageType.QName, "overridden"),
					new PropertyValuePair(BtsProperties.InterchangeID.QName, nameof(BtsProperties.InterchangeID))
				});

			sut[MessagePropertiesExtensions.PROPERTIES_TO_PROMOTE_KEY].Should().BeOfType<List<PropertyValuePair>>().And.BeEquivalentTo(
				new[] {
					new PropertyValuePair(BtsProperties.MessageType.QName, "overridden"),
					new PropertyValuePair(BtsProperties.Operation.QName, nameof(BtsProperties.Operation)),
					new PropertyValuePair(BtsProperties.InterchangeID.QName, nameof(BtsProperties.InterchangeID))
				});
		}

		[Fact]
		public void AddPropertiesToWrite()
		{
			var sut = new MessageProperties();
			sut.AddPropertiesToWrite(
				new[] {
					new PropertyValuePair(BtsProperties.MessageType.QName, nameof(BtsProperties.MessageType)),
					new PropertyValuePair(BtsProperties.InterchangeID.QName, nameof(BtsProperties.InterchangeID))
				});

			sut[MessagePropertiesExtensions.PROPERTIES_TO_WRITE_KEY].Should().BeOfType<List<PropertyValuePair>>().And.BeEquivalentTo(
				new[] {
					new PropertyValuePair(BtsProperties.MessageType.QName, nameof(BtsProperties.MessageType)),
					new PropertyValuePair(BtsProperties.InterchangeID.QName, nameof(BtsProperties.InterchangeID))
				});
		}

		[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
		[Fact]
		public void GetPropertiesToPropagateByPropertyName()
		{
			var sut = new MessageProperties();
			sut.SetProperty(BtsProperties.MessageType, nameof(BtsProperties.MessageType));
			sut.SetProperty(BtsProperties.Operation, nameof(BtsProperties.Operation));
			sut.SetProperty(BtsProperties.ReceiveLocationName, null);

			sut.GetPropertiesToPropagateByPropertyName(
					new[] {
						BtsProperties.MessageType.QName,
						BtsProperties.Operation.QName,
						BtsProperties.ReceiveLocationName.QName,
						BtsProperties.InterchangeID.QName
					})
				.Should().BeEquivalentTo(
					new[] {
						new PropertyValuePair(BtsProperties.MessageType.QName, nameof(BtsProperties.MessageType)),
						new PropertyValuePair(BtsProperties.Operation.QName, nameof(BtsProperties.Operation))
					})
				.And.NotContain(
					kvp => kvp.Key == BtsProperties.ReceiveLocationName.QName
				);
		}
	}
}
