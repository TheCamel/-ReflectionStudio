using System;
using System.IO;
using System.Reflection;

namespace Calibrate.Core
{
	public class LocalLogger
	{
		private string _LogFile;

		#region ----------------SINGLETON----------------
		public static readonly LocalLogger Instance = new LocalLogger();

		/// <summary>
		/// Private constructor for singleton pattern
		/// </summary>
		private LocalLogger()
		{
			_LogFile = Assembly.GetEntryAssembly().Location.Replace(".exe", ".log");
		}
		#endregion

		public void Log(string message)
		{
			FileStream fs = null;
			StreamWriter sw = null;

			try
			{
				fs = File.Open(_LogFile, FileMode.Append);

				using (sw = new StreamWriter(fs))
				{
					sw.WriteLine(message);
				}
			}
			catch (Exception err)
			{
				throw new Exception("Logger.Log", err);
			}
			finally
			{
				if (fs != null) fs.Close();
			}
		}
	}
}
