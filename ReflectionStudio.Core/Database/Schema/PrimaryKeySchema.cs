using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ReflectionStudio.Core.Database.Schema
{
	public class PrimaryKeySchema : SchemaObjectBase
	{
		// Methods
		public PrimaryKeySchema(TableSchema table, string name, List<string> memberColumns)
		{
			base._Database = table.Database;
			base._Name = name;

			_Table = table;

			foreach (string str in memberColumns)
			{
				_MemberColumns.Add((TableColumnSchema)_Table.Columns.First(p => p.Name == str));
			}
		}

		public PrimaryKeySchema(TableSchema table, string name, List<string> memberColumns, ExtendedProperty[] extendedProperties)
			: this(table, name, memberColumns)
		{
			base._ExtendedProperties = new List<ExtendedProperty>(extendedProperties);
		}

		public override bool Equals(object obj)
		{
			PrimaryKeySchema schema = obj as PrimaryKeySchema;
			return (((schema != null) && schema.Table.Equals(this.Table)) && (schema.Name == this.Name));
		}

		public override int GetHashCode()
		{
			return (this.Table.GetHashCode() ^ this.Name.GetHashCode());
		}

		public override void Refresh()
		{
			this.Table.Refresh();
		}

		// Properties
		private List<TableColumnSchema> _MemberColumns = new List<TableColumnSchema>();
		[Browsable(false)]
		public List<TableColumnSchema> MemberColumns
		{
			get
			{
				return this._MemberColumns;
			}
		}

		private TableSchema _Table;
		[Browsable(false)]
		public TableSchema Table
		{
			get
			{
				return this._Table;
			}
		}
	}


}
