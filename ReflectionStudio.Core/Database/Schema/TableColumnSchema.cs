using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;

namespace ReflectionStudio.Core.Database.Schema
{
	public class TableColumnSchema : ColumnBaseSchema
	{
		// Methods
		public TableColumnSchema(TableSchema table, string name, DbType dataType, string nativeType, int size, byte precision, int scale, bool allowDBNull)
			: base( table, name, dataType, nativeType, size, precision, scale, allowDBNull)
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

		public TableColumnSchema(TableSchema table, string name, DbType dataType, string nativeType, int size, byte precision, int scale, bool allowDBNull, List<ExtendedProperty> extendedProperties)
			: this(table, name, dataType, nativeType, size, precision, scale, allowDBNull)
		{
			base._ExtendedProperties = extendedProperties;
		}

		internal TableSchema Table
		{
			get
			{
				return this._Parent as TableSchema;
			}
		}

		public bool IsForeignKeyMember
		{
			get
			{
				for (int i = 0; i < this.Table.ForeignKeys.Count; i++)
				{
					if (this.Table.ForeignKeys[i].ForeignKeyMemberColumns.Contains(this))
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool IsPrimaryKeyMember
		{
			get
			{
				return ((this.Table.PrimaryKey != null) && this.Table.PrimaryKey.MemberColumns.Count(p => p.Name == this.Name) >= 1);
			}
		}

		public bool IsUnique
		{
			get
			{
				for (int i = 0; i < this.Table.Indexes.Count; i++)
				{
					if ((this.Table.Indexes[i].IsUnique && (this.Table.Indexes[i].MemberColumns.Count == 1)) && this.Table.Indexes[i].MemberColumns.Contains(this))
					{
						return true;
					}
				}
				return false;
			}
		}
	}
}
