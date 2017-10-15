using System;
using System.Windows;
using ReflectionStudio.Core.Events;
using System.ComponentModel;
using ReflectionStudio.Classes;

namespace ReflectionStudio.Components.UserControls
{
	/// <summary>
	///TemplateDocument display template content with rich editor
	/// </summary>
	public partial class TemplateDocument : ZoomDocument
	{
		/// <summary>
		/// Contructor
		/// </summary>
		public TemplateDocument()
		{
			InitializeComponent();

			//take the scale zoom from herited class
			this.SyntaxEditor.TextArea.LayoutTransform = ScaleTransformer;
		}

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
				Worker.RunWorkerAsync(base.Context);
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
					if (e.Result != null && e.Result is string)
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
	}
}
