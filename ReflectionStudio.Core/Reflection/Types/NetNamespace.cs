using System.Collections.Generic;
using System;

namespace ReflectionStudio.Core.Reflection.Types
{
	[Serializable]
	public class NetNamespace : NetBaseType
    {
		public NetNamespace(object reflectType)
			: base(reflectType)
        {
        }
    }
}
