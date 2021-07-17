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
using Be.Stateless.BizTalk.ServiceModel.Security.Tokens;
using Newtonsoft.Json;

namespace Be.Stateless.BizTalk.Dummies.ServiceModel.Security.Tokens
{
	internal class AuthorizationToken : IAuthorizationToken
	{
		public static AuthorizationToken Deserialize(string json)
		{
			return JsonConvert.DeserializeObject<AuthorizationToken>(json);
		}

		#region IAuthorizationToken Members

		[JsonProperty(PropertyName = "Expires", Required = Required.Always)]
		public DateTime ExpirationTime { get; set; }

		[JsonProperty(PropertyName = "Token", Required = Required.Always)]
		public string Body { get; set; }

		#endregion
	}
}
