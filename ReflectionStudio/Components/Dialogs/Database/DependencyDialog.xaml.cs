using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using ReflectionStudio.Core.Database.Schema;
using ReflectionStudio.Controls;

namespace ReflectionStudio.Components.Dialogs.Database
{
	/// <summary>
	/// Interaction logic for WPFNewSource.xaml
	/// </summary>
	public partial class DependencyDialog : HeaderedDialogWindow
	{
		public DependencyDialog()
		{
			InitializeComponent();
		}

		public SchemaObjectBase Reference { get; set; }

		private void RadioButton_Checked(object sender, RoutedEventArgs e)
		{
			FillTreeView();
		}

		private void FillTreeView()
		{
			if (Reference == null)
				return;

			this.tbReference.Text = Reference.Name;

			DataTable table = Reference.Database.Provider.GetDependencies(
													Reference.Database.ConnectionString,
													Reference.Database,
													Reference,
													rdDependsOn.IsChecked == true ? false : true);

			List<DataItem> list = FindChildren(table, Reference.Name);

			this.treeView.DataContext = list;
		}

		private List<DataItem> FindChildren(DataTable table, string parentName)
		{
			IEnumerable<DataItem> list = (from child in table.AsEnumerable()
											   join parent in table.AsEnumerable() on child["object_id"] equals parent["relative_id"]
											   where (string)parent["object_name"] == parentName
											   orderby (string)child["object_name"]
											   select new DataItem()
											   {
												   Name = (string)child["object_name"],
												   Id = child.IsNull("object_id") ? 0 : (int)child["object_id"],
												   //RelativeId = child.IsNull("relative_id") ? 0 : (int)child["relative_id"]
											   }).Distinct(new DistinctID());

			List<DataItem> test = list.ToList<DataItem>();

			foreach (DataItem item in test)
			{
				//if (item.RelativeId != 0)
				item.Children = FindChildren(table, item.Name);
			}

			return test;
		}

		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}

	internal class DistinctID : IEqualityComparer<DataItem>
	{
		public bool Equals(DataItem x, DataItem y)
		{
			return x.Id.Equals(y.Id);
		}

		public int GetHashCode(DataItem obj)
		{
			return obj.Id.GetHashCode();
		}
	}

	public class DataItem
	{
		public string Name
		{
			get;
			set;
		}

		public int Id
		{
			get;
			set;
		}

		public int RelativeId
		{
			get;
			set;
		}

		public IEnumerable<DataItem> Children
		{
			get;
			set;
		}
	}
}
