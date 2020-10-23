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

using System.Collections.Generic;
using System.IO;
using Be.Stateless.BizTalk.Dummies;
using FluentAssertions;
using Xunit;
using static Be.Stateless.Unit.DelegateFactory;

namespace Be.Stateless.BizTalk.Unit.ServiceModel.Stub
{
	public class OperationCallSetupCollectionFixture
	{
		[Fact]
		public void AddOrUpdateOperationCallSetup()
		{
			var sut = new OperationCallSetupCollection();
			var operationCallSetup1 = (OperationCallSetup<ISolicitResponse, System.IO.Stream>) sut.Add<ISolicitResponse, System.IO.Stream>("MessageType");
			operationCallSetup1.Returns(new MemoryStream());
			var operationCallSetup2 = (OperationCallSetup<ISolicitResponse, System.IO.Stream>) sut.Add<ISolicitResponse, System.IO.Stream>("MessageType");
			operationCallSetup2.Returns(new MemoryStream());

			sut["MessageType", "Action"].Should().BeSameAs(operationCallSetup2);
			operationCallSetup1.Should().NotBeSameAs(operationCallSetup2);
			operationCallSetup1.Stream.Should().NotBeSameAs(operationCallSetup2.Stream);
		}

		[Fact]
		public void NoSetupHasBeenPerformed()
		{
			Function(() => new OperationCallSetupCollection()["message", "action"])
				.Should().Throw<KeyNotFoundException>()
				.WithMessage("No operation setup has been performed for neither the message type 'message' nor the SOAP action 'action'.");
		}

		[Fact]
		public void OperationCallSetupDefinedForMessageTypeHasPrecedenceOverAction()
		{
			var sut = new OperationCallSetupCollection();
			sut.Add<IWork>("Action");
			var messageSetup = sut.Add<ISolicitResponse>("MessageType");

			sut["MessageType", "Action"].Should().BeSameAs(messageSetup);
		}
	}
}
