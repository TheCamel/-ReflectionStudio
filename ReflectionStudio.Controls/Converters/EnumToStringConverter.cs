using System;
using System.Windows.Data;

namespace ReflectionStudio.Controls
{
	/// <summary>
	/// Converter : Enum to string and revert
	/// </summary>
	[ValueConversion(typeof(Enum), typeof(string))]
	public class EnumToStringConverter : IValueConverter
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
			return value.ToString();
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
			return Enum.Parse(targetType, value.ToString(), true);
		}
	}
}
