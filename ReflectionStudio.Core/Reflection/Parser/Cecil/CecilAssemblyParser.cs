using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Mono.Cecil;
using ReflectionStudio.Core.Helpers;
using ReflectionStudio.Core.Reflection.Types;
using System.Collections;

namespace ReflectionStudio.Core.Reflection
{
	public class CecilAssemblyParser : IAssemblyParser
	{
		#region ----------------------CONSTRUCTORS----------------------

		/// <summary>
		/// Constructor
		/// </summary>
		public CecilAssemblyParser()
		{
		}

		#endregion

		#region ----------------------PARSING FUNCTIONS----------------------

		/// <summary>
		/// Load the assembly object only
		/// </summary>
		/// <param name="assemblyPath"></param>
		/// <returns></returns>
		public NetAssembly LoadAssembly(string assemblyPath)
		{
			// get the cecil assembly
			AssemblyDefinition AsmToProfile = CecilHelper.GetAssembly(assemblyPath);
			if (AsmToProfile == null)
				return null;

			return new NetAssembly(AsmToProfile)
			{
				Name = AsmToProfile.Name.Name,
				FilePath = assemblyPath,
				Version = AsmToProfile.Name.Version.ToString(),
				Culture = AsmToProfile.Name.Culture,
				PublicKey = ByteHelper.ByteToHexString(AsmToProfile.Name.PublicKey),
				PublicKeyToken = ByteHelper.ByteToHexString(AsmToProfile.Name.PublicKeyToken),
				Hash = ByteHelper.ByteToHexString(AsmToProfile.Name.Hash),
				HashAlgorithm = AsmToProfile.Name.HashAlgorithm.ToString(),
				EntryPoint = AsmToProfile.EntryPoint != null ? AsmToProfile.EntryPoint.ToString() : string.Empty,
				Kind = AsmToProfile.Kind != null ? AsmToProfile.Kind.ToString() : string.Empty
				//IsProgram = (AsmToProfile.EntryPoint.ToString() != string.Empty && AsmToProfile.Kind.ToString() != string.Empty)
			};
		}

		/// <summary>
		/// Load all object in the given assembly
		/// </summary>
		/// <param name="assemblyPath"></param>
		/// <returns></returns>
		public NetAssembly LoadAssemblyRecursively(string assemblyPath)
		{
			NetAssembly ass = LoadAssembly(assemblyPath);

			//load the references
			ass.References = GetAssemblyReferences(ass);

			//load the type in the assembly
			ass.Children = GetAssemblyTypesRecursively(ass);
			
			return ass;
		}

		/// <summary>
		/// Load the references of the given assembly
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public ObservableCollection<NetReference> GetAssemblyReferences(NetAssembly assembly)
		{
			ObservableCollection<NetReference> list = new ObservableCollection<NetReference>();

			foreach (ModuleDefinition mod in ((AssemblyDefinition)assembly.Tag).Modules)
			{
				foreach (AssemblyNameReference assRef in mod.AssemblyReferences)
					list.Add(new NetReference(assRef) { Name = assRef.Name } );
			}
			return list;
		}

		/// <summary>
		/// Load all the type in the given assembly
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public ObservableCollection<BindableObjectExtended> GetAssemblyTypes(NetAssembly assembly)
		{
			ObservableCollection<BindableObjectExtended> list = new ObservableCollection<BindableObjectExtended>();

			//load the type in the assembly
			foreach (TypeDefinition modulType in ((AssemblyDefinition)assembly.Tag).MainModule.Types)
			{
				if (modulType.Name == CecilHelper.CecilModul)
					continue;

				if (modulType.IsClass && !modulType.IsEnum)
				{
					list.Add(AddClass(assembly, modulType));
				}
			}

			return list;
		}

		/// <summary>
		/// Load all the types and subtypes in the given assembly
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public ObservableCollection<BindableObjectExtended> GetAssemblyTypesRecursively(NetAssembly assembly)
		{
			//get the type in the assemblu
			ObservableCollection<BindableObjectExtended> list = new ObservableCollection<BindableObjectExtended>();
			list = GetAssemblyTypes(assembly);

			//then load type methods
			foreach (NetBaseType typ in list)
			{
				typ.Children = (ObservableCollection<BindableObjectExtended>)GetMembers(typ);
			}

			return list;
		}

		/// <summary>
		/// Get all members in the given type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public ObservableCollection<BindableObjectExtended> GetMembers(NetBaseType type)
		{
			ObservableCollection<BindableObjectExtended> list = new ObservableCollection<BindableObjectExtended>();

			TypeDefinition modulType = (TypeDefinition)type.Tag;

			ProcessCecilMethod(modulType.Constructors, type);
			ProcessCecilMethod(modulType.Methods, type);

			foreach (MethodDefinition constructorType in modulType.Constructors)
			{
				list.Add(AddMethod(MethodType.Constructor, constructorType)); 
			}
			foreach (MethodDefinition methodType in modulType.Methods)
			{
				list.Add(new NetMethod(MethodType.Method, methodType) { Name = methodType.Name });
			}

			return list;
		}
		
		#endregion

		#region ----------------------REFRESHING FUNCTIONS----------------------

		/// <summary>
		/// Refresh an assembly and all subtypes if loaded
		/// </summary>
		/// <param name="assembly"></param>
		public void RefreshAssembly(NetAssembly assembly)
		{
			//reload cecil assembly definition
			assembly.Tag = CecilHelper.GetAssembly(assembly.FilePath);

			//for each type in cecil assembly,  refresh or load it
			foreach (TypeDefinition modulType in ((AssemblyDefinition)assembly.Tag).MainModule.Types)
			{
				if (modulType.Name == CecilHelper.CecilModul)
					continue;

				if (modulType.IsClass)
				{
					if (!RefreshClass(assembly.Children, modulType))
						AddClass(assembly, modulType);
				}
			}
		}

		/// <summary>
		/// Refresh a assembly element and all subtypes if loaded
		/// </summary>
		/// <param name="node"></param>
		public void RefreshNode(NetBaseType node)
		{

		}

		#endregion

		#region ----------------------INTERNAL FUNCTIONS----------------------

		private NetClass AddClass(NetAssembly assembly, TypeDefinition modulType)
		{
			return new NetClass(modulType)
			{
				Name = modulType.Name,
				BaseType = new NetBaseType(modulType.BaseType),
				NameSpace = modulType.Namespace,
				Parent = assembly
			};
		}

		private bool RefreshClass(ObservableCollection<BindableObjectExtended> childs, TypeDefinition typeDef)
		{
			if (childs == null || childs.Count <= 0)
				return false;

			//find the type in cecil in our NetClass collection
			foreach (NetClass classTyp in childs)
			{
				if (classTyp.Name == typeDef.FullName)
				{
					classTyp.Tag = typeDef;

					ProcessCecilMethod(typeDef.Constructors, classTyp);
					ProcessCecilMethod(typeDef.Methods, classTyp);

					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Loop through a cecil methods collection
		/// </summary>
		/// <param name="cecilMethods"></param>
		/// <param name="classTyp"></param>
		private void ProcessCecilMethod(CollectionBase cecilMethods, NetBaseType classTyp)
		{
			foreach (MethodDefinition def in cecilMethods)
			{
				if (!RefreshMethod(classTyp.Children, def))
				{
					if( cecilMethods.GetType() == typeof(ConstructorCollection) )
						AddMethod(MethodType.Constructor, def);
					if (cecilMethods.GetType() == typeof(MethodDefinitionCollection))
						AddMethod(MethodType.Method, def);
					if (cecilMethods.GetType() == typeof(PropertyDefinitionCollection))
						AddMethod(MethodType.Property, def);
					if (cecilMethods.GetType() == typeof(EventDefinitionCollection))
						AddMethod(MethodType.Event, def);
					if (cecilMethods.GetType() == typeof(FieldDefinitionCollection))
						AddMethod(MethodType.Field, def);
					if (cecilMethods.GetType() == typeof(InterfaceCollection))
						AddMethod(MethodType.Interface, def);

					//process nested types
					//if (cecilMethods.GetType() == typeof(NestedTypeCollection))
				}
			}
		}

		/// <summary>
		/// Loop through existing class children to remove the ones that does not exist in Cecil
		/// </summary>
		/// <param name="childs"></param>
		private void RemoveClass(ObservableCollection<BindableObjectExtended> childs)
		{
			List<BindableObjectExtended> temp = new List<BindableObjectExtended>();

			//loop on types
			foreach (NetBaseType classTyp in childs)
			{
				//if tag is null, means not founded in cecil
				if (classTyp.Tag == null)
				{
					temp.Add(classTyp);
				}
				else
				{
					RemoveMethods(classTyp.Children);
				}
			}

			//remove them all
			foreach (BindableObjectExtended item in temp)
				childs.Remove(item);
		}

		private NetMethod AddMethod(MethodType typ ,MethodDefinition constructorType)
		{
			return new NetMethod(typ, constructorType)
			{
				Name = constructorType.Name,
				NameSpace = CecilHelper.GetMethodParamSignature(constructorType)
			};
		}

		private bool RefreshMethod(ObservableCollection<BindableObjectExtended> childs, MethodDefinition methodDef)
		{
			foreach (NetMethod methodTyp in childs)
			{
				if (methodTyp.Name == methodDef.Name)
				{
					if (methodTyp.NameSpace == CecilHelper.GetMethodParamSignature(methodDef))
					{
						methodTyp.Tag = methodDef;
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Loop through existing methods children to remove the ones that does not exist in Cecil
		/// </summary>
		/// <param name="childs"></param>
		private void RemoveMethods(ObservableCollection<BindableObjectExtended> childs)
		{
			List<BindableObjectExtended> temp = new List<BindableObjectExtended>();

			foreach (NetMethod methodTyp in childs)
			{
				if (methodTyp.Tag == null)
				{
					temp.Add(methodTyp);
				}
			}

			foreach (BindableObjectExtended item in temp)
				childs.Remove(item);
		}

		#endregion
	}
}
