using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using ReflectionStudio.Components.Dialogs.Project;
using ReflectionStudio.Components.Dialogs.Startup;
using ReflectionStudio.Components.UserControls;
using ReflectionStudio.Controls;
using ReflectionStudio.Core.Events;
using ReflectionStudio.Core.Project;
using Fluent;

namespace ReflectionStudio
{
	/// <summary>
	/// 
	/// </summary>
	public partial class MainWindow : Fluent.RibbonWindow
	{
		#region ---------------------PROJECT.NEW---------------------

		public void CanExecuteProjectNewCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = true;
		}

		public void ProjectNewCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			NewProject();
			e.Handled = true;
		}

		/// <summary>
		/// On new project command, display the creation dialog...
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NewProject()
		{
			try
			{
				NewProjectDlg browser = new NewProjectDlg();
				browser.Owner = Application.Current.MainWindow;

				if (browser.ShowDialog() == true)
					ProjectService.Instance.Create(browser.ProjectLocation, browser.ProjectName);
			}
			catch (Exception err)
			{
				Tracer.Error("MainWindow.NewProject", err);
			}
		}
		#endregion

		#region ---------------------PROJECT.OPEN---------------------

		public void CanExecuteProjectOpenCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OpenProjectCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			OpenProject();
		}

		private void OpenProject()
		{
			try
			{
				using (System.Windows.Forms.OpenFileDialog browser = new System.Windows.Forms.OpenFileDialog())
				{
					if (browser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						OpenProject(browser.FileName);
					}
				}
			}
			catch (Exception err)
			{
				Tracer.Error("MainWindow.OpenProject", err);
			}
		}

		private void OpenProject(string filePath)
		{
			ProjectService.Instance.Open(filePath);
		}

		#endregion

		#region ---------------------PROJECT.SAVE---------------------

		public void CanExecuteProjectSaveCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = ProjectService.Instance.Current != null;
		}

		/// <summary>
		/// On save project, call the manager
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void ProjectSaveCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			ProjectService.Instance.Save();
		}

		#endregion

		#region ---------------------PROJECT.SAVEAS---------------------

		public void CanExecuteProjectSaveAsCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = ProjectService.Instance.Current != null;
		}

		/// <summary>
		/// On save project, call the manager
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void ProjectSaveAsCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			ProjectService.Instance.Save();
		}

		#endregion

		#region ---------------------PROJECT.CLOSE---------------------

		public void CanExecuteProjectCloseCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = ProjectService.Instance.Current != null;
		}

		/// <summary>
		/// On save project, call the manager
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void ProjectCloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			CloseProject();
		}

		private bool CloseProject()
		{
			Tracer.Verbose("MainWindow:CloseProject", "START");

			bool bIsClosed = false;

			try
			{
				if (ProjectService.Instance.Current == null) return true;

				if (ProjectService.Instance.Current.IsChanged)
				{
					while (!bIsClosed)
					{
						//message
						MessageBoxResult answer = MessageBoxDlg.Show(
															ReflectionStudio.Properties.Resources.MSG_PRJ_ASK_SAVE,
															ReflectionStudio.Properties.Resources.MSG_TITLE,
															MessageBoxButton.YesNoCancel,
															MessageBoxImage.Error);

						if (answer == MessageBoxResult.No)
							bIsClosed = true;

						if (answer == MessageBoxResult.Yes)
							bIsClosed = ProjectService.Instance.Save();

						if (answer == MessageBoxResult.Cancel)
							bIsClosed = false;
					}
				}
				else bIsClosed = true;

				if (bIsClosed)
					bIsClosed = ProjectService.Instance.Close();

				return bIsClosed;
			}
			catch (Exception err)
			{
				Tracer.Error("MainWindow.CloseProject", err);
				return false;
			}
			finally
			{
				Tracer.Verbose("MainWindow:CloseProject", "END");
			}
		}

		#endregion

		#region ---------------------PROJECTS COMMAMNDS---------------------

		/// <summary>
		/// Project commands can be executed if project exist
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CanExecuteProjectCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = ProjectService.Instance.Current != null;
		}

		#endregion

		private void OnStartupCommand(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			StartupParameters param = (StartupParameters)e.Parameter;
			switch (param.Command)
			{
				case eStartupCommand.New:
					NewProject();
					break;
				case eStartupCommand.Open:
					OpenProject();
					break;
				case eStartupCommand.QuickOpen:
					OpenProject(param.File);
					break;
				case eStartupCommand.Help:
					DisplayHelpDocument(@"Help\ReflectionStudio.Help.xps");
					break;
				case eStartupCommand.HelpTechnical:
					DisplayHelpDocument(@"Help\ReflectionStudio.Technical.xps");
					break;
				case eStartupCommand.Sample:
					OpenProject(param.File);
					break;
			}
		}

		#region ---------------------ASSEMBLIES COMMAMNDS---------------------

		/// <summary>
		/// Can execute assembly command if project exist
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void CanExecuteAddAssemblyCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = ProjectService.Instance.Current != null;
		}

		/// <summary>
		/// Add an assembly from file by displaying the open dialog
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void AddAssemblyFromFileCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			using (System.Windows.Forms.OpenFileDialog browser = new System.Windows.Forms.OpenFileDialog())
			{
				if (browser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					ProjectService.Instance.AddAssembly(browser.FileName);
					return;
				}
			}
		}

		/// <summary>
		/// Add all assemblies in a folder
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void AddAssemblyFromFolderCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			
			using (System.Windows.Forms.FolderBrowserDialog browser = new System.Windows.Forms.FolderBrowserDialog())
			{
				browser.ShowNewFolderButton = false;
				browser.Description = ReflectionStudio.Properties.Resources.MSG_SELECT_FOLDER;
				browser.RootFolder = Environment.SpecialFolder.MyComputer;
				if (browser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					ProjectService.Instance.AddAssemblyFromFolder(browser.SelectedPath, true);
					return;
				}
			}
		}

		private void CanExecuteRemoveAssemblyCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			//e.CanExecute = ProjectService.Instance.Current != null &&
			//                this._projectPanel.ListBoxAssembly.SelectedItem != null;
		}

		/// <summary>
		/// remove the assembly from the left list box
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnRemoveAssembly(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			//ProjectService.Instance.RemoveAssembly(this._projectPanel.ListBoxAssembly.SelectedItem);
		}

		/// <summary>
		/// Re-scan the assemblies to found differences
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnRefreshAssembly(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			ProjectService.Instance.Refresh();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSignAssembly(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
		}

		#endregion

		#region ---------------------BUILDS COMMAMNDS---------------------

		/// <summary>
		/// Can build when the project contains assemblies
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CanExecuteBuildCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = ProjectService.Instance.Current != null 
						&& ProjectService.Instance.Current.Assemblies.Count > 0
						&& _Worker == null;
		}

		private void CanExecuteStopBuildCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = ProjectService.Instance.Current != null && _Worker != null;
		}

		private BackgroundWorker _Worker = null;

		/// <summary>
		/// Display the build panel
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnBuild(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			EventDispatcher.Instance.RaiseStatus( ReflectionStudio.Properties.Resources.MSG_PROJECT_START_BUILD, StatusEventType.StartProgress);

			//init the background worker process
			_Worker = new BackgroundWorker();
			_Worker.WorkerReportsProgress = true;
			_Worker.WorkerSupportsCancellation = true;
			_Worker.DoWork += new DoWorkEventHandler(bw_DoBuildWork);
			_Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
			//_Worker.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);

			// Start the asynchronous operation.
			_Worker.RunWorkerAsync(ProjectService.Instance.Current);

			//SnapshotManager.Instance.SaveSnapshot("d:\\temp\\test.log");
		}

		private void OnStopBuild(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			// Cancel the asynchronous operation.
			_Worker.CancelAsync();
			_Worker = null;
		}

		#endregion


		//void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
		//{
		//    EventDispatcher.Instance.RaiseMessage(e.UserState.ToString());
		//}

		void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			// First, handle the case where an exception was thrown.
			if (e.Error != null)
			{
				EventDispatcher.Instance.RaiseStatus( ReflectionStudio.Properties.Resources.MSG_PROJECT_BUILD_ERROR, StatusEventType.StopProgress);
			}
			else if (e.Cancelled)
			{
				// Next, handle the case where the user canceled the operation.
				// Note that due to a race condition in the DoWork event handler, the Cancelled
				// flag may not have been set, even though CancelAsync was called.
				EventDispatcher.Instance.RaiseStatus( ReflectionStudio.Properties.Resources.MSG_PROJECT_BUILD_CANCEL, StatusEventType.StopProgress);
			}
			else if ((bool)e.Result == true)
			{
				// Finally, handle the case where the operation succeeded.
				EventDispatcher.Instance.RaiseStatus(ReflectionStudio.Properties.Resources.MSG_PROJECT_BUILD_OK, StatusEventType.StopProgress);
			}

			_Worker = null;

			System.Media.SystemSounds.Exclamation.Play();
		}

		void bw_DoBuildWork(object sender, DoWorkEventArgs e)
		{
			ProjectService.Instance.Build(sender, e);
		}
	}
} 
