using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ReflectionStudio.Controls
{
	/// <summary>
	/// Implement a Dialog behavior with 'office like' style
	/// </summary>
	[TemplatePart(Name = "PART_Close", Type = typeof(Button))]
	public class DialogWindow : WindowBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		static DialogWindow()
		{
			// set the key to reference the style for this control
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
				typeof(DialogWindow), new FrameworkPropertyMetadata(typeof(DialogWindow)));
		}

		/// <summary>
		/// change WindowStartupLocation because can't be done by xaml
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInitialized(System.EventArgs e)
		{
			base.OnInitialized(e);

			if (!DesignerProperties.GetIsInDesignMode(this))
				this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		}

		/// <summary>
		/// Find the template part to attach button click event handler
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				Button close = this.Template.FindName("PART_Close", this) as Button;

				if (close != null)
					close.Click += new RoutedEventHandler(close_Click);
			}
		}

		/// <summary>
		/// Property to specify if the dialog must close or hide
		/// </summary>
		public bool HandleCloseAsHide
		{
			get;
			set;
		}

		/// <summary>
		/// Handle the button click event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void close_Click(object sender, RoutedEventArgs e)
		{
			if (HandleCloseAsHide)
				this.Hide();
			else
				this.Close();
		}

		/// <summary>
		/// Handle the move event as for dialog background click
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);
			this.DragMove();
		}
	}
}
