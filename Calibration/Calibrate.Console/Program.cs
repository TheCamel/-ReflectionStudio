using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using Calibrate.Core;

namespace Calibrate.Console
{
	class Program
	{
		static void Main(string[] args)
		{
#if _LOCAL_PERF
			long ellapsed = LocalTimer.Instance.ElapsedMilliseconds;
#elif _SPY
			Performance.StartEvent(1, "Calibrate.Main");
#endif

			System.ComponentModel.BackgroundWorker backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(backgroundWorker1_DoWork);
			backgroundWorker1.RunWorkerAsync();

			RunConsoleProcess();

#if _LOCAL_PERF
			ellapsed = LocalTimer.Instance.ElapsedMilliseconds - ellapsed;
			LocalLogger.Instance.Log("Calibrate.Main;" + ellapsed.ToString());
#elif _SPY
			Performance.EndEvent();
#endif		
		}

		static void RunConsoleProcess()
		{
#if _LOCAL_PERF
			long ellapsed = LocalTimer.Instance.ElapsedMilliseconds;
#elif _SPY
			Performance.StartEvent(3, "Calibrate.RunConsoleProcess");
#endif
			try
			{
				//-------------------------------------------------

				Processing proc = new Processing();
				proc.Test_ThreadSleep();

				//-------------------------------------------------

				proc.RunningProcess(1);

				//-------------------------------------------------

				proc.RunningProcess(10);

				//-------------------------------------------------

				proc.RunningProcess(100);

				for (double i = 0; i < 10000; i += 1000)
				{
					//faire un calcul
					double test = i * 12.0213 + i / 0.1;
					Thread.Sleep(1);
				}

				//-------------------------------------------------

				proc.RunningProcess(1000);

				for (double i = 0; i < 10000; i += 1000)
				{
					//faire un calcul
					double test = i * 12.0213 + i / 0.1;
					Thread.Sleep(1);
				}

				//-------------------------------------------------

			}
			catch (Exception)
			{
			}
			finally
			{
#if _LOCAL_PERF
				ellapsed = LocalTimer.Instance.ElapsedMilliseconds - ellapsed;
				LocalLogger.Instance.Log("Calibrate.RunConsoleProcess;" + ellapsed.ToString());
#elif _SPY
				Performance.EndEvent();
#endif
			}
		}

		static void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			Processing proc = new Processing();
			proc.RunningProcess(10);
		}
	}
}
