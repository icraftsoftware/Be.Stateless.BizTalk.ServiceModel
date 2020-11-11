#region Copyright & License

// Copyright © 2012 - 2022 François Chabot
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

namespace Be.Stateless.BizTalk.Unit.ServiceModel.Channels
{
	public class StreamBodyWriter : System.ServiceModel.Channels.StreamBodyWriter
	{
		public StreamBodyWriter(System.IO.Stream stream)
			: base(false)
		{
			_stream = stream;
		}

		#region Base Class Member Overrides

		protected override void OnWriteBodyContents(System.IO.Stream stream)
		{
			_stream.CopyTo(stream);
		}

		#endregion

		private readonly System.IO.Stream _stream;
	}
}
