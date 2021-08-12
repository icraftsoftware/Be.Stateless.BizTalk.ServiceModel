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

using System.IO;
using Newtonsoft.Json;

namespace Be.Stateless.BizTalk.Dummies.ServiceModel.Security.Tokens
{
	internal class Error
	{
		public static Error Deserialize(System.IO.Stream stream)
		{
			using (var rsr = new StreamReader(stream))
			{
				return Deserialize(rsr.ReadToEnd());
			}
		}

		private static Error Deserialize(string json)
		{
			try
			{
				return JsonConvert.DeserializeObject<Error>(json);
			}
			catch (JsonSerializationException)
			{
				return new() { Message = json };
			}
		}

		[JsonProperty(PropertyName = "Message", Required = Required.Always)]
		public string Message { get; set; }
	}
}
