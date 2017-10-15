using System.Windows;
using ReflectionStudio.Classes.Workspace;
using ReflectionStudio.Controls;
using System.Windows.Input;

namespace ReflectionStudio.Components.Dialogs.Startup
{
    /// <summary>
    /// Startup dialog is displayed at the application launch after splash and before application main window
    /// </summary>
	public partial class StartupDlg : DialogWindow
	{
		#region ---------------------DLG---------------------

		/// <summary>
		/// Constructor
		/// </summary>
        public StartupDlg()
        {
            InitializeComponent();
        }

		/// <summary>
		/// Init the view by showing the first tab page
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void StartupDlg_Loaded(object sender, RoutedEventArgs e)
		{
			OnViewChange(BtnProjectView, null);
		}
		#endregion

		#region ---------------------PROPERTIES---------------------
		/// <summary>
		/// the command
		/// </summary>
		private static RoutedCommand _StartupCmd = new RoutedCommand("StartupCommand", typeof(StartupDlg));

		public static RoutedCommand StartupCmd
		{
			get { return StartupDlg._StartupCmd; }
			set { StartupDlg._StartupCmd = value; }
		}

		#endregion

		private void OnViewChange(object sender, RoutedEventArgs e)
		{
			ProjectView.Visibility = Visibility.Hidden;
			SampleView.Visibility = Visibility.Hidden;
			HelpView.Visibility = Visibility.Hidden;

			if( sender == BtnProjectView )
				ProjectView.Visibility = Visibility.Visible;
			if (sender == BtnSampleView)
				SampleView.Visibility = Visibility.Visible;
			if (sender == BtnHelpView)
				HelpView.Visibility = Visibility.Visible;
		}

		private void BtnNewProject_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
			StartupCmd.Execute(new StartupParameters(eStartupCommand.New), Application.Current.MainWindow);
		}

		private void BtnOpenProject_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
			StartupCmd.Execute(new StartupParameters(eStartupCommand.Open), Application.Current.MainWindow);
		}

		private void RecentProjectList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			this.Close();
			StartupCmd.Execute(
				new StartupParameters(eStartupCommand.QuickOpen, ((RecentFileItem)RecentProjectList.SelectedItem).FullName),
				Application.Current.MainWindow);
		}

		private void BtnTechHelp_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
			StartupCmd.Execute( 
				new StartupParameters(eStartupCommand.HelpTechnical), Application.Current.MainWindow);
		}

		private void BtnHelp_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
			StartupCmd.Execute(
				new StartupParameters(eStartupCommand.Help), Application.Current.MainWindow);
		}
    }

	/// <summary>
	/// Possible startup command options
	/// </summary>
	public enum eStartupCommand
	{
		/// <summary>
		/// create a new project by showing the backstage
		/// </summary>
		New,
		/// <summary>
		/// open a project by showing the open dialog
		/// </summary>
		Open,
		/// <summary>
		/// open a given project directly
		/// </summary>
		QuickOpen,
		/// <summary>
		/// display help
		/// </summary>
		Help,
		/// <summary>
		/// display technical help
		/// </summary>
		HelpTechnical,
		/// <summary>
		/// open sample
		/// </summary>
		Sample
	}

	/// <summary>
	/// Used to transfer information with the StartupCommand
	/// </summary>
	public class StartupParameters
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cmd"></param>
		public StartupParameters(eStartupCommand cmd)
		{
			Command = cmd;
			File = string.Empty;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="file"></param>
		public StartupParameters(eStartupCommand cmd, string file)
		{
			Command = cmd;
			File = file;
		}

		/// <summary>
		/// Associated command option
		/// </summary>
		public eStartupCommand Command
		{
			get;
			set;
		}

		/// <summary>
		/// Associated filename
		/// </summary>
		public string File
		{
			get;
			set;
		}
	}
}
