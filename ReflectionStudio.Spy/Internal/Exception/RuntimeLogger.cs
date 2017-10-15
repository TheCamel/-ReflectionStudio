using System;
using System.Text;
using System.IO;

namespace ReflectionStudio.Spy.Internal
{
	internal enum LogType
	{
		None = 0,
		Trace = 1,
		Info = 2,
		Warning = 3,
		Error = 4
	}

	internal class RuntimeLogger
	{
		// Fields
		private object _Lock = new object();
		private string _LogFilename;


		#region ----------------SINGLETON----------------
		public static readonly RuntimeLogger Instance = new RuntimeLogger();

		/// <summary>
		/// Private constructor for singleton pattern
		/// </summary>
		private RuntimeLogger()
		{
			_LogFilename = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace(".dll", string.Format("_{0}.log", DateTime.Now.ToString("yyyyMMdd-HHmmss")));
		}
		#endregion

		#region ----------------PROPERTIES----------------

		private LogType _LogType = LogType.Info;
		public LogType LogType
		{
			get
			{
				return _LogType;
			}
			set
			{
				_LogType = value;
			}
		}
		#endregion

		#region ----------------INTERNAL FUNCTIONS----------------

		internal string GetLogType(LogType level)
		{
			if( level == LogType.Trace )
				return "[TRACE]";

			if( level == LogType.Info )
				return "[INFO]";

			if( level == LogType.Warning )
				return "[WARNING]";

			if( level == LogType.Error )
				return "[ERROR]";
			
			return "[!]";
		}

		internal void LogEvent(LogType level, string message)
		{
			lock (_Lock)
			{
				using ( TextWriter writer = File.AppendText(_LogFilename) )
				{
					writer.WriteLine(string.Format("{0} {1}: {2}", DateTime.Now.ToString(), GetLogType(level), message));
				}
			}
		}
		#endregion 

		#region ----------------PUBLIC FUNCTIONS----------------

		public void Log(LogType logType, string text)
		{
			if (logType <= _LogType)
			{
				try
				{
					this.LogEvent(LogType.Trace, text);
				}
				catch (Exception error)
				{
					throw new Exception("Log error in ReflectionStudio.Spy", error);
				}
			}
		}

		public void Log(LogType logType, string format, params object[] args)
		{
			if (logType <= _LogType)
			{
				this.Log(logType, string.Format(format, args));
			}
		}
		#endregion
	}
}
