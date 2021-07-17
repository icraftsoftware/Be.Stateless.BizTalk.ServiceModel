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
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel.Configuration;
using Be.Stateless.BizTalk.ServiceModel.Description;

namespace Be.Stateless.BizTalk.ServiceModel.Configuration
{
	[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Configuration element.")]
	public class XmlResponseTranslationBehaviorExtension : BehaviorExtensionElement
	{
		[SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Public API.")]
		public XmlResponseTranslationBehaviorExtension()
		{
			_properties.Add(_xmlResponseTranslationCollectionProperty);
		}

		#region Base Class Member Overrides

		public override Type BehaviorType => typeof(XmlResponseTranslationBehavior);

		protected override object CreateBehavior()
		{
			return new XmlResponseTranslationBehavior(XmlResponseTranslations);
		}

		#endregion

		#region Base Class Member Overrides

		protected override ConfigurationPropertyCollection Properties => _properties;

		#endregion

		[ConfigurationProperty(XML_RESPONSE_TRANSLATION_COLLECTION_PROPERTY_NAME, IsDefaultCollection = true, IsRequired = true)]
		public XmlResponseTranslationCollection XmlResponseTranslations
		{
			get => (XmlResponseTranslationCollection) base[_xmlResponseTranslationCollectionProperty];
			set => base[_xmlResponseTranslationCollectionProperty] = value;
		}

		private const string XML_RESPONSE_TRANSLATION_COLLECTION_PROPERTY_NAME = "translations";

		private static readonly ConfigurationPropertyCollection _properties = new();

		private static readonly ConfigurationProperty _xmlResponseTranslationCollectionProperty = new(
			XML_RESPONSE_TRANSLATION_COLLECTION_PROPERTY_NAME,
			typeof(XmlResponseTranslationCollection),
			null,
			ConfigurationPropertyOptions.IsDefaultCollection | ConfigurationPropertyOptions.IsRequired);
	}
}
