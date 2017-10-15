using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReflectionStudio.Core.Project;

namespace ReflectionStudio.Components.UserControls
{
	/// <summary>
	/// Interaction logic for VisualView.xaml
	/// </summary>
	public partial class VisualView : UserControl
	{
		public VisualView()
		{
			InitializeComponent();
		}

		private void ListAfter_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void ListBefore_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void ListCurrent_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SnapshotEntity entity = (SnapshotEntity)DataContext;
			CallStackInfoAgregated info = (CallStackInfoAgregated)this.ListCurrent.SelectedItem;

			//query the before and after list and update the binding
			this.ListBefore.ItemsSource = entity.CallstackAg.Where(p => p.MethodHandle == info.CalledByHandle);
			this.ListAfter.ItemsSource = entity.CallstackAg.Where(p => p.CalledByHandle == info.MethodHandle);
		}

		private void ListAfter_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			CallStackInfoAgregated info = (CallStackInfoAgregated)this.ListAfter.SelectedItem;

			//change the datagrid selected item to the one here
			SnapshotEntity entity = (SnapshotEntity)DataContext;
			entity.CurrentAgreg = entity.CallstackAg.Where( p => p.MethodHandle == info.MethodHandle );
		}

		private void ListBefore_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			CallStackInfoAgregated info = (CallStackInfoAgregated)this.ListBefore.SelectedItem;

			//change the datagrid selected item to the one here
			SnapshotEntity entity = (SnapshotEntity)DataContext;
			entity.CurrentAgreg = entity.CallstackAg.Where(p => p.MethodHandle == info.MethodHandle);
		}
	}
}
