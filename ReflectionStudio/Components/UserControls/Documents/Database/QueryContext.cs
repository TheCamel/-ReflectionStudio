using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReflectionStudio.Core.Database;
using ReflectionStudio.Core;
using Microsoft.Windows.Controls;
using System.Windows.Threading;

namespace ReflectionStudio.Components.UserControls
{
	internal class QueryContext
	{
		public delegate void AddColumnDelegate(string headerText);
		public delegate void AddRowDelegate(dynamic data);

		#region ---------------------SOURCE INFO---------------------

		public Dispatcher UIDispatcher { get; set; }
		public DataSource Source { get; set; }
		public string Command { get; set; }

		public AddColumnDelegate AddColumn { get; set; }
		public AddRowDelegate AddData { get; set; }
		#endregion

		#region ---------------------RESULTS---------------------
		
		public string Message { get; set; }
		public int LineCount { get; set; }
		public DateTime StartTime { get; set; }
		public bool HasResult { get; set; }

		#endregion
	}
}
