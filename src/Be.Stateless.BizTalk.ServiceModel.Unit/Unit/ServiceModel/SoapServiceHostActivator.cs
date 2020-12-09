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
using System.ServiceModel.Channels;

namespace Be.Stateless.BizTalk.Unit.ServiceModel
{
	/// <summary>
	/// Starts the <see cref="SoapServiceHost{TService,TChannel}"/>-derived in-process <see cref="ServiceHost"/> that hosts a
	/// WCF service endpoint for the purpose of unit testing a WCF service or its their contracts.
	/// </summary>
	/// <remarks>
	/// <see cref="SoapServiceHostActivator{THost,TBinding}"/> ensures that the <see
	/// cref="SoapServiceHost{TService,TChannel}"/>-derived in-process <see cref="ServiceHost"/> is started so that its hosted
	/// WCF service endpoint is listening and started before any unit test depending on it runs.
	/// </remarks>
	/// <seealso cref="SoapServiceHost{TService,TChannel}"/>
	[SuppressMessage("Design", "CA1063:Implement IDisposable Correctly")]
	public abstract class SoapServiceHostActivator<THost, TBinding> : ISoapServiceHost
		where THost : SoapServiceHost, new()
		where TBinding : Binding, new()
	{
		protected SoapServiceHostActivator(Uri address)
		{
			Host = new THost();
			Host.Initialize(address);
			Host.AddServiceEndpoint(Host.ChannelType, Binding, string.Empty);
			Host.Open();
		}

		#region ISoapServiceHost Members

		[SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize")]
		public void Dispose()
		{
			Host.Close();
		}

		public SoapServiceHost Host { get; }

		#endregion

		public static readonly Binding Binding = new TBinding();
	}
}
