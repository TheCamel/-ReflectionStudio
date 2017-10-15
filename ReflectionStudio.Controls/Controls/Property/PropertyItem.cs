using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ReflectionStudio.Controls.Property
{
	public class PropertyItem : PropertyBase, IDisposable
	{
		// Fields
		private object _instance;
		private PropertyDescriptor _property;

		// Methods
		public PropertyItem(object instance, PropertyDescriptor property)
		{
			this._instance = instance;
			this._property = property;
			this._property.AddValueChanged(this._instance, new EventHandler(this.instance_PropertyChanged));
		}

		protected override void Dispose(bool disposing)
		{
			if (!base.Disposed)
			{
				if (disposing)
				{
					this._property.RemoveValueChanged(this._instance, new EventHandler(this.instance_PropertyChanged));
				}
				base.Dispose(disposing);
			}
		}

		private void instance_PropertyChanged(object sender, EventArgs e)
		{
			base.NotifyPropertyChanged("Value");
		}

		// Properties
		public string Description
		{
			get
			{
				return this._property.Description;
			}
		}

		public string Category
		{
			get
			{
				return this._property.Category;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this._property.IsReadOnly;
			}
		}

		public bool IsWriteable
		{
			get
			{
				return !this.IsReadOnly;
			}
		}

		public string Name
		{
			get
			{
				return this._property.Name;
			}
		}

		public Type PropertyType
		{
			get
			{
				return this._property.PropertyType;
			}
		}

		public object Value
		{
			get
			{
				return this._property.GetValue(this._instance);
			}
			set
			{
				object currentValue = this._property.GetValue(this._instance);
				if ((value == null) || !value.Equals(currentValue))
				{
					Type propertyType = this._property.PropertyType;
					if (((propertyType == typeof(object)) || ((value == null) && propertyType.IsClass)) || ((value != null) && propertyType.IsAssignableFrom(value.GetType())))
					{
						this._property.SetValue(this._instance, value);
					}
					else
					{
						object convertedValue = TypeDescriptor.GetConverter(this._property.PropertyType).ConvertFrom(value);
						this._property.SetValue(this._instance, convertedValue);
					}
				}
			}
		}
	}
}
