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
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Xml;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.ServiceModel.Channels.Extensions;
using Be.Stateless.BizTalk.ServiceModel.Configuration;
using Be.Stateless.BizTalk.ServiceModel.Description;
using Be.Stateless.BizTalk.Unit.ServiceModel;
using Be.Stateless.BizTalk.Unit.ServiceModel.Channels;
using Be.Stateless.BizTalk.Unit.ServiceModel.Stub;
using FluentAssertions;
using Flurl;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;
using Xunit.Abstractions;

namespace Be.Stateless.BizTalk.ServiceModel.Dispatcher
{
	public class PropertyPropagatorFixture : IClassFixture<RestStubHostActivator>, IDisposable
	{
		#region Setup/Teardown

		public PropertyPropagatorFixture(RestStubHostActivator restStubHostActivator, ITestOutputHelper output)
		{
			_restStubHostActivator = restStubHostActivator;
			_restStubHostActivator.Host.Reset();
			_output = output;
		}

		public void Dispose()
		{
			// TODO only output WireMock logged messages when test has failed and it is a WireMock exception, ? custom Fact attribute, a la SkippableFact ?
			// see https://github.com/xunit/xunit/issues/416#issuecomment-378512739
			//var type = _output.GetType();
			//var testMember = type.GetField("test", BindingFlags.Instance | BindingFlags.NonPublic);
			//var value = testMember.GetValue(_output);
			//var test = (ITest) value;
			if (Debugger.IsAttached) _output.WriteLine(_restStubHostActivator.LogEntries);
		}

		#endregion

		[Fact]
		public void EmptyResponseHasPropertiesPropagatedFromRequest()
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
						.WithStatusCode(HttpStatusCode.OK)
				);

			var request = WcfWebRequestFactory.Create(url);
			request.SetProperty(BtsProperties.InterchangeID, "interchange");
			var response = RestClient.WithCustomBehaviors(PropertyPropagationBehavior).Invoke(request);

			response.Should().NotBeNull();
			response.IsEmpty.Should().BeTrue();
			response!.Properties
				.Should().NotContainKey(MessagePropertiesExtensions.PROPERTIES_TO_PROMOTE_KEY)
				.And.ContainKey(MessagePropertiesExtensions.PROPERTIES_TO_WRITE_KEY);
			response.Properties.GetPropertiesToWrite()
				.Should().NotBeNull()
				.And.BeEquivalentTo(new KeyValuePair<XmlQualifiedName, object>(BtsProperties.InterchangeID.QName, "interchange"));
		}

		[Fact(Skip = "Don't know how to return a fault with stub.")]
		public void FaultResponseHasPropertiesPropagatedFromRequest()
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

			var request = WcfWebRequestFactory.Create(url);
			request.SetProperty(BtsProperties.InterchangeID, "interchange");
			var response = RestClient.WithCustomBehaviors(PropertyPropagationBehavior).Invoke(request);

			response.Should().NotBeNull();
			response.IsFault.Should().BeTrue();
			response!.Properties
				.Should().NotContainKey(MessagePropertiesExtensions.PROPERTIES_TO_PROMOTE_KEY)
				.And.ContainKey(MessagePropertiesExtensions.PROPERTIES_TO_WRITE_KEY);
			response.Properties.GetPropertiesToWrite()
				.Should().NotBeNull()
				.And.BeEquivalentTo(new KeyValuePair<XmlQualifiedName, object>(BtsProperties.InterchangeID.QName, "interchange"));
		}

		[Fact]
		public void ResponseHasNoPropertiesPropagatedFromRequestWhenItHasNone()
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
						.WithBody("<root>dummy</root>")
						.WithStatusCode(HttpStatusCode.OK)
				);

			var response = RestClient.WithCustomBehaviors(PropertyPropagationBehavior).Get(url);
			response.Should().NotBeNull();
			response.IsEmpty.Should().BeFalse();
			response!.Properties
				.Should().NotContainKey(MessagePropertiesExtensions.PROPERTIES_TO_PROMOTE_KEY)
				.And.NotContainKey(MessagePropertiesExtensions.PROPERTIES_TO_WRITE_KEY);
		}

		[Fact]
		public void ResponseHasPropertiesPropagatedAndOverwrittenFromRequest()
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
						.WithBody("<root>dummy</root>")
						.WithStatusCode(HttpStatusCode.OK)
				);

			var request = WcfWebRequestFactory.Create(url);
			request.SetProperty(BtsProperties.MessageType, "message#type");
			request.Properties.AddPropertiesToPromote(new[] { new KeyValuePair<XmlQualifiedName, object>(BtsProperties.MessageType.QName, "initial-message#type") });
			var response = RestClient.WithCustomBehaviors(PropertyPropagationBehavior).Invoke(request);

			response.Should().NotBeNull();
			response.IsEmpty.Should().BeFalse();
			response!.Properties
				.Should().ContainKey(MessagePropertiesExtensions.PROPERTIES_TO_PROMOTE_KEY)
				.And.NotContainKey(MessagePropertiesExtensions.PROPERTIES_TO_WRITE_KEY);
			response.Properties.GetPropertiesToPromote()
				.Should().NotBeNull()
				.And.BeEquivalentTo(new KeyValuePair<XmlQualifiedName, object>(BtsProperties.MessageType.QName, "message#type"));
		}

		[Fact]
		public void ResponseHasPropertiesPropagatedFromRequest()
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
						.WithBody("<root>dummy</root>")
						.WithStatusCode(HttpStatusCode.OK)
				);

			var request = WcfWebRequestFactory.Create(url);
			request.SetProperty(BtsProperties.MessageType, "message#type");
			request.SetProperty(BtsProperties.InterchangeID, "interchange");
			var response = RestClient.WithCustomBehaviors(PropertyPropagationBehavior).Invoke(request);

			response.Should().NotBeNull();
			response.IsEmpty.Should().BeFalse();
			response!.Properties
				.Should().ContainKey(MessagePropertiesExtensions.PROPERTIES_TO_PROMOTE_KEY)
				.And.ContainKey(MessagePropertiesExtensions.PROPERTIES_TO_WRITE_KEY);
			response.Properties.GetPropertiesToPromote()
				.Should().NotBeNull()
				.And.BeEquivalentTo(new KeyValuePair<XmlQualifiedName, object>(BtsProperties.MessageType.QName, "message#type"));
			response.Properties.GetPropertiesToWrite()
				.Should().NotBeNull()
				.And.BeEquivalentTo(new KeyValuePair<XmlQualifiedName, object>(BtsProperties.InterchangeID.QName, "interchange"));
		}

		private PropertyPropagationBehavior PropertyPropagationBehavior => new(
			new PropertyPropagationCollection {
				new PropertyPropagation { Property = BtsProperties.MessageType, Mode = PropagationMode.Promote },
				new PropertyPropagation { Property = BtsProperties.InterchangeID, Mode = PropagationMode.Write }
			});

		private readonly ITestOutputHelper _output;
		private readonly RestStubHostActivator _restStubHostActivator;
	}
}
