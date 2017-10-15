using System.Windows;
using AvalonDock;

namespace ReflectionStudio.Components.UserControls
{
	/// <summary>
	/// Interaction logic for ProjectDocument.xaml
	/// </summary>
	public partial class ProjectDocument : DocumentContent
	{
		public ProjectDocument()
		{
			InitializeComponent();
		}

		private ProjectPanelView _PanelView = ProjectPanelView.None;
		public ProjectPanelView PanelView
		{
			get { return _PanelView; }
			set
			{
				if (_PanelView != value)
				{
					ShowPanel(value);
					_PanelView = value;
				}
			}
		}

		public void ShowPanel(ProjectPanelView newView)
		{
			switch (_PanelView)
			{
				case ProjectPanelView.None:
					break;
				case ProjectPanelView.Option:
					this.OptionPanel.Save();
					this.OptionPanel.Visibility = Visibility.Hidden;
					break;
				case ProjectPanelView.Target:
					this.TargetPanel.Visibility = Visibility.Hidden;
					break;
			}

			switch (newView)
			{
				case ProjectPanelView.None:
					break;
				case ProjectPanelView.Option:
					this.OptionPanel.Load();
					this.OptionPanel.Visibility = Visibility.Visible;
					break;
				case ProjectPanelView.Target:
					this.TargetPanel.Visibility = Visibility.Visible;
					break;
			}
		}
	}

	public enum ProjectPanelView
	{
		None, Option, Target
	}
}
