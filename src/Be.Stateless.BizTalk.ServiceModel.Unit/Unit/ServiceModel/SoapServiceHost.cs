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
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Be.Stateless.BizTalk.Unit.ServiceModel
{
	public abstract class SoapServiceHost : ServiceHost
	{
		public abstract Type ChannelType { get; }

		public abstract ServiceEndpoint Endpoint { get; }

		public abstract Type ServiceType { get; }

		protected internal abstract void Initialize(Uri address);
	}

	/// <summary>
	/// Provides in-process self-hosting for a single endpoint of some <typeparamref name="TService"/> service class exposing a
	/// <typeparamref name="TChannel"/> service contract.
	/// </summary>
	/// <typeparam name="TService">
	/// The service class that implements the <typeparamref name="TChannel"/> service contract.
	/// </typeparam>
	/// <typeparam name="TChannel">
	/// The shape of the channel to be used to connect to the service's endpoint.
	/// </typeparam>
	/// <remarks>
	/// <para>
	/// The only purpose of the <see cref="SoapServiceHost{TService,TChannel}"/> is to support WCF service hosting for the sake
	/// of unit testing. Even though one can always mock a service contract, sometimes nothing compares to the real thing when
	/// one has to do WCF plumbing.
	/// </para>
	/// <para>
	/// Notice that <see cref="SoapServiceHost{TService,TChannel}"/> will systematically try to register a metadata exchange
	/// endpoint, in accordance to the address' scheme, at the relative <c>mex</c> address. <see
	/// cref="SoapServiceHost{TService,TChannel}"/> might consequently fail to start if there is no <see cref="Binding"/> that
	/// supports the exchange of metadata over the <see cref="Endpoint"/>'s address scheme; see also <see
	/// cref="MetadataExchangeBindings"/>.
	/// </para>
	/// </remarks>
	/// <seealso cref="MetadataExchangeBindings"/>
	public sealed class SoapServiceHost<TService, TChannel> : SoapServiceHost
		where TService : TChannel, new()
		where TChannel : class
	{
		private static bool IsSingleton => typeof(TService).GetCustomAttribute(typeof(ServiceBehaviorAttribute)) is ServiceBehaviorAttribute {
			InstanceContextMode: InstanceContextMode.Single
		};

		#region Base Class Member Overrides

		public override Type ChannelType => typeof(TChannel);

		public override ServiceEndpoint Endpoint => Description!.Endpoints.Find(typeof(TChannel));

		protected internal override void Initialize(Uri address)
		{
			if (IsSingleton) InitializeDescription(new TService(), new(address));
			else InitializeDescription(typeof(TService), new(address));
			var debugBehavior = Description!.Behaviors.Find<ServiceDebugBehavior>();
			if (debugBehavior != null) debugBehavior.IncludeExceptionDetailInFaults = true;
			else Description.Behaviors.Add(new ServiceDebugBehavior { IncludeExceptionDetailInFaults = true });
			AddMexEndpoint(address.Scheme);
		}

		public override Type ServiceType => typeof(TService);

		#endregion

		#region Mex Endpoint Helpers

		private void AddMexEndpoint(string scheme)
		{
			if (scheme == null) throw new ArgumentNullException(nameof(scheme));
			var debugBehavior = Description!.Behaviors.Find<ServiceDebugBehavior>();
			var metaDataBehavior = Description.Behaviors.Find<ServiceMetadataBehavior>();
			if (metaDataBehavior == null)
			{
				metaDataBehavior = new();
				Description.Behaviors.Add(metaDataBehavior);
			}
			debugBehavior.HttpHelpPageEnabled = metaDataBehavior.HttpGetEnabled = scheme.IndexOf("http", StringComparison.OrdinalIgnoreCase) >= 0;
			AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, CreateMexBindingForScheme(scheme), "mex");
		}

		private static Binding CreateMexBindingForScheme(string scheme)
		{
			if (scheme == null) throw new ArgumentNullException(nameof(scheme));
			if (string.Compare(scheme, "http", StringComparison.OrdinalIgnoreCase) == 0) return MetadataExchangeBindings.CreateMexHttpBinding();
			if (string.Compare(scheme, "https", StringComparison.OrdinalIgnoreCase) == 0) return MetadataExchangeBindings.CreateMexHttpsBinding();
			if (string.Compare(scheme, "net.pipe", StringComparison.OrdinalIgnoreCase) == 0) return MetadataExchangeBindings.CreateMexNamedPipeBinding();
			if (string.Compare(scheme, "net.tcp", StringComparison.OrdinalIgnoreCase) == 0) return MetadataExchangeBindings.CreateMexTcpBinding();
			throw new ArgumentException($"MEX for scheme {scheme} is not supported.");
		}

		#endregion
	}
}
