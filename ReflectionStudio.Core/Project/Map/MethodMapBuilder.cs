using System;
using System.Collections.Generic;
using System.ComponentModel;
using ReflectionStudio.Core.Events;
using ReflectionStudio.Core.FileManagement;
using ReflectionStudio.Core.Properties;
using ReflectionStudio.Core.Reflection.Types;

namespace ReflectionStudio.Core.Project
{
	internal class MethodMapBuilder : ProjectWorker
	{
		#region ----------------------INTERNALS----------------------

		private int _HandleCounter = 0;

		#endregion

		#region ----------------------CONSTRUCTORS----------------------

		public MethodMapBuilder(BackgroundWorker worker, DoWorkEventArgs e)
			: base(worker, e)
		{
		}

		#endregion

		#region ----------------------PUBLIC FUNCTION----------------------

		public void Build()
		{
			try
			{
				EventDispatcher.Instance.RaiseStatus(Resources.CORE_MAP_START, StatusEventType.StartProgress);

				List<MethodMapInfo> mapList = new List<MethodMapInfo>();
				_HandleCounter = 0;

				foreach (NetAssembly asm in Project.Assemblies)
				{
					if (asm.IsChecked == true)
					{
						EventDispatcher.Instance.RaiseStatus(string.Format(Resources.CORE_MAP_METHOD, asm.DisplayName), StatusEventType.StartProgress);

						//compile to the list
						AddAssemblyMap(asm, mapList);
					}

					if (CancelPending())
					{
						EventDispatcher.Instance.RaiseStatus(Resources.CORE_MAP_INTERUPT, StatusEventType.StopProgress);
						return;
					}
				}

				// save it
				SerializeHelper.Serialize(Project.MethodMapFile, mapList);
			}
			catch (Exception error)
			{
				Tracer.Error("MethodMapBuilder:Build", error);
				Result = false;
			}
			finally
			{
				EventDispatcher.Instance.RaiseStatus(Resources.CORE_MAP_STOP, StatusEventType.StopProgress);
			}
		}

		#endregion

		/// <summary>
		/// Add all needed types of this assembly into the list
		/// </summary>
		/// <param name="asm"></param>
		/// <param name="mapList"></param>
		private void AddAssemblyMap(NetAssembly asm, List<MethodMapInfo> mapList)
		{
			Tracer.Verbose("MethodMapBuilder.AddAssemblyMap", asm.Name);

			try
			{
				foreach (NetClass classTyp in asm.Children)
				{
					foreach (NetMethod methodType in classTyp.Children)
					{
						methodType.Handle = _HandleCounter++;
						mapList.Add
							(
								new MethodMapInfo()
								{
									Handle = methodType.Handle,
									Method = methodType.DisplayName,
									Namespace = classTyp.Name
								}
							);
						Tracer.Verbose("MethodMapBuilder.AddAssemblyMap / Add type ==> ", methodType.Name);
					}
				}
			}
			catch (Exception error)
			{
				Tracer.Error("MethodMapBuilder.AddAssemblyMap", error);
			}
		}
	}
}
