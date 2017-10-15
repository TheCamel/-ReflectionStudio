using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AvalonDock;
using ReflectionStudio.Classes;
using ReflectionStudio.Classes.Workspace;
using ReflectionStudio.Components.Dialogs.Startup;
using ReflectionStudio.Core.Events;
using ReflectionStudio.Core.Project;
using ReflectionStudio.Components.UserControls;
using ReflectionStudio.Controls.Helpers;
using System.Threading;

namespace ReflectionStudio
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class MainWindow : Fluent.RibbonWindow
	{
		#region ---------------------PROPERTIES---------------------

		public string ApplicationPath
		{
			get { return System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath); }
		}

		public string LayoutFile
		{
			get { return System.IO.Path.Combine(ApplicationPath, "ReflectionStudio.Layout.xml"); }
		}

		private DocumentContent ActiveDocument
		{
			get;
			set;
		}

		#endregion

		#region ---------------------INIT---------------------

		/// <summary>
		/// Constructor
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();

			Tracer.Verbose("MainWindow:MainWindow", "START");

			try
			{
				Tracer.Verbose("MainWindow:MainWindow", "Event attachement");

				DocumentFactory.Instance.Initialize(this._dockingManager);
				
				//get the active documents events 
				_dockingManager.PropertyChanged += new PropertyChangedEventHandler(DockingManagerPropertyChanged);

				//get all states event changes from doc and panels
				DependencyPropertyDescriptor prop = DependencyPropertyDescriptor.FromProperty(
													DockableContent.StatePropertyKey.DependencyProperty, typeof(DockableContent));

				prop.AddValueChanged(this._LogExplorerDock, OnPanelStateChanged);
				prop.AddValueChanged(this._ProjectExplorerDock, OnPanelStateChanged);
				prop.AddValueChanged(this._PropertyExplorerDock, OnPanelStateChanged);
				prop.AddValueChanged(this._TemplateExplorerDock, OnPanelStateChanged);
				prop.AddValueChanged(this._DBExplorerDock, OnPanelStateChanged);
				prop.AddValueChanged(this._DllExplorerDock, OnPanelStateChanged);

				BindingView.Instance.Panels.Add(this._LogExplorerDock);
				BindingView.Instance.Panels.Add(this._ProjectExplorerDock);
				BindingView.Instance.Panels.Add(this._PropertyExplorerDock);
				BindingView.Instance.Panels.Add(this._TemplateExplorerDock);
				BindingView.Instance.Panels.Add(this._DBExplorerDock);
				BindingView.Instance.Panels.Add(this._DllExplorerDock);

				//catch events
				EventDispatcher.Instance.OnProjectChange += new EventHandler<ProjectEventArgs>(this._ProjectExplorerDock.OnProjectChange);
				EventDispatcher.Instance.OnProjectChange += new EventHandler<ProjectEventArgs>(OnProjectChange);
				EventDispatcher.Instance.OnProjectChange += new EventHandler<ProjectEventArgs>(SnapshotService.Instance.OnProjectChange);
				EventDispatcher.Instance.OnStatusChange += new EventHandler<StatusEventArgs>(this.MainStatusBar.OnStatusChange);

				//forward also all changes to the log toolbox
				EventDispatcher.Instance.OnMessage += new EventHandler<MessageEventArgs>(_LogExplorerDock.OnMessage);

				//set the datacontext
				this.DataContext = BindingView.Instance;
				BindingView.Instance.UpdateView();

				////init the theme menu items 
				//foreach (string item in WorkspaceService.Instance.Themes)
				//{
				//    MenuItem menuItem = new MenuItem();
				//    menuItem.Header = item;
				//    menuItem.Click += new RoutedEventHandler(ThemeMenuItem_Click);
				//    this.ThemeRibbonSplitButton.Items.Add(menuItem);
				//}

				//register commands
				InitializeBindings();

				_TemplateExplorerDock.InitializeBindings(this);
				_DBExplorerDock.InitializeBindings(this);
				
				//register the startup dialog command
				this.CommandBindings.Add(new CommandBinding(StartupDlg.StartupCmd, OnStartupCommand));

				DisplayHomeDocument();
			}
			catch (Exception err)
			{
				Tracer.Error("MainWindow.MainWindow", err);
			}
			finally
			{
				Tracer.Verbose("MainWindow:MainWindow", "END");
			}
		}

		/// <summary>
		/// OnInitialized is used to statup with a new project or the dialog without freezing
		/// the main window that must show
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			Tracer.Verbose("MainWindow:OnInitialized", "START");

			try
			{
				if (WorkspaceService.Instance.Entity.StartupProject != null)
				{
					Tracer.Verbose("MainWindow:OnInitialized", "StartupProject");

					OpenProject(WorkspaceService.Instance.Entity.StartupProject);
					WorkspaceService.Instance.Entity.StartupProject = null;
				}
				else
					if (WorkspaceService.Instance.Entity.ShowStartupDlg)
					{
						Tracer.Verbose("MainWindow:OnInitialized", "StartupDlg");

						this.IsEnabled = false;

						Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.SystemIdle, new ThreadStart(delegate
						{
							StartupDlg dlg = new StartupDlg();
							dlg.Owner = this;
							dlg.DataContext = BindingView.Instance;
							dlg.ShowDialog();
						}));

						this.IsEnabled = true;
					}
			}
			catch (Exception err)
			{
				Tracer.Error("MainWindow.OnInitialized", err);
			}
			finally
			{
				Tracer.Verbose("MainWindow:OnInitialized", "END");
			}
		}

		#endregion

		#region ---------------------WINDOW EVENTS---------------------

		/// <summary>
		/// empty
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OfficeWindow_Loaded(object sender, RoutedEventArgs e)
		{
			Tracer.Verbose("MainWindow:OfficeWindow_Loaded", "START");

			try
			{
				
			}
			catch (Exception err)
			{
				Tracer.Error("MainWindow.OfficeWindow_Loaded", err);
			}
			finally
			{
				Tracer.Verbose("MainWindow:OfficeWindow_Loaded", "END");
			}
		}

		/// <summary>
		/// After closing the project and all opened document, release event hadlers
		/// and save the layout. See also Application
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OfficeWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Tracer.Verbose("MainWindow:OfficeWindow_Closing", "START");

			try
			{
				if (CloseProject())
				{
					//catch events
					EventDispatcher.Instance.OnProjectChange -= new EventHandler<ProjectEventArgs>(OnProjectChange);
					EventDispatcher.Instance.OnStatusChange -= new EventHandler<StatusEventArgs>(this.MainStatusBar.OnStatusChange);

					//forward also all changes to the log toolbox
					EventDispatcher.Instance.OnMessage -= new EventHandler<MessageEventArgs>(this._LogExplorerDock.OnMessage);

					if (_dockingManager != null)
						_dockingManager.PropertyChanged -= new PropertyChangedEventHandler(DockingManagerPropertyChanged);

					_dockingManager.SaveLayout(LayoutFile);

					e.Cancel = false;
				}
				else
					e.Cancel = true;
			}
			catch (Exception err)
			{
				Tracer.Error("MainWindow.OfficeWindow_Closing", err);
			}
			finally
			{
				Tracer.Verbose("MainWindow:OfficeWindow_Closing", "END");
			}
		}

		/// <summary>
		/// Manage drag and drop of files onto the main window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OfficeWindow_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] droppedFilePaths =
				e.Data.GetData(DataFormats.FileDrop, true) as string[];

				foreach (string droppedFilePath in droppedFilePaths)
				{
					if (droppedFilePath.Contains(ProjectService.ProjectExtension))
						OpenProject(droppedFilePath);
				}
			}
		}

		#endregion

		#region ---------------------RIBBON EVENTS---------------------
		/// <summary>
		/// Manage the recent file list from ribbon to open projects
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		//private void ribbonApplicationMenu_MostRecentFileSelected(object sender, MostRecentFileSelectedEventArgs e)
		//{
		//    e.Handled = true;
		//    OpenProject(((RecentFileItem)e.SelectedItem).FullName);
		//}

		private void RecentFilesTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//OpenProject(((RecentFileItem)e.SelectedItem).FullName);
		}

		#endregion

		#region ---------------------PROJECT EVENT---------------------

		/// <summary>
		/// Catch the project events to update the recent file list and the view
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnProjectChange(object sender, ProjectEventArgs e)
		{
			switch (e.ProjectType)
			{
				case ProjectEventType.Opening:
					Tracer.Verbose("MainWindow:OnProjectChange", "ProjectEventType.Opening");
					break;

				case ProjectEventType.Opened:
					Tracer.Verbose("MainWindow:OnProjectChange", "ProjectEventType.Opened");
					WorkspaceService.Instance.AddRecentFile(e.FileName);
					BindingView.Instance.UpdateView();
					break;

				case ProjectEventType.Closing:
					Tracer.Verbose("MainWindow:OnProjectChange", "ProjectEventType.Closing");
					
					//CloseProjectDocuments();
					WorkspaceService.Instance.AddRecentFile(e.FileName);
					break;

				case ProjectEventType.Closed:
					Tracer.Verbose("MainWindow:OnProjectChange", "ProjectEventType.Closed");
					BindingView.Instance.UpdateView();
					break;

				default:
					break;
			}
		}

		#endregion

		#region ---------------------DOCUMENT/PANEL---------------------

		private void DisplayHomeDocument()
		{
			DocumentFactory.Instance.OpenDocument(typeof(HomeDocument), new HomeDocumentDataContext());
		}

		private void DisplayHelpDocument(string fileName)
		{
			DocumentFactory.Instance.OpenDocument(typeof(HelpDocument), new HelpDocumentDataContext() { FullName = fileName });
		}

		#endregion

		#region ---------------------DOCMANAGER EVENTS---------------------

		/// <summary>
		/// Have to use this event, because win.onload throw an exception
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void _dockingManager_Loaded(object sender, RoutedEventArgs e)
		{
			Tracer.Verbose("MainWindow:_dockingManager_Loaded", "RestoreLayout");

			if (!File.Exists(LayoutFile))
				return;

			_dockingManager.RestoreLayout(LayoutFile);

			Tracer.Verbose("MainWindow:_dockingManager_Loaded", "RestoreLayout Ok");
		}

		void DockingManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			//manage the active document
			if (e.PropertyName == "ActiveDocument" && _dockingManager.ActiveDocument != null)
			{
				Tracer.Verbose("MainWindow.DockingManagerPropertyChanged", "ActiveDocument property changed");

				// handle inactivation on document
				HandleInactiveDocument(ActiveDocument);

				// save the new active doc in the main window
				ActiveDocument = (DocumentContent)_dockingManager.ActiveDocument;

				// handle activation on document
				HandleActiveDocument(ActiveDocument);
			}
		}

		private void HandleActiveDocument(DocumentContent newDocument)
		{
			if (newDocument == null)
				return;

			Tracer.Verbose("MainWindow.HandleActiveDocument", "[{0}] '{1}' becomes active", DateTime.Now.ToLongTimeString(), newDocument.Title);

			//plug to zoom event handling if needed
			if (newDocument is ZoomDocument)
			{
				ZoomDocument zd = (ZoomDocument)newDocument;

				this.MainStatusBar.CanZoom = true;
				this.MainStatusBar.sliderZoom.Value = zd.Scale;

				//root status bar update to the document
				this.MainStatusBar.ZoomChanged += new EventHandler<ZoomRoutedEventArgs>(zd.OnZoomChanged);

				//root doc evet to the status bar
				zd.ZoomChanged += new ZoomDocument.ZoomChangedEventHandler(this.MainStatusBar.OnZoomChanged);
			}
			else
				
				this.MainStatusBar.CanZoom = false;//disable the zoom slider

			//manage the contextual tab in the ribbon
			if (newDocument is QueryDocument)
			{
				SyntaxedDocumentGroup.Visibility = System.Windows.Visibility.Visible;
				return;
			}
			if (newDocument is TemplateDocument)
			{
				SyntaxedDocumentGroup.Visibility = System.Windows.Visibility.Visible;
				return;
			}
		}

		private void HandleInactiveDocument(DocumentContent previousDocument)
		{
			if (previousDocument == null)
				return;

			Tracer.Verbose("MainWindow.HandlePreviousActiveDocument", "[{0}] '{1}' becomes inactive",
						DateTime.Now.ToLongTimeString(), previousDocument.Title);

			//remove the previous doc from event handling
			if (previousDocument is ZoomDocument)
			{
				ZoomDocument zd = (ZoomDocument)previousDocument;

				this.MainStatusBar.ZoomChanged -= new EventHandler<ZoomRoutedEventArgs>(zd.OnZoomChanged);
				zd.ZoomChanged -= new ZoomDocument.ZoomChangedEventHandler(this.MainStatusBar.OnZoomChanged);
			}

			//manage the contextual tab in the ribbon
			if (previousDocument is QueryDocument)
			{
				SyntaxedDocumentGroup.Visibility = System.Windows.Visibility.Collapsed;
				return;
			}
			if (previousDocument is TemplateDocument)
			{
				SyntaxedDocumentGroup.Visibility = System.Windows.Visibility.Collapsed;
				return;
			}
		}

		void OnPanelStateChanged(object sender, EventArgs e)
		{
			DockableContent content = sender as DockableContent;

			if (content.ContainerPane is DockablePane)
			{
				Tracer.Verbose("MainWindow.OnPanelStateChanged", "[{0}] '{1}' changed state to '{2}' (Anchor={3})",
					DateTime.Now.ToLongTimeString(), content.Title, content.State, ((DockablePane)content.ContainerPane).Anchor);
			}
			else
			{
				Tracer.Verbose("MainWindow.OnPanelStateChanged", "[{0}] '{1}' changed state to '{2}'",
					DateTime.Now.ToLongTimeString(), content.Title, content.State);
			}
		}

		#endregion

		private void BackstageTabItem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{

		}
	}
}
