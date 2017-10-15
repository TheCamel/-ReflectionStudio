using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using ReflectionStudio.Classes.Workspace;
using ReflectionStudio.Controls.Helpers;
using ReflectionStudio.Core;
using ReflectionStudio.Core.Events;
using ReflectionStudio.Core.Project;
using ReflectionStudio.Classes;

namespace ReflectionStudio
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		/// <summary>
		/// Load the workspace values
		/// </summary>
		/// <param name="e"></param>
		protected override void OnStartup(StartupEventArgs e)
		{
			PresentationTraceSources.ResourceDictionarySource.Listeners.Add(new ConsoleTraceListener());
			PresentationTraceSources.ResourceDictionarySource.Switch.Level = SourceLevels.All;
			PresentationTraceSources.DataBindingSource.Listeners.Add(new ConsoleTraceListener());
			PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Error;
			PresentationTraceSources.DependencyPropertySource.Listeners.Add(new ConsoleTraceListener());
			PresentationTraceSources.DependencyPropertySource.Switch.Level = SourceLevels.All;
			PresentationTraceSources.DocumentsSource.Listeners.Add(new ConsoleTraceListener());
			PresentationTraceSources.DocumentsSource.Switch.Level = SourceLevels.All;
			PresentationTraceSources.MarkupSource.Listeners.Add(new ConsoleTraceListener());
			PresentationTraceSources.MarkupSource.Switch.Level = SourceLevels.All;
			PresentationTraceSources.NameScopeSource.Listeners.Add(new ConsoleTraceListener());
			PresentationTraceSources.NameScopeSource.Switch.Level = SourceLevels.All;

			base.OnStartup(e);

			TraceConfiguration();

			////load workspace values
			//WorkspaceService.Instance.Themes = ThemeHelper.DiscoverThemes();
			WorkspaceService.Instance.Load();

			//install unhandled exception handler
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);
			this.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);

			ThemeManager.Instance.Load();

			//if (WorkspaceService.Instance.Entity.CurrentTheme != string.Empty)
			//    ThemeHelper.LoadTheme(WorkspaceService.Instance.Entity.CurrentTheme);
			//else
			//{
			//    if (WorkspaceService.Instance.Themes.Count > 0)
			//        ThemeHelper.LoadTheme(WorkspaceService.Instance.Themes[0]);
			//}

			//if we have a file as starting, tell it to the startup dialog to not show up
			if (e.Args.GetLength(0) != 0)
			{
				string arg = e.Args[0];
				if (File.Exists(arg) && arg.Contains(ProjectService.ProjectExtension))
					WorkspaceService.Instance.Entity.StartupProject = arg;
			}
		}

		/// <summary>
		/// Save the workspace values
		/// </summary>
		/// <param name="e"></param>
		protected override void OnExit(ExitEventArgs e)
		{
			ThemeManager.Instance.Save();
			WorkspaceService.Instance.Save();

			base.OnExit(e);
		}

		/// <summary>
		/// Manage unhandled exception application wide
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
		{
			if (!System.Diagnostics.Debugger.IsAttached)
			{
				Exception exceptionObject = e.ExceptionObject as Exception;
				Tracer.Error("Reflection Studio : Unhandled Exception", exceptionObject);
			}
		}

		void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			if (!System.Diagnostics.Debugger.IsAttached)
			{
				Tracer.Error("Reflection Studio : Unhandled Exception", e.Exception);
			}
		}

		private void TraceConfiguration()
		{
			try
			{
				//delete the old log file
				string logPath = Path.Combine(PathHelper.ApplicationPath, "ReflectionStudio.exe.log");
				if (File.Exists(logPath))
					File.Delete(logPath);

				if (ReflectionStudio.Properties.Settings.Default.UseTraceListener)
				{
					//configure the trace
					System.Diagnostics.Trace.AutoFlush = true;
					System.Diagnostics.Trace.IndentSize = 2;

					//configure the text listenner
					System.Diagnostics.TraceListenerCollection listeners = System.Diagnostics.Trace.Listeners;
					listeners.Add(new System.Diagnostics.TextWriterTraceListener(logPath, "LOG"));
				}
			}
			catch (Exception error)
			{
				Tracer.Error("Reflection Studio.TraceConfiguration", error);
			}
		}
	}
}
