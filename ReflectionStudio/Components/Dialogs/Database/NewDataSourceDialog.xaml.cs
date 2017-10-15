using ReflectionStudio.Controls;
using System.Windows;
using ReflectionStudio.Core.Database;
using System;
using System.Windows.Controls;

namespace ReflectionStudio.Components.Dialogs.Database
{
	/// <summary>
	/// Interaction logic for NewDataSourceDialog.xaml
	/// </summary>
	public partial class NewDataSourceDialog : HeaderedDialogWindow
	{
		public NewDataSourceDialog()
		{
			InitializeComponent();
		}

		private IDbSourcePanel _SourcePanel = null;
		public IDbSourcePanel SourcePanel
		{
			get { return _SourcePanel; }
			set
			{
				if (_SourcePanel != value)
				{
					UserControl ctrl = value as UserControl;
					this.MainGrid.Children.Add(ctrl);

					Grid.SetColumn(ctrl, 0);
					Grid.SetRow(ctrl, 0);
				}
			}
		}

		private void btnTest_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				SourcePanel.TestConnection();
				MessageBox.Show("Success !");
			}
			catch (Exception error)
			{
				MessageBox.Show("Connection failed : " + error.Message);
			}
		}

		private void btnCreate_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			this.Close();
		}
	}
}
