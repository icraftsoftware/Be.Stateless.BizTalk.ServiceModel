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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.ServiceModel;
using Be.Stateless.BizTalk.Unit.ServiceModel.Channels;
using Be.Stateless.BizTalk.Unit.ServiceModel.Channels.Extensions;
using Be.Stateless.BizTalk.Unit.ServiceModel.Stub.Language;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Unit.ServiceModel.Stub
{
	[ServiceBehavior(
		AddressFilterMode = AddressFilterMode.Any,
		ConcurrencyMode = ConcurrencyMode.Single,
		InstanceContextMode = InstanceContextMode.Single,
		ValidateMustUnderstand = false)]
	public class SoapStub : IMessageService
	{
		#region Nested Type: SetupRecorder

		private class SetupRecorder<TContract> : ISetupOperation<TContract>
			where TContract : class
		{
			private static string GetOperationAction(OperationContractAttribute operationContractAttribute, MethodInfo methodInfo)
			{
				if (!operationContractAttribute.Action.IsNullOrEmpty()) return operationContractAttribute.Action;

				var sca = typeof(TContract).GetCustomAttributes(typeof(ServiceContractAttribute), false)
					.Cast<ServiceContractAttribute>()
					.Single();
				var serviceName = sca.Name ?? typeof(TContract).Name;
				var operationName = operationContractAttribute.AsyncPattern ? methodInfo.Name.Substring("Begin".Length) : methodInfo.Name;
				// https://docs.microsoft.com/en-us/dotnet/api/system.servicemodel.operationcontractattribute.action
				return $"{sca.Namespace}/{serviceName}/{operationName}";
			}

			internal SetupRecorder(SoapStub soapStub)
			{
				_operationCallSetupCollection = soapStub._operationCallSetupCollection;
			}

			#region ISetupOperation<TContract> Members

			public IOperationCallSetup<TContract> Setup(Expression<Action<TContract>> operation)
			{
				var callExpression = (MethodCallExpression) operation.Body;
				var sca = typeof(TContract).GetCustomAttributes(typeof(ServiceContractAttribute), false);
				if (!sca.Any()) throw new ArgumentException($"TContract type parameter '{typeof(TContract).Name}' is not a service contract.");

				var method = callExpression.Method;
				var oca = method.GetCustomAttributes(typeof(OperationContractAttribute), false)
					.Cast<OperationContractAttribute>()
					.Single();
				return _operationCallSetupCollection.Add<TContract>(GetOperationAction(oca, method));
			}

			public IOperationCallSetup<TContract, TResult> Setup<TResult>(Expression<Func<TContract, TResult>> operation)
			{
				var callExpression = (MethodCallExpression) operation.Body;

				if (typeof(TContract) == typeof(ISolicitResponse))
				{
					var argument = callExpression.Arguments[0];
					if (Expression.Lambda(argument).Compile().DynamicInvoke() is not DocumentSpec documentSpec)
						throw new ArgumentException("Can only setup a response for a valid and non-null DocumentSpec.");
					return (IOperationCallSetup<TContract, TResult>) _operationCallSetupCollection.Add<ISolicitResponse, TResult>(documentSpec.DocType);
				}

				var sca = typeof(TContract).GetCustomAttributes(typeof(ServiceContractAttribute), false);
				if (!sca.Any()) throw new ArgumentException($"TContract type parameter '{typeof(TContract).Name}' is not a service contract.");

				var method = callExpression.Method;
				var oca = method.GetCustomAttributes(typeof(OperationContractAttribute), false)
					.Cast<OperationContractAttribute>()
					.Single();
				return _operationCallSetupCollection.Add<TContract, TResult>(GetOperationAction(oca, method));
			}

			public void ClearSetups()
			{
				_operationCallSetupCollection.Clear();
			}

			#endregion

			private readonly OperationCallSetupCollection _operationCallSetupCollection;
		}

		#endregion

		#region IMessageService Members

		System.ServiceModel.Channels.Message IMessageService.Invoke(System.ServiceModel.Channels.Message request)
		{
			var action = request.Headers.Action;
			var messageType = request.GetMessageType();
			var operationCallSetup = _operationCallSetupCollection[messageType, action];

			operationCallSetup.CallbackAction?.Invoke();

			if (operationCallSetup.MustAbort) return null;

			// has to return a response payload, i.e. not a void operation
			if (operationCallSetup.GetType().GetGenericTypeDefinition() == typeof(OperationCallSetup<,>))
			{
				return System.ServiceModel.Channels.Message.CreateMessage(
					request.Version,
					action + "/response",
					operationCallSetup.Body);
			}

			// return an empty response, i.e. a void operation
			return System.ServiceModel.Channels.Message.CreateMessage(
				request.Version,
				action + "/response");
		}

		#endregion

		public ISetupOperation<TContract> As<TContract>()
			where TContract : class
		{
			return new SetupRecorder<TContract>(this);
		}

		public void ClearSetups()
		{
			_operationCallSetupCollection.Clear();
		}

		private readonly OperationCallSetupCollection _operationCallSetupCollection = new();
	}
}
