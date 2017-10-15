using System;
using System.IO;
using ReflectionStudio.Core.Events;
using ReflectionStudio.Core.FileManagement;
using System.Collections.Generic;

namespace ReflectionStudio.Core.Project
{
	class SnapshotDAC
	{
		public SnapshotInfo LoadSnapshot(string fileName)
		{
			return (SnapshotInfo)SerializeHelper.Deserialize(fileName, typeof(SnapshotInfo), true);
		}

		//public bool SaveSnapshot(string fileName)
		//{
		//    _Current = new Snapshot() { Project = "1", Version = 2 };
		//    _Current.CallStack = new System.Collections.Generic.List<CallStackInfo>();
		//    _Current.CallStack.Add(new CallStackInfo() { MethodHandle = 1, TotalTick = 1, InternalTick = 2, CalledByHandle = 0 });
		//    _Current.CallStack.Add(new CallStackInfo() { MethodHandle = 1, TotalTick = 1, InternalTick = 2, CalledByHandle = 0 });
		//    _Current.CallStack.Add(new CallStackInfo() { MethodHandle = 1, TotalTick = 1, InternalTick = 2, CalledByHandle = 0 });

		//    return XmlHelper.Serialize(fileName, _Current);
		//}


		public void Delete(string fileName)
		{
			try
			{
				if( File.Exists( fileName ) )
					File.Delete(fileName);
			}
			catch (Exception Error)
			{
				Tracer.Error("SnapshotDAC.Delete", Error);
			}
		}

		internal List<CallStackInfoAgregated> LoadBinarySnapshot(string binarySnap)
		{
			return (List<CallStackInfoAgregated>)SerializeHelper.Deserialize(binarySnap, typeof(List<CallStackInfoAgregated>));
		}

		internal void SaveBinarySnapshot(string binarySnap, List<CallStackInfoAgregated> list)
		{
			SerializeHelper.Serialize(binarySnap, list);
		}
	}
}
