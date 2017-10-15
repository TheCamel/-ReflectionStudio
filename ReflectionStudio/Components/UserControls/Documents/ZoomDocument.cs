using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using AvalonDock;

namespace ReflectionStudio.Components.UserControls
{
	public class ZoomDocument : DocumentBase
	{
		#region --------------------PROPERTIES--------------------

		private ScaleTransform _ScaleTransformer = new ScaleTransform();
		/// <summary>
		/// Scale transform to affect to the content you want to zoom
		/// </summary>
		public ScaleTransform ScaleTransformer
		{
			get { return _ScaleTransformer; }
			set { _ScaleTransformer = value; }
		}

		private double _Scale = 1.0;
		/// <summary>
		/// The zoom scale value
		/// </summary>
		public double Scale
		{
			get { return _Scale; }
			set
			{
				if (_Scale != value)
				{
					_Scale = value;
					UpdateScale();
				}
			}
		}
		#endregion

		#region --------------------ZOOM EVENT & DELEGATE & HANDLER--------------------
		/// <summary>
		/// Zoom registered event 
		/// </summary>
		public static readonly RoutedEvent ZoomChangedEvent = EventManager.RegisterRoutedEvent("ZoomChangedEvent",
																RoutingStrategy.Bubble,
																typeof(ZoomChangedEventHandler), typeof(ZoomDocument));

		/// <summary>
		/// the event handler delegate
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public delegate void ZoomChangedEventHandler(object sender, ZoomRoutedEventArgs e);

		/// <summary>
		/// zoom changed event handler
		/// </summary>
		public event ZoomChangedEventHandler ZoomChanged
		{
			add { AddHandler(ZoomChangedEvent, value); }
			remove { RemoveHandler(ZoomChangedEvent, value); }
		}

		/// <summary>
		/// Raise the zoom changed event
		/// </summary>
		protected void RaiseZoomChanged()
		{
			ZoomRoutedEventArgs args = new ZoomRoutedEventArgs(_Scale);
			args.RoutedEvent = ZoomChangedEvent;
			RaiseEvent(args);
		}

		/// <summary>
		/// Event handler to receive zoom changes from other controls
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnZoomChanged(object sender, ZoomRoutedEventArgs e)
		{
			Scale = e.Scale;
		}

		#endregion
		
		#region --------------------PRIVATES--------------------
		/// <summary>
		/// Update the zoom scale of the control and raise the event
		/// </summary>
		private void UpdateScale()
		{
			this._ScaleTransformer.ScaleX = this._ScaleTransformer.ScaleY = Scale;
			this._ScaleTransformer.CenterX = this._ScaleTransformer.CenterY = 0.5;

			RaiseZoomChanged();
		}

		/// <summary>
		/// Handle CTRL WHEEL for zooming
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreviewMouseWheel(System.Windows.Input.MouseWheelEventArgs e)
		{
			base.OnPreviewMouseWheel(e);

			//zooming
			if (Keyboard.IsKeyDown(Key.LeftCtrl))
			{
				UpdateContent(e.Delta > 0);
				e.Handled = true;
			}
		}

		/// <summary>
		/// Calculate the zoom scale of the page
		/// </summary>
		/// <param name="delta"></param>
		private void UpdateContent(bool delta)
		{
			_Scale += delta ? 0.01 : -0.01;
			_Scale = _Scale < 0.01 ? 0.01 : _Scale;
			_Scale = _Scale > 4 ? 4 : _Scale;

			UpdateScale();
		}

		#endregion
	}
}
