using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ReflectionStudio.Core.FileManagement
{
	[Serializable]
	public class DiskContent
	{
		public DiskContent(string filePath)
		{
			FullName = filePath;
			IsFolder = false;
		}

		public DiskContent(string filePath, bool isFolder)
		{
			FullName = filePath;
			IsFolder = isFolder;
		}

		public string FullName
		{
			get;
			set;
		}

		public string NameWithoutExtension
		{
			get { return Path.GetFileNameWithoutExtension(FullName); }
		}

		public string Name
		{
			get { return Path.GetFileName(FullName); }
		}

		public string Extension
		{
			get { return Path.GetExtension(FullName); }
		}

		public string Folder
		{
			get { return Path.GetDirectoryName(FullName); }
		}

		public bool IsFolder
		{
			get;
			set;
		}

		public bool IsFile
		{
			get { return !IsFolder; }
		}

		public bool Exists
		{
			get { return File.Exists(FullName); }
		}

		public bool Delete()
		{
			try
			{
				if (IsFile)
					File.Delete(FullName);
				else
					Directory.Delete(FullName);

				return true;
			}
			catch (Exception all)
			{
				return false;
			}
		}
	}
}
