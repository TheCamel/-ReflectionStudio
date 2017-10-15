using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using ReflectionStudio.Core.Events;

namespace ReflectionStudio.Components.Converters
{
	/// <summary>
	/// Convert Error log to Images
	/// </summary>
	[ValueConversion(typeof(MessageEventType), typeof(BitmapImage))]
	public class LogTypeToImageConverter : IValueConverter
	{
		/// <summary>
		/// Singleton access
		/// </summary>
		public static readonly LogTypeToImageConverter Instance = new LogTypeToImageConverter(); 

		/// <summary>
		/// Private constructor for singleton pattern
		/// </summary>
		private LogTypeToImageConverter()
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
			MessageEventType val = (MessageEventType)value;
			if (val == MessageEventType.Error)
			{
				return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/error.png"));
			}
			else
			{
				return null;
			}
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
