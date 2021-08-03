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
using System.Net;
using System.ServiceModel;
using Be.Stateless.BizTalk.ServiceModel.Description;
using Be.Stateless.BizTalk.Unit.ServiceModel;
using Be.Stateless.BizTalk.Unit.ServiceModel.Stub;
using Be.Stateless.IO;
using Flurl;
using NUnit.Framework;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Be.Stateless.BizTalk.ServiceModel.Dispatcher
{
	[RestStubHostActivator]
	[TestFixture]
	public class WebHttpErrorInspectorFixture : IRestServiceHostInjection
	{
		[Test]
		public void ThrowsCommunicationExceptionOnHttpStatusCodeInternalServerError()
		{
			var url = RestStubHostActivator.Uri.AppendPathSegment("/api/resource");
			_restServiceHost
				.Given(
					Request.Create()
						.WithPath(url.Path)
						.UsingGet()
				)
				.RespondWith(
					Response.Create()
						.WithStatusCode(HttpStatusCode.InternalServerError)
				);

			Assert.That(
				() => RestClient.WithCustomBehaviors(new WebHttpErrorInspectionBehavior()).Get(url),
				Throws.TypeOf<CommunicationException>()
					.With.Message.EqualTo("500 Internal Server Error."));
		}

		[Test]
		public void ThrowsCommunicationExceptionWithJsonResponseBodyOnHttpStatusCodeInternalServerError()
		{
			var url = RestStubHostActivator.Uri.AppendPathSegment("/api/resource");
			_restServiceHost
				.Given(
					Request.Create()
						.WithPath(url.Path)
						.UsingPost()
				)
				.RespondWith(
					Response.Create()
						.WithHeader("Content-Type", "application/json")
						.WithBody("{\"Error\":\"403\",\"Message\":\"Not authorized\"}")
						.WithStatusCode(HttpStatusCode.InternalServerError)
				);

			Assert.That(
				() => RestClient.WithCustomBehaviors(new WebHttpErrorInspectionBehavior()).Post(url, new StringStream("dummy")),
				Throws.TypeOf<CommunicationException>()
					.With.Message.EqualTo($"500 Internal Server Error.{Environment.NewLine}{{\"Error\":\"403\",\"Message\":\"Not authorized\"}}"));
		}

		[Test]
		public void ThrowsCommunicationExceptionWithXmlResponseBodyOnHttpStatusCodeInternalServerError()
		{
			var url = RestStubHostActivator.Uri.AppendPathSegment("/api/resource");
			_restServiceHost
				.Given(
					Request.Create()
						.WithPath(url.Path)
						.UsingPost()
				)
				.RespondWith(
					Response.Create()
						.WithHeader("Content-Type", "application/xml")
						.WithBody("<Error><Message>Not authorized.</Message></Error>")
						.WithStatusCode(HttpStatusCode.InternalServerError)
				);

			Assert.That(
				() => RestClient.WithCustomBehaviors(new WebHttpErrorInspectionBehavior()).Post(url, new StringStream("dummy")),
				Throws.TypeOf<CommunicationException>()
					.With.Message.EqualTo($"500 Internal Server Error.{Environment.NewLine}<Error><Message>Not authorized.</Message></Error>"));
		}

		[Test]
		public void ThrowsNothingOnHttpStatusOK()
		{
			var url = RestStubHostActivator.Uri.AppendPathSegment("/api/resource");
			_restServiceHost
				.Given(
					Request.Create()
						.WithPath(url.Path)
						.UsingPost()
				)
				.RespondWith(
					Response.Create()
						.WithHeader("Content-Type", "application/xml")
						.WithBody("<Response>dummy</Response>")
						.WithStatusCode(HttpStatusCode.OK)
				);

			Assert.That(
				() => RestClient.WithCustomBehaviors(new WebHttpErrorInspectionBehavior()).Post(url, new StringStream("dummy")),
				Throws.Nothing);
		}

		public void InjectRestServiceHost(WireMockServer restServiceHost)
		{
			_restServiceHost = restServiceHost;
		}

		private WireMockServer _restServiceHost;
	}
}
