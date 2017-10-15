using System;
using System.Windows;
using System.Windows.Xps.Packaging;
using ReflectionStudio.Core.Events;
using System.ComponentModel;
using ReflectionStudio.Classes;

namespace ReflectionStudio.Components.UserControls
{
	/// <summary>
	/// HelpDocument display xps help documents
	/// </summary>
	public partial class HelpDocument : DocumentBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public HelpDocument()
		{
			InitializeComponent();
		}

		#region ---------------------LOAD / SAVE---------------------

		/// <summary>
		/// Start a thread to load the document
		/// </summary>
		override internal void LoadDocument()
		{
			Tracer.Verbose("HelpDocument:LoadDocument", "START");

			try
			{
				base.LoadDocument();

				Worker.DoWork += new DoWorkEventHandler(bw_DoWork);
				Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

				// Start the asynchronous operation.
				Worker.RunWorkerAsync(base.Context);
			}
			catch (Exception all)
			{
				Tracer.Error("HelpDocument.LoadDocument", all);
			}
			finally
			{
				Tracer.Verbose("HelpDocument:LoadDocument", "END");
			}
		}

		/// <summary>
		/// Threaded loading
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void bw_DoWork(object sender, DoWorkEventArgs e)
		{
			Tracer.Verbose("HelpDocument:bw_DoWork", "START");
			try
			{
				IDocumentDataContext ctx = (IDocumentDataContext)e.Argument;
				e.Result = ctx.Load();
			}
			catch (Exception all)
			{
				Tracer.Error("HelpDocument.bw_DoWork", all);
			}
			finally
			{
				Tracer.Verbose("HelpDocument:bw_DoWork", "END");
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
				Tracer.Error("HelpDocument.bw_RunWorkerCompleted", e.Error);
			}
			else
			{
				Tracer.Verbose("HelpDocument:bw_RunWorkerCompleted", "START");
				try
				{
					if (e.Result != null && e.Result is XpsDocument)
					{
						XpsDocument xps = (XpsDocument)e.Result;
						documentViewer1.Document = xps.GetFixedDocumentSequence();
						ContentData = xps;
					}
					WaitPanel.Stop();
				}
				catch (Exception all)
				{
					Tracer.Error("HelpDocument.bw_RunWorkerCompleted", all);
				}
				finally
				{
					Tracer.Verbose("HelpDocument:bw_RunWorkerCompleted", "END");
				}
			}
		}
		#endregion
	}
}
