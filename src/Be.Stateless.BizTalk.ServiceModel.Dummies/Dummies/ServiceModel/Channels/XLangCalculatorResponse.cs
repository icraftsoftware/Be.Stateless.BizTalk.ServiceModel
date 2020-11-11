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

using System.Diagnostics.CodeAnalysis;
using System.Xml.Schema;
using System.Xml.Serialization;
using Be.Stateless.BizTalk.Dummies.Xml.Schema;
using Be.Stateless.BizTalk.ServiceModel.Channels;

namespace Be.Stateless.BizTalk.Dummies.ServiceModel.Channels
{
	[XmlSchemaProvider("GetSchema")]
	public class XLangCalculatorResponse : XLangMessage<CalculatorSchema.Response>
	{
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "XmlSchemaProvider")]
		public static XmlSchemaType GetSchema(XmlSchemaSet schemaSet)
		{
			return ProvideSchema(schemaSet);
		}

		public XLangCalculatorResponse() : base(XmlSchemaContentProcessing.Strict) { }
	}
}
