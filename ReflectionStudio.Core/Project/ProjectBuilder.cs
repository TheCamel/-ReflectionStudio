using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using ReflectionStudio.Core.Events;
using ReflectionStudio.Core.FileManagement;
using ReflectionStudio.Core.Helpers;

namespace ReflectionStudio.Core.Project
{
	internal class ProjectBuilder : ProjectWorker
	{
		#region ----------------------CONSTRUCTORS----------------------

		public ProjectBuilder(BackgroundWorker worker, DoWorkEventArgs e) : base( worker,  e )
		{
		}

		#endregion

		public void Build()
		{
			Tracer.Verbose("ProjectBuilder.Build", "START");

			try
			{
				//init build variables
				Project.Settings.MethodMapGuid = System.Guid.NewGuid();

				//delete the old files
				DeleteOutput(Project.SubFolderProfiled);
				if (Result == false) return;

				//construct and save the map
				MethodMapBuilder map = new MethodMapBuilder(Worker, Event);
				map.Build();
				if (Result == false) return;

				//copy the runtime asembly
				CopyRuntime();
				if (Result == false) return;

				// inject and save
				InjectionProfiler profiler = new InjectionProfiler(Worker, Event);
				profiler.Build();

				if (Result == false) return;

				//save runtime settings
				SerializeHelper.Serialize(Path.Combine(Project.SubFolderProfiled, "ReflectionStudio.Spy.config"),
							Project.Settings.ConvertToSpy(), true);
			}
			catch (Exception error)
			{
				Tracer.Error("ProjectBuilder.Build", error);
			}
			finally
			{
				Tracer.Verbose("ProjectBuilder.Build", "END");
			}
		}

		private void CopyRuntime()
		{
			Tracer.Verbose("ProjectBuilder.CopyRuntime", "START");

			try
			{
				// TODO chemin
				string runtime = Path.Combine(PathHelper.ApplicationPath, "ReflectionStudio.Spy.dll");
				if (File.Exists(runtime))
					File.Copy(runtime, Path.Combine(Project.SubFolderProfiled, "ReflectionStudio.Spy.dll"));
				else
				{
					// TODO = do a generic core function to extract binary ressource
					// and save it to a folder

					//extract from resource into application path
					using (Stream assStream = Assembly.GetEntryAssembly().GetManifestResourceStream("ReflectionStudio.Spy.dll"))
					{
						if (assStream == null)
						{
							throw new Exception("Missing 'ReflectionStudio.Spy.dll' resource");
						}

						byte[] abytResource;
						abytResource = new Byte[assStream.Length];

						BinaryReader reader = new BinaryReader(assStream);
						try
						{
							reader.Read(abytResource, 0, (int)assStream.Length);
							FileStream objFileStream = new FileStream(Path.Combine(Project.SubFolderProfiled, "ReflectionStudio.Spy.dll"), FileMode.Create);
							objFileStream.Write(abytResource, 0, (int)assStream.Length);
							objFileStream.Close();
						}
						catch (Exception error)
						{
							Tracer.Error("ProjectBuilder:CopyRuntime", error);
							Result = false;
						}
						finally
						{
							if (reader != null)
								((IDisposable)reader).Dispose();
						}
					}

					//copy
					File.Copy(runtime, Path.Combine(Project.SubFolderProfiled, "ReflectionStudio.Spy.dll"));
				}
			}
			catch (Exception error)
			{
				Tracer.Error("ProjectBuilder:CopyRuntime", error);
				Result = false;
			}
			finally
			{
				Tracer.Verbose("ProjectBuilder:CopyRuntime", "END");
			}
		}

		/// <summary>
		/// Delete old files
		/// </summary>
		/// <param name="path"></param>
		private void DeleteOutput(string path)
		{
			try
			{
				if (Directory.Exists(path))
				{
					foreach (string file in Directory.GetFiles(path, "*.*"))
					{
						File.Delete(file);
					}
				}
				else
					Directory.CreateDirectory(path);
			}
			catch (Exception error)
			{
				Tracer.Error("ProjectBuilder:DeleteOutput", error);
				Result = false;
			}
		}
	}
}
