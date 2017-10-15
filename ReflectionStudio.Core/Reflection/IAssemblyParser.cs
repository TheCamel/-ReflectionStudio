using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReflectionStudio.Core.Reflection.Types;
using System.Collections.ObjectModel;

namespace ReflectionStudio.Core.Reflection
{
	public interface IAssemblyParser
	{
		#region ----------------------PARSING FUNCTIONS----------------------

		/// <summary>
		/// Load an assembly
		/// </summary>
		/// <param name="assemblyPath"></param>
		/// <returns></returns>
		NetAssembly LoadAssembly(string assemblyPath);

		/// <summary>
		/// Load an assembly recusively with all subtypes
		/// </summary>
		/// <param name="assemblyPath"></param>
		/// <returns></returns>
		NetAssembly LoadAssemblyRecursively(string assemblyPath);

		/// <summary>
		/// Load references
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		ObservableCollection<NetReference> GetAssemblyReferences(NetAssembly assembly);

		/// <summary>
		/// Load all assembly subtypes
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		ObservableCollection<BindableObjectExtended> GetAssemblyTypes(NetAssembly assembly);

		/// <summary>
		/// Load all assembly subtypes and members
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		ObservableCollection<BindableObjectExtended> GetAssemblyTypesRecursively(NetAssembly assembly);

		/// <summary>
		/// Load all members of a given type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		ObservableCollection<BindableObjectExtended> GetMembers(NetBaseType type);

		#endregion

		#region ----------------------REFRESHING FUNCTIONS----------------------

		/// <summary>
		/// Refresh an assembly and all subtypes if loaded
		/// </summary>
		/// <param name="assembly"></param>
		void RefreshAssembly(NetAssembly assembly);

		/// <summary>
		/// Refresh a assembly element and all subtypes if loaded
		/// </summary>
		/// <param name="node"></param>
		void RefreshNode(NetBaseType node);

		#endregion
	}
}
