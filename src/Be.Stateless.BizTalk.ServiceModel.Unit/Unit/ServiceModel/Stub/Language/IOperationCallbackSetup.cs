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

using System;
using Be.Stateless.BizTalk.Unit.ServiceModel.Channels;

namespace Be.Stateless.BizTalk.Unit.ServiceModel.Stub.Language
{
	/// <summary>
	/// Allows to setup the callback against <see cref="IMessageService"/> or <see cref="ISolicitResponse"/> to be carried out
	/// by the soap stub upon either the reception of some request message or the invocation of some SOAP action.
	/// </summary>
	/// <typeparam name="TContract">
	/// The the service contract to which belong operation that is being setup.
	/// </typeparam>
	public interface IOperationCallbackSetup<TContract> : IFluentInterface
		where TContract : class
	{
		/// <summary>
		/// Specifies a callback to invoke when the method is called.
		/// </summary>
		/// <param name="callback">
		/// The callback method to invoke.
		/// </param>
		/// <returns>
		/// An <see cref="IOperationResponseSetup{TContract,TResult}"/> object allowing to setup the actual response to return.
		/// </returns>
		IOperationAbortSetup<TContract> Callback(Action callback);
	}

	/// <summary>
	/// Allows to setup the callback against <see cref="IMessageService"/> or <see cref="ISolicitResponse"/> to be carried out
	/// by the soap stub upon either the reception of some request message or the invocation of some SOAP action.
	/// </summary>
	/// <typeparam name="TContract">
	/// The the service contract to which belong operation that is being setup.
	/// </typeparam>
	/// <typeparam name="TResult">
	/// The type of the result the service contract's operation will produce.
	/// </typeparam>
	public interface IOperationCallbackSetup<TContract, TResult> : IFluentInterface
		where TContract : class
	{
		/// <summary>
		/// Specifies a callback to invoke when the method is called.
		/// </summary>
		/// <param name="callback">
		/// The callback method to invoke.
		/// </param>
		/// <returns>
		/// An <see cref="IOperationResponseSetup{TContract,TResult}"/> object allowing to setup the actual response to return.
		/// </returns>
		IOperationAbortResponseSetup<TContract, TResult> Callback(Action callback);
	}
}
