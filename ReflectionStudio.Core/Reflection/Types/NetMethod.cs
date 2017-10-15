using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReflectionStudio.Core.Reflection.Types;

namespace ReflectionStudio.Core.Reflection.Types
{
	public enum MethodType { Constructor, Field, Property, Method, Interface, Event }

	[Serializable]
	public class NetMethod : NetBaseType
	{
		public MethodType MyType { get; set; }

		[NonSerialized]
		private int _Handle;
		public int Handle
		{
			get { return _Handle; }
			set { _Handle = value; }
		}

		//public string DisplayName { get; set; }

		public NetMethod(MethodType typ, object reflectType)
			: base(reflectType)
		{
			MyType = typ;
		}
	}
}
