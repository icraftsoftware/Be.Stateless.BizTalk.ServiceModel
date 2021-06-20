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
using System.ServiceModel;
using Be.Stateless.BizTalk.Unit.ServiceModel.Channels;

namespace Be.Stateless.BizTalk.Unit.ServiceModel.Stub
{
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
	public class SoapStubHostActivator : SoapServiceHostActivator<SoapServiceHost<SoapStub, IMessageService>, BasicHttpBinding>
	{
		public SoapStubHostActivator() : base(Uri) { }

		public static readonly Uri Uri = new("http://localhost:8000/soap-stub");
	}
}
