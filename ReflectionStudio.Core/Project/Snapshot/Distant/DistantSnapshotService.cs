using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReflectionStudio.Core.Project
{
	public class DistantSnapshotService
	{
		#region ----------------SINGLETON----------------

		public static readonly DistantSnapshotService Instance = new DistantSnapshotService();

		/// <summary>
		/// Private constructor for singleton pattern
		/// </summary>
		private DistantSnapshotService()
		{
		}
		#endregion

		#region ----------------PROPERTIES----------------

		private DistantDAC _DistantDAC = null;
		internal DistantDAC DistantDAC
		{
			get { return _DistantDAC; }
		}

		#endregion

		#region ----------------METHODS----------------

		public bool IsConnected()
		{
			if (_DistantDAC != null)
				return _DistantDAC.IsConnected();
			else
				return false;
		}

		public bool IsCapturing()
		{
			if (_DistantDAC != null)
				return _DistantDAC.IsCapturing();
			else
				return false;
		}

		public bool Connect()
		{
			if (_DistantDAC != null)
				_DistantDAC.Disconnect();

			_DistantDAC = new DistantDAC(ProjectService.Instance.Current.Settings);

			return _DistantDAC.Connect();
		}

		public bool Disconnect()
		{
			if (_DistantDAC != null)
				return _DistantDAC.Disconnect();
			else
				return true;
		}

		public bool StartCapture()
		{
			if (_DistantDAC != null)
				return _DistantDAC.StartCapture();
			else
				return false;
		}

		public bool StopCapture()
		{
			if (_DistantDAC != null)
				return _DistantDAC.StopCapture();
			else
				return false;
		}

		public bool RetreiveCapture(string snapFile, bool withDelete)
		{
			if (_DistantDAC != null)
				return _DistantDAC.RetreiveCapture(ProjectService.Instance.Current.SubFolderSnapshot, snapFile, withDelete);
			else
				return false;
		}

		public bool DeleteCapture(string fileName)
		{
			if (_DistantDAC != null)
				return _DistantDAC.DeleteCapture(fileName);
			else
				return false;
		}

		public string[] GetCaptureList()
		{
			if (_DistantDAC != null)
				return _DistantDAC.GetCaptureList();
			else
				return null;
		}

		#endregion
	}
}
