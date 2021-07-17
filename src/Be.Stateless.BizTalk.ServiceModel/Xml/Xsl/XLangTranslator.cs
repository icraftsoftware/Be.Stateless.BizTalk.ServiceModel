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
using System.Xml.Xsl;
using Be.Stateless.BizTalk.Runtime.Caching;
using Be.Stateless.BizTalk.ServiceModel.Channels;
using Microsoft.XLANGs.BaseTypes;
using XsltArgumentList = Be.Stateless.Xml.Xsl.XsltArgumentList;

namespace Be.Stateless.BizTalk.Xml.Xsl
{
	/// <summary>
	/// Translates <see cref="XmlMessage"/>s back and forth to native generic WCF <see
	/// cref="System.ServiceModel.Channels.Message"/>s by applying an <see cref="TransformBase"/>-derived type XSLT transform to
	/// their payloads.
	/// </summary>
	[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
	public class XLangTranslator<TRequestTransform, TResponseTransform> : XsltTranslatorBase
		where TRequestTransform : TransformBase, new()
		where TResponseTransform : TransformBase, new()
	{
		#region Base Class Member Overrides

		/// <summary>
		/// <see cref="XslCompiledTransform"/> to apply to the request <see cref="XmlMessage"/>'s body.
		/// </summary>
		protected override XslCompiledTransform RequestXslt => XsltCache.Instance[typeof(TRequestTransform)].XslCompiledTransform;

		/// <summary>
		/// <see cref="XslCompiledTransform"/> to apply to get the response <see cref="XmlMessage"/>'s body.
		/// </summary>
		protected override XslCompiledTransform ResponseXslt => XsltCache.Instance[typeof(TResponseTransform)].XslCompiledTransform;

		/// <summary>
		/// Translates a request <see cref="XmlMessage"/>-derived message's body, whose content is given by <paramref
		/// name="reader"/>, and outputs the results to <paramref name="writer"/>.
		/// </summary>
		/// <param name="reader">
		/// The <see cref="XmlReader"/> containing the input document.
		/// </param>
		/// <param name="writer">
		/// The <see cref="XmlWriter"/> that will contain the output of the transform.
		/// </param>
		/// <remarks>
		/// This override reuses the extension objects that have been defined inside the <typeparamref
		/// name="TRequestTransform"/>.
		/// </remarks>
		protected override void TranslateRequest(XmlReader reader, XmlWriter writer)
		{
			RequestXslt.Transform(reader, RequestXsltArguments, writer);
		}

		/// <summary>
		/// Translates a response <see cref="System.ServiceModel.Channels.Message"/>-derived message's body, whose content is
		/// given by <paramref name="reader"/>, and outputs the results to <paramref name="writer"/>.
		/// </summary>
		/// <param name="reader">
		/// The <see cref="XmlReader"/> containing the input document.
		/// </param>
		/// <param name="writer">
		/// The <see cref="XmlWriter"/> that will contain the output of the transform.
		/// </param>
		/// <remarks>
		/// This override reuses the extension objects that have been defined inside the <typeparamref
		/// name="TResponseTransform"/>.
		/// </remarks>
		protected override void TranslateResponse(XmlReader reader, XmlWriter writer)
		{
			ResponseXslt.Transform(reader, ResponseXsltArguments, writer);
		}

		#endregion

		/// <summary>
		/// The extension objects that have been defined inside the <typeparamref name="TRequestTransform"/>.
		/// </summary>
		[SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global", Justification = "Public API.")]
		protected virtual XsltArgumentList RequestXsltArguments => XsltCache.Instance[typeof(TRequestTransform)].Arguments;

		/// <summary>
		/// The extension objects that have been defined inside the <typeparamref name="TResponseTransform"/>.
		/// </summary>
		[SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global", Justification = "Public API.")]
		protected virtual XsltArgumentList ResponseXsltArguments => XsltCache.Instance[typeof(TResponseTransform)].Arguments;
	}
}
