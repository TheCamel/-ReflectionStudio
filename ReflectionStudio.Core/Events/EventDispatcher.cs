using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using ReflectionStudio.Core.Project;

namespace ReflectionStudio.Core.Events
{
	public class EventDispatcher : DispatcherObject
	{
		#region ----------------SINGLETON----------------
		public static readonly EventDispatcher Instance = new EventDispatcher();

		/// <summary>
		/// Private constructor for singleton pattern
		/// </summary>
		private EventDispatcher()
		{
		}
		#endregion

		#region ----------------STATUS BAR----------------
		/// <summary>
		/// Event delegate
		/// </summary>
		public event EventHandler<StatusEventArgs> OnStatusChange;

		/// <summary>
		/// Internal function to raise a status (bar) event
		/// </summary>
		/// <param name="type"></param>
		public void RaiseStatus(string message, StatusEventType type)
		{
			Trace.TraceInformation(message);

			if (CheckAccess())
			{
				StatusEventArgs args = new StatusEventArgs(type, message);

				if (OnStatusChange != null)
					OnStatusChange(this, args);

				//raise event only if it contains message
				if( message != string.Empty )
					RaiseMessage(args);
			}
			else
				Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
				{
					StatusEventArgs args = new StatusEventArgs(type, message);

					if (OnStatusChange != null)
						OnStatusChange(this, args);

					//raise event only if it contains message
					if (message != string.Empty)
						RaiseMessage(args);
				});
		}

		#endregion

		#region ----------------PROJECT----------------
		/// <summary>
		/// Event delegate
		/// </summary>
		public event EventHandler<ProjectEventArgs> OnProjectChange;

		/// <summary>
		/// Internal function to raise a project event
		/// </summary>
		/// <param name="type"></param>
		public void RaiseProject(ProjectEntity entity, ProjectEventType type)
		{
			Trace.TraceInformation("Project event :{0} is {1}.", entity != null ? entity.ProjectFilePath : string.Empty, type.ToString());

			if (CheckAccess())
			{
				ProjectEventArgs args = new ProjectEventArgs(type, entity != null ? entity.ProjectFilePath : string.Empty);

				if (OnProjectChange != null)
					OnProjectChange(this, args);
			}
			else
				Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
				{
					ProjectEventArgs args = new ProjectEventArgs(type, entity != null ? entity.ProjectFilePath : string.Empty);

					if (OnProjectChange != null)
						OnProjectChange(this, args);
				});

		}
		#endregion

		#region ----------------MESSAGE----------------
		/// <summary>
		/// Event delegate
		/// </summary>
		public event EventHandler<MessageEventArgs> OnMessage;

		public void RaiseMessage(MessageInfo info)
		{
			RaiseMessage(new MessageEventArgs(info));
		}

		/// <summary>
		/// Internal function to raise an event
		/// </summary>
		/// <param name="type"></param>
		private void RaiseMessage(MessageEventArgs args)
		{
			Trace.Assert(args != null);

			if (CheckAccess())
			{
				if (OnMessage != null)
					OnMessage(this, args);
			}
			else
				Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
				{
					if (OnMessage != null)
						OnMessage(this, args);
				});
		}

		private void RaiseMessage(string message)
		{
			Trace.TraceInformation(message);

			if (CheckAccess())
			{
				MessageEventArgs args = new MessageEventArgs(message);

				if (OnMessage != null)
					OnMessage(this, args);
			}
			else
				Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
				{
					MessageEventArgs args = new MessageEventArgs(message);

					if (OnMessage != null)
						OnMessage(this, args);
				});
		}

		#endregion
	}
}
