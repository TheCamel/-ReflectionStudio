using System;
using System.IO;
using ReflectionStudio.Core;
using ReflectionStudio.Core.Events;
using ReflectionStudio.Core.FileManagement;

namespace ReflectionStudio.Classes.Workspace
{
	class WorkspaceDAC
	{
		internal const string _FileName = "workspace.bin";

		private string WorkspaceFile
		{
			get { return Path.Combine(PathHelper.ApplicationPath, _FileName); }
		}

		/// <summary>
		/// Load the workspace data from the bin file
		/// </summary>
		/// <returns></returns>
		public WorkspaceEntity Load()
		{
			Tracer.Verbose("WorkspaceDAC.Load", "starting");

			WorkspaceEntity entity = null;
			try
			{
				entity = (WorkspaceEntity)SerializeHelper.Deserialize(WorkspaceFile, typeof(WorkspaceEntity));
			}
			catch (Exception error)
			{
				Tracer.Error("WorkspaceDAC.Load", error);
			}

			Tracer.Verbose("WorkspaceDAC.Load", "ending");
			return entity;
		}

		/// <summary>
		/// Save the worspace data into the bin file
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public bool Save( WorkspaceEntity entity )
		{
			Tracer.Verbose("WorkspaceDAC.Save", "starting");

			bool result = false;
			try
			{
				result = SerializeHelper.Serialize(WorkspaceFile, entity);
			}
			catch (Exception error)
			{
				Tracer.Error("WorkspaceDAC.Save", error);
			}

			Tracer.Verbose("WorkspaceDAC.Save", "ending");
			return result;
		}
	}
}
