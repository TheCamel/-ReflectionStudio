using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ReflectionStudio.Controls.Helpers
{
	/// <summary>
	/// VisualHelper convert the given visual content as an image
	/// </summary>
	public static class VisualHelper
	{
		/// <summary>
		/// Returns the contents of a WPF Visual as a Bitmap in PNG format with the given destination visual size
		/// </summary>
		/// <param name="visualSrc"></param>
		/// <param name="visualDest"></param>
		/// <returns></returns>
		public static BitmapImage PngBitmap(Visual visualSrc, Visual visualDest)
		{
			// Get height and width
			int width = (int)(double)visualDest.GetValue(FrameworkElement.ActualWidthProperty);
			int height = (int)(double)visualDest.GetValue(FrameworkElement.ActualHeightProperty);

			return PngBitmap(visualSrc, width, height);
		}

		/// <summary>
		/// Returns the contents of a WPF Visual as a Bitmap in PNG format.
		/// </summary>
		/// <param name="visual">A WPF Visual.</param>
		/// <returns>A GDI+ System.Drawing.Bitmap.</returns>
		public static BitmapImage PngBitmap(Visual visual, int widthDest, int heightDest)
		{
			// Get height and width
			int width= (int)(double)visual.GetValue(FrameworkElement.ActualWidthProperty);
			int height = (int)(double)visual.GetValue(FrameworkElement.ActualHeightProperty);

			// Render
			RenderTargetBitmap rtb = new RenderTargetBitmap( width, height, 96, 96, PixelFormats.Default);
			rtb.Render(visual);
			// Encode
			PngBitmapEncoder encoder = new PngBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(rtb));
			System.IO.MemoryStream stream = new System.IO.MemoryStream();
			encoder.Save(stream);

			BitmapImage myImage = new BitmapImage();
			myImage.BeginInit();
			myImage.StreamSource = stream;
			if (widthDest != 0)
			{
				myImage.DecodePixelWidth = widthDest;
				myImage.DecodePixelHeight = heightDest;
			} 
			myImage.EndInit();
			return myImage;
		}
	}
}
