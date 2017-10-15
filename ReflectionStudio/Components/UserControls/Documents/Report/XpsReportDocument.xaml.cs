using System;
using System.ComponentModel;
using System.Windows.Xps.Packaging;
using ReflectionStudio.Classes;
using ReflectionStudio.Core.Events;

namespace ReflectionStudio.Components.UserControls
{
	/// <summary>
	/// XpsReportDocument display any xps Document
	/// </summary>
	public partial class XpsReportDocument : DocumentBase
	{
		/// <summary>
		/// Construtor
		/// </summary>
		public XpsReportDocument()
		{
			InitializeComponent();
		}

		#region ---------------------LOAD / SAVE---------------------
		
		/// <summary>
		/// Start a thread to load the document
		/// </summary>
		override internal void LoadDocument()
		{
			Tracer.Verbose("XpsReportDocument:LoadDocument", "START");

			try
			{
				base.LoadDocument();

				Worker.DoWork += new DoWorkEventHandler(bw_DoWork);
				Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

				// Start the asynchronous operation.
				if (!string.IsNullOrEmpty(Context.FullName) || Context.Entity != null) 
					Worker.RunWorkerAsync(base.Context);
				else
					WaitPanel.Stop();
			}
			catch (Exception all)
			{
				Tracer.Error("XpsReportDocument.LoadDocument", all);
			}
			finally
			{
				Tracer.Verbose("XpsReportDocument:LoadDocument", "END");
			}
		}

		/// <summary>
		/// Threaded loading
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void bw_DoWork(object sender, DoWorkEventArgs e)
		{
			Tracer.Verbose("XpsReportDocument:bw_DoWork", "START");
			try
			{
				IDocumentDataContext ctx = (IDocumentDataContext)e.Argument;
				e.Result = ctx.Load();
			}
			catch (Exception all)
			{
				Tracer.Error("XpsReportDocument.bw_DoWork", all);
			}
			finally
			{
				Tracer.Verbose("XpsReportDocument:bw_DoWork", "END");
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
				Tracer.Error("XpsReportDocument.bw_RunWorkerCompleted", e.Error);
			}
			else
			{
				Tracer.Verbose("XpsReportDocument:bw_RunWorkerCompleted", "START");
				try
				{
					XpsDocument xps = (XpsDocument)e.Result;
					documentViewer.Document = xps.GetFixedDocumentSequence();
					ContentData = xps;

					WaitPanel.Stop();
				}
				catch (Exception all)
				{
					Tracer.Error("XpsReportDocument.bw_RunWorkerCompleted", all);
				}
				finally
				{
					Tracer.Verbose("XpsReportDocument:bw_RunWorkerCompleted", "END");
				}
			}
		}


		/// <summary>
		/// Save the content we get before in the load method as XpsDocument
		/// </summary>
		override internal void SaveDocument()
		{
			Tracer.Verbose("XpsReportDocument:SaveDocument", "START");
			try
			{
				Context.Save(base.ContentData);
			}
			catch (Exception all)
			{
				Tracer.Error("XpsReportDocument.SaveDocument", all);
			}
			finally
			{
				Tracer.Verbose("XpsReportDocument:SaveDocument", "END");
			}
		}
		#endregion
	}
}
