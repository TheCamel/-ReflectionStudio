using System;
using System.Collections;
using ReflectionStudio.Core.Generator.Compilers;

namespace ReflectionStudio.Core.Generator
{
	/// <summary>
	/// Permet de compiler ClassWriter(ClassWriterExtended) pour générer le code de la classe métier
	/// </summary>
	public class ClassWriterCompiler
	{
		private const string _RefAssemblies = @"System.dll;System.Data.dll;VSNetPlus.Core.dll;VSNetPlus.DB.Interface.dll";
		private const string _namespace = "VSNetPlus.Core.CompiledTemplates.";

		private IMyCompiler _ICompiler = null;
		public IMyCompiler Compiler
		{
			get { return _ICompiler; }
			set { _ICompiler = value; }
		}
		private TemplateItem _ti = null;

		public bool CompileClass( string sourceCode, TemplateItem ti )
		{
			_ICompiler = CompilerProvider.GetCompilerForTemplate(ti);
			_ti = ti;

			_ICompiler.ClassName = _namespace + "ClassWriter" + _ti.OutputFilePrefix + _ti.OutputFileSufix;

			return _ICompiler.Compile(sourceCode, _RefAssemblies);
		}

		public bool Run( Context context )
		{
			if (_ICompiler.GetInstanceOfObject(_ICompiler.ClassName))
			{
				try
				{
					_ICompiler.InvokeMember("Initialize", new Object[] { context, _ti });
					_ICompiler.InvokeMember("Generate", null);
					_ICompiler.InvokeMember("UnInitialize", null);
					return true;
				}
				catch
				{
					return false;
				}
			}
			return false;
		}
	}
}
