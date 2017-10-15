using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ReflectionStudio.Spy.Internal;
using System.IO;

namespace ReflectionStudio.Spy.Distant
{
	public class CaptureService : ICaptureService
	{
		/// <summary>
		/// Start capturing performance data
		/// </summary>
		/// <returns></returns>
		public bool StartCapture()
		{
			// check the setting to see if distant control allowed, we normally do not start the service
			if (!SettingsManager.Instance.Settings.AllowDistantControl)
				return false;

			// we have to change a flag or create a logger
			Performance._CaptureAllowed = true;

			return true;
		}
		
		/// <summary>
		/// Stop capturing performance data and flush the call stack
		/// </summary>
		/// <returns></returns>
		public bool StopCapture()
		{
			Performance._CaptureAllowed = false;
			return true;
		}

		/// <summary>
		/// Is distant control and capture activated ?
		/// </summary>
		/// <returns></returns>
		public bool IsCapturing()
		{
			return (SettingsManager.Instance.Settings.AllowDistantControl && Performance._CaptureAllowed );
		}

		/// <summary>
		/// Return the current capture filename
		/// </summary>
		/// <returns></returns>
		public string GetCurrentCaptureFileName()
		{
			return new FileInfo(CallStackLogger.Instance.StackFile).Name;
		}

		/// <summary>
		/// Return the list of existing capture file in the current capture directory
		/// </summary>
		/// <returns></returns>
		public List<string> GetCaptureFileList()
		{
			try
			{
				List<string> result = new List<string>();

				DirectoryInfo di = new DirectoryInfo(PathHelper.SnapPath());

				foreach (FileInfo file in di.GetFiles("*.snp"))
					result.Add(file.Name);

				return result;
			}
			catch (Exception error)
			{
				RuntimeLogger.Instance.Log(LogType.Error, "CaptureService.GetCaptureFileList {0}", error.Message);
				return null;
			}
		}

		/// <summary>
		/// Return the content of a given file and delete it after if asked
		/// </summary>
		/// <param name="fileName">if null, it's the current capture</param>
		/// <param name="withDelete"></param>
		/// <returns></returns>
		public string RetreiveCapture(string fileName, bool withDelete)
		{
			try
			{
				if (fileName != null)
				{
					if (CallStackLogger.Instance.StackFile.Contains(fileName))
						return RetreiveCurrentLog(withDelete);
					else
					{
						string fileStack = Path.Combine(PathHelper.SnapPath(), fileName);

						//send the file over the stream
						string result = File.ReadAllText(fileStack);

						if (withDelete)
							File.Delete(fileStack);

						return result;
					}
				}
				else //no value, we are on current
				{
					return RetreiveCurrentLog(withDelete);
				}
			}
			catch (Exception error)
			{
				RuntimeLogger.Instance.Log(LogType.Error, "CaptureService.RetreiveCapture {0}", error.Message);
				return string.Empty;
			}
		}

		/// <summary>
		/// Special management for the current file, because need to flush it and init a new capture
		/// </summary>
		/// <param name="withDelete"></param>
		/// <returns></returns>
		private string RetreiveCurrentLog(bool withDelete)
		{
			try
			{
				//stop capturing
				Performance._CaptureAllowed = false;

				//flush the actual performance log to the file
				CallStackLogger.Instance.Terminate();

				//change the time stamp so we don't get override for the next time
				string fileStack = CallStackLogger.Instance.StackFile;
				CallStackLogger.Instance.Initialize();

				//send the file over the stream
				string result = File.ReadAllText(fileStack);

				if (withDelete)
					File.Delete(fileStack);

				return result;
			}
			catch (Exception error)
			{
				RuntimeLogger.Instance.Log(LogType.Error, "CaptureService.RetreiveCurrentLog {0}", error.Message);
				return string.Empty;
			}
		}

		/// <summary>
		/// Delete a capture file
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public bool DeleteCapture(string fileName)
		{
			try
			{
				if (fileName != null)
				{
					// we are on the current snapshot file
					if (CallStackLogger.Instance.StackFile.Contains(fileName))
					{
						return DeleteCurrentLog();
					}
					else
					{
						File.Delete(Path.Combine(PathHelper.SnapPath(), fileName));
						return true;
					}
				}
				else // no value, delete the current
				{
					return DeleteCurrentLog();
				}
			}
			catch (Exception error)
			{
				RuntimeLogger.Instance.Log(LogType.Error, "CaptureService.DeleteCapture {0}", error.Message);
				return false;
			}
		}

		/// <summary>
		/// Delete the current capture file, init the logger
		/// </summary>
		/// <returns></returns>
		private bool DeleteCurrentLog()
		{
			try
			{
				CallStackLogger.Instance.Terminate();
				File.Delete(CallStackLogger.Instance.StackFile);

				CallStackLogger.Instance.Initialize();

				return true;
			}
			catch (Exception error)
			{
				RuntimeLogger.Instance.Log(LogType.Error, "CaptureService.DeleteCurrentLog {0}", error.Message);
				return false;
			}
		}
	}
}
