using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ReflectionStudio.Core.Database.Schema
{
	public abstract class SchemaObjectBase
	{
		// Fields
		protected bool _gotExtendedProperties = false;

		// Properties
		protected DatabaseSchema _Database;
		[Browsable(false)]
		public virtual DatabaseSchema Database
		{
			get
			{
				return _Database;
			}
		}

		//protected string _Description = string.Empty;
		public virtual string Description
		{
			get
			{
				if (ExtendedProperties != null)
				{
					ExtendedProperty prop = ExtendedProperties.First(p => p.Name == "Description");
					if (prop != null)
						return prop.Value.ToString();
				}
				return string.Empty;
			}
		}

		protected List<ExtendedProperty> _ExtendedProperties = null;
		[Browsable(false)]
		public virtual List<ExtendedProperty> ExtendedProperties
		{
			get
			{
				if ((_ExtendedProperties == null) || !_gotExtendedProperties)
				{
					Database.Check();
					_ExtendedProperties = this.Database.Provider.GetExtendedProperties(this.Database.ConnectionString, this);

					//if (_ExtendedProperties == null)
					//{
					//    _ExtendedProperties = this.Database.Provider.GetExtendedProperties(this.Database.ConnectionString, this);
					//}
					//else
					//{
					//    ExtendedProperty[] extendedProperties = this.Database.Provider.GetExtendedProperties(this.Database.ConnectionString, this);
					//    for (int i = 0; i < extendedProperties.Length; i++)
					//    {
					//        int index = this._ExtendedProperties.IndexOf(extendedProperties[i].Name);
					//        if (index >= 0)
					//        {
					//            this._ExtendedProperties[index] = extendedProperties[i];
					//        }
					//        else
					//        {
					//            this._ExtendedProperties.Add(extendedProperties[i]);
					//        }
					//    }
					//}
				}

				return _ExtendedProperties;
			}
		}

		protected string _Name = string.Empty;
		public virtual string Name
		{
			get
			{
				return _Name;
			}
		}

		protected string _Owner;
		public string Owner
		{
			get
			{
				return _Owner;
			}
		}

		protected bool _CanScript = false;
		public bool CanScript
		{
			get
			{
				return _CanScript;
			}
		}

		// Methods
		public virtual void Refresh()
		{
			_gotExtendedProperties = false;
			_ExtendedProperties = null;
		}
	}
}
