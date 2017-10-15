using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
#if _SPY_DIRECT
using ReflectionStudio.Spy;
#endif

namespace Calibrate.WindowsForms
{
	static class Program
	{
#if _SPY_DIRECT
		static public int BuildKey = 1637340965; 
#endif 

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
#if _LOCAL_PERF
			long ellapsed = LocalTimer.Instance.ElapsedMilliseconds;
#elif _SPY_DIRECT
			Performance.StartEvent(1, BuildKey);
#endif

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());

#if _LOCAL_PERF
				ellapsed = LocalTimer.Instance.ElapsedMilliseconds - ellapsed;
				LocalLogger.Instance.Log("Calibrate.WindowsForms.Main;" + ellapsed.ToString());
#elif _SPY_DIRECT
			Performance.EndEvent();
#endif
		}
	}
}
