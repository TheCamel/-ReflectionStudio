using System.Windows.Input;
using AvalonDock;
using ReflectionStudio.Controls;
using System.Windows;
using System.Windows.Controls;
using Fluent;

namespace ReflectionStudio
{
	/// <summary>
	/// 
	/// </summary>
	public partial class MainWindow : Fluent.RibbonWindow
	{
		#region ---------------------VIEW COMMAMNDS---------------------

		private void ShowExplorerMenuItem_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.CheckBox ck = sender as System.Windows.Controls.CheckBox;
			DockableContent content = ck.DataContext as DockableContent;

			if (ck.IsChecked.HasValue)
			{
				if (ck.IsChecked == true)
					content.Show();
				else
					content.Hide();
			}
		}

		#endregion
	}
} 
