using System;
using System.IO;
using System.Text.RegularExpressions;
using ReflectionStudio.Core.Events;

namespace ReflectionStudio.Core.Generator.Parsers
{
	/// <summary>
	/// TemplateParser, parse the template file
	/// </summary>
	internal class TemplateParser : ReflectionStudio.Core.Generator.Parsers.ITemplateParser
	{
		#region ---------------PRIVATES---------------

		protected StreamWriter _writer = null;

		#endregion

		public TemplateParser()
		{
		}

		public bool GenerateCode(string TemplateFile)
		{
			try
			{
				//check if the template file exist
				if (!File.Exists(TemplateFile))
				{
					Tracer.Error("TemplateParser", "{0} does not exist.", TemplateFile);
					return false;
				}

				//open it and start parsing
				StreamReader sr = File.OpenText(TemplateFile);
				string templateCode = sr.ReadToEnd();
				//close the template reader
				sr.Close();

				Regex regex = new Regex( @"([<#]{2}.*?[#>]{2})", RegexOptions.Multiline | RegexOptions.Singleline );

				//tester

				string[] result = regex.Split(templateCode);

				foreach (string str in result)
				{
					if (str.Contains("<#") && str.Contains("#>"))
						ProcessCode(str.Remove(2, str.Length - 4));
					else
						ProcessText(str);
				}

				return true;
			}
			catch (Exception error)
			{
				Tracer.Error("TemplateParser", error);
				return false;
			}
		}

		virtual public string GetCode()
		{
			//close the file writer 
			_writer.Flush();
			_writer.Close();

			return string.Empty;
		}
		#region ---------------PARSING---------------

		/// <summary>
		/// Generate the write instruction for template text like "public class"
		/// </summary>
		/// <param name="inputLine"></param>
		private void ProcessText(string inputLine)
		{
			_writer.WriteLine( string.Format("_StreamWriter.WriteLine( \" {0} \" );", inputLine.Replace("\"", "\\\"") ) );
		}

		/// <summary>
		/// generate the write instruction for a function, inputLine is GetBOName() for exemple
		/// </summary>
		/// <param name="inputLine"></param>
		private void ProcessCode(string inputLine)
		{
			_writer.WriteLine( string.Format( "_StreamWriter.Write( {0} );\r\n", inputLine ) );
		}
		#endregion
	}
}
