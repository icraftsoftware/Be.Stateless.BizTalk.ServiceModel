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
using System.IO;
using System.Xml;
using Be.Stateless.BizTalk.Unit.ServiceModel.Stub.Language;

namespace Be.Stateless.BizTalk.Unit.ServiceModel.Stub
{
	internal class OperationCallSetup : IOperationCallSetup<ISolicitResponse>
	{
		#region IOperationCallSetup<ISolicitResponse> Members

		IOperationAbortSetup<ISolicitResponse> IOperationCallbackSetup<ISolicitResponse>.Callback(Action callback)
		{
			CallbackAction = callback ?? throw new ArgumentNullException(nameof(callback));
			return this;
		}

		public void Aborts()
		{
			MustAbort = true;
		}

		#endregion

		public XmlReader Body
		{
			get
			{
				Stream.Position = 0;
				return XmlReader.Create(Stream);
			}
		}

		public Action CallbackAction { get; protected set; }

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
		public bool MustAbort { get; protected set; }

		public System.IO.Stream Stream { get; protected set; }
	}

	internal class OperationCallSetup<TContract> : OperationCallSetup, IOperationCallSetup<TContract>
		where TContract : class
	{
		#region IOperationCallSetup<TContract> Members

		IOperationAbortSetup<TContract> IOperationCallbackSetup<TContract>.Callback(Action callback)
		{
			CallbackAction = callback ?? throw new ArgumentNullException(nameof(callback));
			return this;
		}

		#endregion
	}

	internal class OperationCallSetup<TContract, TResult> : OperationCallSetup, IOperationCallSetup<TContract, TResult>
		where TContract : class
	{
		#region IOperationCallSetup<TContract,TResult> Members

		IOperationAbortResponseSetup<TContract, TResult> IOperationCallbackSetup<TContract, TResult>.Callback(Action callback)
		{
			CallbackAction = callback ?? throw new ArgumentNullException(nameof(callback));
			return this;
		}

		public void Returns(string file)
		{
			if (file == null) throw new ArgumentNullException(nameof(file));
			Stream = new MemoryStream(File.ReadAllBytes(file));
		}

		public void Returns(System.IO.Stream stream)
		{
			if (stream == null) throw new ArgumentNullException(nameof(stream));
			if (!stream.CanSeek) throw new ArgumentException("Stream is not seekable.", nameof(stream));
			Stream = stream;
		}

		#endregion
	}
}
