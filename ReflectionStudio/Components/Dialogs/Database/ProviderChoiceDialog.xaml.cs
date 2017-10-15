using ReflectionStudio.Controls;
using ReflectionStudio.Core.Database;

namespace ReflectionStudio.Components.Dialogs.Database
{
	/// <summary>
	/// Interaction logic for NewSourceDialog.xaml
	/// </summary>
	public partial class ProviderChoiceDialog : HeaderedDialogWindow
	{
		public ProviderChoiceDialog()
		{
			InitializeComponent();
		}

		public IDbSchemaProvider SelectedProvider { get; set; }

		private void btnCancel_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.SelectedProvider = null;
			this.DialogResult = false;
			this.Close();
		}

		private void btnOk_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.SelectedProvider = (IDbSchemaProvider)this.ListBoxProviders.SelectedItem;
			this.DialogResult = true;
			this.Close();
		}

	}
}
