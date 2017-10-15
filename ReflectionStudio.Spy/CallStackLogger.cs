using System.Text;
using System.IO;
using System;
using ReflectionStudio.Spy.Internal;
using System.Runtime.InteropServices;
using System.Reflection;

namespace ReflectionStudio.Spy
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct CallStackItem
	{
		public int MethodHandle;
		public long TotalTick;
		public int CalledByHandle;
	}

	internal class CallStackLogger
	{
		#region ----------------INTERNALS----------------

		private const int _BufferMaxSize = 10000;
		private object _Lock = new object();
		private StringBuilder _Buffer = new StringBuilder(_BufferMaxSize, _BufferMaxSize*2);

		#endregion

		#region ----------------PROPERTIES----------------

		private string _StackFile;
		public string StackFile
		{
			get { return _StackFile; }
			set { _StackFile = value; }
		}
		#endregion

		#region ----------------SINGLETON----------------
		
		public static readonly CallStackLogger Instance = new CallStackLogger();

		/// <summary>
		/// Private constructor for singleton pattern
		/// </summary>
		private CallStackLogger()
		{
			Initialize();
		}
		#endregion

		#region ----------------METHODS----------------

		internal void Initialize()
		{
			lock (_Lock)
			{
				_StackFile = Assembly.GetExecutingAssembly().Location.Replace(".dll", string.Format("{0}.snp",DateTime.Now.ToString("yyyyMMdd-HHmmss")) );

				_Buffer.Remove(0, _Buffer.Length);
				_Buffer.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
				_Buffer.AppendLine();
				_Buffer.AppendFormat("<Snapshot Version=\"2\" Project=\"1\" MethodMap=\"{0}\">", SettingsManager.Instance.Settings.MethodMapGuid);
				_Buffer.AppendLine();
			}
		}

		internal void Write(CallStackItem info)
		{
			return;

			lock (_Lock)
			{
				_Buffer.AppendFormat("<I H=\"{0}\" C=\"{1}\" E=\"{2}\"/>",
					info.MethodHandle, info.CalledByHandle, info.TotalTick / 0x2710L);
				_Buffer.AppendLine();
			}
			if (_Buffer.Length > _BufferMaxSize-100)
				Flush();
		}

		internal void Flush()
		{
			return;

			lock (_Lock)
			{
				FileStream fs = File.Open(_StackFile, FileMode.Append, FileAccess.Write);
				using (StreamWriter sw = new StreamWriter(fs))
					sw.Write(_Buffer.ToString());
				
				fs.Close();

				_Buffer.Remove(0, _Buffer.Length);
			}
		}

		internal void Terminate()
		{
			return;

			Flush();

			lock (_Lock)
			{
				FileStream fs = File.Open(_StackFile, FileMode.Append, FileAccess.Write);
				using (StreamWriter sw = new StreamWriter(fs))
					sw.Write("</Snapshot>");

				fs.Close();
			}
		}

		#endregion
	}
}
