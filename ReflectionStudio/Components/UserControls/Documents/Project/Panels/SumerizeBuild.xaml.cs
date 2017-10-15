using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ReflectionStudio.Core.Project;
using ReflectionStudio.Core.Events;

namespace ReflectionStudio.Components.UserControls
{
	/// <summary>
	/// Interaction logic for BuildUC.xaml
	/// </summary>
	public partial class SumerizeBuild : UserControl
	{
		private BackgroundWorker _Worker = null;

		public SumerizeBuild()
		{
			InitializeComponent();
		}

		private void btnStart_Click(object sender, RoutedEventArgs e)
		{
			EventDispatcher.Instance.RaiseStatus( "Start building the project...", StatusEventType.StartProgress);

			//this.tbBuildStep.Text = "";
			this.btnStart.IsEnabled = false;
			this.btnStop.IsEnabled = true;

			//init the background worker process
			_Worker = new BackgroundWorker();
			_Worker.WorkerReportsProgress = true;
			_Worker.WorkerSupportsCancellation = true;
			_Worker.DoWork += new DoWorkEventHandler(bw_DoWork);
			_Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
			//_Worker.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);

			// Start the asynchronous operation.
			_Worker.RunWorkerAsync(ProjectService.Instance.Current);

			//SnapshotManager.Instance.SaveSnapshot("d:\\temp\\test.log");
		}

		//void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
		//{
		//    //this.tbBuildStep.Text += e.UserState.ToString();
		//    //this.ScrollTB.ScrollToBottom();

		//    //EventDispatcher.Instance.RaiseMessage(e.UserState.ToString());
		//}

		void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			// First, handle the case where an exception was thrown.
			if (e.Error != null)
			{
				EventDispatcher.Instance.RaiseStatus("Project build error !", StatusEventType.StopProgress);
			}
			else if (e.Cancelled)
			{
				// Next, handle the case where the user canceled the operation.
				// Note that due to a race condition in the DoWork event handler, the Cancelled
				// flag may not have been set, even though CancelAsync was called.
				EventDispatcher.Instance.RaiseStatus("Project build canceled !", StatusEventType.StopProgress);
			}
			else if ((bool)e.Result == true)
			{
				// Finally, handle the case where the operation succeeded.
				EventDispatcher.Instance.RaiseStatus("Project builded !", StatusEventType.StopProgress);
			}

			// Enable the Start button.
			this.btnStart.IsEnabled = true;

			// Disable the Cancel button.
			this.btnStop.IsEnabled = false;

			System.Media.SystemSounds.Exclamation.Play();
		}

		void bw_DoWork(object sender, DoWorkEventArgs e)
		{
			ProjectService.Instance.Build(sender, e);
		}

		private void btnStop_Click(object sender, RoutedEventArgs e)
		{
			// Cancel the asynchronous operation.
			_Worker.CancelAsync();
			this.btnStop.IsEnabled = false;
		}
	}
}
