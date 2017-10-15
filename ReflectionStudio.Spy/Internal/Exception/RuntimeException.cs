using System;

namespace ReflectionStudio.Spy.Internal
{
	internal class RuntimeException : Exception
	{
		public RuntimeException(string message)
			: base("ReflectionStudio.Spy error: " + message)
		{
			RuntimeLogger.Instance.Log( LogType.Error, this.Message);
		}

		public RuntimeException(string format, params object[] args)
			: base("ReflectionStudio.Spy error: " + string.Format(format, args))
		{
			RuntimeLogger.Instance.Log(LogType.Error, this.Message);
		}

		public RuntimeException(string message, Exception ex)
			: base("ReflectionStudio.Spy error: " + message, ex)
		{
			RuntimeLogger.Instance.Log(LogType.Error, this.Message, ex);
		}
	}
}
