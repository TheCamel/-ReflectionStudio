using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using ReflectionStudio.Core.FileManagement;

namespace ReflectionStudio.Components.Converters
{
	/// <summary>
	/// Convert DiskContent to Images
	/// </summary>
	[ValueConversion(typeof(object), typeof(BitmapImage))]
	class FileInfoToImageConverter : IValueConverter
	{
		/// <summary>
		/// Singleton access
		/// </summary>
		public static readonly FileInfoToImageConverter Instance = new FileInfoToImageConverter(); 

		/// <summary>
		/// Private constructor for singleton pattern
		/// </summary>
		private FileInfoToImageConverter()
		{
		}

		/// <summary>
		/// IValueConverter.Convert
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null) return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/folders/folder_closed.png"));

			DiskContent container = value as DiskContent;

			if (container.IsFolder)
			{
				if (parameter.ToString() == "Closed")
					return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/folders/folder_closed.png"));
				else
					return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/folders/folder_opened.png"));
			}

			return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/plugin.png"));
		}

		/// <summary>
		/// IValueConverter.ConvertBack
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
