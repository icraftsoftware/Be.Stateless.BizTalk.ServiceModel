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

using System.ComponentModel;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using Be.Stateless.BizTalk.ContextProperties;

namespace Be.Stateless.BizTalk.ServiceModel.Configuration
{
	public sealed class PropertyPropagation : ConfigurationElement
	{
		static PropertyPropagation()
		{
			_properties.Add(_propertyNameProperty);
			_properties.Add(_propertyNamespaceProperty);
			_properties.Add(_propagationModeProperty);
		}

		public PropertyPropagation()
		{
			Mode = PropagationMode.Write;
		}

		#region Base Class Member Overrides

		protected override ConfigurationPropertyCollection Properties => _properties;

		#endregion

		[ConfigurationProperty(PROPAGATION_MODE_PROPERTY_NAME, IsKey = false, IsRequired = true)]
		public PropagationMode Mode
		{
			get => (PropagationMode) base[_propagationModeProperty];
			set => base[_propagationModeProperty] = value;
		}

		public IMessageContextProperty Property
		{
			set => QName = value.QName;
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public XmlQualifiedName QName
		{
			get => new(Name, Namespace);
			set => (Name, Namespace) = (value.Name, value.Namespace);
		}

		internal string Key => $"{Name}#{Namespace}";

		[ConfigurationProperty(PROPERTY_NAME_PROPERTY_NAME, IsKey = true, IsRequired = true)]
		private string Name
		{
			get => (string) base[_propertyNameProperty];
			set => base[_propertyNameProperty] = value;
		}

		[ConfigurationProperty(PROPERTY_NAMESPACE_PROPERTY_NAME, IsKey = true, IsRequired = true)]
		private string Namespace
		{
			get => (string) base[_propertyNamespaceProperty];
			set => base[_propertyNamespaceProperty] = value;
		}

		private const string PROPAGATION_MODE_PROPERTY_NAME = "mode";
		private const string PROPERTY_NAME_PROPERTY_NAME = "name";
		private const string PROPERTY_NAMESPACE_PROPERTY_NAME = "namespace";

		private static readonly ConfigurationPropertyCollection _properties = new();

		private static readonly ConfigurationProperty _propertyNameProperty = new(
			PROPERTY_NAME_PROPERTY_NAME,
			typeof(string),
			null,
			ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);

		private static readonly ConfigurationProperty _propertyNamespaceProperty = new(
			PROPERTY_NAMESPACE_PROPERTY_NAME,
			typeof(string),
			null,
			ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);

		private static readonly ConfigurationProperty _propagationModeProperty = new(
			PROPAGATION_MODE_PROPERTY_NAME,
			typeof(PropagationMode),
			PropagationMode.Write,
			ConfigurationPropertyOptions.IsRequired);
	}
}
