using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReflectionStudio.Core.Events;

namespace ReflectionStudio.Classes.Workspace
{
	public class WorkspaceService
	{
		#region ----------------SINGLETON----------------
		public static readonly WorkspaceService Instance = new WorkspaceService();

		/// <summary>
		/// Private constructor for singleton pattern
		/// </summary>
		private WorkspaceService()
		{
			Entity = new WorkspaceEntity();
		}
		#endregion

		#region ----------------PROPERTIES----------------
		
		public WorkspaceEntity Entity
		{
			get;
			set;
		}

		#endregion

		#region ----------------FUNCTIONS----------------

		public bool Load()
		{
			Tracer.Verbose("WorkspaceService.Load", "starting");
			bool result = false;
			try
			{
				Entity = new WorkspaceDAC().Load();
				if (Entity != null)
				{
					CheckRecentFileList();
					result = true;
				}
				else
				{
					Entity = new WorkspaceEntity();
				}
			}
			catch (Exception error)
			{
				Tracer.Error( "WorkspaceService.Load", error );
			}
		
			Tracer.Verbose("WorkspaceService.Load", "ending");
			return result;
		}

		public void AddRecentFile( string file )
		{
			Tracer.Verbose("WorkspaceService.AddRecentFile", "starting");

			try
			{
				if( Entity.RecentFiles.Where( p => p.FullName == file ).Count() == 0 )
					Entity.RecentFiles.Add(new RecentFileItem(file));

				if (Entity.RecentFiles.Count > 9)
					Entity.RecentFiles.RemoveAt(0);
			}
			catch (Exception error)
			{
				Tracer.Error("WorkspaceService.AddRecentFile", error);
			}
			Tracer.Verbose("WorkspaceService.AddRecentFile", "ending");
		}

		private void CheckRecentFileList()
		{
			Tracer.Verbose("WorkspaceService.CheckRecentFileList", "starting");

			try
			{
				List<RecentFileItem> temp = new List<RecentFileItem>();

				foreach (RecentFileItem item in Entity.RecentFiles)
				{
					if (!File.Exists(item.FullName))
						temp.Add(item);
				}

				foreach (RecentFileItem item in temp)
					Entity.RecentFiles.Remove(item);
			}
			catch (Exception error)
			{
				Tracer.Error("WorkspaceService.CheckRecentFileList", error);
			}
			Tracer.Verbose("WorkspaceService.CheckRecentFileList", "ending");
		}

		public bool Save()
		{
			return new WorkspaceDAC().Save(Entity);
		}

		#endregion
	}
}
