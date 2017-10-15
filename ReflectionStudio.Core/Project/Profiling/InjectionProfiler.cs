using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using Mono.Cecil;
using Mono.Cecil.Cil;
using ReflectionStudio.Core.Reflection.Types;
using ReflectionStudio.Core.Helpers;
using ReflectionStudio.Core.Project.Settings;
using ReflectionStudio.Core.Events;
using ReflectionStudio.Core.Properties;

namespace ReflectionStudio.Core.Project
{
	internal class ProfilingStatus
	{
		public int IgnoredMethods;
		public int ProfiledMethods;
		//public int MethodHandleCounter;
	}

	/// <summary>
	/// Class to inject profiling trace 
	/// </summary>
	internal class InjectionProfiler : ProjectWorker
	{
		#region ----------------------INTERNALS----------------------

		internal const string ProfilingAssembly = "ReflectionStudio.Spy.dll";
		internal const string ProfilingClass = "ReflectionStudio.Spy.Performance";

		internal const string ProfilingStartMethod = "StartEvent";
		internal const string ProfilingEndMethod = "EndEvent";
		internal const string ProfilingStartEndMethod = "StartEndEvent";

		private ProfilingStatus _Status = null;
		internal ProfilingStatus Status
		{
			get { return _Status; }
			set { _Status = value; }
		}

		private ProfilingContext _Context = null;

		#endregion

		#region ----------------------CONSTRUCTORS----------------------

		public InjectionProfiler(BackgroundWorker worker, DoWorkEventArgs e)
			: base(worker, e)
		{
			Project = (ProjectEntity)e.Argument;
		}

		#endregion

		#region ----------------------PUBLIC FUNCTION----------------------
		/// <summary>
		/// Start the profiling of a given project
		/// </summary>
		/// <param name="project">the project to profile</param>
		/// <param name="worker">the background worker, can be null</param>
		/// <param name="e">the event associated to the background worker, can be null</param>
		/// <returns></returns>
		public void Build()
		{
			Tracer.Verbose("InjectionProfiler.Build", "START");
			
			_Status = new ProfilingStatus();
			ProfileAssemblies();

			Tracer.Verbose("InjectionProfiler.Build", "END");
		}
		#endregion

		#region ----------------------INTERNAL FUNCTIONS----------------------

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private void PrepareContext()
		{
			Tracer.Verbose("InjectionProfiler.PrepareContext", "START");

			//create a new context
			_Context = new ProfilingContext();

			try
			{
				// TODO chemin !
				// get the references to the runtime tracer
				string prof = Path.Combine(PathHelper.ApplicationPath, ProfilingAssembly);

				_Context.SpyAsm = System.Reflection.Assembly.LoadFrom(prof);
				_Context.SpyAsmDefinition = AssemblyFactory.GetAssembly(prof);
				_Context.SpyAsmNameReference = new AssemblyNameReference(_Context.SpyAsmDefinition.Name.Name,
																		_Context.SpyAsmDefinition.Name.Culture,
																		_Context.SpyAsmDefinition.Name.Version);
				_Context.SpyAsmNameReference.PublicKeyToken = _Context.SpyAsmDefinition.Name.PublicKeyToken;

				//get a reference to the logger class
				Type type = _Context.SpyAsm.GetType(ProfilingClass);

				//and the timer methods
				_Context.StartMethodInfo = type.GetMethod(ProfilingStartMethod,
					System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
					Type.DefaultBinder, new Type[] { typeof(int), typeof(int) }, null);

				_Context.EndMethodInfo = type.GetMethod(ProfilingEndMethod,
					System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
					Type.DefaultBinder, new Type[] { }, null);

				//and the counter method
				_Context.StartEndMethodInfo = type.GetMethod(ProfilingStartEndMethod,
					System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
					Type.DefaultBinder, new Type[] { typeof(int), typeof(int) }, null);
			}
			catch (Exception error)
			{
				Tracer.Error("InjectionProfiler:PrepareContext", error);
			}
			finally
			{
				Tracer.Verbose("InjectionProfiler.PrepareContext", "END");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private void ProfileAssemblies()
		{
			try
			{
				foreach (NetAssembly asm in Project.Assemblies)
				{
					//if (asm.IsChecked == true)
					{
						EventDispatcher.Instance.RaiseStatus(string.Format(Resources.CORE_PROFILING_START, asm.DisplayName),
							StatusEventType.StartProgress);

						ProfileAssembly(asm);
					}

					if (CancelPending())
					{
						EventDispatcher.Instance.RaiseStatus(Resources.CORE_MAP_INTERUPT, StatusEventType.StopProgress);
						return;
					}
				}
			}
			catch (Exception error)
			{
				Tracer.Error("InjectionProfiler:ProfileAssemblies", error);
				Result = false;
			}
			finally
			{
				EventDispatcher.Instance.RaiseStatus(
					string.Format(Resources.CORE_PROFILING_RESUME,
									_Status.ProfiledMethods, _Status.IgnoredMethods), StatusEventType.StopProgress);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="AsmToProfile"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		private void ProfileAssembly(NetAssembly AsmToProfile)
		{
			Tracer.Verbose("InjectionProfiler.ProfileAssembly START ", AsmToProfile.Name);

			PrepareContext();

			try
			{
				AssemblyDefinition CecilAssDefinition = (AssemblyDefinition)AsmToProfile.Tag;

				// add a build tag class so we know later if already built
				CecilAssDefinition.MainModule.Types.Add(_Context.TagType);

				//add the runtime spy reference
				CecilAssDefinition.MainModule.AssemblyReferences.Add(_Context.SpyAsmNameReference);

				//get back the reference of enter method
				_Context.StartMethodRef = CecilAssDefinition.MainModule.Import(_Context.StartMethodInfo);
				_Context.EndMethodRef = CecilAssDefinition.MainModule.Import(_Context.EndMethodInfo);
				_Context.StartEndMethodRef = CecilAssDefinition.MainModule.Import(_Context.StartEndMethodInfo);

				//for each type we get in our tree
				foreach (NetClass classTyp in AsmToProfile.Children)
				{
					//if need to be profiled
					if (classTyp.IsChecked == true)
					{
						Tracer.Info( "InjectionProfiler.ProfileAssembly", string.Format(Resources.CORE_PROFILING_CLASS, classTyp.Name));

						foreach (NetMethod methodType in classTyp.Children)
						{
							if (methodType.IsChecked == true)
								this.PrepareMethod(CecilAssDefinition, methodType);
						}

						CecilAssDefinition.MainModule.Import((TypeDefinition)classTyp.Tag);
					}

					if (CancelPending()) return;
				}

				// TODO
				// gestion chemin
				string ProfiledFilePath = Path.Combine(Project.SubFolderProfiled, new FileInfo(AsmToProfile.FilePath).Name);

				//call cecil to save it
				AssemblyFactory.SaveAssembly(CecilAssDefinition, ProfiledFilePath);

				Tracer.Info("InjectionProfiler.ProfileAssembly", string.Format(Resources.CORE_PROFILING_SAVE, AsmToProfile.Name));
			}
			catch (Exception error)
			{
				Tracer.Error("InjectionProfiler:ProfileAssembly", error);
				Result = false;
			}
			finally
			{
				Tracer.Verbose("InjectionProfiler.ProfileAssembly END", AsmToProfile.Name);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="method"></param>
		/// <param name="rtmodule"></param>
		private bool MustSkipMethod(MethodDefinition method)
		{
			Tracer.Verbose("InjectionProfiler.MustSkipMethod START ", method.Name);

			try
			{
				if (!method.HasBody || (method.Body == null))		//empty body
				{
					this._Status.IgnoredMethods++;
					return true;
				}
				if (method.Body.Instructions.Count == 1)			//empty method
				{
					this._Status.IgnoredMethods++;
					return true;
				}

				//skip event listenner
				if ((method.IsSpecialName && (method.Name.StartsWith("add_") || method.Name.StartsWith("remove_")))
					&& (method.Body.Instructions.Count <= 8))
				{
					this._Status.IgnoredMethods++;
					return true;
				}

				//skip small methods
				if (Project.Settings.SkipSmallMethods)
				{
					if (CecilHelper.IsSmallMethod(method.Body))
					{
						this._Status.IgnoredMethods++;
						return true;
					}
				}
			}
			catch (Exception error)
			{
				Tracer.Error("InjectionProfiler:ProfileAssembly", error);
				return false;
			}
			finally
			{
				Tracer.Verbose("InjectionProfiler.MustSkipMethod" , "END" );
			}

			this._Status.ProfiledMethods++;
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="method"></param>
		/// <param name="rtmodule"></param>
		private void PrepareMethod(AssemblyDefinition assembly, NetMethod method)
		{
			Tracer.Verbose("InjectionProfiler.PrepareMethod START ", method.Name);

			// no profiling on this method
			if (MustSkipMethod((MethodDefinition)method.Tag))
				return;

			//trace the number of call in methods
			if (Project.Settings.Action == ProfilMode.CallCount)
				PrepareMethodForCallCount(method);

			//trace for performance
			if (Project.Settings.Action == ProfilMode.TimeSpent)
				PrepareMethodForTiming( assembly, method );

			Tracer.Verbose("InjectionProfiler.PrepareMethod END ", method.Name);
		}

		private void PrepareMethodForTiming(AssemblyDefinition assembly, NetMethod method)
		{
			MethodDefinition CecilType = (MethodDefinition)method.Tag;
			MethodBody body = CecilType.Body;

			BodyWorker bw = new BodyWorker(body);

			VariableDefinition definition = null;
			if (CecilType.ReturnType.ReturnType != assembly.MainModule.TypeReferences["System.Void"])
			{
				definition = new VariableDefinition(CecilType.ReturnType.ReturnType);
				body.Variables.Add(definition);
			}

			Instruction instruction6 = null;
			Instruction instruction7 = body.Instructions[0];
			Instruction instruction8 = bw.InsertBefore(instruction7, bw.Create(OpCodes.Ldc_I4, method.Handle));
			Instruction instruction9 = bw.AppendAfter(instruction8, bw.Create(OpCodes.Ldc_I4, Project.Settings.BuildKey));
			bw.AppendAfter(instruction9, bw.Create(OpCodes.Call, _Context.StartMethodRef));

			Instruction instruction10 = bw.OriginalInstructions[bw.OriginalInstructions.Count - 1];
			Instruction instruction11 = bw.AppendAfter(instruction10, bw.Create(OpCodes.Call, _Context.EndMethodRef));
			Instruction instruction12 = bw.AppendAfter(instruction11, bw.Create(OpCodes.Endfinally));
			Instruction outside = body.Instructions.Outside;

			//gestion des methodes de debut/fin ?
			// TODO

			foreach (Instruction original in bw.OriginalInstructions)
			{
				Instruction instruction15;

				if (!(original.OpCode == OpCodes.Ret) || (original == instruction6))
					continue;
				
				//return type exist
				if (definition != null)
				{
					//return the value
					instruction15 = bw.AppendAfter(instruction12, bw.Create(OpCodes.Ldloc, definition));
					bw.AppendAfter(instruction15, bw.Create(OpCodes.Ret));
				}
				else
				{
					//else quit
					instruction15 = bw.AppendAfter(instruction12, bw.Create(OpCodes.Ret));
				}

				foreach (Instruction instruction16 in bw.OriginalInstructions)
				{
					if ((instruction16.OpCode == OpCodes.Ret) && (instruction16 != instruction6))
					{
						if (definition != null)
						{
							Instruction instruction17 = bw.Replace(instruction16, bw.Create(OpCodes.Stloc, definition));
							bw.AppendAfter(instruction17, bw.Create(OpCodes.Leave, instruction15));
						}
						else
						{
							bw.Replace(instruction16, bw.Create(OpCodes.Leave, instruction15));
						}
					}
				}
				outside = instruction15;
				break;
			}
			bw.RemapBodyOutside(instruction11);

			bw.Done();

			//define the exception handling arround the existing code
			ExceptionHandler handler = new ExceptionHandler(ExceptionHandlerType.Finally)
			{
				TryStart = instruction7,
				TryEnd = instruction11,
				HandlerStart = instruction11,
				HandlerEnd = outside
			};

			body.ExceptionHandlers.Add(handler);

			//if return type, be sure that variables are initialized
			if (definition != null)
			{
				body.InitLocals = true;
			}
		}

		private void PrepareMethodForCallCount(NetMethod method)
		{
			MethodDefinition CecilType = (MethodDefinition)method.Tag;
			BodyWorker worker = new BodyWorker(CecilType.Body);

			Instruction beforeIns = CecilType.Body.Instructions[0];
			Instruction afterIns = worker.InsertBefore(beforeIns, worker.Create(OpCodes.Ldc_I4, method.Handle));
			Instruction instruction4 = worker.AppendAfter(afterIns, worker.Create(OpCodes.Ldc_I4, Project.Settings.BuildKey));
			
			worker.AppendAfter(instruction4, worker.Create(OpCodes.Call, _Context.StartEndMethodRef));
            worker.Done();
        }
		#endregion
	}
}
