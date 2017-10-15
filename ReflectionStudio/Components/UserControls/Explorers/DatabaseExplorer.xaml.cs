using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AvalonDock;
using ReflectionStudio.Classes;
using ReflectionStudio.Classes.Workspace;
using ReflectionStudio.Components.Dialogs.Database;
using ReflectionStudio.Controls;
using ReflectionStudio.Core.Database;
using ReflectionStudio.Core.Database.Schema;
using ReflectionStudio.Core.Events;
using System.Windows.Threading;
using System.Collections.Generic;

namespace ReflectionStudio.Components.UserControls
{
	/// <summary>
	/// DatabaseExplorer allow to browse db schema based on various plugin
	/// </summary>
	public partial class DatabaseExplorer : DockableContent
	{
		#region ----------------------CONSTRUCTORS----------------------

		/// <summary>
		/// Constructor
		/// </summary>
		public DatabaseExplorer()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Initialize commands and event handler
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			this.treeViewDB.OnItemNeedPopulate += new TreeViewExtended.ItemNeedPopulateEventHandler(treeViewDB_OnItemNeedPopulate);
		}

		/// <summary>
		/// Init the command binding to the main window
		/// </summary>
		/// <param name="form"></param>
		public void InitializeBindings(UIElement form)
		{
			form.CommandBindings.Add(new CommandBinding(DataSourceAdd, AddDataSourceCommandHandler));
			form.CommandBindings.Add(new CommandBinding(DataSourceRefresh, RefreshDataSourceCommandHandler, CanExecuteDataSourceCommand));
			form.CommandBindings.Add(new CommandBinding(DataSourceRemove, RemoveDataSourceCommandHandler, CanExecuteDataSourceCommand));

			form.CommandBindings.Add(new CommandBinding(QueryNewEditor, QueryNewEditorCommandHandler, CanExecuteQueryNewEditor));

			form.CommandBindings.Add(new CommandBinding(QueryScriptCreate, QueryScriptCreateCommandHandler, CanExecuteModifyQueryScript));
			form.CommandBindings.Add(new CommandBinding(QueryScriptAlter, QueryQueryScriptAlterCommandHandler, CanExecuteModifyQueryScript));
			form.CommandBindings.Add(new CommandBinding(QueryScriptDrop, QueryScriptDropCommandHandler, CanExecuteModifyQueryScript));

			form.CommandBindings.Add(new CommandBinding(QueryScriptSelect, QueryScriptSelectCommandHandler, CanExecuteQueryScript));
			form.CommandBindings.Add(new CommandBinding(QueryScriptInsert, QueryScriptInsertCommandHandler, CanExecuteQueryScript));
			form.CommandBindings.Add(new CommandBinding(QueryScriptUpdate, QueryScriptUpdateCommandHandler, CanExecuteQueryScript));
			form.CommandBindings.Add(new CommandBinding(QueryScriptDelete, QueryScriptDeleteCommandHandler, CanExecuteQueryScript));

			form.CommandBindings.Add(new CommandBinding(QueryViewDependencies, QueryViewDependenciesCommandHandler, CanExecuteQueryViewDependencies));
			form.CommandBindings.Add(new CommandBinding(QueryControlQuality, QueryControlQualityCommandHandler, CanExecuteQueryControlQuality));
		}

		#endregion

		#region ----------------------COMMANDS----------------------

		static public RoutedCommand DataSourceAdd = new RoutedCommand("DataSourceAdd", typeof(DatabaseExplorer));
		static public RoutedCommand DataSourceRefresh = new RoutedCommand("DataSourceRefresh", typeof(DatabaseExplorer));
		static public RoutedCommand DataSourceRemove = new RoutedCommand("DataSourceRemove", typeof(DatabaseExplorer));

		static public RoutedCommand QueryNewEditor = new RoutedCommand("QueryNewEditor", typeof(DatabaseExplorer));

		static public RoutedCommand QueryScriptCreate = new RoutedCommand("QueryScriptCreate", typeof(DatabaseExplorer));
		static public RoutedCommand QueryScriptAlter = new RoutedCommand("QueryScriptAlter", typeof(DatabaseExplorer));
		static public RoutedCommand QueryScriptDrop = new RoutedCommand("QueryScriptDrop", typeof(DatabaseExplorer));

		static public RoutedCommand QueryScriptSelect = new RoutedCommand("QueryScriptSelect", typeof(DatabaseExplorer));
		static public RoutedCommand QueryScriptInsert = new RoutedCommand("QueryScriptInsert", typeof(DatabaseExplorer));
		static public RoutedCommand QueryScriptUpdate = new RoutedCommand("QueryScriptUpdate", typeof(DatabaseExplorer));
		static public RoutedCommand QueryScriptDelete = new RoutedCommand("QueryScriptDelete", typeof(DatabaseExplorer));

		static public RoutedCommand QueryScriptExecute = new RoutedCommand("QueryScriptExecute", typeof(DatabaseExplorer));

		static public RoutedCommand QueryViewDependencies = new RoutedCommand("QueryViewDependencies", typeof(DatabaseExplorer));
		static public RoutedCommand QueryControlQuality = new RoutedCommand("QueryControlQuality", typeof(DatabaseExplorer));

		#endregion

		#region ----------------------PROPERTIES----------------------

		public object ActiveDBObject()
		{
			TreeViewItem item = ActiveItem;
			if (item != null)
				return ActiveItem.Tag;
			else
				return null;
		}

		public bool CanQueryScript
		{
			get
			{
				TreeViewItem item = ActiveItem;
				if (item != null)
					if (item.Tag is TableSchema || item.Tag is ViewSchema)
						return true;
				return false;
			}
		}

		internal TreeViewItem ActiveItem
		{
			get
			{
				if (this.treeViewDB != null)
				{
					return this.treeViewDB.SelectedItem as TreeViewItem;
				}
				return null;
			}
		}

		private ReadOnlyCollection<IDbSchemaProvider> Providers
		{
			get
			{
				return DatabaseService.Instance.Providers;
			}
		}

		#endregion

		#region ----------------------EVENTS----------------------

		/// <summary>
		/// Respond to dropdown button menu item click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			BindingView.Instance.SelectedDataSource = (DataSource)((MenuItem)sender).DataContext;
			FillTreeView(BindingView.Instance.SelectedDataSource);
		}

		#endregion

		#region ----------------------DATASOURCE COMMANDS----------------------

		/// <summary>
		/// CanExecute for Delete and Refresh data source commands
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void CanExecuteDataSourceCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = this.treeViewDB.Items.Count == 1;
		}

		/// <summary>
		/// Execute for AddDataSource command, display the provider dialog
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void AddDataSourceCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			//load the providers?
			if (Providers.Count > 0)
			{
				//display the dialog
				ProviderChoiceDialog providerDlg = new ProviderChoiceDialog();
				providerDlg.Owner = Application.Current.MainWindow;
				providerDlg.DataContext = Providers;

				//if ok, display the dialog from selected provider
				if (providerDlg.ShowDialog() == true)
				{
					//if ok, create the source
					IDbSourcePanel providerNewSourcePanel = providerDlg.SelectedProvider.GetSourcePanelInterface() as IDbSourcePanel;

					//display the dialog
					NewDataSourceDialog sourceDlg = new NewDataSourceDialog();
					sourceDlg.Owner = Application.Current.MainWindow;
					sourceDlg.SourcePanel = providerNewSourcePanel;

					if (sourceDlg.ShowDialog() == true)
					{
						// will add a new source to the workspace
						AddSource(providerNewSourcePanel.SourceName, providerDlg.SelectedProvider, providerNewSourcePanel.ConnectionString);
					}
				}
			}
			else
				//warning
				MessageBoxDlg.Show(ReflectionStudio.Properties.Resources.MSG_NO_DBPROVIDER,
										ReflectionStudio.Properties.Resources.MSG_TITLE,
										MessageBoxButton.OK,
										MessageBoxImage.Warning);
		}

		/// <summary>
		/// Execute for RefreshDataSource command, clear the whole tree
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void RefreshDataSourceCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			//get the treeview current item, and call the refresh method
			Refresh(((TreeViewItem)this.treeViewDB.Items[0]).Items[0] as TreeViewItem);
		}


		/// <summary>
		/// Execute for RemoveDataSource command, delete the datasource and clear the whole tree
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void RemoveDataSourceCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			RemoveSource(((TreeViewItem)this.treeViewDB.Items[0]).Tag as DataSource);
		}

		/// <summary>
		/// Create a new DataSource with DatabaseService and add it to the workspace
		/// </summary>
		/// <param name="sourceName"></param>
		/// <param name="provider"></param>
		/// <param name="connectionString"></param>
		internal void AddSource(string sourceName, IDbSchemaProvider provider, string connectionString)
		{
			Tracer.Verbose("DatabaseExplorerService:AddSource", "START");

			if (this.treeViewDB == null) return;
			try
			{
				DataSource ds = DatabaseService.Instance.CreateSource(sourceName, provider, connectionString);
				WorkspaceService.Instance.Entity.DataSources.Add(ds);
			}
			catch (Exception err)
			{
				Tracer.Error("DatabaseExplorerService.AddSource", err);
			}
				
			Tracer.Verbose("DatabaseExplorerService:AddSource", "END");
		}

		internal void Refresh(TreeViewItem item)
		{
			Tracer.Verbose("DatabaseExplorerService:Refresh", "START");

			try
			{
				SchemaObjectBase dbObject = (SchemaObjectBase)item.Tag;

				if (dbObject != null)
				{
					dbObject.Refresh();

					item.IsExpanded = false;
					item.Items.Clear();
					this.treeViewDB.AddDummyItem(item); 
				}
			}
			catch (Exception err)
			{
				Tracer.Error("DatabaseExplorerService.Refresh", err);
			}
			finally
			{
				Tracer.Verbose("DatabaseExplorerService:Refresh", "END");
			}
		}

		/// <summary>
		/// Remove a data source. As tree as got just one, clear it and modify workspace
		/// </summary>
		/// <param name="ds"></param>
		internal void RemoveSource(DataSource ds)
		{
			Tracer.Verbose("DatabaseExplorerService:RemoveSource", "START");

			try
			{
				this.treeViewDB.Items.Clear();

				//remove from workspace first so combo will be updated
				WorkspaceService.Instance.Entity.DataSources.Remove(ds);
			}
			catch (Exception err)
			{
				Tracer.Error("DatabaseExplorerService.RemoveSource", err);
			}
			finally
			{
				Tracer.Verbose("DatabaseExplorerService:RemoveSource", "END");
			}
		}

		#endregion

		#region ---------------------NEW EDITOR COMMAMND---------------------

		public void CanExecuteQueryNewEditor(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = true; // this.ComboBoxSource.SelectedItem != null;
		}

		public void QueryNewEditorCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			CreateQueryEditor();
		}

		#endregion

		#region ---------------------CREATE/ALTER/DROP COMMAMND---------------------

		public void CanExecuteModifyQueryScript(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = ActiveItem.Tag != null;
		}

		public void QueryScriptCreateCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			object dbo = this.ActiveDBObject();
			if (dbo != null)
			{
				QueryDocument qd = GetQueryEditor();

				qd.ScriptCreate((SchemaObjectBase)dbo);
			}
		}

		public void QueryQueryScriptAlterCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			object dbo = this.ActiveDBObject();
			if (dbo != null)
			{
				QueryDocument qd = GetQueryEditor();

				qd.ScriptAlter((SchemaObjectBase)dbo);
			}
		}

		public void QueryScriptDropCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			object dbo = this.ActiveDBObject();
			if (dbo != null)
			{
				QueryDocument qd = GetQueryEditor();

				qd.ScriptDrop((SchemaObjectBase)dbo);
			}
		}

		#endregion

		#region ---------------------SELECT/INSERT/UPDATE/DELETE COMMAMND---------------------

		public void CanExecuteQueryScript(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = this.CanQueryScript;
		}

		public void QueryScriptSelectCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			object dbo = this.ActiveDBObject();
			if (dbo is ITabularObjectBase)
			{
				QueryDocument qd = GetQueryEditor();

				qd.ScriptSelect((ITabularObjectBase)dbo);
			}
		}

		public void QueryScriptInsertCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			object dbo = this.ActiveDBObject();
			if (dbo is ITabularObjectBase)
			{
				QueryDocument qd = GetQueryEditor();

				qd.ScriptInsert((ITabularObjectBase)dbo);
			}
		}

		public void QueryScriptUpdateCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			object dbo = this.ActiveDBObject();
			if (dbo is ITabularObjectBase)
			{
				QueryDocument qd = GetQueryEditor();

				qd.ScriptUpdate((ITabularObjectBase)dbo);
			}
		}

		public void QueryScriptDeleteCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			object dbo = this.ActiveDBObject();
			if (dbo is ITabularObjectBase)
			{
				QueryDocument qd = GetQueryEditor();

				qd.ScriptDelete((ITabularObjectBase)dbo);
			}
		}

		#endregion

		#region ---------------------DEPENDENCIES and CONTROL COMMAMND---------------------

		public void CanExecuteQueryViewDependencies(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = e.Parameter != null;
		}

		public void QueryViewDependenciesCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			DependencyDialog Dlg = new DependencyDialog();
			Dlg.Reference = (SchemaObjectBase)e.Parameter;
			Dlg.Owner = Application.Current.MainWindow;
			Dlg.ShowDialog();
		}

		public void CanExecuteQueryControlQuality(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = true;
		}

		public void QueryControlQualityCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			DocumentFactory.Instance.CreateDocument(typeof(XpsReportDocument),
				new XpsGeneratedReportDocumentDataContext()
				{
					Entity = this.SelectedDataSource.Database.Provider.GetSQLVerifierInterface(this.SelectedDataSource.Database.ConnectionString, IDbVerifierOutputMode.eXpsReport)
				});
		}

		#endregion

		#region ----------------TREEVIEW----------------

		public void FillTreeView(DataSource source)
		{
			Tracer.Verbose("DatabaseExplorerService:FillTreeView", source.Name);

			if (this.treeViewDB == null) return;
			try
			{
				this.treeViewDB.Items.Clear();

				//server connection
				TreeViewItem tn = this.treeViewDB.AddItem(null, source.Name, source);
			}
			catch (Exception err)
			{
				Tracer.Error("DatabaseExplorerService.FillTreeView", err);
			}
			finally
			{
				Tracer.Verbose("DatabaseExplorerService:FillTreeView", "END");
			}
		}

		protected void treeViewDB_OnItemNeedPopulate(object sender, System.Windows.RoutedEventArgs e)
		{
			try
			{
				this.Cursor = Cursors.Wait;
				TreeViewItem openedItem = (TreeViewItem)e.OriginalSource;

				if (openedItem.Tag == null)
					FillFolderItems(openedItem);
				else
					FillDBItems(openedItem);
			}
			catch (Exception all)
			{
			}
			finally
			{
				this.Cursor = Cursors.Arrow;
			}
		}

		protected void FillFolderItems(TreeViewItem openedItem)
		{
			if (openedItem.Tag == null)
			{
				if (openedItem.Header.ToString() == "Tables")
				{
					TreeViewItem parent = this.treeViewDB.FindParentNode(openedItem, typeof(DatabaseSchema));

					foreach (TableSchema ts in ((DatabaseSchema)parent.Tag).Tables)
						this.treeViewDB.AddItem(openedItem, ts.Name, ts);

					return;
				}
				if (openedItem.Header.ToString() == "Views")
				{
					TreeViewItem parent = this.treeViewDB.FindParentNode(openedItem, typeof(DatabaseSchema));

					foreach (ViewSchema ts in ((DatabaseSchema)parent.Tag).Views)
						this.treeViewDB.AddItem(openedItem, ts.Name, ts);

					return;
				}
				if (openedItem.Header.ToString() == "Commands")
				{
					TreeViewItem parent = this.treeViewDB.FindParentNode(openedItem, typeof(DatabaseSchema));

					foreach (CommandSchema ts in ((DatabaseSchema)parent.Tag).Commands)
						this.treeViewDB.AddItem(openedItem, ts.Name, ts);

					return;
				}

				if (openedItem.Header.ToString() == "Columns")
				{
					TreeViewItem parent = this.treeViewDB.ParentNode(openedItem);

					if (parent.Tag is TableSchema)
					{
						foreach (ColumnBaseSchema ts in ((TableSchema)parent.Tag).Columns)
							this.treeViewDB.AddItem(openedItem, ts.Name, ts, false);
					}
					else
						if (parent.Tag is ViewSchema)
						{
							foreach (ColumnBaseSchema ts in ((ViewSchema)parent.Tag).Columns)
								this.treeViewDB.AddItem(openedItem, ts.Name, ts, false);
						}

					return;

				}
				if (openedItem.Header.ToString() == "Indexes")
				{
					TreeViewItem parent = this.treeViewDB.FindParentNode(openedItem, typeof(TableSchema));

					foreach (IndexSchema ts in ((TableSchema)parent.Tag).Indexes)
						this.treeViewDB.AddItem(openedItem, ts.Name, ts, false);

					return;
				}
				if (openedItem.Header.ToString() == "Keys")
				{
					TreeViewItem parent = this.treeViewDB.FindParentNode(openedItem, typeof(TableSchema));

					this.treeViewDB.AddItem(openedItem, ((TableSchema)parent.Tag).PrimaryKey.Name, ((TableSchema)parent.Tag).PrimaryKey, false);

					foreach (TableKeySchema ts in ((TableSchema)parent.Tag).Keys)
						this.treeViewDB.AddItem(openedItem, ts.Name, ts, false);

					return;
				}

				if (openedItem.Header.ToString() == "Parameters")
				{
					TreeViewItem parent = this.treeViewDB.FindParentNode(openedItem, typeof(CommandSchema));

					foreach (ParameterSchema ts in ((CommandSchema)parent.Tag).Parameters)
						this.treeViewDB.AddItem(openedItem, ts.Name, ts, false);

					return;
				}
			}
		}

		protected void FillDBItems(TreeViewItem openedItem)
		{
			if (openedItem.Tag != null)
			{
				object dbobject = openedItem.Tag;

				if (dbobject is DataSource)
				{
					//add the database
					this.treeViewDB.AddItem(openedItem, ((DataSource)dbobject).Database.Name, ((DataSource)dbobject).Database);
					return;
				}

				if (dbobject is DatabaseSchema)
				{
					//add the folders
					this.treeViewDB.AddItem(openedItem, "Tables", null);
					this.treeViewDB.AddItem(openedItem, "Views", null);
					this.treeViewDB.AddItem(openedItem, "Commands", null);
					return;
				}

				if (dbobject is TableSchema)
				{
					//add the folders
					this.treeViewDB.AddItem(openedItem, "Columns", null);
					this.treeViewDB.AddItem(openedItem, "Indexes", null);
					this.treeViewDB.AddItem(openedItem, "Keys", null);
					return;
				}

				if (dbobject is ViewSchema)
				{
					//add the folders
					this.treeViewDB.AddItem(openedItem, "Columns", null);
					return;
				}

				if (dbobject is CommandSchema)
				{
					//if (((CommandSchema)openedItem.Tag).Parameters.Count > 0)
					this.treeViewDB.AddItem(openedItem, "Parameters", null);
					return;
				}
			}
		}

		private void treeViewDB_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			BindingView.Instance.PropertyContext = (object)ActiveDBObject();
		}

		#endregion

		#region ----------------INTERNALS----------------

		internal QueryDocument GetQueryEditor()
		{
			if (DocumentFactory.Instance.ActiveDocument is QueryDocument)
				return DocumentFactory.Instance.ActiveDocument as QueryDocument;
			else
				return CreateQueryEditor();
		}

		internal QueryDocument CreateQueryEditor()
		{
			return (QueryDocument)DocumentFactory.Instance.CreateDocument( typeof(QueryDocument), new QueryDocumentDataContext() );
		}

		internal DataSource SelectedDataSource
		{
			get { return BindingView.Instance.SelectedDataSource; }
		}

		#endregion
	}
}
