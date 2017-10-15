using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ReflectionStudio.Core.Project
{
	[Serializable]
	public class MethodMapInfo
	{
		private int _Handle;
		[XmlAttribute("Handle")]
		public int Handle
		{
			get { return _Handle; }
			set { _Handle = value; }
		}

		private string _Method;
		[XmlAttribute("Method")]
		public string Method
		{
			get { return _Method; }
			set { _Method = value; }
		}

		private string _Namespace;
		[XmlAttribute("Namespace")]
		public string Namespace
		{
			get { return _Namespace; }
			set { _Namespace = value; }
		}
	}
}
