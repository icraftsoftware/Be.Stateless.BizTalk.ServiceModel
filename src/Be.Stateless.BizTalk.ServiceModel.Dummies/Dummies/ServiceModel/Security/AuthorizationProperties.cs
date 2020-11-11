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
using System.Xml;
using Be.Stateless.BizTalk.ContextProperties;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Dummies.ServiceModel.Security
{
	internal static class AuthorizationProperties
	{
		public static readonly MessageContextProperty<TokenId, string> TokenId = new();
	}

	internal sealed class TokenId : MessageContextPropertyBase
	{
		#region Base Class Member Overrides

		public override XmlQualifiedName Name => new(nameof(TokenId), "urn:stateless.be:dummy-properties");

		public override Type Type => typeof(string);

		#endregion
	}
}
