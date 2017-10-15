using System.Collections.Generic;
using System;

namespace ReflectionStudio.Core.Reflection.Types
{
	[Serializable]
	public class NetReference : NetBaseType
	{
		#region ---------------CONSTRUCTORS---------------

		public NetReference(object reflectType)
			: base(reflectType)
        {
        }

		#endregion
	}
}
