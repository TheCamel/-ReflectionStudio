using System;
using System.ServiceModel.Syndication;
using System.Windows.Data;

namespace ReflectionStudio.Controls
{
    /// <summary>
    /// Converts a SyndicationItem into a Uri.
    /// </summary>
    public class SyndicationItemToUriConverter : IValueConverter
    {
        /// <summary>
        /// Returns the converted value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The converter value.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is SyndicationItem)
            {
                SyndicationItem item = (SyndicationItem)value;
                if (item.Links.Count > 0)
                {
                    return item.Links[0].Uri;
                }
            }

            return null;
        }

        /// <summary>
        /// Converts a value back.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The converted back value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
