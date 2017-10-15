using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReflectionStudio.Core;
using System.IO;

namespace ReflectionStudio.Core.Project
{
	public class SnapshotEntity : BindableObject
	{
		#region -------------------FILE PROPERTIES-------------------

		public string DirectoryName { get; set; }
		public string FileName { get; set; }
		public string FullFileName
		{
			get
			{
				return Path.Combine(DirectoryName, FileName);
			}
		}

		public string BinaryFullFileName
		{
			get
			{
				return Path.Combine(DirectoryName, FileName.Replace(".snp", ".snb" ) );
			}
		}

		public long Length { get; set; }
		public DateTime LastWriteTime { get; set; }

		#endregion

		#region -------------------FROM XML PROPERTIES-------------------

		public int Version { get; set; }
		public string Project { get; set; }
		public string MethodMapFile { get; set; }

		#endregion

		private bool _Loaded;
		public bool Loaded
		{
			get { return _Loaded; }
			set
			{
				if (_Loaded != value)
					_Loaded = value;
				else return;

				base.RaisePropertyChanged("Loaded");
			}
		}

		public List<CallStackInfoExtended> CallstackEx { get; set; }
		public List<CallStackInfoAgregated> CallstackAg { get; set; }

		public IEnumerable<CallStackInfoAgregated> CallstackAgBefore { get; set; }
		public IEnumerable<CallStackInfoAgregated> CurrentAgreg { get; set; }
		public IEnumerable<CallStackInfoAgregated> CallstackAgAfter { get; set; }

	}
}
