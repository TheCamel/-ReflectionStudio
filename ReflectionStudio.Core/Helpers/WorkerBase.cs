using System.ComponentModel;
using System.Windows.Threading;
using System;

namespace ReflectionStudio.Core.Helpers
{
	/// <summary>
	/// WorkerBase help in the management of a BackgroundWorker
	/// </summary>
	public class WorkerBase
	{
		#region ----------------------INTERNALS----------------------

		/// <summary>
		/// The thread we are running in, but it can be null
		/// </summary>
		public BackgroundWorker Worker
		{
			get;
			set;
		}

		/// <summary>
		/// Thread event arg associated to the background worker, can be null
		/// </summary>
		public DoWorkEventArgs Event
		{
			get;
			set;
		}

		/// <summary>
		/// Thread result as boolean stored in the event
		/// </summary>
		public bool Result
		{
			get { if (Event != null) return (bool)Event.Result; else return false; }
			set { Event.Result = value; }
		}

		#endregion

		#region ----------------------CONSTRUCTORS----------------------

		/// <summary>
		/// Contructor
		/// </summary>
		/// <param name="worker"></param>
		/// <param name="e"></param>
		public WorkerBase(BackgroundWorker worker, DoWorkEventArgs e)
		{
			Worker = worker;
			Event = e;
			//init to ok
			Event.Result = true;
		}

		#endregion

		#region ----------------------HELPERS----------------------
		/// <summary>
		/// Send progress mesage through the background worker
		/// </summary>
		/// <param name="message"></param>
		protected void Progress(string message)
		{
			if (Worker != null)
				Worker.ReportProgress(0, message);

			//Thread.Sleep(10);
		}

		/// <summary>
		/// Check if cancelling has been asked
		/// </summary>
		/// <returns></returns>
		protected bool CancelPending()
		{
			if (Worker == null)
				return false;

			if (Worker.CancellationPending)
			{
				Event.Cancel = true;
				return true;
			}
			else return false;
		}

		#endregion
	}
}
