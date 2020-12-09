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
using System.ServiceModel.Description;

namespace Be.Stateless.BizTalk.Unit.ServiceModel
{
	public class SoapClient<TChannel> : ClientBase<TChannel>
		where TChannel : class
	{
		#region Operators

		[SuppressMessage("Usage", "CA2225:Operator overloads have named alternates")]
		public static implicit operator TChannel(SoapClient<TChannel> client)
		{
			return client?.Channel;
		}

		#endregion

		[SuppressMessage("Design", "CA1000:Do not declare static members on generic types")]
		public static TChannel For(ServiceEndpoint serviceEndpoint)
		{
			if (serviceEndpoint == null) throw new ArgumentNullException(nameof(serviceEndpoint));
			return new SoapClient<TChannel>(serviceEndpoint.Binding, serviceEndpoint.Address);
		}

		private SoapClient(Binding binding, EndpointAddress address) : base(binding, address) { }
	}
}
