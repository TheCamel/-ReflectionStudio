using System;
using System.ComponentModel;

namespace ReflectionStudio.Core.Generator
{
	[Serializable]
	public class TemplateItem
	{
		//-------------------------------------------------------
		private string _TemplateName = string.Empty;

		[Browsable(false)]
		[DefaultValue("")]
		public string TemplateName
		{
			get { return _TemplateName; }
			set { _TemplateName = value; }
		}
		//-------------------------------------------------------
		private string _TemplateFile = string.Empty;

		[ReadOnly(true)]
		[Browsable(true)]
		[DefaultValue("")]
		[Description("C# Code generation template")]
		[Category("File settings")]
		public string TemplateFile
		{
			get { return this._TemplateFile; }
			set { this._TemplateFile = value; }
		}
		//-------------------------------------------------------
		private string _OutputPath = string.Empty;

		[Browsable(true)]
		[DefaultValue("")]
		[Category("File settings")]
		[Description("Output directory")]
		public string OutputPath
		{
			get { return _OutputPath; }
			set { _OutputPath = value; }		
		}
		//-------------------------------------------------------
		private string _OutputFilePrefix = string.Empty;

		[Browsable(true)]
		[DefaultValue("")]
		[Category("File settings")]
		[Description("Prefix to add before the generated file")]
		public string OutputFilePrefix
		{
			get { return _OutputFilePrefix; }
			set { _OutputFilePrefix = value; }
		}

		//-------------------------------------------------------
		private string _OutputFileSufix = string.Empty;

		[Browsable(true)]
		[DefaultValue("")]
		[Category("File settings")]
		[Description("Sufix to add after the generated file")]
		public string OutputFileSufix
		{
			get { return _OutputFileSufix; }
			set { _OutputFileSufix = value; }
		}

		//-------------------------------------------------------
		private string _OutputFileExtension = "cs";

		[Browsable(true)]
		[DefaultValue("cs")]
		[Category("File settings")]
		[Description("Extension of the file to generate")]
		public string OutputFileExtension
		{
			get { return _OutputFileExtension; }
			set { _OutputFileExtension = value; }
		}

		//-------------------------------------------------------
		private bool _OverwriteIfExist = false;

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Overwrite the generated file if it exist")]
		[Category("File settings")]
		public bool OverwriteIfExist
		{
			get { return _OverwriteIfExist; }
			set { _OverwriteIfExist = value; }
		}

		//-------------------------------------------------------

		public override string ToString()
		{
			return this._TemplateName;
		}
	}
}
