using System;

namespace ReflectionStudio.Core.Events
{
	/// <summary>
	/// Define the message event type and also the log level
	/// </summary>
	public enum MessageEventType
	{
		/// <summary>
		/// Type used for no log but not in messaging system
		/// </summary>
		None = 0,
		/// <summary>
		/// Only in case of exception
		/// </summary>
		Error = 1,
		/// <summary>
		/// Used by all messages
		/// </summary>
		Info = 2,
		/// <summary>
		/// Used by trace messages
		/// </summary>
		Verbose = 3
	}

	/// <summary>
	/// Classe used to store message information which are mapped to the log grid
	/// </summary>
	public class MessageInfo
	{
		#region ----------------PROPERTIES----------------

		private MessageEventType _Type = MessageEventType.Info;
		/// <summary>
		/// Type of message = the log level
		/// </summary>
		public MessageEventType Type
		{
			get { return _Type; }
			set { _Type = value; }
		}

		private DateTime _When = DateTime.Now;
		/// <summary>
		/// When does this event occurs
		/// </summary>
		public DateTime When
		{
			get { return _When; }
			set { _When = value; }
		}

		private string _Where;
		/// <summary>
		/// Where it occurs in the code, only used by trace event in verbose mode
		/// </summary>
		public string Where
		{
			get { return _Where; }
			set { _Where = value; }
		}

		private string _Details;
		/// <summary>
		/// Contains details about the event
		/// </summary>
		public string Details
		{
			get { return _Details; }
			set { _Details = value; }
		}
		#endregion

		#region ----------------CONSTRUCTORS----------------

		/// <summary>
		/// Constructor
		/// </summary>
		public MessageInfo()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="typ"></param>
		/// <param name="where"></param>
		/// <param name="details"></param>
		public MessageInfo(MessageEventType typ, string where, string details)
		{
			_Type = typ;
			_Where = where;
			_Details = details;
		}
		#endregion

		#region ----------------OVERRIDES----------------

		/// <summary>
		/// override the method for use in the trace methods
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			switch (_Type)
			{
				case MessageEventType.Error:
					return string.Format("ERROR: {0}; into:{1}; details:{2};", _When, _Where, _Details);
				case MessageEventType.Info:
				case MessageEventType.Verbose:
					return string.Format("INFO: {0}; into:{1}; details:{2};", _When, _Where, _Details);
				default:
					return string.Empty;
			}
		}

		#endregion
	}
}
