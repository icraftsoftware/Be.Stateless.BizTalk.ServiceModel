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
using System.Diagnostics.CodeAnalysis;
using System.Xml.Schema;

namespace Be.Stateless.BizTalk.Xml.Schema.Extensions
{
	public static class XmlSchemaExtensions
	{
		/// <summary>
		/// Merge an <see cref="XmlSchema"/> into another <see cref="XmlSchema"/>.
		/// </summary>
		/// <param name="existingSchema">
		/// The <see cref="XmlSchema"/> into which the other <see cref="XmlSchema"/> will be merged.
		/// </param>
		/// <param name="schema">
		/// The <see cref="XmlSchema"/> to be merged into the other <see cref="XmlSchema"/>.
		/// </param>
		/// <returns>
		/// The resulting <see cref="XmlSchema"/>; i.e. the <paramref name="existingSchema"/> into which the other <paramref
		/// name="schema"/> has been merged.
		/// </returns>
		/// <seealso href="http://stackoverflow.com/questions/6312154/xmlschema-removing-duplicate-types" />
		[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
		public static XmlSchema Merge(this XmlSchema existingSchema, XmlSchema schema)
		{
			if (existingSchema == null) throw new ArgumentNullException(nameof(existingSchema));
			if (schema == null) throw new ArgumentNullException(nameof(schema));
			foreach (var item in schema.Items)
			{
				switch (item)
				{
					case XmlSchemaType type when !existingSchema.SchemaTypes.Contains(type.QualifiedName):
						existingSchema.Items.Add(type);
						break;
					case XmlSchemaElement element when !existingSchema.Elements.Contains(element.QualifiedName):
						existingSchema.Items.Add(element);
						break;
					case XmlSchemaAnnotation:
						// ignore XmlSchemaAnnotation for merge operations
						break;
					default:
						throw new InvalidOperationException(item.GetType().Name);
				}
			}
			return existingSchema;
		}
	}
}
