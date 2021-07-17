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
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel.Configuration;
using Be.Stateless.BizTalk.ServiceModel.Description;

namespace Be.Stateless.BizTalk.ServiceModel.Configuration
{
	/// <summary>
	/// Represents a configuration element that contains the sub-element that specifies the <see
	/// cref="FaultMessageMinificationBehavior"/> behavior extension, which enables to customize endpoint behaviors.
	/// </summary>
	[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Configuration element.")]
	public class FaultMessageMinificationBehaviorExtension : BehaviorExtensionElement
	{
		#region Base Class Member Overrides

		/// <summary>
		/// Gets the type of behavior.
		/// </summary>
		/// <returns>
		/// A <see cref="Type"/>.
		/// </returns>
		public override Type BehaviorType => typeof(FaultMessageMinificationBehavior);

		/// <summary>
		/// Creates a behavior extension based on the current configuration settings.
		/// </summary>
		/// <returns>
		/// The behavior extension.
		/// </returns>
		protected override object CreateBehavior()
		{
			return new FaultMessageMinificationBehavior();
		}

		#endregion
	}
}
