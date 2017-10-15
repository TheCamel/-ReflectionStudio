using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ReflectionStudio.Core.Helpers
{
	public class CecilHelper
	{
		//----------------------CECIL----------------------
		internal static readonly string CecilModul = "<Module>";

		internal static AssemblyDefinition GetAssembly(string pathFile)
		{
			AssemblyDefinition AsmToProfile = null;

			try
			{
				AsmToProfile = AssemblyFactory.GetAssembly(pathFile);
			}
			catch (Exception exception)
			{
				if (exception.Message != "The image is not a managed assembly")
				{
				}
			}
			return AsmToProfile;
		}

		internal static bool HasCustomAttribute(CustomAttributeCollection cac, string fullname)
		{
			if (cac.Count != 0)
			{
				foreach (CustomAttribute attribute in cac)
				{
					if (attribute.Constructor.DeclaringType.FullName == fullname)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool IsSmallMethod(MethodBody body)
		{
			if (body.Instructions.Count == 0)
				return true;
			
			Instruction ins = body.Instructions[0];
			if (ins.OpCode == OpCodes.Nop)
				ins = ins.Next;
			
			if (MatchCILSequence(ins, new OpCode[] { OpCodes.Ret }))
				return true;
			
			if (MatchCILSequence(ins, new OpCode[] { OpCodes.Ldarg_0, OpCodes.Stsfld, OpCodes.Ret }))
				return true;
			
			if (MatchCILSequence(ins, new OpCode[] { OpCodes.Ldsfld, OpCodes.Stloc_0, OpCodes.Br_S, OpCodes.Ldloc_0, OpCodes.Ret }))
				return true;
		
			if (MatchCILSequence(ins, new OpCode[] { OpCodes.Ldarg_0, OpCodes.Ldarg_1, OpCodes.Stfld, OpCodes.Ret }))
				return true;
			
			if (MatchCILSequence(ins, new OpCode[] { OpCodes.Ldarg_0, OpCodes.Ldfld, OpCodes.Stloc_0, OpCodes.Br_S, OpCodes.Ldloc_0, OpCodes.Ret }))
				return true;
			
			if (((((ins.OpCode == OpCodes.Ldstr) || (ins.OpCode == OpCodes.Ldnull)) || ((ins.OpCode == OpCodes.Ldc_I4) || (ins.OpCode == OpCodes.Ldc_I4_0))) || (((ins.OpCode == OpCodes.Ldc_I4_1) || (ins.OpCode == OpCodes.Ldc_I4_2)) || ((ins.OpCode == OpCodes.Ldc_I4_3) || (ins.OpCode == OpCodes.Ldc_I4_4)))) || ((((ins.OpCode == OpCodes.Ldc_I4_5) || (ins.OpCode == OpCodes.Ldc_I4_6)) || ((ins.OpCode == OpCodes.Ldc_I4_7) || (ins.OpCode == OpCodes.Ldc_I4_8))) || (((ins.OpCode == OpCodes.Ldc_I4_M1) || (ins.OpCode == OpCodes.Ldc_I4_S)) || (((ins.OpCode == OpCodes.Ldc_I8) || (ins.OpCode == OpCodes.Ldc_R4)) || (ins.OpCode == OpCodes.Ldc_R8)))))
			{
				if (MatchCILSequence(ins.Next, new OpCode[] { OpCodes.Stloc_0, OpCodes.Br_S, OpCodes.Ldloc_0, OpCodes.Ret }))
					return true;
			}
			
			return false;
		}

		internal static bool MatchCILSequence(Instruction ins, params OpCode[] sequence)
		{
			foreach (OpCode code in sequence)
			{
				if (ins.OpCode != code)
				{
					return false;
				}
				ins = ins.Next;
			}
			return true;
		}

		internal static string GetMethodParamSignature(MethodDefinition methodDef)
		{
			string result= string.Empty;

			foreach (ParameterDefinition def in methodDef.Parameters)
				result += def.ParameterType.Name + ";";

			result += methodDef.ReturnType.ReturnType.Name;

			return result;
		}

		internal static string GetMethodParam(MethodDefinition methodDef)
		{
			string result = string.Empty;

			//foreach (ParameterDefinition def in methodDef.Parameters)
			//    result += def.ParameterType.Name + ", ";

			for (int i = 0; i < methodDef.Parameters.Count; i++)
			{
				ParameterDefinition def = methodDef.Parameters[i];
				result += def.ParameterType.Name;
				if( i < methodDef.Parameters.Count-1 )
					result += ", ";
			}

			return result;
		}


		internal static string MethodDisplayName(MethodDefinition methodDef)
		{
			if (methodDef != null)
			{
				if (methodDef.ReturnType.ReturnType.ToString().Equals("System.Void"))
					//return methodDef.ToString().Substring(methodDef.ToString().IndexOf("::") + 2);
					return string.Format("{0}({1})", methodDef.Name, GetMethodParam(methodDef));
				else
					//return methodDef.ToString().Substring(methodDef.ToString().IndexOf("::") + 2) + " : " + methodDef.ReturnType.ReturnType.Name;
					return string.Format("{0}({1}) : {2}", methodDef.Name, GetMethodParam(methodDef), methodDef.ReturnType.ReturnType.Name);
			}
			else
				return string.Empty;
		}
	}
}
