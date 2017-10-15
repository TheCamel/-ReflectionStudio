using System;
using System.Diagnostics;

namespace ReflectionStudio.Core.Events
{
	static public class Tracer
	{
		public static void Error(string from, Exception error)
		{
			Send(MessageEventType.Error, from, error.Message);
			System.Media.SystemSounds.Beep.Play();
		}

		public static void Error(string from, string message)
		{
			Send(MessageEventType.Error, from, message);
			System.Media.SystemSounds.Beep.Play();
		}

		public static void Error(string from, string format, params object[] args)
		{
			Error(from, string.Format(format, args));
		}

		public static void Info(string from, string message)
		{
			Send(MessageEventType.Info, from, message);
		}

		public static void Info(string from, string format, params object[] args)
		{
			Info(from, string.Format(format, args));
		}

		public static void Verbose(string from, string message)
		{
			Send(MessageEventType.Verbose, from, message);
		}

		public static void Verbose(string from, string format, params object[] args)
		{
			Verbose( from, string.Format(format, args));
		}

		private static void Send(MessageEventType typ, string from, string message)
		{
			MessageInfo info = new MessageInfo(typ, from, message);

			//trace is allways used for all levels and can be de-activated in config
			Trace.TraceInformation(info.ToString());
	
			//send message
			EventDispatcher.Instance.RaiseMessage(info);
		}
	}
}