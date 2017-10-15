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
using ReflectionStudio.Core.Project;

namespace ReflectionStudio.Components.UserControls
{
	/// <summary>
	/// Interaction logic for SnapshotViewDocument.xaml
	/// </summary>
	public partial class SnapshotViewDocument : DocumentContent
	{
		public SnapshotViewDocument()
		{
			InitializeComponent();
		}

		public SnapshotEntity Snap
		{
			get;
			set;
		}

		private SnapshotViewMode _ViewMode = SnapshotViewMode.Grid;
		public SnapshotViewMode ViewMode
		{
			get { return _ViewMode; }
			set { _ViewMode = value; ChangeView(); }
		}

		public void ChangeView()
		{
			this.PanelGrid.Visibility = _ViewMode == SnapshotViewMode.Grid ? Visibility.Visible : Visibility.Hidden;
			this.PanelVisual.Visibility = _ViewMode == SnapshotViewMode.Visual ? Visibility.Visible : Visibility.Hidden;

			switch (_ViewMode)
			{
				case SnapshotViewMode.Grid:
					SnapshotService.Instance.LoadSnapshot(Snap);
					DataContext = Snap;
					break;
				case SnapshotViewMode.Visual:
					SnapshotService.Instance.LoadSnapshot(Snap);
					DataContext = Snap;
					break;
			}
		}
	}

	public enum SnapshotViewMode
	{
		Grid = 1, Visual, Compare
	}
}
