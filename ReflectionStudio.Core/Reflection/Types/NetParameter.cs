using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReflectionStudio.Core.Reflection.Types
{
	[Serializable]
	public class NetParameter : NetBaseType
	{
		public NetParameter(object reflectType)
			: base(reflectType)
		{
		}
	}
}
