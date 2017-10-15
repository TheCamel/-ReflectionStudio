using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ReflectionStudio.Core.Events;
using System.Windows.Threading;
using System.Threading;

namespace ReflectionStudio.Core.Project
{
	internal class SnapshotWatcher : DispatcherObject
	{
		#region ----------------CONSTRUCTOR------------

		/// <summary>
		/// Private constructor for singleton pattern
		/// </summary>
		public SnapshotWatcher()
		{
			//Subscribe to the Created event.
			_Watcher.Created += new FileSystemEventHandler(WatcherEvent);
			_Watcher.Changed += new FileSystemEventHandler(WatcherEvent);
			_Watcher.Deleted += new FileSystemEventHandler(WatcherEvent);

			//Set the filter to only catch snap files.
			_Watcher.Filter = ProjectConstants.SnapshotExtensionFilter;
		}
		#endregion

		#region ----------------PROPERTIES----------------

		private FileSystemWatcher _Watcher = new FileSystemWatcher();
		public FileSystemWatcher Watcher
		{
			get { return _Watcher; }
			set { _Watcher = value; }
		}

		#endregion

		private void WatcherEvent(object sender, FileSystemEventArgs e)
		{
			Tracer.Verbose("InjectionProfiler.WatcherEvent START", e.Name);

			try
			{
				if (e.ChangeType == WatcherChangeTypes.Created)
				{
					FileInfo fi = new FileInfo(e.FullPath);
					SnapshotEntity entity = new SnapshotEntity()
					{
						DirectoryName = fi.DirectoryName,
						FileName = fi.Name,
						Length = fi.Length,
						LastWriteTime = fi.LastWriteTime
					};

					base.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
					{
						SnapshotService.Instance.Snapshots.Add(entity);
					});
				}

				if (e.ChangeType == WatcherChangeTypes.Deleted)
					base.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
					{
						SnapshotService.Instance.Snapshots.Remove(SnapshotService.Instance.Snapshots.Single(p => p.FullFileName == e.FullPath));
					});

				if (e.ChangeType == WatcherChangeTypes.Changed)
				{
					SnapshotEntity se = SnapshotService.Instance.Snapshots.Single(p => p.FullFileName == e.FullPath);

					if (se.Loaded)
					{
						base.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
						{
							EventDispatcher.Instance.RaiseProject(null, ProjectEventType.SnapshotChange);
							// TODO !
							//if (MessageBoxDlg.Show(string.Format("Snapshot {0} has changed! Do you want to reload it ?", se.FileName),
							//    "Warning", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question) == System.Windows.MessageBoxResult.Yes)
							//{
							//    se.Loaded = false;
							//    LoadSnapshot(se);
							//}
						});
					}
				}
			}
			catch (Exception error)
			{
				Tracer.Error("InjectionProfiler.WatcherEvent", error);
			}
			finally
			{
				Tracer.Verbose("InjectionProfiler.WatcherEvent END", e.Name);
			}
		}

		internal void Start(string path2Watch)
		{
			_Watcher.Path = path2Watch;
			_Watcher.EnableRaisingEvents = true;
		}

		internal void Stop()
		{
			_Watcher.EnableRaisingEvents = false;
		}
	}
}
