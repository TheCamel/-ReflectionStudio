using System;
using System.IO;

namespace ReflectionStudio.Core.Generator.Compilers
{
	internal class CompilerProvider
	{
		/// <summary>
		/// Return the compiler required for the provided template
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		static public IMyCompiler GetCompilerForTemplate(TemplateItem item)
		{
			if ( Path.GetExtension( item.TemplateFile) == ".cs")
				return new CSharpCodeCompiler();
			//else
			//    if (item.OutputFileExtension == "vb")
			//        return new VBCodeCompiler();
				else
					return null;
		}
	}
}
