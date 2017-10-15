using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using ReflectionStudio.Core.Database.SQL2KProvider.Helper;
using ReflectionStudio.Core.Events;

namespace ReflectionStudio.Core.Database.SQL2KProvider.UI
{
	/// <summary>
	/// Interaction logic for WPFNewSource.xaml
	/// </summary>
	public partial class WPFNewSource : UserControl, IDbSourcePanel
	{
		public WPFNewSource()
		{
			InitializeComponent();
		}

		public string ConnectionString
		{
			get
			{
				if (this.radioButtonNT.IsChecked == true)
				{
					return string.Format("Data Source={0};Initial Catalog={1};Integrated Security=True", this.textBoxServer.Text, this.comboBoxDatabase.SelectedValue);
				}
				else
				{
					return string.Format("Data Source={0};Initial Catalog={1};User Id={2};Password={3};Trusted_Connection=false", 
						this.textBoxServer.Text,
						DatabaseName,
						this.textBoxLogin.Text, this.passwordBox.Password);
				}
			}
		}

		public string SourceName
		{
			get { return this.textBoxServer.Text + "/" + DatabaseName; }
		}

		internal string DatabaseName
		{
			get { return this.comboBoxDatabase.SelectedValue != null ? this.comboBoxDatabase.SelectedValue.ToString() : this.comboBoxDatabase.Text; }
		}

		public bool TestConnection()
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(ConnectionString))
				{
					connection.Open();
				}
			}
			catch (Exception error)
			{
				Tracer.Error( "Connection failed", error );
				return false;
			}

			return true;
		}

		private void comboBoxDatabase_DropDownOpened(object sender, EventArgs e)
		{
			if (this.radioButtonNT.IsChecked == true)
			{
				if (this.comboBoxDatabase.Items.Count == 0)
				{
					DataTable table = new SchemaHelper().GetDataBases(ConnectionString);
					for (int i = 0; i < table.Rows.Count; i++)
						this.comboBoxDatabase.Items.Add(table.Rows[i]["database_name"]);
				}
			}
		}
	}
}
