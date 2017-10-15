using System;

namespace ReflectionStudio.Core.Events
{
	/// <summary>
	/// The event arguments associated to all messages
	/// </summary>
	public class MessageEventArgs : EventArgs
	{
		public MessageEventArgs()
		{
		}

		public MessageEventArgs(MessageInfo info)
		{
			_Info = info;
		}

		public MessageEventArgs(MessageEventType type)
		{
			_Info.Type = type;
		}

		public MessageEventArgs(string message)
		{
			_Info.Details = message;
		}

		public MessageEventArgs(MessageEventType type, string message)
		{
			_Info.Type = type;
			_Info.Details = message;
		}

		private MessageInfo _Info = new MessageInfo();
		public MessageInfo Info
		{
			get { return _Info; }
			set { _Info = value; }
		}
	}
}
