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
using System.Linq;
using System.Text.RegularExpressions;

namespace Be.Stateless.BizTalk.ServiceModel.Configuration
{
	[ConfigurationCollection(typeof(XmlResponseTranslation), AddItemName = XML_RESPONSE_TRANSLATION_COLLECTION_ITEM_NAME)]
	public sealed class XmlResponseTranslationCollection : ConfigurationElementCollection
	{
		public XmlResponseTranslationCollection() : base(StringComparer.OrdinalIgnoreCase)
		{
			AddElementName = XML_RESPONSE_TRANSLATION_COLLECTION_ITEM_NAME;
		}

		#region Base Class Member Overrides

		protected override ConfigurationElement CreateNewElement()
		{
			return new XmlResponseTranslation();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((XmlResponseTranslation) element).Url;
		}

		#endregion

		public void Add(XmlResponseTranslation xmlResponseTranslation)
		{
			BaseAdd(xmlResponseTranslation);
		}

		internal XmlResponseTranslation GetXmlResponseTranslationByRequestUrl(Uri url)
		{
			// ensure slashes are not repeated
			var path = Regex.Replace(url.GetComponents(UriComponents.Path | UriComponents.Fragment, UriFormat.Unescaped), "/+", "/");
			return this.Cast<XmlResponseTranslation>().SingleOrDefault(dce => dce.Url == path);
		}

		private const string XML_RESPONSE_TRANSLATION_COLLECTION_ITEM_NAME = "translation";
	}
}
