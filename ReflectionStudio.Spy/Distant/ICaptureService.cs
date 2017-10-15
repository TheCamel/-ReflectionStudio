using System.Collections.Generic;
using System.ServiceModel;
using System.IO;
using System;

namespace ReflectionStudio.Spy.Distant
{
	[ServiceContract]
	public interface ICaptureService
	{
		[OperationContract]
		bool StartCapture();

		[OperationContract]
		bool StopCapture();

		[OperationContract]
		bool IsCapturing();

		[OperationContract]
		List<string> GetCaptureFileList();

		[OperationContract]
		string GetCurrentCaptureFileName();

		[OperationContract]
		string RetreiveCapture(string fileName, bool withDelete);

		[OperationContract]
		bool DeleteCapture(string fileName);
	}
}
