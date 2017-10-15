using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ReflectionStudio.Core;

namespace ReflectionStudio.Classes
{
	abstract internal class DocumentDataContext : IDocumentDataContext
	{
		virtual public bool CanSave
		{
			get { return true; }
		}

		public Type FileServiceType
		{
			get;
			set;
		}

		private IFileService _FileService = null;
		internal IFileService FileService
		{
			get
			{
				if( _FileService == null )
					_FileService = (IFileService)Activator.CreateInstance(FileServiceType);

				return _FileService;
			}
		}

		public object Entity
		{
			get;
			set;
		}

		private string _FullName;
		public string FullName
		{
			get
			{
				if (!string.IsNullOrEmpty(_FullName))
					return PathHelper.GetFullQualifedFileName(_FullName);
				else
					return string.Empty;
			}
			set { _FullName = value; }
		}

		public string NameWithoutExtension
		{
			get
			{
				if (!string.IsNullOrEmpty(FullName))
					return Path.GetFileNameWithoutExtension(FullName);
				else
					return string.Empty;
			}
		}

		public string Name
		{
			get
			{
				if (!string.IsNullOrEmpty(FullName))
					return Path.GetFileName(FullName);
				else
					return string.Empty;
			}
		}

		public string Extension
		{
			get
			{
				if (!string.IsNullOrEmpty(FullName))
					return Path.GetExtension(FullName);
				else
					return string.Empty;
			}
		}

		virtual public object Load()
		{
			return FileService.Load(FullName);
		}

		virtual public bool Save(object content)
		{
			return FileService.Save(FullName, content);
		}

		virtual public bool SaveAs(object content, string newFileName)
		{
			this.FullName = newFileName;
			return FileService.Save(FullName, content);
		}
	}
}
