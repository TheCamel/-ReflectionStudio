using ReflectionStudio.Controls;
using ReflectionStudio.Classes;
using System;

namespace ReflectionStudio.Components.Dialogs
{
	/// <summary>
	/// Interaction logic for NewDocumentDlg.xaml
	/// </summary>
	public partial class NewDocumentDlg : HeaderedDialogWindow
	{
		public NewDocumentDlg()
		{
			InitializeComponent();
		}

        public SupportedDocumentInfo DocumentTypeSelected { get; set; }

		private void btnCancel_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.DocumentTypeSelected = null;
			this.DialogResult = false;
			this.Close();
		}

		private void btnOk_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.DocumentTypeSelected = (SupportedDocumentInfo)this.listBoxDocType.SelectedItem;
			this.DialogResult = true;
			this.Close();
		}
	}
}
