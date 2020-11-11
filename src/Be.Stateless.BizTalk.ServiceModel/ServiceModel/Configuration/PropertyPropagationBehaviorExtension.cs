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
	public class PropertyPropagationBehaviorExtension : BehaviorExtensionElement
	{
		[SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Public API.")]
		public PropertyPropagationBehaviorExtension()
		{
			_properties.Add(_propertyPropagationCollectionProperty);
		}

		#region Base Class Member Overrides

		public override Type BehaviorType => typeof(PropertyPropagationBehavior);

		protected override object CreateBehavior()
		{
			return new PropertyPropagationBehavior(PropertyPropagations);
		}

		#endregion

		#region Base Class Member Overrides

		protected override ConfigurationPropertyCollection Properties => _properties;

		#endregion

		[ConfigurationProperty(PROPERTY_PROPAGATION_COLLECTION_PROPERTY_NAME, IsDefaultCollection = true, IsRequired = true)]
		public PropertyPropagationCollection PropertyPropagations
		{
			get => (PropertyPropagationCollection) base[_propertyPropagationCollectionProperty];
			set => base[_propertyPropagationCollectionProperty] = value;
		}

		private const string PROPERTY_PROPAGATION_COLLECTION_PROPERTY_NAME = "properties";

		private static readonly ConfigurationPropertyCollection _properties = new();

		private static readonly ConfigurationProperty _propertyPropagationCollectionProperty = new(
			PROPERTY_PROPAGATION_COLLECTION_PROPERTY_NAME,
			typeof(PropertyPropagationCollection),
			null,
			ConfigurationPropertyOptions.IsDefaultCollection | ConfigurationPropertyOptions.IsRequired);
	}
}
