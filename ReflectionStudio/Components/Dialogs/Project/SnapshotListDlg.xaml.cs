using ReflectionStudio.Controls;
using ReflectionStudio.Core.Project;
using System.Windows;

namespace ReflectionStudio.Components.Dialogs.Project
{
	/// <summary>
	/// Interaction logic for SnapshotListDlg.xaml
	/// </summary>
	public partial class SnapshotListDlg : HeaderedDialogWindow
	{
		public SnapshotListDlg()
		{
			InitializeComponent();
		}

		private void BtnDelete_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (DistantSnapshotService.Instance.DeleteCapture(dataGridSnap.SelectedItem.ToString()))
			{
				dataGridSnap.Items.RemoveAt(dataGridSnap.SelectedIndex);

				//message
				MessageBoxDlg.Show(
									ReflectionStudio.Properties.Resources.MSG_SNAP_DELETE_SUCCESS,
									ReflectionStudio.Properties.Resources.MSG_TITLE,
									MessageBoxButton.OK,
									MessageBoxImage.Information);
			}
		}

		private void BtnDownload_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (DistantSnapshotService.Instance.RetreiveCapture(dataGridSnap.SelectedItem.ToString(), true))
			{
				//message
				MessageBoxResult answer = MessageBoxDlg.Show(
													ReflectionStudio.Properties.Resources.MSG_SNAP_RETREIVE_SUCCESS,
													ReflectionStudio.Properties.Resources.MSG_TITLE,
													MessageBoxButton.OK,
													MessageBoxImage.Information);

			}
		}
	}
}
