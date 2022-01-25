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
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Be.Stateless.BizTalk.Unit.ServiceModel.Channels;

namespace Be.Stateless.BizTalk.Unit.ServiceModel.Stub.Language
{
	/// <summary>
	/// Allows to clear all setups that has been performed against the <see cref="IMessageService"/> or <see
   /// cref="ISolicitResponse"/> soap stub.
	/// </summary>
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
	public interface ISetupOperation
	{
		/// <summary>
		/// Clear all setups that has been performed so far against the <see cref="IMessageService"/> or <see
      /// cref="ISolicitResponse"/> soap stub.
		/// </summary>
		void ClearSetups();
	}

	/// <summary>
	/// Allows to setup what the <see cref="IMessageService"/> or <see cref="ISolicitResponse"/> soap stub has to perform upon
	/// the invocation of some SOAP action, that is a service contract's operation.
	/// </summary>
	/// <typeparam name="TContract">
	/// The the service contract to which belong operation that is being setup.
	/// </typeparam>
	public interface ISetupOperation<TContract> : ISetupFunctionOperation<TContract>, ISetupActionOperation<TContract>, ISetupOperation
		where TContract : class { }

	/// <summary>
	/// Allows to setup what the <see cref="IMessageService"/> or <see cref="ISolicitResponse"/> soap stub has to perform upon
	/// the invocation of some SOAP action, that is a service contract's operation.
	/// </summary>
	/// <typeparam name="TContract">
	/// The the service contract to which belong operation that is being setup.
	/// </typeparam>
	public interface ISetupActionOperation<TContract> : IFluentInterface
		where TContract : class
	{
		/// <summary>
		/// Allows to setup what the <see cref="IMessageService"/> or <see cref="ISolicitResponse"/> soap stub has to perform
		/// upon either the invocation of some SOAP action, that is service contract operation, that produces no result.
		/// </summary>
		/// <param name="operation">
		/// The expression tree denoting the operation contract for which a setup is being made.
		/// </param>
		/// <returns>
		/// An object that allows to record/setup the action to perform upon the invocation of some SOAP action, that is service
		/// contract operation. 
		/// </returns>
		IOperationCallSetup<TContract> Setup(Expression<Action<TContract>> operation);
	}

	/// <summary>
	/// Allows to setup what the <see cref="IMessageService"/> or <see cref="ISolicitResponse"/> soap stub has to perform upon
	/// the invocation of some SOAP action, that is a service contract's operation.
	/// </summary>
	/// <typeparam name="TContract">
	/// The the service contract to which belong operation that is being setup.
	/// </typeparam>
	public interface ISetupFunctionOperation<TContract> : IFluentInterface
		where TContract : class
	{
		/// <summary>
		/// Allows to setup what the <see cref="IMessageService"/> or <see cref="ISolicitResponse"/> soap stub has to perform
		/// upon either the invocation of some SOAP action, that is service contract operation, that produces a result of type
      /// <typeparamref name="TResult"/>.
		/// </summary>
		/// <typeparam name="TResult">
		/// The type of the result produced by the <paramref name="operation"/> being performed.
		/// </typeparam>
		/// <param name="operation">
		/// The expression tree denoting the operation contract for which a setup is being made.
		/// </param>
		/// <returns>
		/// An object that allows to record/setup the action to perform upon the invocation of some SOAP action, that is service
		/// contract operation. 
		/// </returns>
		IOperationCallSetup<TContract, TResult> Setup<TResult>(Expression<Func<TContract, TResult>> operation);
	}
}
