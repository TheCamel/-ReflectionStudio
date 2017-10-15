using System;

namespace ReflectionStudio.Core.Generator.Parsers
{
	internal class ParserFactory
	{
		public ITemplateParser GetParser( eParserType typ )
		{
			if( typ == eParserType.File )
				return new InFileParser();
			else
				return new InMemoryParser();
		}
	}

	internal enum eParserType { None, Memory, File };
}
