using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;

namespace ReflectionStudio.Core.Database.Schema
{
	public class ColumnBaseSchema : DataObjectBase
	{
		public ColumnBaseSchema(ITabularObjectBase table, string name, DbType dataType, string nativeType, int size, byte precision, int scale, bool allowDBNull)
		{
			base._Database = table.Database;
			this._Parent = table;
			base._Name = name;
			base._DataType = dataType;
			base._NativeType = nativeType;
			base._Size = size;
			base._Precision = precision;
			base._Scale = scale;
			base._AllowDBNull = allowDBNull;
		}

		public ColumnBaseSchema(ITabularObjectBase table, string name, DbType dataType, string nativeType, int size, byte precision, int scale, bool allowDBNull, List<ExtendedProperty> extendedProperties)
			: this(table, name, dataType, nativeType, size, precision, scale, allowDBNull)
		{
			base._ExtendedProperties = extendedProperties;
		}
		
		protected ITabularObjectBase _Parent;
		[Browsable(false)]
		public ITabularObjectBase Parent
		{
			get
			{
				return this._Parent;
			}
		}

		public bool IsUnique
		{
			get;
			set;
		}

		public bool IsReadOnly
		{
			get;
			set;
		}

		public bool IsKey
		{
			get;
			set;
		}

		public bool IsIdentity
		{
			get;
			set;
		}

		public bool IsAutoIncrement
		{
			get;
			set;
		}

		public override bool Equals(object obj)
		{
			ColumnBaseSchema schema = obj as ColumnBaseSchema;
			return (((schema != null) && schema.Parent.Equals(this.Parent)) && (schema.Name == this.Name));
		}

		public override int GetHashCode()
		{
			return (this.Parent.GetHashCode() ^ this.Name.GetHashCode());
		}

		public override void Refresh()
		{
			this.Parent.Refresh();
		}
	}
}
