﻿#region Copyright & License

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
using System.ServiceModel;
using System.Xml;
using Be.Stateless.Extensions;
using Microsoft.XLANGs.RuntimeTypes;

namespace Be.Stateless.BizTalk.Unit.ServiceModel.Channels.Extensions
{
	public static class MessageExtensions
	{
		public static EndpointAddress GetEndpointAddress(this System.ServiceModel.Channels.Message message)
		{
			return new(message.Headers.To.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped));
		}

		public static string GetMessageType(this System.ServiceModel.Channels.Message message)
		{
			if (message == null) throw new ArgumentNullException(nameof(message));
			var body = new XmlDocument();
			body.Load(message.GetReaderAtBodyContents());
			var root = body.DocumentElement;
			return root.IfNotNull(r => PartTypeMetadata.ComposeMessageType(r.NamespaceURI, r.LocalName));
		}
	}
}
