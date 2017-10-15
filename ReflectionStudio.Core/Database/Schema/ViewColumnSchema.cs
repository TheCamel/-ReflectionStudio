using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;

namespace ReflectionStudio.Core.Database.Schema
{
	public class ViewColumnSchema : ColumnBaseSchema
	{
		// Methods
		public ViewColumnSchema(ViewSchema view, string name, DbType dataType, string nativeType, int size, byte precision, int scale, bool allowDBNull)
			: base (view, name, dataType, nativeType, size, precision, scale, allowDBNull)
		{
		}

		public ViewColumnSchema(ViewSchema view, string name, DbType dataType, string nativeType, int size, byte precision, int scale, bool allowDBNull, List<ExtendedProperty> extendedProperties)
			: this(view, name, dataType, nativeType, size, precision, scale, allowDBNull)
		{
			base._ExtendedProperties = new List<ExtendedProperty>(extendedProperties);
		}

		public override bool Equals(object obj)
		{
			ViewColumnSchema schema = obj as ViewColumnSchema;
			return (((schema != null) && schema.Parent.Equals(Parent)) && (schema.Name == _Name));
		}

		public override int GetHashCode()
		{
			return (Parent.GetHashCode() ^ _Name.GetHashCode());
		}

		public override void Refresh()
		{
			Parent.Refresh();
		}
	}
}
