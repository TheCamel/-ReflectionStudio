using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ReflectionStudio.Core.Events;

namespace ReflectionStudio.Controls.Helpers
{
	/// <summary>
	/// Class that manage a long operation by changing the cursor and start/stop the progressbar
	/// Need to be used in a using statement
	/// </summary>
	public class LongOperation : IDisposable
	{
		/// <summary>
		/// Start a long operation : change the cursor and send a progress start if needed
		/// </summary>
		/// <param name="element">the element to superseed</param>
		/// <param name="message">the progress message to send</param>
		public LongOperation(Control element, string message = null)
        {
			_element = element;
			_element.Cursor = Cursors.Wait;

			if( !string.IsNullOrEmpty(message) )
				EventDispatcher.Instance.RaiseStatus(message, StatusEventType.StartProgress);
        }

		#region ---------------------IDisposable---------------------

		/// <summary>
		/// Override the dispose to change the cursor back to arrow and raise a stop progress automatically
		/// </summary>
		public void Dispose()
        {
			_element.Cursor = Cursors.Arrow;
			EventDispatcher.Instance.RaiseStatus("", StatusEventType.StopProgress);
        }

        #endregion

		/// <summary>
		/// The control element that we superseed
		/// </summary>
		private Control _element;
	}
}
