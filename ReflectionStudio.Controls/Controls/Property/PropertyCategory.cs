using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReflectionStudio.Controls.Property
{
	public class PropertyCategory : PropertyBaseList
	{
		// Fields
		private readonly string _categoryName;

		// Methods
		public PropertyCategory()
		{
			this._categoryName = "Misc";
		}

		public PropertyCategory(string categoryName)
		{
			this._categoryName = categoryName;
		}

		// Properties
		public string Category
		{
			get
			{
				return this._categoryName;
			}
		}
	}
}
