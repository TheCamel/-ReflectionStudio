using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using ReflectionStudio.Core.Database;
using ReflectionStudio.Core.Database.Schema;

namespace ReflectionStudio.Components.Converters
{
	/// <summary>
	/// Used by the DatabaseExplorer treeview template to convert DB type to images
	/// </summary>
	[ValueConversion(typeof(object), typeof(BitmapImage))]
	class DBTypeToImageConverter : IValueConverter
	{
		/// <summary>
		/// Singleton access
		/// </summary>
		public static readonly DBTypeToImageConverter Instance = new DBTypeToImageConverter(); 

		/// <summary>
		/// Private constructor for singleton pattern
		/// </summary>
		private DBTypeToImageConverter()
		{
		}

		/// <summary>
		/// IValueConverter.Convert DBType to Images
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null) return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/folders/folder_closed.png"));

			Type container = value.GetType();

			if (container == typeof(DataSource))
				return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/database/db_server.png"));

			if (container == typeof(DatabaseSchema))
				return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/database/db.png"));

			if (container == typeof(TableSchema))
				return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/database/table.png"));
			if (container == typeof(ViewSchema))
				return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/database/view.png"));

			if (container == typeof(TableColumnSchema))
			{
				TableColumnSchema info = value as TableColumnSchema;
				if( info.IsPrimaryKeyMember )
					return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/database/key.png"));
				else
					if (info.IsForeignKeyMember)
						return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/database/foreign_key.png"));
					else
				return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/database/column.png"));
			}

			if (container == typeof(PrimaryKeySchema))
				return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/database/key.png"));
			if (container == typeof(TableKeySchema))
				return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/database/foreign_key.png"));


			if (container == typeof(ViewColumnSchema))
				return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/database/column.png"));
			if (container == typeof(IndexSchema))
				return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/database/index.png"));

			if (container == typeof(CommandSchema))
				return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/database/command.png"));
			if (container == typeof(ParameterSchema))
				return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/database/command_param.png"));

			return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/16x16/folders/folder_closed.png"));
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
