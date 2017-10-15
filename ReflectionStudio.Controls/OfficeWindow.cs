using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using ReflectionStudio.Controls.Helpers;
using System.Windows.Controls.Primitives;

namespace ReflectionStudio.Controls
{
	/// <summary>
	/// A custom window office like implementation
	/// </summary>
	[TemplatePart(Name = "PART_Minimize", Type = typeof(Button))]
	[TemplatePart(Name = "PART_Close", Type = typeof(Button))]
	[TemplatePart(Name = "PART_Maximize", Type = typeof(Button))]
	[TemplatePart(Name = "PART_Titlebar", Type = typeof(Control))]
	[TemplatePart(Name = "PART_ResizeLeft", Type = typeof(Rectangle))]
	[TemplatePart(Name = "PART_ResizeRight", Type = typeof(Rectangle))]
	[TemplatePart(Name = "PART_ResizeBottom", Type = typeof(Rectangle))]
	[TemplatePart(Name = "PART_ResizeTop", Type = typeof(Rectangle))]
	public class OfficeWindow : WindowBase
	{
		const string CloseButtonPart = "PART_Close";
		const string MaximizeButtonPart = "PART_Maximize";
		const string MinimizeButtonPart = "PART_Minimize";
		const string TitleBarPart = "PART_Titlebar";
		const string ResizeTopPart = "PART_ResizeTop";
		const string ResizeLeftPart = "PART_ResizeLeft";
		const string ResizeRightPart = "PART_ResizeRight";
		const string ResizeBottomPart = "PART_ResizeBottom";

		/// <summary>
		/// Initializes the metadata for the window
		/// </summary>
		static OfficeWindow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(OfficeWindow),
				new FrameworkPropertyMetadata(typeof(OfficeWindow)));
		}

		#region Initialization logic

		/// <summary>
		/// Setting up the MaximizeHelper on this window
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSourceInitialized(System.EventArgs e)
		{
			base.OnSourceInitialized(e);
			MaximizeHelper.Manage(this);
		}

		/// <summary>
		/// Applies the control template to the window
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			Button minimizeButton = (Button)GetTemplateChild(MinimizeButtonPart);
			Button maximizeButton = (Button)GetTemplateChild(MaximizeButtonPart);
			Button closeButton = (Button)GetTemplateChild(CloseButtonPart);
			closeButton.Click += OnCloseButtonClick;
			minimizeButton.Click += OnMinimizeClick;
			maximizeButton.Click += OnMaximizeClick;

			Control titlebar = (Control)GetTemplateChild(TitleBarPart);
			titlebar.MouseDown += OnTitleBarMouseDown;
			titlebar.MouseDoubleClick += OnTitleBarDoubleClick;

			AttachResizeRegions();
		}

		/// <summary>
		/// Attaches the eventhandlers to the resize helper regions
		/// </summary>
		void AttachResizeRegions()
		{
			FrameworkElement resizeTop = (FrameworkElement)GetTemplateChild(ResizeTopPart);
			FrameworkElement resizeLeft = (FrameworkElement)GetTemplateChild(ResizeLeftPart);
			FrameworkElement resizeRight = (FrameworkElement)GetTemplateChild(ResizeRightPart);
			FrameworkElement resizeBottom = (FrameworkElement)GetTemplateChild(ResizeBottomPart);

			resizeTop.MouseDown += OnResizeRectMouseDown;
			resizeTop.MouseMove += OnResizeRectMouseMove;
			resizeTop.MouseUp += OnResizeRectMouseUp;

			resizeLeft.MouseDown += OnResizeRectMouseDown;
			resizeLeft.MouseMove += OnResizeRectMouseMove;
			resizeLeft.MouseUp += OnResizeRectMouseUp;

			resizeRight.MouseDown += OnResizeRectMouseDown;
			resizeRight.MouseMove += OnResizeRectMouseMove;
			resizeRight.MouseUp += OnResizeRectMouseUp;

			resizeBottom.MouseDown += OnResizeRectMouseDown;
			resizeBottom.MouseMove += OnResizeRectMouseMove;
			resizeBottom.MouseUp += OnResizeRectMouseUp;
		}

		#endregion

		#region Resize logic

		/// <summary>
		/// Handles the mouse move event for a resize helper region
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		override protected void OnResizeRectMouseMove(object sender, MouseEventArgs e)
		{
			Rectangle senderRectangle = (Rectangle)sender;

			if (!_IsResizing) return;
			if (senderRectangle == null) return;

			switch (senderRectangle.Name)
			{
				case ResizeLeftPart:
					ResizeFromLeft(senderRectangle, e);
					break;
				case ResizeRightPart:
					ResizeFromRight(senderRectangle, e);
					break;
				case ResizeBottomPart:
					ResizeFromBottom(senderRectangle, e);
					break;
				case ResizeTopPart:
					ResizeFromTop(senderRectangle, e);
					break;
				case ResizeGripPART:
					ResizeFromBottomRight(senderRectangle, e);
					break;
			}
		}

		#endregion

		#region Window chrome eventhandlers

		/// <summary>
		/// Handles the double click event of the titlebar
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnTitleBarDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (ResizeMode == ResizeMode.NoResize ||
				ResizeMode == ResizeMode.CanMinimize)
			{
				return;
			}

			WindowState = WindowState == WindowState.Maximized ?
				WindowState.Normal : WindowState.Maximized;
		}

		/// <summary>
		/// Handles the mouse down event of the titlebar
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}

		/// <summary>
		/// Handles the click event of the maximize button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnMaximizeClick(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState == WindowState.Maximized ?
				WindowState.Normal : WindowState.Maximized;
		}

		/// <summary>
		/// Handles the click event of the minimize button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnMinimizeClick(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		/// <summary>
		/// Handles the click event of the close button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnCloseButtonClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		#endregion
	}
}
