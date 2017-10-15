using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReflectionStudio.Core.Events;

namespace ReflectionStudio.Core.Project.Settings
{
	[Serializable]
	public class ProjectSettings
	{
		[NonSerialized()]
		private bool _IsChanged = false;
		public bool IsChanged
		{
			get { return _IsChanged ; }
			set
			{
				if (_IsChanged != value)
				{
					_IsChanged = value;
					EventDispatcher.Instance.RaiseProject(null, ProjectEventType.SettingsChange);
				}
			}
		}

		private int _BuildKey = (int)DateTime.Now.Ticks;
		public int BuildKey
		{
			get { return _BuildKey; }
			set { if (_BuildKey != value) { _BuildKey = value; IsChanged = true; } }
		}

		private Guid _MethodMapGuid = System.Guid.NewGuid();
		public Guid MethodMapGuid
		{
			get { return _MethodMapGuid; }
			set { if (_MethodMapGuid != value) { _MethodMapGuid = value; IsChanged = true; } }
		}

		private ProfilerLogLevel _LogLevel = ProfilerLogLevel.Error;
		public ProfilerLogLevel LogLevel
		{
			get { return _LogLevel; }
			set { if (_LogLevel != value) { _LogLevel = value; IsChanged = true; } }
		}

		private ProfilMode _Action = ProfilMode.TimeSpent;
		public ProfilMode Action
		{
			get { return _Action; }
			set { if (_Action != value) { _Action = value; IsChanged = true; } }
		}

		private bool _SkipSmallMethods = true;
		public bool SkipSmallMethods
		{
			get { return _SkipSmallMethods; }
			set { if (_SkipSmallMethods != value) { _SkipSmallMethods = value; IsChanged = true; } }
		}

		private bool _AllowDistantControl = true;
		public bool AllowDistantControl
		{
			get { return _AllowDistantControl; }
			set { if (_AllowDistantControl != value) { _AllowDistantControl = value; IsChanged = true; } }
		}

		private bool _CaptureOnStart = true;
		public bool CaptureOnStart
		{
			get { return _CaptureOnStart; }
			set { if (_CaptureOnStart != value) { _CaptureOnStart = value; IsChanged = true; } }
		}

		private Transport _TransportMode = Transport.Http;
		public Transport TransportMode
		{
			get { return _TransportMode; }
			set { if (_TransportMode != value) { _TransportMode = value; IsChanged = true; } }
		}

		private int _DistantPort = 4433;
		public int DistantPort
		{
			get { return _DistantPort; }
			set { if (_DistantPort != value) { _DistantPort = value; IsChanged = true; } }
		}

		private string _Machine = "localhost";
		public string Machine
		{
			get { return _Machine; }
			set { if (_Machine != value) { _Machine = value; IsChanged = true; } }
		}

		private bool _UseCompression = false;
		public bool UseCompression
		{
			get { return _UseCompression; }
			set { if (_UseCompression != value) { _UseCompression = value; IsChanged = true; } }
		}

		/// <summary>
		/// Convert to spy need only struct for xml serialize
		/// </summary>
		/// <returns></returns>
		public SpySettings ConvertToSpy()
		{
			return new SpySettings()
			{
				Action = _Action,
				AllowDistantControl = _AllowDistantControl,
				BuildKey = _BuildKey,
				CaptureOnStart = _CaptureOnStart,
				DistantPort = _DistantPort,
				LogLevel = _LogLevel,
				Machine = _Machine,
				MethodMapGuid = _MethodMapGuid,
				TransportMode = _TransportMode,
				UseCompression = _UseCompression
			};
		}
	}

	public enum ProfilerLogLevel
	{
		None = 0,
		Error = 1,
		All = 2
	}

	public enum ProfilMode
	{
		TimeSpent = 0, CallCount = 1
	}

	public enum Transport
	{
		Http = 0, Tcp = 1
	}
}
