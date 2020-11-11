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

using System.Configuration;
using Be.Stateless.BizTalk.Stream;

namespace Be.Stateless.BizTalk.ServiceModel.Configuration
{
	public sealed class XmlResponseTranslation : ConfigurationElement
	{
		#region Operators

		public static implicit operator XmlNamespaceTranslation[](XmlResponseTranslation xmlResponseTranslation)
		{
			return new[] { (XmlNamespaceTranslation) xmlResponseTranslation };
		}

		public static implicit operator XmlNamespaceTranslation(XmlResponseTranslation xmlResponseTranslation)
		{
			return new(xmlResponseTranslation.MatchingPattern, xmlResponseTranslation.ReplacementPattern);
		}

		#endregion

		static XmlResponseTranslation()
		{
			_properties.Add(_urlProperty);
			_properties.Add(_matchingPatternProperty);
			_properties.Add(_replacementPatternProperty);
		}

		#region Base Class Member Overrides

		protected override ConfigurationPropertyCollection Properties => _properties;

		#endregion

		[ConfigurationProperty(MATCHING_PATTERN_PROPERTY_NAME, IsKey = false, IsRequired = true)]
		public string MatchingPattern
		{
			get => (string) base[_matchingPatternProperty];
			set => base[_matchingPatternProperty] = value;
		}

		[ConfigurationProperty(REPLACEMENT_PATTERN_PROPERTY_NAME, IsKey = false, IsRequired = true)]
		public string ReplacementPattern
		{
			get => (string) base[_replacementPatternProperty];
			set => base[_replacementPatternProperty] = value;
		}

		/// <summary>
		/// Request's URL sub-path.
		/// </summary>
		[ConfigurationProperty(URL_PROPERTY_NAME, IsKey = true, IsRequired = true)]
		public string Url
		{
			get => (string) base[_urlProperty];
			set => base[_urlProperty] = value;
		}

		private const string MATCHING_PATTERN_PROPERTY_NAME = "matchingPattern";
		private const string REPLACEMENT_PATTERN_PROPERTY_NAME = "replacementPattern";
		private const string URL_PROPERTY_NAME = "url";

		private static readonly ConfigurationPropertyCollection _properties = new();

		private static readonly ConfigurationProperty _matchingPatternProperty = new(
			MATCHING_PATTERN_PROPERTY_NAME,
			typeof(string),
			null,
			ConfigurationPropertyOptions.IsRequired);

		private static readonly ConfigurationProperty _replacementPatternProperty = new(
			REPLACEMENT_PATTERN_PROPERTY_NAME,
			typeof(string),
			null,
			ConfigurationPropertyOptions.IsRequired);

		private static readonly ConfigurationProperty _urlProperty = new(
			URL_PROPERTY_NAME,
			typeof(string),
			null,
			ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);
	}
}
