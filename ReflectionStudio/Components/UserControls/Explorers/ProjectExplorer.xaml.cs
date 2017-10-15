using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AvalonDock;
using ReflectionStudio.Core.Events;
using ReflectionStudio.Core.Project;
using System.IO;
using ReflectionStudio.Core.FileManagement;

namespace ReflectionStudio.Components.UserControls
{
	/// <summary>
	/// Interaction logic for ProjectExplorer.xaml
	/// </summary>
	public partial class ProjectExplorer : DockableContent
	{
		public ProjectExplorer()
		{
			InitializeComponent();
		}

		internal void OnProjectChange(object sender, ProjectEventArgs e)
		{
			switch (e.ProjectType)
			{
				case ProjectEventType.Opened:
					this.treeViewTemplate.Items.Add(new TreeViewItem() { Header = ProjectService.Instance.Current.ProjectName, Tag = ProjectService.Instance.Current } );
					FillTreeView(ProjectService.Instance.Current.ProjectPath);
					break;

				case ProjectEventType.Closed:
					this.treeViewTemplate.Items.Clear();
					break;

				default:
					break;
			}
		}

		internal void FillTreeView(string source)
		{
			Tracer.Verbose("TemplateExplorer:FillTreeView", source);

			if (this.treeViewTemplate == null) return;
			try
			{
				this.treeViewTemplate.Items.Clear();

				ParseDirectoryRecursively(null, source);
			}
			catch (Exception err)
			{
				Tracer.Error("TemplateExplorer.FillTreeView", err);
			}
			finally
			{
				Tracer.Verbose("TemplateExplorer:FillTreeView", "END");
			}
		}

		private void ParseDirectoryRecursively(TreeViewItem parent, string path)
		{
			DirectoryInfo DirRef = new DirectoryInfo(path);

			TreeViewItem DirNode = new TreeViewItem() { Header = DirRef.Name, Tag = new DiskContent(DirRef.FullName, true) };

			if (parent == null) //root
				this.treeViewTemplate.Items.Add(DirNode);
			else
				parent.Items.Add(DirNode);

			FileInfo[] filesRef;

			//get all files in the current dir
			filesRef = DirRef.GetFiles("*.*");

			//loop through each file
			foreach (FileInfo fRef in filesRef)
			{
				TreeViewItem Node = new TreeViewItem() { Header = fRef.Name, Tag = new DiskContent(fRef.FullName) };
				DirNode.Items.Add(Node);
			}

			//cleanup
			filesRef = null;

			DirectoryInfo[] dirsRef;
			dirsRef = DirRef.GetDirectories();

			foreach (DirectoryInfo subdirRef in dirsRef)
			{
				ParseDirectoryRecursively(DirNode, subdirRef.FullName);
			}

			dirsRef = null;
		}
	}
}
