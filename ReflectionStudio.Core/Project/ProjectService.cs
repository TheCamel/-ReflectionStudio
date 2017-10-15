using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using ReflectionStudio.Core.Properties;
using ReflectionStudio.Core.Events;
using ReflectionStudio.Core.Reflection;
using ReflectionStudio.Core.Reflection.Types;
using System.Diagnostics;
using ReflectionStudio.Core.FileManagement;

namespace ReflectionStudio.Core.Project
{
	public class ProjectService
	{
		public const string ProjectExtension = ".rns";

		#region ----------------SINGLETON----------------
		public static readonly ProjectService Instance = new ProjectService();

		/// <summary>
		/// Private constructor for singleton pattern
		/// </summary>
		private ProjectService()
		{
		}
		#endregion

		#region ----------------PROPERTIES----------------

		/// <summary>
		/// The current project
		/// </summary>
		public ProjectEntity Current
		{
			get;
			set;
		}

		#endregion

		#region ----------------PROJECT----------------

		/// <summary>
		/// Create a new project
		/// </summary>
		/// <returns></returns>
		public bool Create( string projectLocation, string projectName )
		{
			Tracer.Verbose("ProjectService:Create", "START");

			try
			{
				EventDispatcher.Instance.RaiseProject(Current, ProjectEventType.Opening);

				Current = new ProjectDAC().Create(projectLocation, projectName);
	
				EventDispatcher.Instance.RaiseProject(Current, ProjectEventType.Opened);

				return true;
			}
			catch (Exception err)
			{
				Tracer.Error("ProjectService.Create", err);
				return false;
			}
			finally
			{
				Tracer.Verbose("ProjectService:Create", "END");
			}
		}

		/// <summary>
		/// Open a given project file
		/// </summary>
		/// <returns></returns>
		public bool Open( string fileName )
		{
			Tracer.Verbose("ProjectService:Open", "START");

			try
			{
				Current = new ProjectEntity(fileName);
				return LoadProject();
			}
			catch (Exception err)
			{
				Tracer.Error("ProjectService.Open", err);
				return false;
			}
			finally
			{
				Tracer.Verbose("ProjectService:Open", "END");
			}
		}

		/// <summary>
		/// Load the project in the Current property
		/// </summary>
		/// <returns></returns>
		private bool LoadProject()
		{
			try
			{
				EventDispatcher.Instance.RaiseStatus(Resources.CORE_LOADING, StatusEventType.StartProgress);

				EventDispatcher.Instance.RaiseProject(Current, ProjectEventType.Opening);

				Current = new ProjectDAC().Load(Current.ProjectFilePath);

				if (Current != null)
				{
					//refresh by assembly parsing
					Refresh();

					EventDispatcher.Instance.RaiseProject(Current, ProjectEventType.Opened);
					return true;
				}
				else
				{
					EventDispatcher.Instance.RaiseStatus(Resources.CORE_LOADING_ERROR, StatusEventType.StopProgress);
					return false;
				}
			}
			catch (Exception err)
			{
				Tracer.Error("ProjectService.LoadProject", err);
				return false;
			}
			finally
			{
				EventDispatcher.Instance.RaiseStatus(Resources.CORE_PROJECT_LOADED, StatusEventType.StopProgress);
			}
		}

		/// <summary>
		/// Save the current project
		/// </summary>
		/// <returns></returns>
		public bool Save()
		{
			try
			{
				EventDispatcher.Instance.RaiseStatus(Resources.CORE_SAVING, StatusEventType.StartProgress);

				if (new ProjectDAC().Save(Current))
				{
					EventDispatcher.Instance.RaiseStatus(Resources.CORE_PROJECT_SAVED, StatusEventType.StopProgress);
					return true;
				}
				else
				{
					EventDispatcher.Instance.RaiseStatus(Resources.CORE_SAVING_ERROR, StatusEventType.StopProgress);
					return false;
				}
			}
			catch (Exception err)
			{
				Tracer.Error("ProjectService.SaveProject", err);
				return false;
			}
			finally
			{
				EventDispatcher.Instance.RaiseStatus(Resources.CORE_PROJECT_SAVED, StatusEventType.StopProgress);
			}
		}

		public bool Close()
		{
			try
			{
				EventDispatcher.Instance.RaiseStatus(Resources.CORE_PROJECT_CLOSED, StatusEventType.StartProgress);
				EventDispatcher.Instance.RaiseProject(this.Current, ProjectEventType.Closing);

				ProjectEntity entity = this.Current;
				this.Current = null;

				EventDispatcher.Instance.RaiseStatus(Resources.CORE_PROJECT_CLOSED, StatusEventType.StopProgress);
				EventDispatcher.Instance.RaiseProject(entity, ProjectEventType.Closed);

				entity = null;
				return true;
			}
			catch (Exception err)
			{
				Tracer.Error("ProjectService.Close", err);
				return false;
			}
			finally
			{
				EventDispatcher.Instance.RaiseStatus(Resources.CORE_PROJECT_CLOSED, StatusEventType.StopProgress);
			}
		}

		#endregion

		#region ----------------REFRESH ASSEMBLIES----------------

		/// <summary>
		/// Refresh the projects assemblies by parsing the original ones in a background thread
		/// </summary>
		public void Refresh()
		{
			if (Current != null)
			{
				//EventDispatcher.Instance.RaiseStatus(Resources.CORE_PARSING_START, StatusEventType.StartProgress);

				//ask for a worker to parse the assembly
				AssemblyBackgroundWorkerParser parser = AssemblyParserFactory.GetBackgroundWorkerParser(eParserType.CecilParser);
				parser.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ParsingWorkCompleted);
				parser.Refresh(ProjectService.Instance.Current.Assemblies);

				////Init the background worker process
				//BackgroundWorker worker = new BackgroundWorker();
				//worker.DoWork += new DoWorkEventHandler(bw_DoWorkRefresh);
				//worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
				//worker.RunWorkerAsync(ProjectService.Instance.Current.Assemblies);
			}
		}

		//private void bw_DoWorkRefresh(object sender, DoWorkEventArgs e)
		//{
		//    Tracer.Verbose("ProjectService:bw_DoWorkRefresh", "START");

		//    try
		//    {
		//        CecilAssemblyParser parser = new CecilAssemblyParser((BackgroundWorker)sender, e);

		//        foreach (AssemblyType typ in ((ProjectEntity)e.Argument).Assemblies)
		//            parser.RefreshAssembly(typ);
		//    }
		//    catch (Exception err)
		//    {
		//        Tracer.Error("ProjectService.bw_DoWorkRefresh", err);
		//    }
		//    finally
		//    {
		//        Tracer.Verbose("ProjectService:bw_DoWorkRefresh", "END");
				
		//        //be sure progress is stoped
		//        EventDispatcher.Instance.RaiseStatus("", StatusEventType.StopProgress);
		//    }
		//}

		
		#endregion

		#region ----------------ADD/REMOVE ASSEMBLIES----------------

		/// <summary>
		/// Add an assembly from a given filename
		/// </summary>
		/// <param name="pathFile"></param>
		public void AddAssembly(string pathFile)
		{
			if (Current == null) return;

			DiskContent dc = new DiskContent(pathFile);
			string dest = Path.Combine( Current.SubFolderProfiled, dc.Name );
			File.Copy( pathFile, dest );

			EventDispatcher.Instance.RaiseStatus(Resources.CORE_PARSING_START, StatusEventType.StartProgress);

			//ask for a worker to parse the assembly
			AssemblyBackgroundWorkerParser parser = AssemblyParserFactory.GetBackgroundWorkerParser(eParserType.CecilParser);
			parser.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ParsingWorkCompleted);
			parser.Parse(dest);

			////Init the background worker process
			//BackgroundWorker worker = new BackgroundWorker();
			//worker.DoWork += new DoWorkEventHandler(bw_DoWorkParse);
			//worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ParsingWorkCompleted);
			//worker.RunWorkerAsync(pathFile);
		}

		private void ParsingWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Tracer.Verbose("ProjectService:ParsingWorkCompleted", "START");

			try
			{
				if (e.Result != null)
				{
					if (e.Result.GetType() == typeof(NetAssembly))
						Current.Assemblies.Add((NetAssembly)e.Result);
				}
			}
			catch (Exception err)
			{
				Tracer.Error("ProjectService.ParsingWorkCompleted", err);
			}
			finally
			{
				Tracer.Verbose("ProjectService:ParsingWorkCompleted", "END");
			}

		}
		//private void bw_DoWorkParse(object sender, DoWorkEventArgs e)
		//{
		//    Tracer.Verbose("ProjectService:bw_DoWorkParse", "START");
		//    try
		//    {
		//        CecilAssemblyParser parser = new CecilAssemblyParser((BackgroundWorker)sender, e);
		//        AssemblyType typ = parser.Parse();

		//        if (typ != null)
		//            e.Result = typ;
		//    }
		//    catch (Exception err)
		//    {
		//        Tracer.Error("ProjectService.bw_DoWorkParse", err);
		//    }
		//    finally
		//    {
		//        Tracer.Verbose("ProjectService:bw_DoWorkParse", "END");

		//        //be sure progress is stoped
		//        EventDispatcher.Instance.RaiseStatus("", StatusEventType.StopProgress);
		//    }
		//}

		/// <summary>
		/// Add all matching assemblies from a given folder
		/// </summary>
		/// <param name="path"></param>
		/// <param name="recursive"></param>
		public void AddAssemblyFromFolder(string path, bool recursive)
		{
			if (Current == null) return;

			Tracer.Verbose("ProjectService:AddAssemblyFromFolder", "Parsing the folder {0}", path);

			try
			{
				DirectoryInfo inf = new DirectoryInfo(path);
				
				FileInfo[] tab = inf.GetFiles("*.dll", SearchOption.TopDirectoryOnly);
				foreach (FileInfo fil in tab)
				{
					Tracer.Verbose("ProjectService:AddAssemblyFromFolder", "Parsing DLL {0}", fil.Name);
					AddAssembly(fil.FullName);
				}

				tab = inf.GetFiles("*.exe", SearchOption.TopDirectoryOnly);
				foreach (FileInfo fil in tab)
				{
					Tracer.Verbose("ProjectService:AddAssemblyFromFolder", "Parsing EXE {0}", fil.Name);
					AddAssembly(fil.FullName);
				}
			}
			catch (Exception err)
			{
				Tracer.Error("ProjectService.AddAssemblyFromFolder", err);
			}
			finally
			{
				Tracer.Verbose("ProjectService:AddAssemblyFromFolder", "END");

				//be sure progress is stoped
				EventDispatcher.Instance.RaiseStatus("", StatusEventType.StopProgress);
			}
		}

		/// <summary>"
		/// Remove a given assembly object from the project collection
		/// </summary>
		/// <param name="assObj"></param>
		public void RemoveAssembly(object assObj)
		{
			Tracer.Verbose( "ProjectService.RemoveAssembly", "Remove assembly {0}", assObj);
			
			if (Current != null)
				Current.Assemblies.Remove((NetAssembly)assObj);
		}
		#endregion

		#region ----------------BUILD----------------
		
		public void Build(object sender, DoWorkEventArgs e)
		{
			// Get the BackgroundWorker that raised this event.
			ProjectBuilder builder = new ProjectBuilder((BackgroundWorker)sender, e);
			builder.Build();
		}

		#endregion

		#region ----------------OTHERS----------------

		/// <summary>
		/// Run the main programm of the project
		/// </summary>
		public void Run()
		{
			// TODO - manage process arguments

			ProcessStartInfo p = new ProcessStartInfo(Current.MainProgramToRun, "");
			Process bat = System.Diagnostics.Process.Start(p);
		}

		#endregion

		
		#region ----------------CHART----------------

		//public void GetStandardChartData( Visifire.Charts.Chart ctrl )
		//{
		//    // Create a new instance of DataSeries
		//    DataSeries dataSerie1 = new DataSeries();
		//    dataSerie1.XValueType = ChartValueTypes.Auto;

		//    DataSeries dataSerie2 = new DataSeries();
		//    dataSerie1.RenderAs = RenderAs.Line;

		//    IEnumerable<CallStackInfo> calledBy = (from item in _Current.Extend.CallStack orderby item.TotalTick descending select item);
		//    if (calledBy != null)
		//    {
		//        foreach (CallStackInfo item in calledBy)
		//        {
		//            DataPoint dataPoint = new DataPoint();

		//            // Set YValue for a DataPoint
		//            //dataPoint.AxisXLabel = item.MethodName.Remove( 0, item.MethodName.Length - 10) + "...";
		//            dataPoint.YValue = item.TotalTick;

		//            // Add dataPoint to DataPoints collection.
		//            dataSerie1.DataPoints.Add(dataPoint);

		//            dataPoint = new DataPoint();

		//            // Set YValue for a DataPoint
		//            //dataPoint.AxisXLabel = item.MethodName;
		//            dataPoint.YValue = item.InternalTick;

		//            // Add dataPoint to DataPoints collection.
		//            dataSerie2.DataPoints.Add(dataPoint);
		//        }
		//    }

		//    ctrl.Series.Add(dataSerie1);
		//    ctrl.Series.Add(dataSerie2);
		//}
		#endregion
	}
}
