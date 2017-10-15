using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using ReflectionStudio.Core.Properties;
using ReflectionStudio.Core.Events;
using ReflectionStudio.Core.FileManagement;

namespace ReflectionStudio.Core.Project
{
	public class SnapshotService
	{
		#region ----------------SINGLETON----------------

		public static readonly SnapshotService Instance = new SnapshotService();

		/// <summary>
		/// Private constructor for singleton pattern
		/// </summary>
		private SnapshotService()
		{
		}
		#endregion

		#region ----------------PROPERTIES----------------

		private SnapshotWatcher _Watcher = new SnapshotWatcher();

		private Dictionary<string, List<MethodMapInfo>> _Maps = new Dictionary<string, List<MethodMapInfo>>();

		private ObservableCollection<SnapshotEntity> _Snapshots = new ObservableCollection<SnapshotEntity>();
		public ObservableCollection<SnapshotEntity> Snapshots
		{
			get { return _Snapshots; }
			set { _Snapshots = value; }
		}

		#endregion

		public void OnProjectChange(object sender, ProjectEventArgs e)
		{
			Tracer.Verbose("SnapshotService.OnProjectChange START", e.ProjectType.ToString());

			try
			{
				switch (e.ProjectType)
				{
					//case ProjectEventType.Opening:
					case ProjectEventType.Opened:
						_Watcher.Start( ProjectService.Instance.Current.SubFolderSnapshot );
						ScanForSnapshotFile( ProjectService.Instance.Current.SubFolderSnapshot );
						Tracer.Verbose("SnapshotService.OnProjectChange", "OPEN event processed");
						break;

					case ProjectEventType.Closing:
					case ProjectEventType.Closed:
						_Watcher.Stop();
						Snapshots.Clear();
						Tracer.Verbose("SnapshotService.OnProjectChange", "CLOSE event processed");
						break;
					default:
						break;
				}
			}
			catch (Exception error)
			{
				Tracer.Error("SnapshotService:OnProjectChange", error);
			}
			finally
			{
				Tracer.Verbose("SnapshotService.OnProjectChange END", e.ProjectType.ToString());
			}
		}

		public void Delete(object snapEntity)
		{
			Tracer.Verbose("InjectionProfiler.Delete START", ((SnapshotEntity)snapEntity).FullFileName);

			try
			{
				new SnapshotDAC().Delete(((SnapshotEntity)snapEntity).FullFileName);
			}
			catch (Exception error)
			{
				Tracer.Error("InjectionProfiler.Delete", error);
			}
			finally
			{
				Tracer.Verbose("InjectionProfiler.Delete END", ((SnapshotEntity)snapEntity).FullFileName);
			}
		}

		public string GetMethodMapFile( string guid )
		{
			return System.IO.Path.Combine(ProjectService.Instance.Current.SubFolderSnapshot,
				string.Format("MethodMap_{0}.xml", guid));
		}

		public void ScanForSnapshotFile(string folderPath)
		{
			Tracer.Verbose("InjectionProfiler.ScanForSnapshotFile START", folderPath);

			try
			{
				DirectoryInfo inf = new DirectoryInfo(folderPath);
				foreach (FileInfo fi in inf.GetFiles( ProjectConstants.SnapshotExtensionFilter, SearchOption.TopDirectoryOnly))
				{
					Snapshots.Add(new SnapshotEntity()
										{
											DirectoryName = fi.DirectoryName,
											FileName = fi.Name,
											Length = fi.Length,
											LastWriteTime = fi.LastWriteTime
										});
				}
			}
			catch (Exception error)
			{
				Tracer.Error("InjectionProfiler.ScanForSnapshotFile", error);
			}
			finally
			{
				Tracer.Verbose("InjectionProfiler.ScanForSnapshotFile END", folderPath);
			}
		}

		public bool LoadSnapshot(SnapshotEntity sf)
		{
			if (sf.Loaded)
			{
				Tracer.Verbose("InjectionProfiler.LoadSnapshot allready loaded", sf.FileName);
				return true;
			}

			EventDispatcher.Instance.RaiseStatus(Resources.CORE_LOADING, StatusEventType.StartProgress);

			try
			{
				if (File.Exists(sf.BinaryFullFileName))
					//everything is pre-calculated in binary format, just load it
					LoadBinary(sf);
				else
				{
					LoadXml(sf);

					TransformToAgregated(sf);

					//save as binary format all the agregated data
					SaveBinary(sf);

					sf.Loaded = true;
				}
			}
			catch (Exception error)
			{
				Tracer.Error("SnapshotService.LoadSnapshot", error);
				EventDispatcher.Instance.RaiseStatus(Resources.CORE_LOADING_ERROR, StatusEventType.StopProgress);
			}
			finally
			{
				Tracer.Verbose("InjectionProfiler.LoadSnapshot finished", sf.FileName);
				EventDispatcher.Instance.RaiseStatus(Resources.CORE_SNAPSHOT_LOADED, StatusEventType.StopProgress);
			}

			return (sf != null);
		}

		private void SaveBinary(SnapshotEntity sf)
		{
			new SnapshotDAC().SaveBinarySnapshot(sf.BinaryFullFileName, sf.CallstackAg);
		}

		private void LoadBinary(SnapshotEntity sf)
		{
			sf.CallstackAg = new SnapshotDAC().LoadBinarySnapshot(sf.BinaryFullFileName);
		}

		internal bool TransformToAgregated(SnapshotEntity sf)
		{
			Tracer.Verbose("SnapshotService.TransformToAgregated started", sf.FileName);

			try
			{
				List<CallStackInfoAgregated> result = new List<CallStackInfoAgregated>();

				foreach (CallStackInfoExtended item in sf.CallstackEx)
				{
					CallStackInfoAgregated agre = result.Find(p => p.MethodHandle == item.MethodHandle);
					if (agre != null)
					{
						agre.CallCount++;
						agre.InternalTick += item.InternalTick;
						agre.AverageTick = agre.InternalTick / agre.CallCount;
					}
					else
					{
						CallStackInfoAgregated ex = new CallStackInfoAgregated(item);
						ex.CallCount = 1;
						ex.InternalTick = item.InternalTick;
						ex.AverageTick = ex.InternalTick;
						result.Add(ex);
					}
				}

				sf.CallstackAg = result;
				sf.CurrentAgreg = result;

				return true;
			}
			catch (Exception error)
			{
				Tracer.Error("SnapshotService.TransformToAgregated", error);
				EventDispatcher.Instance.RaiseStatus(Resources.CORE_LOADING_ERROR, StatusEventType.StopProgress);

				return false;
			}
			finally
			{
				Tracer.Verbose("SnapshotService.TransformToAgregated finished", sf.FileName);
				EventDispatcher.Instance.RaiseStatus(Resources.CORE_SNAPSHOT_LOADED, StatusEventType.StopProgress);
			}
		}

		internal bool LoadXml(SnapshotEntity sf)
		{
			Tracer.Verbose("SnapshotService.LoadXml started", sf.FileName);

			try
			{
				SnapshotInfo si = new SnapshotDAC().LoadSnapshot(sf.FullFileName);

				sf.Project = si.Project;
				sf.Version = si.Version;
				sf.MethodMapFile = si.Map;

				//the maps for methods name
				if( !LoadMap(sf) ) return false;

				//calculate the data : resolve methods name, calculate times, agregate
				if( !CalculateXml(si, sf) ) return false;

				sf.Loaded = true;

				return true;
			}
			catch (Exception error)
			{
				Tracer.Error("SnapshotService.LoadXml", error);
				return false;
			}
			finally
			{
				Tracer.Verbose("SnapshotService.LoadXml finished", sf.FileName);
			}
		}

		internal bool LoadMap(SnapshotEntity sf)
		{
			Tracer.Verbose("SnapshotService.LoadMap started", sf.FileName);

			try
			{
				if (!_Maps.ContainsKey(sf.MethodMapFile))
				{
					Tracer.Info( "SnapshotService.LoadMap", Resources.CORE_SNAPSHOT_LOAD_MAP);

					List<MethodMapInfo> mapinfo = (List<MethodMapInfo>)SerializeHelper.Deserialize(GetMethodMapFile(sf.MethodMapFile),
																										typeof(List<MethodMapInfo>));
					if (mapinfo != null)
						this._Maps.Add(sf.MethodMapFile, mapinfo);
				}
				return true;
			}
			catch (Exception error)
			{
				Tracer.Error("SnapshotService.LoadMap", error);
				return false;
			}
			finally
			{
				Tracer.Verbose("SnapshotService.LoadMap finished", sf.FileName);
			}
		}

		internal bool CalculateXml(SnapshotInfo si, SnapshotEntity sf)
		{
			Tracer.Verbose("SnapshotService.CalculateXml started", sf.FileName);

			try
			{
				//calculate the data : resolve methods name, calculate times, agregate
				Tracer.Info("SnapshotService.CalculateXml", Resources.CORE_SNAPSHOT_CALCULATE);
				sf.CallstackEx = CalculateSnapshot(si.CallStack);

				Tracer.Info("SnapshotService.CalculateXml", Resources.CORE_SNAPSHOT_RESOLVE_MAP);
				ResolveMethodNames(sf);

				return true;
			}
			catch (Exception error)
			{
				Tracer.Error("SnapshotService.CalculateXml", error);
				return false;
			}
			finally
			{
				Tracer.Verbose("SnapshotService.CalculateXml finished", sf.FileName);
			}
		}

		internal void ResolveMethodNames(SnapshotEntity sf)
		{
			Tracer.Verbose("SnapshotService.ResolveMethodNames START", sf.FileName);

			try
			{
				List<MethodMapInfo> mapinfo;
				_Maps.TryGetValue(sf.MethodMapFile, out mapinfo);

				foreach (CallStackInfoExtended item in sf.CallstackEx)
				{
					MethodMapInfo tmp = mapinfo.Single(p => p.Handle == item.MethodHandle);
					item.Namespace = tmp.Namespace;
					item.MethodName = tmp.Method;

					tmp = mapinfo.Single(p => p.Handle == item.CalledByHandle);
					item.CalledBy = tmp.Method;

					Tracer.Info("SnapshotService.ResolveMethodNames METHOD", "Handle {0}==> Method{1} + Handle {2}==> Method {3}",
						item.MethodHandle, item.MethodName, item.CalledByHandle, item.CalledBy);
				}
			}
			catch (Exception error)
			{
				Tracer.Error("SnapshotService.ResolveMethodNames", error);
			}
			finally
			{
				Tracer.Verbose("SnapshotService.ResolveMethodNames END", sf.FileName);
			}
		}

		internal List<CallStackInfoExtended> CalculateSnapshot(List<CallStackInfo> stack)
		{
			Tracer.Verbose("SnapshotService.CalculateSnapshot", "START calculating the snapshot");

			List<CallStackInfoExtended> result = new List<CallStackInfoExtended>();

			try
			{
				IEnumerable<CallStackInfo> mainCall = stack.Where(p => p.CalledByHandle == 0);
				foreach (CallStackInfo item in mainCall)
				{
					CallStackInfoExtended ex = new CallStackInfoExtended(item);
					result.Add(ex);
					CalculateCallStackInfoIternals(result, stack, ex);
				}
			}
			catch (Exception error)
			{
				Tracer.Error("SnapshotService.CalculateSnapshot", error);
			}
			finally
			{
				Tracer.Verbose("SnapshotService.CalculateSnapshot", "END calculating the snapshot");
			}
			return result;
		}

		internal void CalculateCallStackInfoIternals(List<CallStackInfoExtended> result, List<CallStackInfo> stack, CallStackInfoExtended parent)
		{
			Tracer.Verbose("SnapshotService.CalculateCallStackInfoIternals", "START calculating method times");

			try
			{
				IEnumerable<CallStackInfo> calledBy = stack.Where(p => p.CalledByHandle == parent.MethodHandle);
				if (calledBy != null)
				{
					//when starting the internal time method is equal to total time
					parent.InternalTick = parent.TotalTick;

					foreach (CallStackInfo item in calledBy)
					{
						//we remove the time spent in each called methods
						parent.InternalTick -= item.TotalTick;

						//we create a extended callstak object based on it
						CallStackInfoExtended ex = new CallStackInfoExtended(item);
						result.Add(ex);

						//recursive call through the call stack
						CalculateCallStackInfoIternals(result, stack, ex);
					}
				}
			}
			catch (Exception error)
			{
				Tracer.Error("SnapshotService.CalculateCallStackInfoIternals", error);
			}
			finally
			{
				Tracer.Verbose("SnapshotService.CalculateCallStackInfoIternals", "END calculating method times");
			}
		}

		#region ----------------DISTANT ACCESS----------------


		#endregion
	}
}
