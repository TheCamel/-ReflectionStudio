using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using ReflectionStudio.Core.Reflection.Types;

namespace ReflectionStudio.Components.Converters
{
	/// <summary>
	/// Convert assembly schema object to image
	/// </summary>
	[ValueConversion(typeof(object), typeof(BitmapImage))]
	public class NetTypeToImageConverter : IValueConverter
	{
		/// <summary>
		/// Singleton access
		/// </summary>
		public static readonly NetTypeToImageConverter Instance = new NetTypeToImageConverter(); 

		/// <summary>
		/// Private constructor for singleton pattern
		/// </summary>
		private NetTypeToImageConverter()
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
			if (value == null) return null;

			if (value.ToString() == "FolderGroup")
				return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/assembly/references.png"));

			if (value.GetType() == typeof(NetAssembly))
				return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/assembly/assembly.png"));

			if (value.GetType() == typeof(NetReference))
				return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/assembly/assembly.png"));
			if (value.GetType() == typeof(NetNamespace))
				return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/assembly/namespace.png"));

			if (value.GetType() == typeof(NetClass))
				return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/assembly/class.png"));
			if (value.GetType() == typeof(NetInterface))
				return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/assembly/interfaces.png"));
			if (value.GetType() == typeof(NetEnum))
				return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/assembly/enum.png"));


			if (value.GetType() == typeof(NetMethod))
			{
				NetMethod method = value as NetMethod;

				if (method.Tag.GetType().ToString() == "System.Reflection.RuntimeConstructorInfo")
					return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/assembly/constructor.png"));

				if (method.Tag.GetType().ToString() == "System.Reflection.RuntimeMethodInfo")
					return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/assembly/method.png"));

				if (method.Tag.GetType().ToString() == "System.Reflection.RuntimePropertyInfo")
					return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/assembly/properties.png"));

				if (method.Tag.GetType().ToString() == "System.Reflection.RtFieldInfo")
					return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/assembly/member.png"));

				if (method.Tag.GetType().ToString() == "System.Reflection.MdFieldInfo")
					return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/assembly/member.png"));

				if (method.Tag.GetType().ToString() == "System.Reflection.RuntimeEventInfo")
					return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/assembly/event.png"));
			}
			return null;
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
