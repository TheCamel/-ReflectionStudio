using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using Calibrate.Core;
#if _SPY_DIRECT
using ReflectionStudio.Spy;
#endif

namespace Calibrate.WindowsForms
{
	public partial class Form1 : Form
	{
		public Form1()
		{
#if _LOCAL_PERF
			long ellapsed = LocalTimer.Instance.ElapsedMilliseconds;
#elif _SPY_DIRECT
			Performance.StartEvent(2, Program.BuildKey);
#endif
			InitializeComponent();

#if _LOCAL_PERF
				ellapsed = LocalTimer.Instance.ElapsedMilliseconds - ellapsed;
				LocalLogger.Instance.Log("NPerf.Calibrate.WinForm.Form1.ctor;" + ellapsed.ToString());
#elif _SPY_DIRECT
				Performance.EndEvent();
#endif
		}

		private void Form1_Load(object sender, EventArgs e)
		{
#if _LOCAL_PERF
			            long ellapsed = LocalTimer.Instance.ElapsedMilliseconds;
#elif _SPY_DIRECT
			Performance.StartEvent(3, Program.BuildKey);
#endif
			try
			{
				//-------------------------------------------------

				//backgroundWorker1.RunWorkerAsync();

				//-------------------------------------------------

				Processing proc = new Processing();
				proc.Test_ThreadSleep();

				//-------------------------------------------------

				proc.RunningProcess(1000);

				for (double i = 0; i < 1000; i += 100)
				{
					//faire un calcul
					double test = i * 12.0213 + i / 0.1;
				}

				//-------------------------------------------------

				proc.RunningProcess(1000);

				//-------------------------------------------------

				for (double i = 0; i < 1000; i += 100)
				{
					//faire un calcul
					double test = i * 12.0213 + i / 0.1;
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
			                LocalLogger.Instance.Log("NPerf.Calibrate.WinForm.Form1_Load;" + ellapsed.ToString());
#elif _SPY_DIRECT
			                Performance.EndEvent();
#endif
			}
		}

		private void btnTest_Click(object sender, EventArgs e)
		{
#if _LOCAL_PERF
			                long ellapsed = LocalTimer.Instance.ElapsedMilliseconds;
#elif _SPY_DIRECT
			Performance.StartEvent(4, Program.BuildKey);
#endif

			backgroundWorker1.RunWorkerAsync();

#if _LOCAL_PERF
			                ellapsed = LocalTimer.Instance.ElapsedMilliseconds - ellapsed;
			                LocalLogger.Instance.Log("NPerf.Calibrate.WinForm.btnTest_Click;" + ellapsed.ToString());
#elif _SPY_DIRECT
			                Performance.EndEvent();
#endif
		}

		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
#if _LOCAL_PERF
			                long ellapsed = LocalTimer.Instance.ElapsedMilliseconds;
#elif _SPY_DIRECT
			Performance.StartEvent(5, Program.BuildKey);
#endif
			Processing proc = new Processing();
			proc.RunningProcess(100);

#if _LOCAL_PERF
			                ellapsed = LocalTimer.Instance.ElapsedMilliseconds - ellapsed;
			                LocalLogger.Instance.Log("NPerf.Calibrate.WinForm.btnTest_Click;" + ellapsed.ToString());
#elif _SPY_DIRECT
			                Performance.EndEvent();
#endif
		}

		private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
		}
	}
}
