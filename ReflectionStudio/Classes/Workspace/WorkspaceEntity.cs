using System;
using System.Collections.ObjectModel;
using ReflectionStudio.Core.Database;
using ReflectionStudio.Core.Events;
using ReflectionStudio.Core.Reflection.Types;

namespace ReflectionStudio.Classes.Workspace
{
	[Serializable]
	public class WorkspaceEntity
	{
		public WorkspaceEntity()
		{
		}

		ObservableCollection<RecentFileItem> _RecentFiles = new ObservableCollection<RecentFileItem>();
		public ObservableCollection<RecentFileItem> RecentFiles
		{
			get { return _RecentFiles; }
			set { _RecentFiles = value; }
		}

		private string _CurrentTheme = string.Empty;
		public string CurrentTheme
		{
			get { return _CurrentTheme; }
			set { _CurrentTheme = value; }
		}

		private bool _ShowStartupDlg = true;
		public bool ShowStartupDlg
		{
			get { return _ShowStartupDlg; }
			set { _ShowStartupDlg = value; }
		}

		private double _ZoomScale = 1;
		public double ZoomScale
		{
			get { return _ZoomScale; }
			set { _ZoomScale = value; }
		}

		[NonSerialized()]
		private string _StartupProject = null;
		public string StartupProject
		{
			get { return _StartupProject; }
			set { _StartupProject = value; }
		}

		private MessageEventType _LogLevel = MessageEventType.None;
		public MessageEventType LogLevel
		{
			get { return _LogLevel; }
			set
			{
				if(_LogLevel != value )
					_LogLevel = value;
			}
		}

		private ObservableCollection<DataSource> _DataSources = new ObservableCollection<DataSource>();
		public ObservableCollection<DataSource> DataSources
		{
			get
			{
				return _DataSources;
			}
			set
			{
				if (_DataSources != value)
					_DataSources = value;
			}
		}

		private ObservableCollection<NetAssembly> _Assemblies = new ObservableCollection<NetAssembly>();
		public ObservableCollection<NetAssembly> Assemblies
		{
			get { return _Assemblies; }
			set
			{
				if (_Assemblies != value)
					_Assemblies = value;
			}
		}
	}
}
