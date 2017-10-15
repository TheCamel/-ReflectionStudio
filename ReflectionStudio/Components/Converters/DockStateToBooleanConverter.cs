using System;
using System.Windows.Data;
using AvalonDock;

namespace ReflectionStudio.Components.Converters
{
	/// <summary>
	/// convert DockableContentState from Avalon to boolean, used to check explorers view checkboxes
	/// </summary>
	[ValueConversion(typeof(DockableContentState), typeof(bool))]
	class DockStateToBooleanConverter : IValueConverter
	{
		/// <summary>
		/// Singleton access
		/// </summary>
		public static readonly DockStateToBooleanConverter Instance = new DockStateToBooleanConverter(); 

		/// <summary>
		/// Private constructor for singleton pattern
		/// </summary>
		private DockStateToBooleanConverter()
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
			if (value != null)
			{
				DockableContentState state = (DockableContentState)value;
				if (state == DockableContentState.Hidden)
					return false;
			}
			return true;
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
