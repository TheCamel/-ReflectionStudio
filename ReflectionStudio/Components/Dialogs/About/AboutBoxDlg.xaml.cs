using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using ReflectionStudio.Controls;
using ReflectionStudio.Core.Helpers;

namespace ReflectionStudio.Components.Dialogs.About
{
    /// <summary>
    /// AboutBoxDlg display information about Reflection Studio, his related assemblies and version
    /// </summary>
	public partial class AboutBoxDlg : HeaderedDialogWindow
    {
		#region --------------------CONSTRUCTOR--------------------
		
		/// <summary>
		/// Constructor
		/// </summary>
		public AboutBoxDlg()
        {
            InitializeComponent();
		}

		#endregion

		#region --------------------FUNCTIONS--------------------

		/// <summary>
		/// Fill the assemblies
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
			Assembly EntryAssembly = Assembly.GetEntryAssembly();

			// fill the executable version
			this.labelApplicationVersion.Content = EntryAssembly.GetName().Version.ToString();

			//fill with the dependencies
			this.listBoxAssembliesList.Items.Clear();

			foreach (AssemblyName assembly in EntryAssembly.GetReferencedAssemblies())
			{
				this.listBoxAssembliesList.Items.Add(assembly.Name + ", Version=" + assembly.Version);
			}
        }
        #endregion
	}
}
