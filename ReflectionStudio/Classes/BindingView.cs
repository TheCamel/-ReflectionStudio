using System.Collections.Generic;
using System.Collections.ObjectModel;
using AvalonDock;
using ReflectionStudio.Classes.Workspace;
using ReflectionStudio.Controls.Helpers;
using ReflectionStudio.Core;
using ReflectionStudio.Core.Events;
using ReflectionStudio.Core.Project;
using ReflectionStudio.Core.Database;
namespace ReflectionStudio.Classes
{
	/// <summary>
	/// Class that holds everything that need to be binded to the main window
	/// </summary>
	internal class BindingView : BindableObject
	{
		#region ----------------SINGLETON----------------
		/// <summary>
		/// Singleton
		/// </summary>
		public static readonly BindingView Instance = new BindingView();

		/// <summary>
		/// Private constructor for singleton pattern
		/// </summary>
		private BindingView()
		{
		}
		#endregion

		#region ----------------PROPERTIES----------------

		/// <summary>
		/// Currently selected datasource in the db explorer
		/// </summary>
		public DataSource SelectedDataSource { get; set; }

		/// <summary>
		/// Workspace values
		/// </summary>
		public WorkspaceEntity Workspace
		{
			get { return WorkspaceService.Instance.Entity; }
		}


		private ProjectEntity _Project;
		/// <summary>
		/// Project
		/// </summary>
		public ProjectEntity Project
		{
			get { return _Project; }
			set
			{
				if (_Project != value)
					_Project = value;
				else return;

				base.RaisePropertyChanged("Project");
			}
		}

		/// <summary>
		/// Themes
		/// </summary>
		public List<ThemeElement> Themes
		{
			get { return ThemeManager.Instance.ThemeElements; }
		}

		/// <summary>
		/// Current selected skin from ThemeManager
		/// </summary>
		public ThemeElement CurrentSkin
		{
			get { return ThemeManager.Instance.CurrentSkin; }
			set { ThemeManager.Instance.CurrentSkin = value; }
		}

		/// <summary>
		/// Current selected color from ThemeManager
		/// </summary>
		public ThemeElement CurrentColor
		{
			get { return ThemeManager.Instance.CurrentColor; }
			set { ThemeManager.Instance.CurrentColor = value; }
		}

		private ObservableCollection<SnapshotEntity> _Snapshots;
		public ObservableCollection<SnapshotEntity> Snapshots
		{
			get { return _Snapshots; }
			set
			{
				if (_Snapshots != value)
					_Snapshots = value;
				else return;

				base.RaisePropertyChanged("Snapshots");
			}
		}

		private string _Title;
		/// <summary>
		/// Main window title
		/// </summary>
		public string Title
		{
			get { return _Title; }
			set
			{
				if (_Title != value) 
					_Title = value;
				else return;
				
				base.RaisePropertyChanged("Title");
			}
		}

		private object _PropertyContext;
		/// <summary>
		/// Object binded to the property explorer
		/// </summary>
		public object PropertyContext
		{
			get { return _PropertyContext; }
			set
			{
				if (_PropertyContext != value)
					_PropertyContext = value;
				else return;

				base.RaisePropertyChanged("PropertyContext");
			}
		}

		private List<MessageInfo> _LogEvents = new List<MessageInfo>(200);
		/// <summary>
		/// Log info list binded to the LogExplorer
		/// </summary>
		public List<MessageInfo> LogEvents
		{
			get { return _LogEvents; }
			set
			{
				if (_LogEvents != value)
					_LogEvents = value;
				else return;

				base.RaisePropertyChanged("LogEvents");
			}
		}

		private ObservableCollection<DockableContent> _Panels = new ObservableCollection<DockableContent>();
		/// <summary>
		/// Panel list for activation or not
		/// </summary>
		public ObservableCollection<DockableContent> Panels
		{
			get { return _Panels; }
			set
			{
				if (_Panels != value)
					_Panels = value;
				else return;

				base.RaisePropertyChanged("Panels");
			}
		}

		#endregion

		#region ----------------METHODS----------------

		/// <summary>
		/// Update the ui by updating the view
		/// </summary>
		public void UpdateView()
		{
			Tracer.Verbose("BindingView:UpdateView", "START");

			if (ProjectService.Instance.Current == null)
			{
				Title = ReflectionStudio.Properties.Resources.PROGRAMM_TITLE;
			}
			else
			{
				Title = string.Format("{0} : {1}", ReflectionStudio.Properties.Resources.PROGRAMM_TITLE,
												ProjectService.Instance.Current.ProjectName);
			}

			//Workspace = WorkspaceService.Instance.Entity;
			Project = ProjectService.Instance.Current;
			Snapshots = SnapshotService.Instance.Snapshots;

			Tracer.Verbose("BindingView:UpdateView", "END");
		}

		/// <summary>
		/// Add a log to the log stack, and manage the max value
		/// </summary>
		/// <param name="log"></param>
		public void AddLogEvent( MessageInfo log )
		{
			_LogEvents.Add( log );

			if (_LogEvents.Count > 180)
				_LogEvents.RemoveRange(0, 50);
		}

		#endregion
	}
}
