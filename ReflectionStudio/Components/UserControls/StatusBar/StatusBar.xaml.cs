using System;
using System.Windows;
using System.Windows.Controls;
using ReflectionStudio.Core.Events;

namespace ReflectionStudio.Components.UserControls
{
	/// <summary>
	/// StatusBar is giving zoom, message, progress functionnalities
	/// </summary>
	public partial class StatusBar : UserControl
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public StatusBar()
		{
			InitializeComponent();
		}

		#region --------------------PROPERTIES--------------------

		/// <summary>
		/// Allow or not the zoom part
		/// </summary>
		public bool CanZoom
		{
			get { return sliderZoom.IsEnabled; }
			set { sliderZoom.IsEnabled = value; }
		}

		#endregion

		#region --------------------EVENT HANDLING--------------------

		/// <summary>
		/// Status handler to update the message and ProgressBar
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnStatusChange(object sender, StatusEventArgs e)
		{
			switch (e.StatusType)
			{
				case StatusEventType.StartProgress:
					this.tbMessage.Text = e.Info.Details;
					//this.progressBar.Visibility = Visibility.Visible;
					this.progressBar.IsIndeterminate = true;
					break;
				case StatusEventType.StopProgress:
					this.tbMessage.Text = e.Info.Details;
					//this.progressBar.Visibility = Visibility.Hidden;
					this.progressBar.IsIndeterminate = false;
					break;
			}
		}

		/// <summary>
		/// Message handler to update the statusbar
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnMessage(object sender, MessageEventArgs e)
		{
			if (e.Info.Details != string.Empty && e.Info.Type == MessageEventType.Info)
				this.tbMessage.Text = e.Info.Details;
		}

		#endregion

		#region --------------------ZOOM HANDLING--------------------

		/// <summary>
		/// Event delegate
		/// </summary>
		public event EventHandler<ZoomRoutedEventArgs> ZoomChanged;

		/// <summary>
		/// Internal function to raise zzom event
		/// </summary>
		/// <param name="zoom"></param>
		private void RaiseZoomEvent(double zoom)
		{
			if (ZoomChanged != null)
				ZoomChanged(this, new ZoomRoutedEventArgs(zoom));
		}

		/// <summary>
		/// When slider value changes, raise an event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void sliderZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			RaiseZoomEvent(e.NewValue);
		}

		/// <summary>
		/// If zoom change in document, receive the event here (must be plugged in doc activation)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnZoomChanged(object sender, ZoomRoutedEventArgs e)
		{
			this.sliderZoom.Value = e.Scale;
		}
		#endregion
	}

	/// <summary>
	/// Event argument that is send with the new scale value
	/// </summary>
	public class ZoomRoutedEventArgs : RoutedEventArgs
	{
		/// <summary>
		/// Scale value
		/// </summary>
		public double Scale { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="scale"></param>
		public ZoomRoutedEventArgs(double scale)
		{
			Scale = scale;
		}
	}
}
