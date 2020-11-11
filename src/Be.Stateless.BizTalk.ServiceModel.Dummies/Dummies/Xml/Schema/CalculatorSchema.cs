#region Copyright & License

// Copyright © 2012 - 2020 François Chabot
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
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Dummies.Xml.Schema
{
	[SchemaType(SchemaTypeEnum.Document)]
	[SchemaRoots(new[] { @"CalculatorRequest", @"CalculatorResponse", @"LaxArguments", @"LaxCalculatorRequest", @"LaxCalculatorResponse", @"LaxResult" })]
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Generic parameter.")]
	public sealed class CalculatorSchema : SchemaBase
	{
		#region Nested Type: LaxArguments

		[Schema(@"urn:services.stateless.be:unit:calculator", @"LaxArguments")]
		[SchemaRoots(new[] { @"LaxArguments" })]
		[Serializable]
		public sealed class LaxArguments : SchemaBase
		{
			#region Base Class Member Overrides

			protected override object RawSchema
			{
				get => null;
				set { }
			}

			public override string[] RootNodes
			{
				get { return new[] { "LaxArguments" }; }
			}

			public override string XmlContent => XML_CONTENT;

			#endregion
		}

		#endregion

		#region Nested Type: Request

		[Schema(@"urn:services.stateless.be:unit:calculator", @"CalculatorRequest")]
		[SchemaRoots(new[] { @"CalculatorRequest" })]
		[Serializable]
		public sealed class Request : SchemaBase
		{
			#region Base Class Member Overrides

			protected override object RawSchema
			{
				get => null;
				set { }
			}

			public override string[] RootNodes
			{
				get { return new[] { "CalculatorRequest" }; }
			}

			public override string XmlContent => XML_CONTENT;

			#endregion
		}

		#endregion

		#region Nested Type: Response

		[Schema(@"urn:services.stateless.be:unit:calculator", @"CalculatorResponse")]
		[SchemaRoots(new[] { @"CalculatorResponse" })]
		[Serializable]
		public sealed class Response : SchemaBase
		{
			#region Base Class Member Overrides

			protected override object RawSchema
			{
				get => null;
				set { }
			}

			public override string[] RootNodes
			{
				get { return new[] { "CalculatorResponse" }; }
			}

			public override string XmlContent => XML_CONTENT;

			#endregion
		}

		#endregion

		#region Base Class Member Overrides

		protected override object RawSchema
		{
			get => null;
			set { }
		}

		public override string[] RootNodes
		{
			get { return new[] { @"CalculatorRequest", @"CalculatorResponse", @"LaxArguments", @"LaxCalculatorRequest", @"LaxCalculatorResponse", @"LaxResult" }; }
		}

		public override string XmlContent => XML_CONTENT;

		#endregion

		[NonSerialized]
		private const string XML_CONTENT = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xs:schema xmlns=""urn:services.stateless.be:unit:calculator"" xmlns:b=""http://schemas.microsoft.com/BizTalk/2003"" elementFormDefault=""qualified"" targetNamespace=""urn:services.stateless.be:unit:calculator"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:element name=""CalculatorRequest"">
    <xs:complexType>
      <xs:sequence>
        <xs:element name=""Arguments"">
          <xs:complexType>
            <xs:sequence>
              <xs:element name=""Term"" type=""xs:int"" />
              <xs:element name=""Term"" type=""xs:int"" />
            </xs:sequence>
          </xs:complexType>
        </xs:element>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:element name=""CalculatorResponse"">
    <xs:complexType>
      <xs:sequence>
        <xs:element name=""Result"" type=""xs:int"" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <!-- the following element is there to experience with various and alternate ways of declaring XML schema elements and how they behave through IXmlSchemaProvider.ProvideSchema() -->
  <xs:element name=""LaxArguments"">
    <xs:complexType>
      <xs:sequence>
        <xs:element name=""Term"" type=""xs:int"" />
        <xs:element name=""Term"" type=""xs:int"" />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>";
	}
}
