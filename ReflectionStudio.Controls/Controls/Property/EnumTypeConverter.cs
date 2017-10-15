using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace ReflectionStudio.Controls.Property
{
	public class EnumTypeConverter : IValueConverter
	{
		// Methods
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Enum.GetValues(value.GetType());
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
