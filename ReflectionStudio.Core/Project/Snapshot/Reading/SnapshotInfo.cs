using System.Collections.Generic;
using System.Xml.Serialization;

namespace ReflectionStudio.Core.Project
{
	[XmlRootAttribute("Snapshot", Namespace="", IsNullable=false)]
	public class SnapshotInfo
	{
		private int _Version;
		[XmlAttribute("Version")]
		public int Version
		{
			get { return _Version; }
			set { _Version = value; }
		}

		private string _Project;
		[XmlAttribute("Project")]
		public string Project
		{
			get { return _Project; }
			set { _Project = value; }
		}

		private string _Map;
		[XmlAttribute("MethodMap")]
		public string Map
		{
			get { return _Map; }
			set { _Map = value; }
		}

		private List<CallStackInfo> _CallStack;
		[XmlElement("Item")]
		public List<CallStackInfo> CallStack
		{
			get { return _CallStack; }
			set { _CallStack = value; }
		}
	}
}
