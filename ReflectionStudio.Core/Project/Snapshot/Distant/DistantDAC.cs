using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.IO;
using ReflectionStudio.Core.Project.Settings;
using ReflectionStudio.Core.Events;
using ReflectionStudio.Core.CaptureService;

namespace ReflectionStudio.Core.Project
{
	class DistantDAC
	{
		#region ----------------PROPERTIES----------------
		
		CaptureServiceClient _DistantService = null;
		public CaptureServiceClient DistantService
		{
			get { return _DistantService; }
			set { _DistantService = value; }
		}

		#endregion

		#region ----------------CONSTRUCTOR & PRIVATE----------------

		public DistantDAC(ProjectSettings settings)
		{
			try
			{
				Binding bind;
				if (settings.TransportMode == Transport.Http)
					bind = new WSHttpBinding("HttpBinding");
				else
					bind = new NetTcpBinding("NetTcpBinding");

				_DistantService = new CaptureServiceClient
					(
						bind,
						new EndpointAddress(ServiceUri(settings))
						);
			}
			catch (Exception error)
			{
				Tracer.Error("DistantDAC.DistantDAC", error);
			}
		}
		
		private Uri ServiceUri(ProjectSettings settings)
		{
			string format;

			if (settings.TransportMode == Transport.Http)
				format = "http://{0}:{1}/CaptureService/";
			else
				format = "net.tcp://{0}:{1}/CaptureService/";

			try
			{
				return new Uri
					(
					string.Format(format, settings.Machine, settings.DistantPort)
					);
			}
			catch (Exception error)
			{
				Tracer.Error("DistantDAC.ServiceUri", error);
				return null;
			}
		}

		#endregion

		#region ----------------PUBLIC METHODS----------------

		public bool IsConnected()
		{
			return _DistantService.State == CommunicationState.Opened ? true : false;
		}

		public bool Connect()
		{
			try
			{
				_DistantService.Open();
				return true;
			}
			catch (Exception error)
			{
				Tracer.Error("DistantDAC.Connect", error);
				return false;
			}
		}

		public bool Disconnect()
		{
			try
			{
				_DistantService.Close();
				return true;
			}
			catch (Exception error)
			{
				Tracer.Error("DistantDAC.Disconnect", error);
				return false;
			}
		}

		public bool StartCapture()
		{
			if (_DistantService != null)
				return _DistantService.StartCapture();
			else
				return false;
		}

		public bool StopCapture()
		{
			if (_DistantService != null)
				return _DistantService.StopCapture();
			else
				return false;
		}

		public bool IsCapturing()
		{
			if (_DistantService != null)
				return _DistantService.IsCapturing();
			else
				return false;
		}

		public bool RetreiveCapture(string snapPath, string snapFile, bool withDelete)
		{
			if (_DistantService != null)
			{
				try
				{
					if (snapFile == null)
						snapFile = _DistantService.GetCurrentCaptureFileName();

					using (StreamWriter outfile = new StreamWriter(Path.Combine(snapPath, snapFile)))
					{
						outfile.Write(_DistantService.RetreiveCapture(snapFile, withDelete));
					}

					return true;
				}
				catch (Exception error)
				{
					Tracer.Error("DistantDAC.RetreiveCapture", error);
					return false;
				}
			}
			else return false;
		}

		public bool DeleteCapture(string fileName)
		{
			if (_DistantService != null)
			{
				return _DistantService.DeleteCapture(fileName);
			}
			else return false;
		}

		public string[] GetCaptureList()
		{
			if (_DistantService != null)
				return _DistantService.GetCaptureFileList();
			else
				return null;
		}

		#endregion
	}
}
