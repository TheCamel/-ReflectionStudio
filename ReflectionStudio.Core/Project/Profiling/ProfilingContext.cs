using System.Reflection;
using Mono.Cecil;

namespace ReflectionStudio.Core.Project
{
	internal class ProfilingContext
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ProfilingContext()
		{
			_TagType = new TypeDefinition("Profiled", "ReflectionStudio.Spy.Tag", Mono.Cecil.TypeAttributes.Class, null);
		}

		private Assembly _SpyAsm;
		/// <summary>
		/// 
		/// </summary>
		public Assembly SpyAsm
		{
			get { return _SpyAsm; }
			set { _SpyAsm = value; }
		}

		private AssemblyDefinition _SpyAsmDefinition;
		/// <summary>
		/// Spy assembly definition
		/// </summary>
		public AssemblyDefinition SpyAsmDefinition
		{
			get { return _SpyAsmDefinition; }
			set { _SpyAsmDefinition = value; }
		}

		private AssemblyNameReference _SpyAsmNameReference;
		/// <summary>
		/// Spy assembly reference
		/// </summary>
		public AssemblyNameReference SpyAsmNameReference
		{
			get { return _SpyAsmNameReference; }
			set { _SpyAsmNameReference = value; }
		}

		private TypeDefinition _TagType;
		/// <summary>
		/// The user defined type to mark an assembly as allready builded
		/// </summary>
		public TypeDefinition TagType
		{
			get { return _TagType; }
			set { _TagType = value; }
		}
		
		public MethodInfo StartMethodInfo;
		public MethodReference StartMethodRef;
		
		public MethodInfo EndMethodInfo;
		public MethodReference EndMethodRef;

		public MethodInfo StartEndMethodInfo;
		public MethodReference StartEndMethodRef;
	}
}
