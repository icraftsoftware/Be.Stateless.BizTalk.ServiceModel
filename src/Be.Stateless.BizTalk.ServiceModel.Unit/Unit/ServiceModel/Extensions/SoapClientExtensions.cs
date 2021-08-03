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
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;

namespace Be.Stateless.BizTalk.Unit.ServiceModel.Extensions
{
	/// <summary>
	/// Allows to skip <see cref="ICommunicationObject"/> cast when calling either <see cref="ICommunicationObject.Abort()"/>
	/// or <see cref="ICommunicationObject.Close()"/> on a <see cref="ClientBase{TChannel}"/> instance.
	/// </summary>
	[SuppressMessage("ReSharper", "LocalizableElement")]
	public static class SoapClientExtensions
	{
		public static void Abort<TChannel>(this TChannel client) where TChannel : class
		{
			if (client is not ICommunicationObject communicationObject) throw new ArgumentException("client does support ICommunicationObject.", nameof(client));
			communicationObject.Abort();
		}

		public static void Close<TChannel>(this TChannel client) where TChannel : class
		{
			if (client is not ICommunicationObject communicationObject) throw new ArgumentException("client does support ICommunicationObject.", nameof(client));
			communicationObject.Close();
		}
	}
}
