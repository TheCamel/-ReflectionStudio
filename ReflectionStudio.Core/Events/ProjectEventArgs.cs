using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReflectionStudio.Core.Properties;

namespace ReflectionStudio.Core.Events
{
	/// <summary>
	/// Project event types
	/// </summary>
	public enum ProjectEventType
	{
		Opening,
		Opened,
		Closing,
		Closed,
		Saving,
		Saved,
		SettingsChange,
		SnapshotChange
	}

	public class ProjectEventArgs : MessageEventArgs
	{
		public ProjectEventArgs(ProjectEventType type)
			: base( MessageEventType.Info)
		{
			ProjectType = type;
			FileName = string.Empty;
		}

		public ProjectEventArgs(ProjectEventType type, string fileName)
			: base( MessageEventType.Info)
		{
			switch( type )
			{
				case ProjectEventType.Opened:
					Info.Details = string.Format(Resources.CORE_PROJECT_LOADED, fileName);
					break;
				case ProjectEventType.Saved:
				case ProjectEventType.Closed:
					Info.Details = string.Format(Resources.CORE_PROJECT_SAVED, fileName);
					break;
			}
			ProjectType = type;
			FileName = fileName;
		}

		public ProjectEventType ProjectType { get; set; }

		public string FileName { get; set; }
	}
}
