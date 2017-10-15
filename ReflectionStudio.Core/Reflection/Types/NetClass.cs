using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReflectionStudio.Core.Reflection.Types;
using System.Collections.ObjectModel;

namespace ReflectionStudio.Core.Reflection.Types
{
	[Serializable]
	public class NetClass : NetBaseType
	{
		public NetClass(object reflectType)
			: base(reflectType)
		{
		}

		public IEnumerable<NetMethod> Constructors
		{
			get { return (IEnumerable<NetMethod>)Children.Cast<NetMethod>().Select(item => item.MyType == MethodType.Constructor); }
		}
	}
}
