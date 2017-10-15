using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ReflectionStudio.Core.Database.Schema
{
	public class IndexSchema : SchemaObjectBase
	{
		public IndexSchema(TableSchema table, string name, bool isPrimaryKey, bool isUnique, bool isClustered, List<string> memberColumns)
		{
			base._Database = table.Database;
			base._Name = name;
			_Table = table;
			_IsPrimaryKey = isPrimaryKey;
			_IsUnique = isUnique;
			_IsClustered = isClustered;
			_MemberColumns = new List<TableColumnSchema>();

			foreach (string str in memberColumns)
			{
				_MemberColumns.Add((TableColumnSchema)_Table.Columns.First(p => p.Name == str));
			}
		}

		public IndexSchema(TableSchema table, string name, bool isPrimaryKey, bool isUnique, bool isClustered, List<string> memberColumns, ExtendedProperty[] extendedProperties)
			: this(table, name, isPrimaryKey, isUnique, isClustered, memberColumns)
		{
			base._ExtendedProperties = new List<ExtendedProperty>(extendedProperties);
		}

		// Properties
		private bool _IsClustered;
		public bool IsClustered
		{
			get
			{
				return this._IsClustered;
			}
		}

		private bool _IsPrimaryKey;
		public bool IsPrimaryKey
		{
			get
			{
				return this._IsPrimaryKey;
			}
		}

		private bool _IsUnique;
		public bool IsUnique
		{
			get
			{
				return this._IsUnique;
			}
		}

		private List<TableColumnSchema> _MemberColumns;
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


		public override bool Equals(object obj)
		{
			IndexSchema schema = obj as IndexSchema;
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
	}
}
