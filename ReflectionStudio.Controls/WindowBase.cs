using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;

namespace ReflectionStudio.Controls
{
	/// <summary>
	/// WindowBase handle a gripper part and resizing logic
	/// </summary>
	[TemplatePart(Name = "PART_ResizeGrip", Type = typeof(ResizeGrip))]
	public class WindowBase : Window
	{
		/// <summary>
		/// isResizing
		/// </summary>
		protected bool _IsResizing;

		/// <summary>
		/// ResizeGripPART
		/// </summary>
		protected const string ResizeGripPART = "PART_ResizeGrip";

		/// <summary>
		/// Hook up the gripper part to handle the resizing or hide it
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				if (this.ResizeMode == System.Windows.ResizeMode.CanResizeWithGrip)
				{
					FrameworkElement resizeBottomRight = (FrameworkElement)GetTemplateChild(ResizeGripPART);
					resizeBottomRight.MouseDown += OnResizeRectMouseDown;
					resizeBottomRight.MouseMove += OnResizeRectMouseMove;
					resizeBottomRight.MouseUp += OnResizeRectMouseUp;
				}
				else
				{
					FrameworkElement resizeBottomRight = (FrameworkElement)GetTemplateChild(ResizeGripPART);
					resizeBottomRight.Visibility = System.Windows.Visibility.Hidden;
				}
			}
		}

		#region Resize logic

		/// <summary>
		/// Handles the mouse down event of a resize helper region
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void OnResizeRectMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				_IsResizing = true;

				Rectangle senderRectangle = sender as Rectangle;
				senderRectangle.CaptureMouse();
			}
		}

		/// <summary>
		/// Handles the mouse up event for a resize helper region
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void OnResizeRectMouseUp(object sender, MouseButtonEventArgs e)
		{
			Rectangle senderRectangle = (Rectangle)sender;
			senderRectangle.ReleaseMouseCapture();

			_IsResizing = false;
		}

		/// <summary>
		/// Handles the mouse move event for a resize helper region
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		virtual protected void OnResizeRectMouseMove(object sender, MouseEventArgs e)
		{
			Rectangle senderRectangle = (Rectangle)sender;

			if (!_IsResizing) return;
			if (senderRectangle == null) return;

			if (senderRectangle.Name == ResizeGripPART)
					ResizeFromBottomRight(senderRectangle, e);
		}

		/// <summary>
		/// Resize the window from the bottom-right corner of the window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ResizeFromBottomRight(Rectangle sender, MouseEventArgs e)
		{
			Point mousePosition = e.GetPosition(this);
			sender.CaptureMouse();

			double newHeight = Height + (mousePosition.Y - Height);
			double newWidth = Width + (mousePosition.X - Width);

			BeginInit();

			if (newHeight <= MaxHeight)
			{
				Height = newHeight;
			}
			else
			{
				Height = MaxHeight;
			}

			if (newWidth <= MaxWidth && newWidth > MinWidth)
			{
				Width = newWidth;
			}
			else if (newWidth > MaxWidth)
			{
				Width = MaxWidth;
			}

			EndInit();
		}

		/// <summary>
		/// Resizes from the bottom edge of the window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ResizeFromBottom(Rectangle sender, MouseEventArgs e)
		{
			Point mousePosition = e.GetPosition(this);
			sender.CaptureMouse();

			double newHeight = Height + (mousePosition.Y - Height);

			if (newHeight <= MaxHeight)
			{
				Height = newHeight;
			}
			else
			{
				Height = MaxHeight;
			}
		}

		/// <summary>
		/// Resizes from the right edge of the window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ResizeFromRight(Rectangle sender, MouseEventArgs e)
		{
			Point mousePosition = e.GetPosition(this);
			sender.CaptureMouse();

			double newWidth = Width + (mousePosition.X - Width);

			if (newWidth <= MaxWidth && newWidth > MinWidth)
			{
				Width = newWidth;
			}
			else if (newWidth > MaxWidth)
			{
				Width = MaxWidth;
			}
		}

		/// <summary>
		/// Resize from the top edge of the window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ResizeFromTop(Rectangle sender, MouseEventArgs e)
		{
			Point mousePosition = e.GetPosition(this);

			sender.CaptureMouse();

			double newHeight = Height - mousePosition.Y;

			if (newHeight <= MaxHeight && newHeight > MinHeight)
			{
				Point absoluteMousePosition = PointToScreen(mousePosition);

				Height = newHeight;
				Top = absoluteMousePosition.Y;
			}
		}

		/// <summary>
		/// Resize from the left edge of the window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ResizeFromLeft(Rectangle sender, MouseEventArgs e)
		{
			Point mousePosition = e.GetPosition(this);

			sender.CaptureMouse();

			double newWidth = Width - mousePosition.X;

			if (newWidth > MinWidth)
			{
				Point absoluteMousePosition = PointToScreen(mousePosition);

				Left = absoluteMousePosition.X;
				Width = newWidth;
			}
		}

		#endregion
	}
}
