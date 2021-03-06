﻿using System;
using System.Xml.Serialization;

namespace ReflectionStudio.Core.Project.Settings
{
	[Serializable, XmlRoot(ElementName = "config")]
	public class SpySettings
	{
		[XmlAttribute]
		private int _BuildKey;
		public int BuildKey
		{
			get { return _BuildKey; }
			set { _BuildKey = value; }
		}

		[XmlAttribute]
		private Guid _MethodMapGuid;
		public Guid MethodMapGuid
		{
			get { return _MethodMapGuid; }
			set { _MethodMapGuid = value; }
		}

		[XmlAttribute]
		private ProfilerLogLevel _LogLevel;
		public ProfilerLogLevel LogLevel
		{
			get { return _LogLevel; }
			set { _LogLevel = value; }
		}

		[XmlAttribute]
		private ProfilMode _Action;
		public ProfilMode Action
		{
			get { return _Action; }
			set { _Action = value; }
		}

		[XmlAttribute]
		private bool _AllowDistantControl;
		public bool AllowDistantControl
		{
			get { return _AllowDistantControl; }
			set { _AllowDistantControl = value; }
		}

		[XmlAttribute]
		private bool _CaptureOnStart;
		public bool CaptureOnStart
		{
			get { return _CaptureOnStart; }
			set { _CaptureOnStart = value; }
		}

		[XmlAttribute]
		private Transport _TransportMode;
		public Transport TransportMode
		{
			get { return _TransportMode; }
			set { _TransportMode = value; }
		}

		[XmlAttribute]
		private int _DistantPort;
		public int DistantPort
		{
			get { return _DistantPort; }
			set { _DistantPort = value; }
		}

		[XmlAttribute]
		private string _Machine;
		public string Machine
		{
			get { return _Machine; }
			set { _Machine = value; }
		}

		[XmlAttribute]
		private bool _UseCompression;
		public bool UseCompression
		{
			get { return _UseCompression; }
			set { _UseCompression = value; }
		}
	}
}
