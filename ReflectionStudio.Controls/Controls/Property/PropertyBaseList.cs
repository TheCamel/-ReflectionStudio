using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace ReflectionStudio.Controls.Property
{
	public class PropertyBaseList : PropertyBase
	{
		// Fields
		private readonly ObservableCollection<PropertyBase> _items = new ObservableCollection<PropertyBase>();

		// Methods
		protected PropertyBaseList()
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (!base.Disposed)
			{
				if (disposing)
				{
					foreach (PropertyBase item in this.Items)
					{
						item.Dispose();
					}
				}
				base.Dispose(disposing);
			}
		}

		// Properties
		public ObservableCollection<PropertyBase> Items
		{
			get
			{
				return this._items;
			}
		}
	}
}
