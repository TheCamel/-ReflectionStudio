using System;
using System.Collections.ObjectModel;
using System.IO;
using ReflectionStudio.Core.Project.Settings;
using ReflectionStudio.Core.Reflection.Types;
using System.Collections.Generic;
using ReflectionStudio.Core.FileManagement;

namespace ReflectionStudio.Core.Project
{
	[Serializable]
	public class ProjectEntity
	{
		#region ----------------------PROPERTIES----------------------

		private List<DiskContent> _FileItems = new List<DiskContent>();
		public List<DiskContent> FileItems
		{
			get { return _FileItems; }
			set { _FileItems = value; }
		}

		internal const string SnapshotFolder = "Snapshot";
		internal const string ProfiledFolder = "Profiled";

		private ObservableCollection<NetAssembly> _Assemblies = new ObservableCollection<NetAssembly>();
		public ObservableCollection<NetAssembly> Assemblies
		{
			get { return _Assemblies; }
			set { _Assemblies = value; }
		}

		[NonSerialized()]
		private bool _IsChanged = false;
		public bool IsChanged
		{
			get { return _IsChanged || _Settings.IsChanged; }
			set { _IsChanged = value; }
		}

		private string _ProjectPath;
		public string ProjectPath
		{
			get { return _ProjectPath; }
			set { _ProjectPath = value; IsChanged = true; }
		}

		private string _ProjectName;
		public string ProjectName
		{
			get { return _ProjectName; }
			set { _ProjectName = value; IsChanged = true; }
		}

		private ProjectSettings _Settings = new ProjectSettings();
		public ProjectSettings Settings
		{
			get { return _Settings; }
			set { _Settings = value; }
		}

		#endregion

		#region ----------------------CALCULATED----------------------

		public string SubFolderProfiled
		{
			get { return System.IO.Path.Combine(ProjectPath, ProfiledFolder); }
		}

		public string SubFolderSnapshot
		{
			get { return System.IO.Path.Combine(ProjectPath, SnapshotFolder); }
		}

		public string MethodMapFile
		{
			get
			{
				return System.IO.Path.Combine(SubFolderSnapshot,
					string.Format("MethodMap_{0}.xml", _Settings.MethodMapGuid)); 
			}
		}

		public string ProjectFilePath
		{
			get { return Path.Combine(_ProjectPath, _ProjectName); }
			set
			{
				_ProjectPath = Path.GetDirectoryName(value);
				_ProjectName = Path.GetFileName(value);
			}
		}

		public bool HasProgram
		{
			get
			{
				int count = 0;
				foreach (NetAssembly typ in Assemblies)
				{
					if (typ.IsProgram)
						count++;
				}
				return count == 1 ? true : false;
			}
		}

		public string MainProgramToRun
		{
			get
			{
				foreach (NetAssembly typ in Assemblies)
				{
					if (typ.IsProgram)
						return Path.Combine(SubFolderProfiled, new FileInfo(typ.FilePath).Name);
				}
				return string.Empty;
			}
		}


		public bool IsBuilded
		{
			get
			{
				if (File.Exists(MethodMapFile))
					return true;

				return false;
			}
		}
		#endregion

		#region ----------------------CONSTRUCTORS----------------------

		public ProjectEntity()
		{
		}
	
		public ProjectEntity(string filePath)
		{
			ProjectFilePath = filePath;
		}

		public ProjectEntity(string location, string fileName)
		{
			_ProjectPath = Path.Combine(location, fileName);
			_ProjectName = fileName + ProjectService.ProjectExtension;
		}

		#endregion
	}
}
