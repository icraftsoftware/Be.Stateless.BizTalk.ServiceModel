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
using System.Diagnostics.CodeAnalysis;
using WireMock.Server;
using WireMock.Settings;

namespace Be.Stateless.BizTalk.Unit.ServiceModel.Stub
{
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
	public class RestStubHostActivator : IDisposable
	{
		public static Uri Uri { get; } = new("http://localhost:49185/");

		public RestStubHostActivator()
		{
			Host = WireMockServer.Start(new WireMockServerSettings { Port = Uri.Port });
		}

		#region IDisposable Members

		public void Dispose()
		{
			Host.Stop();
		}

		#endregion

		public WireMockServer Host { get; }

		public string LogEntries => Newtonsoft.Json.JsonConvert.SerializeObject(Host.LogEntries);
	}
}
