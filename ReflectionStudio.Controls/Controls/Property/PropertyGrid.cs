using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ReflectionStudio.Controls.Property;
using System.ComponentModel;
using System.Collections.Generic;
using System;

namespace ReflectionStudio.Controls
{
	public class PropertyGrid : Control
	{
		// Fields
		public static readonly DependencyProperty InstanceProperty =
			DependencyProperty.Register("Instance", typeof(object), typeof(PropertyGrid),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(PropertyGrid.OnInstanceChanged),
													new CoerceValueCallback(PropertyGrid.CoerceInstance)));
		public static readonly DependencyProperty NullInstanceProperty =
			DependencyProperty.Register("NullInstance", typeof(object), typeof(PropertyGrid),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(PropertyGrid.OnNullInstanceChanged)));

		public static readonly DependencyProperty PropertiesProperty =
			DependencyProperty.Register("Properties", typeof(ObservableCollection<PropertyBase>), typeof(PropertyGrid),
				new FrameworkPropertyMetadata(new ObservableCollection<PropertyBase>(),
												new PropertyChangedCallback(PropertyGrid.OnPropertiesChanged)));

		// Methods
		static PropertyGrid()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGrid), new FrameworkPropertyMetadata(typeof(PropertyGrid)));
		}

		private static object CoerceInstance(DependencyObject d, object value)
		{
			PropertyGrid propertyGrid = d as PropertyGrid;
			if (value == null)
			{
				return propertyGrid.NullInstance;
			}
			return value;
		}

		private static void OnInstanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PropertyGrid propertyGrid = d as PropertyGrid;
			if (e.NewValue == null)
			{
				propertyGrid.Properties = new ObservableCollection<PropertyBase>();
			}
			else
			{
				propertyGrid.Properties = propertyGrid.Parse(e.NewValue);
			}
		}

		private static void OnNullInstanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
		}

		private static void OnPropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PropertyGrid propertyGrid = d as PropertyGrid;

			ObservableCollection<PropertyBase> properties = e.OldValue as ObservableCollection<PropertyBase>;
			foreach (PropertyBase item in properties)
			{
				item.Dispose();
			}
		}

		// Properties
		public object Instance
		{
			get
			{
				return base.GetValue(InstanceProperty);
			}
			set
			{
				base.SetValue(InstanceProperty, value);
			}
		}

		public object NullInstance
		{
			get
			{
				return base.GetValue(NullInstanceProperty);
			}
			set
			{
				base.SetValue(NullInstanceProperty, value);
			}
		}

		public ObservableCollection<PropertyBase> Properties
		{
			get
			{
				return (ObservableCollection<PropertyBase>)base.GetValue(PropertiesProperty);
			}
			set
			{
				base.SetValue(PropertiesProperty, value);
			}
		}


		private ObservableCollection<PropertyBase> Parse(object instance)
		{
			ObservableCollection<PropertyBase> Items = new ObservableCollection<PropertyBase>();
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(instance);

			Dictionary<string, PropertyCategory> groups = new Dictionary<string, PropertyCategory>();
			List<PropertyItem> propertyCollection = new List<PropertyItem>();

			foreach (PropertyDescriptor propertyDescriptor in properties)
			{
				this.CollectProperties(instance, propertyDescriptor, propertyCollection);
			}

			foreach (PropertyItem property in propertyCollection)
			{
				PropertyCategory propertyCategory;
				if (groups.ContainsKey(property.Category))
				{
					propertyCategory = groups[property.Category];
				}
				else
				{
					propertyCategory = new PropertyCategory(property.Category);
					groups[property.Category] = propertyCategory;
					Items.Add(propertyCategory);
				}

				propertyCategory.Items.Add(property);
			}

			return Items;
		}

		private void CollectProperties(object instance, PropertyDescriptor descriptor, List<PropertyItem> propertyCollection)
		{
			if (descriptor.Attributes[typeof(FlatAttribute)] == null)
			{
				if (descriptor.IsBrowsable)
				{
					PropertyItem property = new PropertyItem(instance, descriptor);
					propertyCollection.Add(property);
				}
			}
			else
			{
				instance = descriptor.GetValue(instance);
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(instance);
				foreach (PropertyDescriptor propertyDescriptor in properties)
				{
					this.CollectProperties(instance, propertyDescriptor, propertyCollection);
				}
			}
		}
	}
}
