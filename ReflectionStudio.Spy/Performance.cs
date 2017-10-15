using System;
using System.Collections.Generic;
using System.Threading;
using ReflectionStudio.Spy.Internal;
using ReflectionStudio.Spy.Timer;
using WcfServer;
using System.Diagnostics;

namespace ReflectionStudio.Spy
{
	public static class Performance
	{
		private static object _Lock = new object();
		internal static bool _Initialized = false;
		public static bool _CaptureAllowed = false;
		internal static int _ThreadSlotCount = 0;
		internal static LocalDataStoreSlot _ThreadLocalSlot = Thread.AllocateDataSlot();
		internal static HighResolutionTimer _Timer = HighResolutionTimer.StartNew();
		internal static WcfHostService _Host = null;

		internal static void Initialize()
		{
			try
			{
				lock (_Lock)
				{
					if (!_Initialized)
					{
						//init the file logger
						CallStackLogger.Instance.Initialize();

						//start the wcf server if needed
						if (SettingsManager.Instance.Settings.AllowDistantControl)
						{
							_Host = new WcfHostService();
							_Host.StartService();
							_Initialized = true;
						}

						//allow capture from the first run
						if (SettingsManager.Instance.Settings.CaptureOnStart)
							_CaptureAllowed = true;
					}
				}
			}
			catch (Exception err)
			{
				RuntimeLogger.Instance.Log(LogType.Error, "ReflectionStudio.Spy.Performance.StartEvent {0}", err.Message);
				_Initialized = false;
			}
		}

		internal static void Control(int buildKey)
		{
			if (buildKey != SettingsManager.Instance.Settings.BuildKey)
			{
				RuntimeLogger.Instance.Log(LogType.Error, "Wrong build key : {0}", buildKey);
				throw new Exception("Wrong build key!");
			}
		}

		public static void StartEvent(int methodHandle, int buildKey)
		{
			Debug.WriteLine( string.Format( "StartEvent : method {0},  ThreadId {1}", methodHandle, Thread.CurrentThread.ManagedThreadId) );

			if (!_Initialized)
				Initialize();

			if (!_CaptureAllowed) return;

			Control(buildKey);

			try
			{
				Stack<CallStackItem> callStack = GetThreadCallStack();
				
				CallStackItem item = new CallStackItem();
				item.MethodHandle = methodHandle;
				item.TotalTick = _Timer.ElapsedTicks;

				callStack.Push(item);
				Debug.WriteLine(string.Format("StartEvent empile: method {0},  ThreadId {1}", methodHandle, Thread.CurrentThread.ManagedThreadId));
			}
			catch (Exception err)
			{
				RuntimeLogger.Instance.Log(LogType.Error, "ReflectionStudio.Spy.Performance.StartEvent {0}", err.Message);
			}
		}

		public static void EndEvent()
		{
			if (!_CaptureAllowed) return;
			Debug.WriteLine(string.Format("EndEvent : ThreadId {0}", Thread.CurrentThread.ManagedThreadId));

			long tickCount = _Timer.ElapsedTicks;
			try
			{
				//get the local thread callstack data
				Stack<CallStackItem> callStack = GetThreadCallStack();

				//un-pile the last start event
				CallStackItem info = callStack.Pop();
				info.TotalTick = tickCount - info.TotalTick;

				Debug.WriteLine(string.Format("EndEvent depile: method {0},  ThreadId {1}", info.MethodHandle, Thread.CurrentThread.ManagedThreadId));

				if (callStack.Count != 0)
				{
					//the one before is the one that called me
					CallStackItem infoCaller = callStack.Peek();
					info.CalledByHandle = infoCaller.MethodHandle;
				}

				//write the log
				CallStackLogger.Instance.Write(info);

				//if no more call on the stack, flush the log writer
				if (callStack.Count == 0)
				{
					CallStackLogger.Instance.Flush();
					_ThreadSlotCount--;
					Debug.WriteLine(string.Format("EndEvent : nb slot {0},  ThreadId {1}", _ThreadSlotCount, Thread.CurrentThread.ManagedThreadId));

					Thread.SetData(_ThreadLocalSlot, null);
				}

				//no more threads with a start event and a callstack
				if (_ThreadSlotCount == 0)
				{
					CallStackLogger.Instance.Terminate();
				
					if (_Host != null)
						_Host.StopService();
				}
			}
			catch (Exception exception)
			{
				RuntimeLogger.Instance.Log(LogType.Error, "ReflectionStudio.Spy.Performance.EndEvent", exception);
			}
		}

		/// <summary>
		/// for counting mode
		/// </summary>
		/// <param name="methodHandle"></param>
		/// <param name="methodName"></param>
		public static void StartEndEvent(int methodHandle, int buildKey)
		{
			if (!_CaptureAllowed) return;

			Control(buildKey);

			if (!_Initialized)
			{
				Initialize();
			}

			try
			{
				CallStackItem item = new CallStackItem();
				item.MethodHandle = methodHandle;
				item.TotalTick = _Timer.ElapsedTicks;
			}
			catch (Exception err)
			{
				RuntimeLogger.Instance.Log(LogType.Error, "ReflectionStudio.Spy.Performance.StartEndEvent", err);
			}
		}

		internal static Stack<CallStackItem> GetThreadCallStack()
		{
			Stack<CallStackItem> threadCallStack = null;

			lock (_Lock)
			{
				try
				{
					threadCallStack = Thread.GetData(_ThreadLocalSlot) as Stack<CallStackItem>;
				}
				catch (Exception err)
				{
					throw new RuntimeException("ReflectionStudio.Spy.Performance.GetThreadCallStack", err);
				}

				if (threadCallStack == null)
				{
					threadCallStack = new Stack<CallStackItem>(100);
					Thread.SetData(_ThreadLocalSlot, threadCallStack);
					_ThreadSlotCount++;

					Debug.WriteLine(string.Format("GetThreadCallStack : nb slot {0},  ThreadId {1}", _ThreadSlotCount, Thread.CurrentThread.ManagedThreadId));
				}
			}
			return threadCallStack;
		}
	}
}
