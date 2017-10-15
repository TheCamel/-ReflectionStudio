using System;

namespace ReflectionStudio.Core.Generator.Parsers
{
	interface ITemplateParser
	{
		bool GenerateCode(string TemplateFile);
		string GetCode();
	}
}
