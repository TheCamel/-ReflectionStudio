using System;
using System.IO;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using ReflectionStudio.Core.Generator.Parsers;
using ReflectionStudio.Core.Events;
using ReflectionStudio.Core.Helpers;

namespace ReflectionStudio.Core.Generator
{
	/// <summary>
	/// TemplateGenerator, lit un template et créer la classe à compiler (basée sur ClassWriter)
	/// qui elle va generer le véritable code métier.
	/// </summary>
	public class GeneratorService
	{
		#region ---------------SINLETON---------------
		/// <summary>
		/// Singleton instance
		/// </summary>
		public static readonly GeneratorService Instance = new GeneratorService();

		private GeneratorService()
		{
		}
		#endregion

		//tableau de compilateurs disponibles pour les differents templates
		private List<ClassWriterCompiler> _Compilers = new List<ClassWriterCompiler>();

		public bool CreateCompiler( TemplateItem ti )
		{
			string ClassWriterCode = GenerateCodeWriter( ti );

			if( ClassWriterCode !=  null )
			{
				ClassWriterCompiler cwc = new ClassWriterCompiler();
				if( !cwc.CompileClass( ClassWriterCode, ti ) )
				{
					Tracer.Error("GeneratorService", "Compilation error : {0} ", cwc.Compiler.GetLastErrorMessage() );
					cwc = null;
					return false;
				}
				else
				{
					this._Compilers.Add( cwc );
				}
				return true;
			}
			else return false;
		}

		public void ClearCompilers()
		{
			this._Compilers.Clear();
		}

		public bool GenerateCode( Context context )
		{
			foreach( ClassWriterCompiler cwc in this._Compilers )
			{
				if (!cwc.Run(context))
				{
					Tracer.Error("GeneratorService", "Error running compiler for DB Object: {0} ", context.CurrentTable.Name);

					return false;
				}
			}
							
			return true;
		}

		/// <summary>
		/// génère la chaine qui contient le code de création du ClassWriter.
		/// </summary>
		private string GenerateCodeWriter( TemplateItem ti )
		{
			string mainCode = string.Empty;
			eParserType typ = eParserType.None;
#if DEBUG
			typ = eParserType.File;
#else
			typ = eParserType.Memory;
#endif

			ITemplateParser Parser = new ParserFactory().GetParser( eParserType.File );
			bool bResult = Parser.GenerateCode(ti.TemplateFile);

			if( bResult )	//le parsing est ok
			{
				string classCode = new ResourceHelper().ReadResource("ReflectionStudio.Core",
					"ReflectionStudio.Core.Generator.ClassWriter.ClassWriterExtended.txt");

				//on remplace ClassWriter<#=DOM=#> pour avoir une classe du type "ClassWriter + BO + Base"
				classCode = classCode.Replace( "<#DOM#>", ti.OutputFilePrefix + ti.OutputFileSufix );

				//on remplace avec le code pour la fonction generate()
				classCode = classCode.Replace( "<#CODE#>", Parser.GetCode() );

#if DEBUG
				StreamWriter sw = File.CreateText( AppDomain.CurrentDomain.BaseDirectory + "\\ClassWriterResult.txt");
				sw.Write(classCode);
				sw.Close();
#endif
				return classCode;
			}
			else return null;
		}
	}
}
