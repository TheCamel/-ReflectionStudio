using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using AvalonDock;
using ReflectionStudio.Classes;
using ReflectionStudio.Controls;
using ReflectionStudio.Core.Events;

namespace ReflectionStudio.Components.UserControls
{
	/// <summary>
	/// DocumentBase is the based class for all Document user controls
	/// </summary>
	public class DocumentBase : DocumentContent
	{
		#region ----------------CONSTRUCTOR----------------

		/// <summary>
		/// Constructor
		/// </summary>
		public DocumentBase()
        {
        }

		/// <summary>
		/// Plug the load event
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			this.Loaded += new RoutedEventHandler(DocumentBase_Loaded);
		}
		
		#endregion

		#region ----------------PROPERTIES----------------
		
		/// <summary>
		/// Document data context
		/// </summary>
		public IDocumentDataContext Context
		{
			get { return this.DataContext as IDocumentDataContext; }
			set { this.DataContext = value; }
		}

		/// <summary>
		/// Content data
		/// </summary>
		public object ContentData { get; set; }

		/// <summary>
		/// Flag property that indicate if the document content is loaded
		/// </summary>
		public bool IsLoaded { get; set; }

		/// <summary>
		/// BackgroundWorker variable
		/// </summary>
		protected BackgroundWorker _Worker = null;

		/// <summary>
		/// Internal BackgroundWorker that can be used for document loading
		/// </summary>
		protected BackgroundWorker Worker
		{
			get
			{
				if (_Worker == null)
				{
					_Worker = new BackgroundWorker();
					_Worker.WorkerReportsProgress = false;
					_Worker.WorkerSupportsCancellation = false;
					
				}
				return _Worker;
			}
		}

		#endregion

		#region ---------------------LOAD / SAVE---------------------

		virtual internal void LoadDocument()
		{
			this.IsLoaded = true;
		}

		virtual internal void SaveDocument()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ----------------EVENTS----------------

		void DocumentBase_Loaded(object sender, RoutedEventArgs e)
		{
			Tracer.Verbose("DocumentBase:DocumentBase_Loaded", "START");

			try
			{
				if (!IsLoaded)
					LoadDocument();
			}
			catch (Exception all)
			{
				Tracer.Error("DocumentBase.DocumentBase_Loaded", all);
			}
			finally
			{
				Tracer.Verbose("DocumentBase:DocumentBase_Loaded", "END");
			}
		}
		
		/// <summary>
		/// Manage the closing, for exemple saving dirty document
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			//document never saved
			if (string.IsNullOrEmpty(Context.FullName))
			{
				MessageBoxResult answer = MessageBoxDlg.Show(
													ReflectionStudio.Properties.Resources.MSG_PRJ_ASK_SAVE,
													ReflectionStudio.Properties.Resources.MSG_TITLE,
													MessageBoxButton.YesNoCancel,
													MessageBoxImage.Question);

				if (answer == MessageBoxResult.Yes)
					ApplicationCommands.Save.Execute(this, Application.Current.MainWindow);

				if (answer == MessageBoxResult.Cancel)
					e.Cancel = true;
			}

			//document is dirty ??

			base.OnClosing(e);
		}

		#endregion
	}
}