using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Windows.Controls;
using ReflectionStudio.Core.Database;
using ReflectionStudio.Core.Database.Schema;
using ReflectionStudio.Core.Events;
using ReflectionStudio.Classes;

namespace ReflectionStudio.Components.UserControls
{
	/// <summary>
	/// QueryDocument allow to edit, modify and run SQL queries
	/// </summary>
	public partial class QueryDocument : ZoomDocument
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public QueryDocument()
		{
			InitializeComponent();
		}

		#region ---------------------PROPERTIES---------------------

		private enum QueryOutputMode { Grid, Text };
		private QueryOutputMode OutputMode { get; set; }

		static public RoutedCommand CheckQuery = new RoutedCommand("CheckQuery", typeof(QueryDocument));
		static public RoutedCommand ExecuteQuery = new RoutedCommand("ExecuteQuery", typeof(QueryDocument));
		static public RoutedCommand StopExecuteQuery = new RoutedCommand("StopExecuteQuery", typeof(QueryDocument));
		static public RoutedCommand OutputQueryMode = new RoutedCommand("OutputQueryMode", typeof(QueryDocument));

		#endregion

		#region ---------------------LOAD / SAVE---------------------

		/// <summary>
		/// Start a thread to load the document
		/// </summary>
		override internal void LoadDocument()
		{
			Tracer.Verbose("TemplateDocument:LoadDocument", "START");

			try
			{
				base.LoadDocument();

				this.SyntaxEditor.TextArea.LayoutTransform = ScaleTransformer;

				Worker.DoWork += new DoWorkEventHandler(bw_DoWork);
				Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

				// Start the asynchronous operation.
				if (!string.IsNullOrEmpty(Context.FullName))
					Worker.RunWorkerAsync(base.Context);
				else
					WaitPanel.Stop();
			}
			catch (Exception all)
			{
				Tracer.Error("TemplateDocument.LoadDocument", all);
			}
			finally
			{
				Tracer.Verbose("TemplateDocument:LoadDocument", "END");
			}
		}

		/// <summary>
		/// Threaded function for document loading
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void bw_DoWork(object sender, DoWorkEventArgs e)
		{
			Tracer.Verbose("TemplateDocument:bw_DoWork", "START");
			try
			{
				IDocumentDataContext ctx = (IDocumentDataContext)e.Argument;
				e.Result = ctx.Load();
			}
			catch (Exception all)
			{
				Tracer.Error("TemplateDocument.bw_DoWork", all);
			}
			finally
			{
				Tracer.Verbose("TemplateDocument:bw_DoWork", "END");
			}
		}

		/// <summary>
		/// Thread completed management
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			// First, handle the case where an exception was thrown.
			if (e.Error != null)
			{
				Tracer.Error("TemplateDocument.bw_RunWorkerCompleted", e.Error);
				return;
			}
			else
			{
				Tracer.Verbose("TemplateDocument:bw_RunWorkerCompleted", "START");

				try
				{
					if( e.Result != null )
						this.SyntaxEditor.Text = e.Result as string;
					
					WaitPanel.Stop();
				}
				catch (Exception all)
				{
					Tracer.Error("TemplateDocument.bw_RunWorkerCompleted", all);
				}
				finally
				{
					Tracer.Verbose("TemplateDocument:bw_RunWorkerCompleted", "END");
				}
			}
		}

		/// <summary>
		/// Save the content of the rich editor
		/// </summary>
		override internal void SaveDocument()
		{
			Tracer.Verbose("TemplateDocument:SaveDocument", "START");
			try
			{
				Context.Save(this.SyntaxEditor.Text);
			}
			catch (Exception all)
			{
				Tracer.Error("TemplateDocument.SaveDocument", all);
			}
			finally
			{
				Tracer.Verbose("TemplateDocument:SaveDocument", "END");
			}
		}

		#endregion

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				SetOutputMode( QueryOutputMode.Grid );
				SyntaxEditor.TextArea.LayoutTransform = base.ScaleTransformer;
				//this.textblockSource.Text = DataSource.Name;
				this.MainGrid.RowDefinitions[2].Height = new GridLength(0);

				CommandBindings.Add(new CommandBinding(CheckQuery, CheckQueryCommandHandler, CanExecuteQueryCommand));
				CommandBindings.Add(new CommandBinding(ExecuteQuery, ExecuteQueryCommandHandler, CanExecuteQueryCommand));
				CommandBindings.Add(new CommandBinding(StopExecuteQuery, StopQueryCommandHandler, CanExecuteStopQuery));
				CommandBindings.Add(new CommandBinding(OutputQueryMode, OutputQueryModeCommandHandler, CanExecuteOutputQueryMode));
			}
		}

		#region ---------------------COMMAMND HANDLERS---------------------

		/// <summary>
		/// CanExecute command handler for executing query
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void CanExecuteQueryCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = !string.IsNullOrEmpty(SyntaxEditor.Text);
		}

		/// <summary>
		/// Execute command handler for checking query
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void CheckQueryCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			string checkQuery = string.Format("SET PARSEONLY ON {0}; SET PARSEONLY OFF;", SyntaxEditor.Text);
			try
			{
				BindingView.Instance.SelectedDataSource.Database.Provider.GetSQLQueryInterface().ExecuteData(BindingView.Instance.SelectedDataSource.ConnectionString, checkQuery);
			}
			catch (Exception all)
			{
			}
		}

		private BackgroundWorker _QueryWorker = null;

		/// <summary>
		///  Execute command handler for executing query
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void ExecuteQueryCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			//execute the thread that run the query on the database as ExecuteReader is blocking
			try
			{
				//display the context options 
				ShowResultPane();
				this.dataGridResult.Columns.Clear();
				
				//display the progress, 
				EventDispatcher.Instance.RaiseStatus("Executing query !", StatusEventType.StartProgress);

				QueryContext context = new QueryContext()
					{
						UIDispatcher = this.Dispatcher,
						Command = SyntaxEditor.Text,
						Source = BindingView.Instance.SelectedDataSource,
						StartTime = DateTime.Now
					};

				if (OutputMode == QueryOutputMode.Grid)
				{
					context.AddColumn = new QueryContext.AddColumnDelegate(GridAddColumnHandler);
					context.AddData = new QueryContext.AddRowDelegate(GridAddDataHandler);
				}
				else
				{
					context.AddColumn = new QueryContext.AddColumnDelegate(TextAddColumnHandler);
					context.AddData = new QueryContext.AddRowDelegate(TextAddDataHandler);
				}

				// Start the asynchronous operation.
				_QueryWorker = new BackgroundWorker();
				_QueryWorker.WorkerSupportsCancellation = true;
				_QueryWorker.DoWork += new DoWorkEventHandler(bw_RequestWork);
				_QueryWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RequestCompleted);
				_QueryWorker.ProgressChanged += new ProgressChangedEventHandler(bw_RequestProgressChanged);

				_QueryWorker.RunWorkerAsync(context);
			}
			catch (Exception all)
			{
			}
		}

		public void CanExecuteStopQuery(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = (_QueryWorker != null);
		}

		public void StopQueryCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			// Cancel the asynchronous operation.
			_QueryWorker.CancelAsync();
			_QueryWorker = null;
		}

		public void CanExecuteOutputQueryMode(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = true;
		}

		public void OutputQueryModeCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			SetOutputMode( e.Parameter .ToString()== "Grid" ? QueryOutputMode.Grid : QueryOutputMode.Text );
		}

		private void SetOutputMode(QueryOutputMode mode)
		{
			OutputMode = mode;

			if (OutputMode == QueryOutputMode.Grid)
			{
				(this.tabControlResult.Items[0] as System.Windows.Controls.TabItem).Visibility = System.Windows.Visibility.Visible;
				(this.tabControlResult.Items[1] as System.Windows.Controls.TabItem).Visibility = System.Windows.Visibility.Hidden;
			}
			else
			{
				(this.tabControlResult.Items[0] as System.Windows.Controls.TabItem).Visibility = System.Windows.Visibility.Hidden;
				(this.tabControlResult.Items[1] as System.Windows.Controls.TabItem).Visibility = System.Windows.Visibility.Visible;
			}
		}
		#endregion

		#region ---------------------REQUEST WORKER---------------------

		protected void GridAddColumnHandler(string headerText)
		{
			if (this.dataGridResult.ItemsSource == null)
				this.dataGridResult.ItemsSource = new ObservableCollection<dynamic>();

			DataGridTextColumn dt = new DataGridTextColumn();
			dt.Header = headerText;
			dt.Binding = new Binding(headerText);
			this.dataGridResult.Columns.Add(dt);
		}

		protected void GridAddDataHandler(dynamic data)
		{
			(this.dataGridResult.ItemsSource as ObservableCollection<dynamic>).Add(data);
		}

		protected void TextAddColumnHandler(string header)
		{
			this.textBlockResult.Text += header;
			this.textBlockResult.Text += ";";
		}

		protected void TextAddDataHandler(dynamic data)
		{
			foreach (var property in (IDictionary<String, Object>)data)
			{
				this.textBlockResult.Text += property.Value.ToString();
				this.textBlockResult.Text += ";";
			}
			this.textBlockResult.Text += "\n";
		}

		protected void bw_RequestWork(object sender, DoWorkEventArgs e)
		{
			QueryWorker qw = new QueryWorker((BackgroundWorker)sender, e);
			qw.ExecuteQuery();
			e.Result = qw.Context;
		}

		protected void bw_RequestProgressChanged(object sender, ProgressChangedEventArgs e)
		{
		}

		protected void bw_RequestCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			// First, handle the case where an exception was thrown.
			if (e.Error != null)
			{
				EventDispatcher.Instance.RaiseStatus("Query error !", StatusEventType.StopProgress);
			}
			else if (e.Cancelled)
			{
				// Next, handle the case where the user canceled the operation.
				// Note that due to a race condition in the DoWork event handler, the Cancelled
				// flag may not have been set, even though CancelAsync was called.
				EventDispatcher.Instance.RaiseStatus("Query canceled !", StatusEventType.StopProgress);
			}
			else if (e.Result != null)
			{
				// Finally, handle the case where the operation succeeded.
				EventDispatcher.Instance.RaiseStatus("Query executed !", StatusEventType.StopProgress);

				this.Dispatcher.Invoke((ThreadStart)delegate
				{
					QueryContext qc = e.Result as QueryContext;
					this.textblockMessage.Text = qc.Message;
					this.textblockTime.Text = (qc.StartTime - DateTime.Now).ToString(@"hh\:mm\:ss\:ff");
					this.textblockLines.Text = qc.LineCount.ToString();
				});
			}

			_QueryWorker = null;
		}

		#endregion

		#region ---------------------SCRIPTING---------------------

		public void ScriptCreate(SchemaObjectBase dbo)
		{
			IDbScriptWriter writer = BindingView.Instance.SelectedDataSource.Database.Provider.GetSQLWriterInterface();
			SyntaxEditor.Text = writer.Create(dbo);
		}

		public void ScriptAlter(SchemaObjectBase dbo)
		{
			IDbScriptWriter writer = BindingView.Instance.SelectedDataSource.Database.Provider.GetSQLWriterInterface();
			SyntaxEditor.Text = writer.Alter(dbo);
		}

		public void ScriptDrop(SchemaObjectBase dbo)
		{
			IDbScriptWriter writer = BindingView.Instance.SelectedDataSource.Database.Provider.GetSQLWriterInterface();
			SyntaxEditor.Text = writer.Drop(dbo);
		}

		public void ScriptSelect(ITabularObjectBase dbo)
		{
			IDbScriptWriter writer = BindingView.Instance.SelectedDataSource.Database.Provider.GetSQLWriterInterface();
			SyntaxEditor.Text = writer.Select(dbo);
		}
		public void ScriptInsert(ITabularObjectBase dbo)
		{
			IDbScriptWriter writer = BindingView.Instance.SelectedDataSource.Database.Provider.GetSQLWriterInterface();
			SyntaxEditor.Text = writer.Insert(dbo);
		}
		public void ScriptUpdate(ITabularObjectBase dbo)
		{
			IDbScriptWriter writer = BindingView.Instance.SelectedDataSource.Database.Provider.GetSQLWriterInterface();
			SyntaxEditor.Text = writer.Update(dbo);
		}
		public void ScriptDelete(ITabularObjectBase dbo)
		{
			IDbScriptWriter writer = BindingView.Instance.SelectedDataSource.Database.Provider.GetSQLWriterInterface();
			SyntaxEditor.Text = writer.Delete(dbo);
		}
		#endregion

		#region ---------------------INTERNALS---------------------

		private void ShowResultPane()
		{
			this.MainGrid.RowDefinitions[2].Height = new GridLength( 1, GridUnitType.Star);
		}

		#endregion
	}
}
