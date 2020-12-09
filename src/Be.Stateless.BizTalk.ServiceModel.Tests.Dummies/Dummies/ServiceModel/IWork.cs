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

using System.ServiceModel;

namespace Be.Stateless.BizTalk.Dummies.ServiceModel
{
	[ServiceContract]
	public interface IWork
	{
		[OperationContract(Action = "urn:services.stateless.be:unit:work:execute:request", ReplyAction = "urn:services.stateless.be:unit:work:execute:request/response")]
		void Execute(System.ServiceModel.Channels.Message message);

		[OperationContract(Action = "urn:services.stateless.be:unit:work:perform:request", ReplyAction = "urn:services.stateless.be:unit:work:perform:request/response")]
		System.ServiceModel.Channels.Message Perform(System.ServiceModel.Channels.Message message);
	}
}
