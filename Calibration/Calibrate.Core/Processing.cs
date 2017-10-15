using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Calibrate.Core
{
	public class Processing
	{
		public void Test_ThreadSleep()
		{
#if _LOCAL_PERF
			long ellapsed = LocalTimer.Instance.ElapsedMilliseconds;
#elif _SPY_DIRECT
			Performance.StartEvent(6, "NPerf.Calibrate.Core.Test_ThreadSleep");
#endif
			try
			{
				Thread.Sleep(100);
			}
			catch (Exception)
			{
			}
			finally
			{
#if _LOCAL_PERF
				ellapsed = LocalTimer.Instance.ElapsedMilliseconds - ellapsed;
				LocalLogger.Instance.Log("NPerf.Calibrate.Core.Test_ThreadSleep;" + ellapsed.ToString());
#elif _SPY_DIRECT
				Performance.EndEvent();
#endif
			}
		}

		public void RunningProcess(int param)
		{
#if _LOCAL_PERF
			long ellapsed = LocalTimer.Instance.ElapsedMilliseconds;
#elif _SPY_DIRECT
			Performance.StartEvent(7, "NPerf.Winform.Test.Process.RunningProcess");
#endif
			try
			{
				for (double i = 0; i < 10000; i += param)
				{
					//faire un calcul
					double test = i * 12.0213 + i / 0.1;
					Thread.Sleep(1);
				}
			}
			catch (Exception)
			{
			}
			finally
			{
#if _LOCAL_PERF
				ellapsed = LocalTimer.Instance.ElapsedMilliseconds - ellapsed;
				LocalLogger.Instance.Log("NPerf.Winform.Test.Process.RunningProcess;" + ellapsed.ToString());
#elif _SPY_DIRECT
				Performance.EndEvent();
#endif
			}
		}
	}
}
