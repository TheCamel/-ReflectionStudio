
using System;
namespace ReflectionStudio.Core.Reflection.Types
{
	[Serializable]
	public class NetEnum : NetBaseType
	{
		#region ---------------CONSTRUCTORS---------------

		public NetEnum(object reflectType)
			: base(reflectType)
		{
		}

		#endregion
	}
}
