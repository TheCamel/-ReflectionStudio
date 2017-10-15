using System;

namespace ReflectionStudio.Core.Generator.Compilers
{
	/// <summary>
	/// 
	/// </summary>
	public interface IMyCompiler
	{
		/// <summary>
		/// 
		/// </summary>
		string ClassName { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="SourceString"></param>
		/// <param name="LinkedAssemblies"></param>
		/// <returns></returns>
		bool Compile(string SourceString, string LinkedAssemblies);

		/// <summary>
		/// 
		/// </summary>
		System.Reflection.Assembly CompiledAssembly { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ObjectType"></param>
		/// <returns></returns>
		bool GetInstanceOfObject(string ObjectType);

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		string GetLastErrorMessage();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Method"></param>
		/// <param name="Parameters"></param>
		/// <returns></returns>
		bool InvokeMember(string Method, object[] Parameters);
	}
}
