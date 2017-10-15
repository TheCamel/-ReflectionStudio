using System;
using System.Windows;
using System.Windows.Input;
using ReflectionStudio.Controls;

namespace ReflectionStudio.Components.Dialogs.Project
{
	/// <summary>
	/// Interaction logic for NewProjectDlg.xaml
	/// </summary>
	public partial class NewProjectDlg : HeaderedDialogWindow
	{
		public NewProjectDlg()
		{
			InitializeComponent();
		}

		#region --------------------properties--------------------

		private string _ProjectLocation;
		public string ProjectLocation
		{
			get { return _ProjectLocation; }
			set { _ProjectLocation = value; }
		}

		private string _ProjectName;
		public string ProjectName
		{
			get { return _ProjectName; }
			set { _ProjectName = value; }
		}

		#endregion

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}

		private void Ok_Click(object sender, RoutedEventArgs e)
		{
			this._ProjectLocation = this.textBoxLocation.Text;
			this._ProjectName = this.textBoxName.Text;

			this.DialogResult = true;
			this.Close();
		}

		private void btnBrowse_Click(object sender, RoutedEventArgs e)
		{
			using (System.Windows.Forms.FolderBrowserDialog browser = new System.Windows.Forms.FolderBrowserDialog())
			{
				browser.ShowNewFolderButton = false;
				browser.Description = "Select a folder...";
				browser.RootFolder = Environment.SpecialFolder.MyComputer;
				if (browser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					this.textBoxLocation.Text = browser.SelectedPath;
				}
			}
		}
	}
}
