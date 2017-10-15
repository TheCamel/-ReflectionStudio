using System;
using System.IO;
using System.Text;

namespace ReflectionStudio.Core.Generator.Parsers
{
	/// <summary>
	/// InFileParser.
	/// </summary>
	internal class InFileParser : TemplateParser
	{
		protected string Output
		{
			get { return AppDomain.CurrentDomain.BaseDirectory + "\\ClassTemp.txt"; }
		}

		public InFileParser()
		{
			//first delete temp file if exist
			string tempfile = Output;
			if (File.Exists(tempfile))
			{
				File.Delete(tempfile);
			}

			//create the stream writer as a file output
			_writer = File.CreateText(tempfile);
			_writer.AutoFlush = true;
		}

		override public string GetCode()
		{
			//close the file writer 
			base.GetCode();

			//re-open it, read complete, send the result
			StreamReader srResult = File.OpenText(Output);
			string classCode = srResult.ReadToEnd();
			srResult.Close();

			return classCode;
		}
	}
}
