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

using System;
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;
using System.ServiceModel.Description;
using Be.Stateless.BizTalk.Unit.ServiceModel.Stub;
using Be.Stateless.BizTalk.Unit.ServiceModel.Stub.Language;

namespace Be.Stateless.BizTalk.Unit.ServiceModel
{
	public class StubServiceHost : ServiceHost
	{
		static StubServiceHost()
		{
			AppDomain.CurrentDomain.DomainUnload += (sender, args) => {
				if (DefaultInstance.State != CommunicationState.Opened) return;
				try
				{
					DefaultInstance.Close();
				}
				catch
				{
					DefaultInstance.Abort();
					throw;
				}
			};
		}

		public static System.ServiceModel.Channels.Binding DefaultBinding => DefaultInstance.Description.Endpoints.Find(typeof(IStubService))!.Binding;

		public static EndpointAddress DefaultEndpointAddress => DefaultInstance.Description.Endpoints.Find(typeof(IStubService))!.Address;

		public static StubServiceHost DefaultInstance { get; private set; } = new StubServiceHost();

		public static ISetupOperation<ISolicitResponse> DefaultService => DefaultInstance.Service;

		public static ISetupOperation<TContract> FindDefaultService<TContract>() where TContract : class
		{
			return DefaultInstance.FindService<TContract>();
		}

		private StubServiceHost() : base(new StubService(), new Uri("http://localhost:8000/"))
		{
			var debugBehavior = Description.Behaviors.Find<ServiceDebugBehavior>();
			if (debugBehavior != null)
			{
				debugBehavior.IncludeExceptionDetailInFaults = true;
			}
			else
			{
				Description.Behaviors.Add(new ServiceDebugBehavior { IncludeExceptionDetailInFaults = true });
			}
			AddServiceEndpoint(typeof(IStubService), new BasicHttpBinding(), "stubservice");
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
		public ISetupOperation<ISolicitResponse> Service => ((StubService) SingletonInstance).FindSetupRecorder<ISolicitResponse>();

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
		public ISetupOperation<TContract> FindService<TContract>() where TContract : class
		{
			return ((StubService) SingletonInstance).FindSetupRecorder<TContract>();
		}

		public void Recycle()
		{
			try
			{
				Close();
			}
			catch
			{
				Abort();
				throw;
			}
			finally
			{
				DefaultInstance = new StubServiceHost();
			}
		}
	}
}
