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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.Xml.Schema;
using Be.Stateless.BizTalk.Xml.Schema.Extensions;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.ServiceModel.Channels
{
	/// <summary>
	/// Represents an <see cref="IXmlSerializable"/> unit of communication between endpoints in a distributed environment whose
	/// content is defined by an <see cref="SchemaBase"/>-derived XML schema.
	/// </summary>
	/// <typeparam name="TSchemaBase">
	/// The <see cref="SchemaBase"/>-derived XML schema.
	/// </typeparam>
	/// <remarks>
	/// This class helps its derived classes to fulfill the <see cref="XmlSchemaProviderAttribute"/>'s contract, see <see
	/// cref="XmlMessage{TSchemaProvider}.ProvideSchema"/>.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// [XmlSchemaProvider("GetSchema")]
	/// public class CalculatorArguments : XLangMessage<CalculatorMessageSchema.Arguments> {
	///    public new static XmlSchemaType GetSchema(XmlSchemaSet schemaSet) {
	///       return ProvideSchema(schemaSet);
	///    }
	/// 
	///    ...
	/// 
	/// }
	/// ]]></code>
	/// </example>
	[XmlSchemaProvider("GetXmlSchemaForXLangMessage")]
	public class XLangMessage<TSchemaBase> : XmlMessage<XLangMessage<TSchemaBase>.XLangSchemaBaseSchemaProvider>
		where TSchemaBase : SchemaBase, new()
	{
		#region Nested Type: XLangSchemaBaseSchemaProvider

		/// <summary>
		/// Adapter class that provides <see cref="IXmlSchemaProvider"/> support to <see cref="SchemaBase"/>-derived classes.
		/// </summary>
		public class XLangSchemaBaseSchemaProvider : IXmlSchemaProvider
		{
			#region IXmlSchemaProvider Members

			/// <summary>
			/// The <see cref="XmlSchema"/> against which to validate its XML representation.
			/// </summary>
			XmlSchema IXmlSchemaProvider.Schema => new TSchemaBase().CreateResolvedSchema();

			/// <summary>
			/// Provides the <see cref="XmlSchema"/> to the <paramref name="schemaSet"/> and the <see cref="XmlSchemaType"/> that
			/// controls the serialization of the type.
			/// </summary>
			/// <param name="schemaSet">
			/// The <see cref="XmlSchemaSet"/> that will be populated with the <see cref="XmlSchema"/>.
			/// </param>
			/// <returns>
			/// The <see cref="XmlSchemaType"/> that defines its data type.
			/// </returns>
			/// <remarks>
			/// This is a scaffolding method that is meant to be called from within the derived classes' static method identified
			/// by the <see cref="XmlSchemaProviderAttribute"/>.
			/// </remarks>
			XmlSchemaType IXmlSchemaProvider.ProvideSchema(XmlSchemaSet schemaSet)
			{
				schemaSet.Merge(((IXmlSchemaProvider) this).Schema);
				schemaSet.Compile();
				var schemaMetadata = SchemaMetadata.For<TSchemaBase>();
				var element = (XmlSchemaElement) schemaSet.GlobalElements[new XmlQualifiedName(schemaMetadata.RootElementName, schemaMetadata.TargetNamespace)];
				return element.ElementSchemaType;
			}

			#endregion
		}

		#endregion

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "XmlSchemaProvider")]
		public static XmlSchemaType GetXmlSchemaForXLangMessage(XmlSchemaSet schemaSet)
		{
			return ProvideSchema(schemaSet);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XLangMessage{TSchemaBase}"/> class that will skip XML validation.
		/// </summary>
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
		public XLangMessage() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="XLangMessage{TSchemaBase}"/> class that will validate its XML
		/// representation.
		/// </summary>
		/// <param name="schemaContentProcessing">
		/// The <see cref="XmlSchemaContentProcessing"/>, or validation strictness, that will be applied when validating the XML
		/// representation.
		/// </param>
		/// <seealso cref="XmlSchemaContentProcessing"/>
		public XLangMessage(XmlSchemaContentProcessing schemaContentProcessing) : base(schemaContentProcessing) { }
	}
}
