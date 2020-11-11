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
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;

namespace Be.Stateless.BizTalk.ServiceModel.Configuration
{
	[ConfigurationCollection(typeof(ConfigurationElement), AddItemName = PROPERTY_COLLECTION_ITEM_NAME)]
	public sealed class PropertyPropagationCollection : ConfigurationElementCollection
	{
		public PropertyPropagationCollection() : base(StringComparer.OrdinalIgnoreCase)
		{
			AddElementName = PROPERTY_COLLECTION_ITEM_NAME;
		}

		#region Base Class Member Overrides

		protected override ConfigurationElement CreateNewElement()
		{
			return new PropertyPropagation();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((PropertyPropagation) element).Key;
		}

		#endregion

		public void Add(PropertyPropagation propertyPropagation)
		{
			BaseAdd(propertyPropagation);
		}

		internal IEnumerable<XmlQualifiedName> GetPropertyNamesToPromote()
		{
			return GetPropertyNamesByPropagationMode(PropagationMode.Promote);
		}

		internal IEnumerable<XmlQualifiedName> GetPropertyNamesToWrite()
		{
			return GetPropertyNamesByPropagationMode(PropagationMode.Write);
		}

		private IEnumerable<XmlQualifiedName> GetPropertyNamesByPropagationMode(PropagationMode mode)
		{
			return this.Cast<PropertyPropagation>().Where(e => e.Mode == mode).Select(p => p.QName);
		}

		private const string PROPERTY_COLLECTION_ITEM_NAME = "property";
	}
}
