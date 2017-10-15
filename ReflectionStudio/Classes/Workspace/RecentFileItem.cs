using System;
using System.IO;
using ReflectionStudio.Core.FileManagement;

namespace ReflectionStudio.Classes.Workspace
{
	[Serializable]
	public class RecentFileItem : DiskContent
	{
		public RecentFileItem(string filePath) : base( filePath )
		{
			LastDateTime = DateTime.Now;
		}

		public bool IsFixed
		{
			get;
			set;
		}

		public DateTime LastDateTime
		{
			get;
			set;
		}
	}
}
