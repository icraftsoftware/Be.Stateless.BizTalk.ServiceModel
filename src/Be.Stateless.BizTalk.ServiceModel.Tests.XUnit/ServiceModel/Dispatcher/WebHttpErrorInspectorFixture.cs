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
using System.Diagnostics;
using System.Net;
using System.ServiceModel;
using Be.Stateless.BizTalk.ServiceModel.Description;
using Be.Stateless.BizTalk.Unit.ServiceModel;
using Be.Stateless.BizTalk.Unit.ServiceModel.Stub;
using Be.Stateless.IO;
using FluentAssertions;
using Flurl;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;
using Xunit.Abstractions;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.ServiceModel.Dispatcher
{
	public class WebHttpErrorInspectorFixture : IClassFixture<RestStubHostActivator>, IDisposable
	{
		#region Setup/Teardown

		public WebHttpErrorInspectorFixture(RestStubHostActivator restStubHostActivator, ITestOutputHelper output)
		{
			_restStubHostActivator = restStubHostActivator;
			_restStubHostActivator.Host.Reset();
			_output = output;
		}

		public void Dispose()
		{
			if (Debugger.IsAttached) _output.WriteLine(_restStubHostActivator.LogEntries);
		}

		#endregion

		[Fact]
		public void ThrowsCommunicationExceptionOnHttpStatusCodeInternalServerError()
		{
			var url = RestStubHostActivator.Uri.AppendPathSegment("/api/resource");
			_restStubHostActivator.Host
				.Given(
					Request.Create()
						.WithPath(url.Path)
						.UsingGet()
				)
				.RespondWith(
					Response.Create()
						.WithStatusCode(HttpStatusCode.InternalServerError)
				);

			Invoking(() => RestClient.WithCustomBehaviors(new WebHttpErrorInspectionBehavior()).Get(url))
				.Should().Throw<CommunicationException>()
				.WithMessage("500 Internal Server Error.");
		}

		[Fact]
		public void ThrowsCommunicationExceptionWithJsonResponseBodyOnHttpStatusCodeInternalServerError()
		{
			var url = RestStubHostActivator.Uri.AppendPathSegment("/api/resource");
			_restStubHostActivator.Host
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

			Invoking(() => RestClient.WithCustomBehaviors(new WebHttpErrorInspectionBehavior()).Post(url, new StringStream("dummy")))
				.Should().Throw<CommunicationException>()
				.WithMessage($"500 Internal Server Error.{Environment.NewLine}{{\"Error\":\"403\",\"Message\":\"Not authorized\"}}");
		}

		[Fact]
		public void ThrowsCommunicationExceptionWithXmlResponseBodyOnHttpStatusCodeInternalServerError()
		{
			var url = RestStubHostActivator.Uri.AppendPathSegment("/api/resource");
			_restStubHostActivator.Host
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

			Invoking(() => RestClient.WithCustomBehaviors(new WebHttpErrorInspectionBehavior()).Post(url, new StringStream("dummy")))
				.Should().Throw<CommunicationException>()
				.WithMessage($"500 Internal Server Error.{Environment.NewLine}<Error><Message>Not authorized.</Message></Error>");
		}

		[Fact]
		public void ThrowsNothingOnHttpStatusOK()
		{
			var url = RestStubHostActivator.Uri.AppendPathSegment("/api/resource");
			_restStubHostActivator.Host
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

			Invoking(() => RestClient.WithCustomBehaviors(new WebHttpErrorInspectionBehavior()).Post(url, new StringStream("dummy")))
				.Should().NotThrow();
		}

		private readonly ITestOutputHelper _output;
		private readonly RestStubHostActivator _restStubHostActivator;
	}
}
