using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ReflectionStudio.Controls.Property
{
	public class PropertyTemplateSelector : DataTemplateSelector
	{
		// Methods
		private DataTemplate FindDataTemplate(PropertyItem property, FrameworkElement element)
		{
			Type propertyType = property.PropertyType;
			DataTemplate template = TryFindDataTemplate(element, propertyType);
			while ((template == null) && (propertyType.BaseType != null))
			{
				propertyType = propertyType.BaseType;
				template = TryFindDataTemplate(element, propertyType);
			}
			if (template == null)
			{
				template = TryFindDataTemplate(element, "default");
			}
			return template;
		}

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			PropertyItem property = item as PropertyItem;
			if (property == null)
			{
				throw new ArgumentException("item must be of type Property");
			}
			FrameworkElement element = container as FrameworkElement;
			if (element == null)
			{
				return base.SelectTemplate(property.Value, container);
			}
			return this.FindDataTemplate(property, element);
		}

		private static DataTemplate TryFindDataTemplate(FrameworkElement element, object dataTemplateKey)
		{
			object dataTemplate = element.TryFindResource(dataTemplateKey);
			if (dataTemplate == null)
			{
				dataTemplateKey = new ComponentResourceKey(typeof(PropertyGrid), dataTemplateKey);
				dataTemplate = element.TryFindResource(dataTemplateKey);
			}
			return (dataTemplate as DataTemplate);
		}
	}

}
