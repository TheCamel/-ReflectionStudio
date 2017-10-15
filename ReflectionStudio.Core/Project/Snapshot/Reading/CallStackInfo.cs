using System.Xml.Serialization;
using System;

namespace ReflectionStudio.Core.Project
{
	[Serializable]
	public class CallStackInfo
	{
		[XmlAttribute("Handle")]
		public int MethodHandle { get; set; }

		[XmlAttribute("Ellapsed")]
		public long TotalTick { get; set; }

		[XmlAttribute("CalledBy")]
		public int CalledByHandle { get; set; }
	}

	[Serializable]
	public class CallStackInfoExtended : CallStackInfo
	{
		public CallStackInfoExtended(CallStackInfo item)
		{
			TotalTick = item.TotalTick;
			CalledByHandle = item.CalledByHandle;
			MethodHandle = item.MethodHandle;
		}

		public string Namespace { get; set; }
		public string MethodName { get; set; }
		public long InternalTick { get; set; }
		public string CalledBy { get; set; }
	}

	[Serializable]
	public class CallStackInfoAgregated : CallStackInfoExtended
	{
		public CallStackInfoAgregated(CallStackInfoExtended item) : base( item )
		{
			TotalTick = item.TotalTick;
			CalledByHandle = item.CalledByHandle;
			MethodHandle = item.MethodHandle;
			Namespace = item.Namespace;

			MethodName = item.MethodName;
			InternalTick = item.InternalTick;
			CalledBy = item.CalledBy;
		}
		public long CallCount { get; set; }
		public long AverageTick { get; set; }
	}
}
