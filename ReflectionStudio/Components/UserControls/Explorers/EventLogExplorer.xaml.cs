using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ReflectionStudio.Classes;
using ReflectionStudio.Classes.Workspace;
using ReflectionStudio.Core.Events;
using AvalonDock;

namespace ReflectionStudio.Components.UserControls
{
	/// <summary>
	/// Interaction logic for EventLogExplorer.xaml
	/// </summary>
	public partial class EventLogExplorer : DockableContent
	{
		public EventLogExplorer()
		{
			InitializeComponent();
		}

		/// <summary>
		/// User control load - init the data
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInitialized(System.EventArgs e)
		{
			base.OnInitialized(e);

			//take care that avalon is loading controls each time a panel show
			if (this.dataGridLogEvent.ItemsSource == null)
			{
				ListCollectionView view = new ListCollectionView(BindingView.Instance.LogEvents);
				view.Filter = FilterPredicate;
				this.dataGridLogEvent.ItemsSource = view;

				this.cbLogLevel.SelectionChanged += new SelectionChangedEventHandler(cbLogLevel_SelectionChanged);
			}
		}

		/// <summary>
		/// Event handler that process every messages
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnMessage(object sender, MessageEventArgs e)
		{
			//add event in the message stack
			BindingView.Instance.AddLogEvent(e.Info);

			RefreshGrid();

			//be sure the datagrid scroll to the last event
			this.dataGridLogEvent.ScrollIntoView(e.Info);
		}

		/// <summary>
		/// Clear the collection and the datagrid display
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BtnClear_Click(object sender, RoutedEventArgs e)
		{
			BindingView.Instance.LogEvents.Clear();

			RefreshGrid();
		}

		/// <summary>
		/// When the selection level change, update the workspace setting
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbLogLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//Save the level in the settings to share it with the tracer and event dispatcher
			if (this.cbLogLevel.SelectedItem != null)
				WorkspaceService.Instance.Entity.LogLevel = (MessageEventType)this.cbLogLevel.SelectedItem;

			RefreshGrid();
		}

		private void RefreshGrid()
		{
			if (this.dataGridLogEvent != null && this.dataGridLogEvent.ItemsSource != null)
				((ListCollectionView)this.dataGridLogEvent.ItemsSource).Refresh();
		}

		/// <summary>
		/// Predicate filter to log level
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		private bool FilterPredicate(object item)
		{
			MessageInfo info = (MessageInfo)item;
			if (info.Type <= WorkspaceService.Instance.Entity.LogLevel)
				return true;
			else
				return false;
		}
	}
}
