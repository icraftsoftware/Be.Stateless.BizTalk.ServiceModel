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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.ServiceModel.Channels;
using System.Xml;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.Linq;
using Microsoft.XLANGs.BaseTypes;
using PropertyValuePair = System.Collections.Generic.KeyValuePair<System.Xml.XmlQualifiedName, object>;

namespace Be.Stateless.BizTalk.ServiceModel.Channels.Extensions
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
	public static class MessagePropertiesExtensions
	{
		#region Propagation Extensions

		public static void AddPropertiesToPromote(this MessageProperties messageProperties, IEnumerable<PropertyValuePair> propertiesToWrite)
		{
			messageProperties.AddPropertiesToPropagate(PROPERTIES_TO_PROMOTE_KEY, propertiesToWrite);
		}

		public static void AddPropertiesToWrite(this MessageProperties messageProperties, IEnumerable<PropertyValuePair> propertiesToWrite)
		{
			messageProperties.AddPropertiesToPropagate(PROPERTIES_TO_WRITE_KEY, propertiesToWrite);
		}

		[SuppressMessage("ReSharper", "ReturnTypeCanBeEnumerable.Global")]
		public static List<PropertyValuePair> GetPropertiesToPromote(this MessageProperties messageProperties)
		{
			return (List<PropertyValuePair>) messageProperties[PROPERTIES_TO_PROMOTE_KEY];
		}

		[SuppressMessage("ReSharper", "ReturnTypeCanBeEnumerable.Global")]
		public static List<PropertyValuePair> GetPropertiesToWrite(this MessageProperties messageProperties)
		{
			return (List<PropertyValuePair>) messageProperties[PROPERTIES_TO_WRITE_KEY];
		}

		internal static IEnumerable<PropertyValuePair> GetPropertiesToPropagateByPropertyName(
			this MessageProperties messageProperties,
			IEnumerable<XmlQualifiedName> propertyNames)
		{
			foreach (var name in propertyNames)
			{
				if (messageProperties.TryGetProperty(name, out var value)) yield return new(name, value);
			}
		}

		[SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "Any() only enumerates first item.")]
		private static void AddPropertiesToPropagate(
			this MessageProperties messageProperties,
			string propertiesToPropagateKey,
			IEnumerable<PropertyValuePair> propertiesToPropagate)
		{
			if (!propertiesToPropagate.Any()) return;
			messageProperties[propertiesToPropagateKey] = messageProperties.TryGetValue(propertiesToPropagateKey, out var existingPropertiesToPropagate)
				// Merge gives precedence to values coming from propertiesToPropagate over those coming from existingPropertiesToPropagate
				? propertiesToPropagate.Merge((List<PropertyValuePair>) existingPropertiesToPropagate)
				: propertiesToPropagate.ToList();
		}

		private static List<PropertyValuePair> Merge(this IEnumerable<PropertyValuePair> first, IEnumerable<PropertyValuePair> second)
		{
			return first.Union(second, new LambdaComparer<PropertyValuePair>((vp1, vp2) => vp1.Key == vp2.Key)).ToList();
		}

		internal const string PROPERTIES_TO_PROMOTE_KEY = "http://schemas.microsoft.com/BizTalk/2006/01/Adapters/WCF-properties/Promote";
		internal const string PROPERTIES_TO_WRITE_KEY = "http://schemas.microsoft.com/BizTalk/2006/01/Adapters/WCF-properties/WriteToContext";

		#endregion

		#region GetProperty Extensions

		public static string GetProperty<T>(this MessageProperties properties, MessageContextProperty<T, string> property)
			where T : MessageContextPropertyBase, new()
		{
			return properties.TryGetProperty(property, out var value) ? value : default;
		}

		public static TR GetProperty<T, TR>(this MessageProperties properties, MessageContextProperty<T, TR> property)
			where T : MessageContextPropertyBase, new()
			where TR : struct
		{
			return properties.TryGetProperty(property, out var value) ? value : default;
		}

		public static object GetProperty(this MessageProperties properties, XmlQualifiedName propertyName)
		{
			return properties.TryGetProperty(propertyName, out var value) ? value : default;
		}

		#endregion

		#region SetProperty Extensions

		public static void SetProperty<T>(this MessageProperties properties, MessageContextProperty<T, string> property, string value)
			where T : MessageContextPropertyBase, new()
		{
			properties.SetProperty(property.QName, value);
		}

		public static void SetProperty<T, TV>(this MessageProperties properties, MessageContextProperty<T, TV> property, TV value)
			where T : MessageContextPropertyBase, new()
			where TV : struct
		{
			properties.SetProperty(property.QName, value);
		}

		public static void SetProperty(this MessageProperties properties, XmlQualifiedName propertyName, object value)
		{
			if (properties == null) throw new ArgumentNullException(nameof(properties));
			properties[propertyName.AsPropertyKey()] = value;
		}

		#endregion

		#region TryGetProperty Extensions

		public static bool TryGetProperty<T>(this MessageProperties properties, MessageContextProperty<T, string> property, out string value)
			where T : MessageContextPropertyBase, new()
		{
			if (properties.TryGetProperty(property.QName, out var @object))
			{
				value = @object.ToString();
				return true;
			}
			value = default;
			return false;
		}

		public static bool TryGetProperty<T, TR>(this MessageProperties properties, MessageContextProperty<T, TR> property, out TR value)
			where T : MessageContextPropertyBase, new()
			where TR : struct
		{
			if (properties.TryGetProperty(property.QName, out var @object))
			{
				value = (TR) @object;
				return true;
			}
			value = default;
			return false;
		}

		public static bool TryGetProperty(this MessageProperties properties, XmlQualifiedName property, out object value)
		{
			if (properties == null) throw new ArgumentNullException(nameof(properties));
			return properties.TryGetValue(property.AsPropertyKey(), out value);
		}

		private static string AsPropertyKey(this XmlQualifiedName qualifiedName)
		{
			return $"{qualifiedName.Namespace}#{qualifiedName.Name}";
		}

		#endregion
	}
}
