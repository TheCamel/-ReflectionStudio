using System.Windows.Input;
using ReflectionStudio.Controls;
using ReflectionStudio.Core.Project;

namespace ReflectionStudio
{
	/// <summary>
	/// 
	/// </summary>
	public partial class MainWindow : Fluent.RibbonWindow
	{
		#region ---------------------INIT---------------------

		private void InitializeBindings()
		{
			CommandBindings.Add(new CommandBinding(ApplicationCommands.New, NewCommandHandler, CanExecuteNewCommand));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, OpenCommandHandler, CanExecutOpenCommand));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, SaveCommandHandler, CanExecuteSaveCommand));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.SaveAs, SaveAsCommandHandler, CanExecuteSaveAsCommand));
			//CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, CutCommandHandler, CanExecuteClipboardCommand));
			//CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, DeleteFileCommandHandler, CanExecuteDeleteFileCommand));

			//CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, CutCommandHandler, CanExecuteClipboardCommand));
			//CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, CopyCommandHandler, CanExecuteClipboardCommand));
			//CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, PasteCommandHandler, CanExecuteClipboardCommand));

			//CommandBindings.Add(new CommandBinding(ApplicationCommands.Redo, PasteCommandHandler, CanExecuteClipboardCommand));
			//CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, PasteCommandHandler, CanExecuteClipboardCommand));

			CommandBindings.Add(new CommandBinding(ApplicationCommands.Help, HelpCommandHandler, null));

			CommandBindings.Add(new CommandBinding(ApplicationCommands.Properties, PropertiesCommandHandler, null));

			//CommandBindings.Add(new CommandBinding(NavigationCommands.DecreaseZoom, PasteCommandHandler, CanExecuteClipboardCommand));
			//CommandBindings.Add(new CommandBinding(NavigationCommands.IncreaseZoom, PasteCommandHandler, CanExecuteClipboardCommand));
			//CommandBindings.Add(new CommandBinding(NavigationCommands.Zoom, PasteCommandHandler, CanExecuteClipboardCommand));
			//CommandBindings.Add(new CommandBinding(NavigationCommands.Refresh, PasteCommandHandler, CanExecuteClipboardCommand));

			//CommandBindings.Add(new CommandBinding(EditingCommands.DecreaseIndentation, PasteCommandHandler, CanExecuteClipboardCommand));
			//CommandBindings.Add(new CommandBinding(EditingCommands.IncreaseIndentation, PasteCommandHandler, CanExecuteClipboardCommand));

			////CUSTOM COMMANDS
			//CommandBindings.Add(new CommandBinding(ProjectNew, ProjectNewCommandHandler, CanExecuteProjectNewCommand));
			//CommandBindings.Add(new CommandBinding(ProjectOpen, OpenProjectCommandHandler, CanExecuteProjectOpenCommand));
			//CommandBindings.Add(new CommandBinding(ProjectSave, ProjectSaveCommandHandler, CanExecuteProjectSaveCommand));
			//CommandBindings.Add(new CommandBinding(ProjectSaveAs, ProjectSaveAsCommandHandler, CanExecuteProjectSaveAsCommand));
			//CommandBindings.Add(new CommandBinding(ProjectClose, ProjectCloseCommandHandler, CanExecuteProjectCloseCommand));

			//CommandBindings.Add(new CommandBinding(FileAddExisting, DeleteFileCommandHandler, CanExecuteDeleteFileCommand));

			//CommandBindings.Add(new CommandBinding(AddAssemblyFromFile, AddAssemblyFromFileCommandHandler, CanExecuteAddAssemblyCommand));
			//CommandBindings.Add(new CommandBinding(AddAssemblyFromFolder, AddAssemblyFromFolderCommandHandler, CanExecuteAddAssemblyCommand));

			//CommandBindings.Add(new CommandBinding(OpenSnapshot, OnOpenSnapshotCommand, CanExecute));
		}

		#endregion

		#region ---------------------COMMAMNDS---------------------

		// for files, added to Application commands
		static public RoutedCommand FileDelete = new RoutedCommand("FileDelete", typeof(MainWindow));
		static public RoutedCommand FileAddExisting = new RoutedCommand("FileAddExisting", typeof(MainWindow));

		//project

		static public RoutedCommand ProjectNew = new RoutedCommand("ProjectNew", typeof(MainWindow));
		static public RoutedCommand ProjectOpen = new RoutedCommand("ProjectOpen", typeof(MainWindow));
		static public RoutedCommand ProjectSave = new RoutedCommand("ProjectSave", typeof(MainWindow));
		static public RoutedCommand ProjectSaveAs = new RoutedCommand("ProjectSaveAs", typeof(MainWindow));
		static public RoutedCommand ProjectClose = new RoutedCommand("ProjectClose", typeof(MainWindow));

		static public RoutedCommand AddAssemblyFromFile = new RoutedCommand("AddAssemblyFromFile", typeof(MainWindow));
		static public RoutedCommand AddAssemblyFromFolder = new RoutedCommand("AddAssemblyFromFolder", typeof(MainWindow));
		static public RoutedCommand OpenSnapshot = new RoutedCommand("OpenSnapshot", typeof(MainWindow));

		#endregion



		public void CanExecuteAllways(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = true;
		}

		private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = ProjectService.Instance.Current != null;
		}
	}
} 
