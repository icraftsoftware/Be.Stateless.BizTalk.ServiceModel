﻿#region Copyright & License

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
using Be.Stateless.BizTalk.Unit.ServiceModel.Channels;

namespace Be.Stateless.BizTalk.Unit.ServiceModel.Stub.Language
{
	/// <summary>
	/// Allows to setup an abort to be carried out by the <see cref="IMessageService"/> or <see cref="ISolicitResponse"/> soap
	/// stub upon either the reception of some request message or the invocation of some SOAP action.
	/// </summary>
	/// <typeparam name="TContract">
	/// The the service contract to which belong operation that is being setup.
	/// </typeparam>
	[SuppressMessage("ReSharper", "UnusedTypeParameter", Justification = "Public API.")]
	public interface IOperationAbortSetup<TContract> : IFluentInterface
		where TContract : class
	{
		/// <summary>
		/// Will setup the <see cref="IMessageService"/> soap stub to abort.
		/// </summary>
		void Aborts();

		// TODO support setup for Throws(Exception or Exception factory)
	}

	/// <summary>
	/// Allows to setup an abort to be carried out by the <see cref="IMessageService"/> or <see cref="ISolicitResponse"/> soap
	/// stub upon either the reception of some request message or the invocation of some SOAP action.
	/// </summary>
	/// <typeparam name="TContract">
	/// The the service contract to which belong operation that is being setup.
	/// </typeparam>
	/// <typeparam name="TResult">
	/// </typeparam>
	[SuppressMessage("ReSharper", "UnusedTypeParameter", Justification = "Public API.")]
	public interface IOperationAbortSetup<TContract, TResult> : IOperationAbortSetup<TContract>
		where TContract : class { }
}
