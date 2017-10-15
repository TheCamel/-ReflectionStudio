using System;
using System.Text;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using ReflectionStudio.Core.Events;

namespace ReflectionStudio.Core.Generator.Compilers
{
	/// <summary>
	/// CSharpCodeCompiler: Compile du code CSharp en dynamique
	/// </summary>
	internal class CSharpCodeCompiler : IMyCompiler
	{
		private Object _objInstance = null;
		private Type _objType = null;

		#region ---------------- PROPERTIES ----------------

		private StringBuilder _ErrorMsg = null;
		/// <summary>
		/// Return the last errors from the compiler
		/// </summary>
		/// <returns></returns>
		public string GetLastErrorMessage()
		{
			return _ErrorMsg.ToString();
		}

		private System.Reflection.Assembly _assembly = null;
		/// <summary>
		/// The resulted assembly
		/// </summary>
		public System.Reflection.Assembly CompiledAssembly
		{
			get { return _assembly; }
		}
		
		private string _ClassName = string.Empty;
		/// <summary>
		/// 
		/// </summary>
		public string ClassName
		{
			get { return _ClassName; }
			set { _ClassName = value; }
		}
		#endregion

		/// <summary>
		/// Compile the sharp code in SourceString and link it with the assemblies
		/// provided in the parameter LinkedAssemblies
		/// </summary>
		/// <param name="SourceString">the code to compile as string</param>
		/// <param name="LinkedAssemblies">assemblies to link with separated by semi-column ';'</param>
		/// <returns>true if success, else false and you can call GetLastError</returns>
		public bool Compile( string SourceString, string LinkedAssemblies )
		{
			CodeDomProvider codeProvider = new Microsoft.CSharp.CSharpCodeProvider();

			System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
			parameters.GenerateInMemory = true;
			parameters.GenerateExecutable = false;

			// add the linked assemblies
			foreach( string assembly in LinkedAssemblies.Split( ';' ) )
				parameters.ReferencedAssemblies.Add( assembly );
			
			// compile
			CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, SourceString);

			//if errors 
			if (results.Errors.Count > 0)
			{
				_ErrorMsg = new StringBuilder();

				// append all errors 
				foreach(CompilerError CompErr in results.Errors)
				{
					_ErrorMsg.AppendFormat( "Line: {0} , Code: {1}, Msg: {2}", 
						CompErr.Line, CompErr.ErrorNumber, CompErr.ErrorText );

					Tracer.Error( "CSharpCodeCompiler", "Line: {0} , Code: {1}, Msg {2}", CompErr.Line, CompErr.ErrorNumber, CompErr.ErrorText);
					//EventManager.Instance.SendMessage(SourceString);
				}
			}

			if (results.Errors.Count == 0 && results.CompiledAssembly != null)
			{
				_assembly = results.CompiledAssembly;
				return true;
			}
			else return false;
		}

		/// <summary>
		/// Create the instance of the asked type contained in the compiled assembly
		/// </summary>
		/// <param name="ObjectType"></param>
		/// <returns>true if object is created</returns>
		public bool GetInstanceOfObject( string ObjectType )
		{
			if( _assembly == null ) return false;

			try
			{		
				_objType = _assembly.GetType( ObjectType );
				if ( _objType != null )
				{
					_objInstance = _assembly.CreateInstance( ObjectType );
					return true;
				}
				else return false;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Allow to call a method of the object created with GetInstanceOfObject
		/// </summary>
		/// <param name="Method"></param>
		/// <param name="Parameters"></param>
		/// <returns></returns>
		public bool InvokeMember( string Method, object[] Parameters )
		{
			if( (_objType != null) && (_objInstance != null) )
			{
				try
				{
					MethodInfo m = _objType.GetMethod( Method );
					m.Invoke( _objInstance, Parameters );
				}
				catch
				{
					return false;
				}
				return true;
			}
			else return false;
		}
	}
}
