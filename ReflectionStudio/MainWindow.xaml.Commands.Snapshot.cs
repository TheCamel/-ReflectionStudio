using System;
using System.Windows.Input;
using ReflectionStudio.Components.UserControls;
using ReflectionStudio.Controls;
using ReflectionStudio.Core.Project;
using System.Windows;
using ReflectionStudio.Components.Dialogs.Project;
using Fluent;

namespace ReflectionStudio
{
	/// <summary>
	/// 
	/// </summary>
	public partial class MainWindow : Fluent.RibbonWindow
	{
		private void CanDisplayChartCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			//e.CanExecute = SnapshotManager.Instance.Current != null;
		}

		private void OnDisplayChartCommand(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			//this.TransitionBox.Content = this.SnapChartUserControl;
		}

		#region ---------------------SNAPSHOT COMMAMNDS---------------------
		/// <summary>
		/// Can run program if exist in project
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CanRunProgramCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = ProjectService.Instance.Current != null && ProjectService.Instance.Current.HasProgram;
		}

		/// <summary>
		/// Run the existing program
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnRunProgramCommand(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			ProjectService.Instance.Run();
		}

		/// <summary>
		/// Can delete a snapshot file
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CanDeleteSnapshotCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			//e.CanExecute = ProjectService.Instance.Current != null &&
			//                this._projectPanel.ListBoxSnapshot.SelectedItem != null;
		}

		/// <summary>
		/// Delete a snapshot file
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnDeleteSnapshotCommand(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			//SnapshotService.Instance.Delete(this._projectPanel.ListBoxSnapshot.SelectedItem);
		}

		/// <summary>
		/// Can delete a snapshot file
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CanOpenSnapshotCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			//e.CanExecute = ProjectService.Instance.Current != null &&
			//                this._projectPanel.ListBoxSnapshot.SelectedItem != null;
		}

		/// <summary>
		/// Delete a snapshot file
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnOpenSnapshotCommand(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			//SnapshotViewDocument doc = GetSnapshotViewDocument( ((SnapshotEntity)e.Parameter).FileName );
			//doc.Snap = (SnapshotEntity)e.Parameter;
			//doc.ViewMode = SnapshotViewMode.Grid;
		}

		private void CanChangeSnapshotViewCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = _dockingManager.ActiveDocument is SnapshotViewDocument;
		}

		private void OnChangeSnapshotViewCommand(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			SnapshotViewDocument doc = (SnapshotViewDocument)_dockingManager.ActiveDocument;
			doc.ViewMode = (SnapshotViewMode)Convert.ToInt32(e.Parameter);
		}
		#endregion

		#region ---------------------DISTANT COMMAMNDS---------------------

		private void CanCaptureCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = ProjectService.Instance.Current != null
							&& ProjectService.Instance.Current.Settings.AllowDistantControl
							&& DistantSnapshotService.Instance.IsConnected();
		}

		private void OnConnectCommand(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			if (btnConnected.IsChecked == true)
			{
				btnConnected.IsChecked = DistantSnapshotService.Instance.Connect();
				btnStartCapture.IsChecked = DistantSnapshotService.Instance.IsCapturing();
				btnStopCapture.IsChecked = !btnStartCapture.IsChecked;
			}
			else
			{
				DistantSnapshotService.Instance.Disconnect();
			}
		}

		private void OnStartCaptureCommand(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			btnStartCapture.IsChecked = DistantSnapshotService.Instance.StartCapture();
			btnStopCapture.IsChecked = !btnStartCapture.IsChecked;
		}

		private void OnStopCaptureCommand(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			btnStopCapture.IsChecked = DistantSnapshotService.Instance.StopCapture();
			btnStartCapture.IsChecked = !btnStopCapture.IsChecked;
		}

		private void OnGetCaptureCommand(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			//message
			MessageBoxResult answer = MessageBoxDlg.Show(	ReflectionStudio.Properties.Resources.MSG_DELETE_DISTANT_SNAP,
															ReflectionStudio.Properties.Resources.MSG_TITLE,
															MessageBoxButton.YesNoCancel,
															MessageBoxImage.Error);

			DistantSnapshotService.Instance.RetreiveCapture(null, answer == MessageBoxResult.No ? false : true);

		}

		private void OnListCaptureCommand(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			SnapshotListDlg Dlg = new SnapshotListDlg();
			Dlg.Owner = this;
			Dlg.DataContext = DistantSnapshotService.Instance.GetCaptureList();
			Dlg.ShowDialog();
		}

		#endregion
	}
} 
