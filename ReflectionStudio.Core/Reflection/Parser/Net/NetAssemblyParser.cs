using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ReflectionStudio.Core.Reflection.Types;
using System.IO;
using ReflectionStudio.Core.Helpers;
using System.Collections.ObjectModel;
using ReflectionStudio.Diagram.Manager.Core.Reflection;

namespace ReflectionStudio.Core.Reflection.Parser
{
	[Serializable]
	class NetAssemblyParser : IAssemblyParser
	{
		private string _temp;

		#region ----------------------CONSTRUCTOR----------------------

		/// <summary>
		/// Constructor
		/// </summary>
		public NetAssemblyParser()
		{
			AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += new ResolveEventHandler(_reflectDomain_ReflectionOnlyAssemblyResolve);
		}

		/// <summary>
		/// Delegate to handle the unresolved assemblies
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		Assembly _reflectDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
		{
			AssemblyName name = new AssemblyName(args.Name);
			String asmToCheck = _temp + "\\" + name.Name + ".dll";

			if (File.Exists(asmToCheck))
			{
				return Assembly.ReflectionOnlyLoadFrom(asmToCheck);
			}

			return Assembly.ReflectionOnlyLoad(args.Name);
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
			if ( !new AssemblyTester().isDotNetAssembly(assemblyPath) )
				return null;

			_temp = Path.GetDirectoryName(assemblyPath);

			Assembly ass = Assembly.ReflectionOnlyLoadFrom( assemblyPath );
			return new NetAssembly(ass)
				{
					Name = ass.GetName().Name,
					FilePath = assemblyPath,
//					Version = ass.Name.Version.ToString(),
//					Culture = ass..Name.Culture,
//					PublicKey = ByteHelper.ByteToHexString(AsmToProfile.Name.PublicKey),
//					PublicKeyToken = ByteHelper.ByteToHexString(AsmToProfile.Name.PublicKeyToken),
					//Hash = ByteHelper.ByteToHexString(AsmToProfile.Name.Hash),
					//HashAlgorithm = AsmToProfile.Name.HashAlgorithm.ToString(),
					EntryPoint = ass.EntryPoint != null ? ass.EntryPoint.ToString() : string.Empty
					//Kind = AsmToProfile.Kind.ToString(),
					//IsProgram = (ass.EntryPoint.ToString() != string.Empty && AsmToProfile.Kind.ToString() != string.Empty)
				};
		}

		/// <summary>
		/// Load all object in the given assembly
		/// </summary>
		/// <param name="assemblyPath"></param>
		/// <returns></returns>
		public NetAssembly LoadAssemblyRecursively(string assemblyPath)
		{
			//load the assembly
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
			foreach (AssemblyName assemblyName in ((Assembly)assembly.Tag).GetReferencedAssemblies())
			{
				list.Add(new NetReference(assemblyName) { Parent=assembly, Name = assemblyName.Name } );
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
			
			foreach (Type typ in ((Assembly)assembly.Tag).GetTypes())
			{
				// get the namespace from the types
				if (assembly.Namespaces.Where(p => p.Name == typ.Namespace).Count() == 0)
				{
					assembly.Namespaces.Add(new NetNamespace(null) { Parent = assembly, Name = typ.Namespace } );
				}

				if (typ.IsClass)
				{
					list.Add(new NetClass(typ)
					{
						Name = typ.Name,
						BaseType = new NetBaseType(typ.BaseType),
						NameSpace = typ.Namespace
					});
				}
				else if (typ.IsInterface)
				{
					list.Add(new NetInterface(typ)
					{
						Name = typ.Name,
						NameSpace = typ.Namespace
					});
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
			ObservableCollection<BindableObjectExtended> list = new ObservableCollection<BindableObjectExtended>();
			list = GetAssemblyTypes(assembly);
			
			//load type methods
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
			BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static
				| BindingFlags.Instance | BindingFlags.DeclaredOnly;

			ObservableCollection<BindableObjectExtended> list = new ObservableCollection<BindableObjectExtended>();

			ConstructorInfo[] cinfo = ((Type)type.Tag).GetConstructors(flags);
			foreach (ConstructorInfo mi in cinfo)
			{
				list.Add(new NetMethod( MethodType.Constructor, mi) { Name = mi.Name });
			}

			//MemberInfo[] memberinfo = ((Type)type.Tag).GetMembers(flags);
			//foreach (MemberInfo mi in memberinfo)
			//{
			//    list.Add(new NetMethod(mi) { Name = mi.Name });
			//}

			FieldInfo[] fieldinfo = ((Type)type.Tag).GetFields(flags);
			foreach (FieldInfo mi in fieldinfo)
			{
				list.Add(new NetMethod(MethodType.Field, mi) { Name = mi.Name });
			}

			PropertyInfo[] propinfo = ((Type)type.Tag).GetProperties(flags);
			foreach (PropertyInfo mi in propinfo)
			{
				list.Add(new NetMethod(MethodType.Property, mi) { Name = mi.Name });
			}

			MethodInfo[] methodinfo = ((Type)type.Tag).GetMethods(flags);
			foreach (MethodInfo mi in methodinfo)
			{
				if (!mi.Name.Contains("get_") && !mi.Name.Contains("set_"))
					list.Add(new NetMethod(MethodType.Method, mi) { Name = mi.Name });
			}

			Type[] inter = ((Type)type.Tag).GetInterfaces();
			foreach (Type mi in inter)
			{
				list.Add(new NetMethod(MethodType.Interface, mi) { Name = mi.Name });
			}

			EventInfo[] eventinfo = ((Type)type.Tag).GetEvents();
			foreach (EventInfo mi in eventinfo)
			{
				list.Add(new NetMethod(MethodType.Event, mi) { Name = mi.Name });
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
			
		}

		/// <summary>
		/// Refresh a assembly element and all subtypes if loaded
		/// </summary>
		/// <param name="node"></param>
		public void RefreshNode(NetBaseType node)
		{
		}

		#endregion
	}
}
