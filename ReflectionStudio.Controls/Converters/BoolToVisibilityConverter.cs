using System;
using System.Windows.Data;

namespace ReflectionStudio.Controls
{
	/// <summary>
	/// Converter : Boolean to System.Windows.Visibility and not revert
	/// </summary>
	[ValueConversion(typeof(bool), typeof(System.Windows.Visibility))]
	public class BoolToVisibilityConverter : IValueConverter
	{
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
			bool val = (bool)value;
			if (val)
			{
				return System.Windows.Visibility.Visible;
			}
			else
			{
				return System.Windows.Visibility.Hidden;
			}
		}

		/// <summary>
		/// IValueConverter.ConvertBack (not implemented)
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return false;
		}
	}
}
