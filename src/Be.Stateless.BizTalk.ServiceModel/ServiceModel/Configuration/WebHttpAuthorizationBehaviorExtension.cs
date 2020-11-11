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
using System.ServiceModel.Configuration;
using Be.Stateless.BizTalk.ServiceModel.Description;

namespace Be.Stateless.BizTalk.ServiceModel.Configuration
{
	public class WebHttpAuthorizationBehaviorExtension : BehaviorExtensionElement
	{
		[SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Public API.")]
		public WebHttpAuthorizationBehaviorExtension()
		{
			_properties.Add(_authorizationTokenServiceMiddlewareTypeProperty);
			_properties.Add(_authorizationTokenServiceUriProperty);
		}

		#region Base Class Member Overrides

		public override Type BehaviorType => typeof(WebHttpAuthorizationBehavior);

		protected override object CreateBehavior()
		{
			return new WebHttpAuthorizationBehavior(AuthorizationTokenServiceMiddlewareType, new(AuthorizationTokenServiceUri));
		}

		#endregion

		#region Base Class Member Overrides

		protected override ConfigurationPropertyCollection Properties => _properties;

		#endregion

		[ConfigurationProperty(AUTHORIZATION_TOKEN_SERVICE_MIDDLEWARE_TYPE_PROPERTY_NAME, IsKey = true, IsRequired = true)]
		public Type AuthorizationTokenServiceMiddlewareType
		{
			get => Type.GetType((string) this[_authorizationTokenServiceMiddlewareTypeProperty], true);
			set => this[_authorizationTokenServiceMiddlewareTypeProperty] = value.AssemblyQualifiedName;
		}

		[ConfigurationProperty(AUTHORIZATION_TOKEN_SERVICE_URI_PROPERTY_NAME, IsKey = true, IsRequired = true)]
		public string AuthorizationTokenServiceUri
		{
			get => (string) this[_authorizationTokenServiceUriProperty];
			set => this[_authorizationTokenServiceUriProperty] = value;
		}

		private const string AUTHORIZATION_TOKEN_SERVICE_MIDDLEWARE_TYPE_PROPERTY_NAME = "authorizationTokenServiceMiddlewareType";
		private const string AUTHORIZATION_TOKEN_SERVICE_URI_PROPERTY_NAME = "authorizationTokenServiceUri";

		private static readonly ConfigurationPropertyCollection _properties = new();

		private static readonly ConfigurationProperty _authorizationTokenServiceMiddlewareTypeProperty = new(
			AUTHORIZATION_TOKEN_SERVICE_MIDDLEWARE_TYPE_PROPERTY_NAME,
			typeof(string),
			null,
			ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);

		private static readonly ConfigurationProperty _authorizationTokenServiceUriProperty = new(
			AUTHORIZATION_TOKEN_SERVICE_URI_PROPERTY_NAME,
			typeof(string),
			null,
			ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);
	}
}
