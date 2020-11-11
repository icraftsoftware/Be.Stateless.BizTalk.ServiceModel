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
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Net.Security;
using System.ServiceModel.Configuration;
using Be.Stateless.BizTalk.ServiceModel.Description;

namespace Be.Stateless.BizTalk.ServiceModel.Configuration
{
	public class ServiceContractProtectionBehaviorExtension : BehaviorExtensionElement
	{
		[SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Public API.")]
		public ServiceContractProtectionBehaviorExtension()
		{
			_properties.Add(_protectionLevelProperty);
		}

		#region Base Class Member Overrides

		public override Type BehaviorType => typeof(ServiceContractProtectionBehavior);

		protected override object CreateBehavior()
		{
			return new ServiceContractProtectionBehavior(ProtectionLevel);
		}

		#endregion

		#region Base Class Member Overrides

		protected override ConfigurationPropertyCollection Properties => _properties;

		#endregion

		[ConfigurationProperty(PROTECTION_LEVEL_PROPERTY_NAME, IsKey = true, IsRequired = true)]
		public ProtectionLevel ProtectionLevel
		{
			get => (ProtectionLevel) base[_protectionLevelProperty];
			set => base[_protectionLevelProperty] = value;
		}

		private const string PROTECTION_LEVEL_PROPERTY_NAME = "protectionLevel";

		private static readonly ConfigurationPropertyCollection _properties = new();

		private static readonly ConfigurationProperty _protectionLevelProperty = new(
			PROTECTION_LEVEL_PROPERTY_NAME,
			typeof(ProtectionLevel),
			null,
			ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);
	}
}
