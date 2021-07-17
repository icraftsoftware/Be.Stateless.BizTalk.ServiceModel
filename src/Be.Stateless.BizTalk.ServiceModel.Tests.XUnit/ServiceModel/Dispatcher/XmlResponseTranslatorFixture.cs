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
using Be.Stateless.BizTalk.ServiceModel.Channels.Extensions;
using Be.Stateless.BizTalk.ServiceModel.Configuration;
using Be.Stateless.BizTalk.ServiceModel.Description;
using Be.Stateless.BizTalk.Unit.ServiceModel;
using Be.Stateless.BizTalk.Unit.ServiceModel.Stub;
using FluentAssertions;
using Flurl;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;
using Xunit.Abstractions;

namespace Be.Stateless.BizTalk.ServiceModel.Dispatcher
{
	public class XmlResponseTranslatorFixture : IClassFixture<RestStubHostActivator>, IDisposable
	{
		#region Setup/Teardown

		public XmlResponseTranslatorFixture(RestStubHostActivator restStubHostActivator, ITestOutputHelper output)
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
		public void DoesNotTranslateXmlResponseNamespaceWhenUrlDoesNotMatch()
		{
			var url = RestStubHostActivator.Uri.AppendPathSegment("/other/resource");
			_restStubHostActivator.Host
				.Given(
					Request.Create()
						.WithPath(url.Path)
						.UsingGet()
				)
				.RespondWith(
					Response.Create()
						.WithHeader("Content-Type", "application/xml")
						.WithBody("<payload>dummy</payload>")
						.WithStatusCode(HttpStatusCode.OK)
				);

			using (var response = RestClient.WithCustomBehaviors(XmlResponseTranslationBehavior).Get(url))
			{
				response.Should().NotBeNull();
				var reader = response.GetXmlReaderAtWebBodyContents();
				reader.MoveToContent();
				reader.ReadOuterXml().Should().Be("<payload>dummy</payload>");
			}
		}

		[Fact]
		public void TranslateXmlResponseNamespaceWhenUrlMatches()
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
						.WithHeader("Content-Type", "application/xml")
						.WithBody("<payload>dummy</payload>")
						.WithStatusCode(HttpStatusCode.OK)
				);

			using (var response = RestClient.WithCustomBehaviors(XmlResponseTranslationBehavior).Get(url))
			{
				response.Should().NotBeNull();
				var reader = response.GetXmlReaderAtWebBodyContents();
				reader.MoveToContent();
				reader.ReadOuterXml().Should().Be("<payload xmlns=\"urn:ns\">dummy</payload>");
			}
		}

		private XmlResponseTranslationBehavior XmlResponseTranslationBehavior { get; } = new(
			new XmlResponseTranslationCollection {
				new XmlResponseTranslation { MatchingPattern = string.Empty, ReplacementPattern = "urn:ns", Url = "/api/resource" }
			});

		private readonly ITestOutputHelper _output;
		private readonly RestStubHostActivator _restStubHostActivator;
	}
}
