using ReflectionStudio.Core.Reflection.Parser;

namespace ReflectionStudio.Core.Reflection
{
	/// <summary>
	/// Parser type that can be asked to the factory
	/// </summary>
	public enum eParserType { NetParser, CecilParser };

	public class AssemblyParserFactory
	{
		/// <summary>
		/// Retreive a given parser as an IAssemblyParser interface
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		static public IAssemblyParser GetParser( eParserType type )
		{
			if (type == eParserType.CecilParser)
				return new CecilAssemblyParser();
			else
				return new NetAssemblyParser();
		}

		static public AssemblyBackgroundWorkerParser GetBackgroundWorkerParser(eParserType type)
		{
			return new AssemblyBackgroundWorkerParser( type );
		}
	}
}
