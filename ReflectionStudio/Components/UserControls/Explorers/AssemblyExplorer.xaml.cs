using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AvalonDock;
using ReflectionStudio.Classes;
using ReflectionStudio.Controls;
using ReflectionStudio.Controls.Helpers;
using ReflectionStudio.Core.Events;
using ReflectionStudio.Core.Reflection;
using ReflectionStudio.Core.Reflection.Types;
using System.ComponentModel;

namespace ReflectionStudio.Components.UserControls
{
	/// <summary>
	/// Interaction logic for AssemblyExplorer.xaml
	/// </summary>
	public partial class AssemblyExplorer : DockableContent
	{
		public AssemblyExplorer()
		{
			InitializeComponent();
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				this.treeViewAss.OnItemNeedPopulate += new TreeViewExtended.ItemNeedPopulateEventHandler(treeViewAss_OnItemNeedPopulate);

				foreach (NetAssembly ass in BindingView.Instance.Workspace.Assemblies)
					treeViewAss.AddItem(null, ass.Name, ass);
			}
		}

		#region ----------------COMMANDS----------------

		private void BtnAddAssembly_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				using (System.Windows.Forms.OpenFileDialog browser = new System.Windows.Forms.OpenFileDialog())
				{
					if (browser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						AddAssembly(browser.FileName);
					}
				}
			}
			catch (Exception all)
			{
				Tracer.Error("AssemblyExplorer.BtnAddAssembly_Click", all);
			}
		}

		private void BtnAddAssemblyFromFolder_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				using (System.Windows.Forms.FolderBrowserDialog browser = new System.Windows.Forms.FolderBrowserDialog())
				{
					browser.ShowNewFolderButton = false;
					browser.Description = ReflectionStudio.Properties.Resources.MSG_SELECT_FOLDER;
					browser.RootFolder = Environment.SpecialFolder.MyComputer;
					if (browser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						AddAssemblyFromFolder(browser.SelectedPath, true);
					}
				}
			}
			catch (Exception all)
			{
				Tracer.Error("AssemblyExplorer.BtnAddAssembly_Click", all);
			}
		}

		private void BtnRefresh_Click(object sender, RoutedEventArgs e)
		{
			//this.treeViewAss.Items.Clear();

			//foreach (NetAssembly ass in Assemblies)
			//    AddAssemblyTreeView(this.treeViewAss, ass);

			//AssemblyExplorerService.Instance.Refresh(treeViewAss.SelectedItem);
		}

		private void BtnRemove_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				this.Cursor = Cursors.Wait;

				TreeViewItem assNode = treeViewAss.FindParentNode(treeViewAss.SelectedItem, typeof(NetAssembly));
				if (assNode != null)
				{
					BindingView.Instance.Workspace.Assemblies.Remove((NetAssembly)assNode.Tag);
					treeViewAss.Items.Remove(assNode);
				}
			}
			catch (Exception all)
			{
				Tracer.Error("AssemblyExplorer.BtnRemove_Click", all);
			}
			finally
			{
				this.Cursor = Cursors.Arrow;
			}
		}

		#endregion

		#region ----------------INTERNALS----------------

		public void AddAssembly(string filePath)
		{
			Tracer.Verbose("AssemblyExplorer:AddAssembly", "Parsing the file {0}", filePath);

			try
			{
				using ( new LongOperation(this, "Parsing the assembly") )
				{
					IAssemblyParser parser = AssemblyParserFactory.GetParser(eParserType.NetParser);
					NetAssembly ass = parser.LoadAssemblyRecursively(filePath);
					// net assembly or parsing ok
					if (ass != null)
					{
						BindingView.Instance.Workspace.Assemblies.Add(ass);
						treeViewAss.AddItem(null, ass.Name, ass);
					}
				}
			}
			catch (Exception err)
			{
				Tracer.Error("AssemblyExplorer.AddAssembly", err);
			}
			finally
			{
				Tracer.Verbose("AssemblyExplorer:AddAssembly", "END");
			}
		}

		public void AddAssemblyFromFolder(string path, bool recursive)
		{
			Tracer.Verbose("AssemblyExplorer:AddAssemblyFromFolder", "Parsing the folder {0}", path);

			try
			{
				DirectoryInfo inf = new DirectoryInfo(path);

				FileInfo[] tab = inf.GetFiles("*.dll", SearchOption.TopDirectoryOnly);
				foreach (FileInfo fil in tab)
					AddAssembly(fil.FullName);

				tab = inf.GetFiles("*.exe", SearchOption.TopDirectoryOnly);
				foreach (FileInfo fil in tab)
					AddAssembly(fil.FullName);

				tab = null;
			}
			catch (Exception err)
			{
				Tracer.Error("AssemblyExplorer.AddAssemblyFromFolder", err);
			}
			finally
			{
				Tracer.Verbose("AssemblyExplorer:AddAssemblyFromFolder", "END");
			}
		}

		#endregion

		#region ----------------TREEVIEW----------------

		void treeViewAss_OnItemNeedPopulate(object sender, RoutedEventArgs e)
		{
			try
			{
				using (new LongOperation(this))
				{
					TreeViewItem openedItem = (TreeViewItem)e.OriginalSource;

					if (openedItem.Tag == null)
					{
						TreeViewItem parent = treeViewAss.FindParentNode(openedItem, typeof(NetAssembly));

						foreach (NetReference typ in ((NetAssembly)parent.Tag).References)
							treeViewAss.AddItem(openedItem, typ.Name, typ);
					}
					else
					{
						object assobject = openedItem.Tag;

						if (assobject is NetAssembly)
						{
							treeViewAss.AddItem(openedItem, "References", null);

							foreach (NetNamespace typ in ((NetAssembly)assobject).Namespaces)
								treeViewAss.AddItem(openedItem, typ.Name, typ);

							return;
						}

						if (assobject is NetNamespace)
						{
							TreeViewItem parent = treeViewAss.FindParentNode(openedItem, typeof(NetAssembly));

							foreach (NetBaseType tr in ((NetAssembly)parent.Tag).Children.Cast<NetBaseType>().Where(p => p.NameSpace == ((NetNamespace)assobject).Name))
							{
								//add the class
								TreeViewItem classObject = treeViewAss.AddItem(openedItem, tr.Name, tr);
							}

							return;
						}

						if (assobject is NetEnum || assobject is NetClass || assobject is NetInterface)
						{
							//add methods
							foreach (NetMethod method in ((NetBaseType)assobject).Children)
								treeViewAss.AddItem(openedItem, method.Name, method, false);

							return;
						}
					}
				}
			}
			catch (Exception all)
			{
				Tracer.Error("AssemblyExplorer.treeViewAss_OnItemNeedPopulate", all);
			}
		}

		private void treeViewAss_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				DragDropContainer dataObject = new DragDropContainer();
				dataObject.Data = (object)((TreeViewItem)this.treeViewAss.SelectedItem).Tag;

				DragDrop.DoDragDrop(this.treeViewAss, dataObject, DragDropEffects.Copy);

				e.Handled = true;
			}
		}

		/// <summary>
		/// Update the property explorer
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void treeViewAss_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			BindingView.Instance.PropertyContext = ((TreeViewItem)e.NewValue).Tag;
		}

		#endregion
	}
}
