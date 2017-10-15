using System;
using System.IO;
using ReflectionStudio.Core.Events;
	
namespace ReflectionStudio.Core.Generator
{
	/// <summary>
	/// ClassWriter: Classe de base qui va ensuite etre dérivé, puis compilé pour générer
	/// les classes à partir des templates.
	/// </summary>
	internal class ClassWriter
	{
		private TemplateItem _ti = null;
		protected StreamWriter _StreamWriter = null;
		
		#region properties

		private Context _Context = null;
		protected Context Context
		{
			get { return _Context; }
		}

		#endregion

		public ClassWriter()
		{
		}

		public void Initialize(Context context, TemplateItem ti)
		{
			_Context = context;
			_ti = ti;

			if( !Directory.Exists( _ti.OutputPath ) )
			{
				Directory.CreateDirectory( _ti.OutputPath );
				//EventManager.Instance.SendMessage( string.Format( "Create the directory {0}...", _ti.OutputPath  ) );
			}

			string outputfile = GetOutputFile();

			if (File.Exists(outputfile))
			{
				if (!_ti.OverwriteIfExist)
				{
					//EventManager.Instance.SendMessage(string.Format("Do not overwrite the {0} file", outputfile));
					return;
				}
				if( (File.GetAttributes( outputfile ) & FileAttributes.ReadOnly ) != FileAttributes.ReadOnly )
				{
					File.Delete(outputfile);
				}
				else
				{
					//EventManager.Instance.SendMessage(string.Format("The file {0} is write protected", outputfile));
					return;
				}
			}

			_StreamWriter = File.CreateText(outputfile);

			EventDispatcher.Instance.RaiseStatus( 
				string.Format( "Generate the file {0} ", outputfile  ), StatusEventType.StartProgress );
		}

		virtual public void Generate()
		{
			//si le _StreamWriter est null, on ecrit rien
			throw new Exception( "Generate fonction of ClassWriter not overrired" );
		}

		public void UnInitialize()
		{
			if( _StreamWriter != null )
			{
				_StreamWriter.Flush();
				_StreamWriter.Close();
				_StreamWriter = null;
			}
			_Context = null;
		}

		#region FILES
		protected string GetOutputFile()
		{
			return string.Format( "{0}\\{1}{2}{3}.{4}",
						_ti.OutputPath, _ti.OutputFilePrefix, _Context.CurrentTable.Name, _ti.OutputFileSufix, _ti.OutputFileExtension);
		}
		protected string GetRelativeFile()
		{
			return string.Format("{0}{1}{2}.cs", _ti.OutputFilePrefix, _Context.CurrentTable.Name, _ti.OutputFileSufix);
		}
		#endregion
	}
}
