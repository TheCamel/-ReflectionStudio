using System;
using System.IO;

namespace ReflectionStudio.Core.Generator.Parsers
{
	/// <summary>
	/// Description résumée de InMemoryParser.
	/// </summary>
	internal class InMemoryParser : TemplateParser
	{
		public InMemoryParser()
		{
			//create the stream writer as a memory output
			_writer = new StreamWriter(new MemoryStream());
			_writer.AutoFlush = true;
		}

		override public string GetCode()
		{
			_writer.Flush();

			string resultCode = ((MemoryStream)_writer.BaseStream).ToString();
			
			base.GetCode();

			return resultCode;
		}
	}
}
