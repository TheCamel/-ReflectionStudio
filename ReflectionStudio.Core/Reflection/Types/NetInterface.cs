
using System;
namespace ReflectionStudio.Core.Reflection.Types
{
	[Serializable]
	public class NetInterface : NetBaseType
	{
		#region ---------------CONSTRUCTORS---------------

		public NetInterface(object reflectType)
			: base(reflectType)
		{
		}

		#endregion

	}
}