
namespace ReflectionStudio.Core.Events
{
	public enum StatusEventType
	{
		StartProgress, StopProgress
	}

	public class StatusEventArgs : MessageEventArgs
	{
		public StatusEventArgs(StatusEventType type, string message)
			: base(MessageEventType.Info, message)
		{
			StatusType = type;
			Info.Where = "Status Event";
		}

		public StatusEventType StatusType
		{
			get;
			set;
		}
	}
}
