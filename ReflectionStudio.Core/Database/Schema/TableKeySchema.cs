using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ReflectionStudio.Core.Database.Schema
{
	public class TableKeySchema : SchemaObjectBase
	{
		// Methods
		public TableKeySchema(DatabaseSchema database, string name, string[] foreignKeyMemberColumns, string foreignKeyTable, 
			string[] primaryKeyMemberColumns, string primaryKeyTable)
		{
			base._Database = database;
			base._Name = name;

			_ForeignKeyTable = this.Database.Tables.First( p => p.Name == foreignKeyTable );
			
			foreach (string str in foreignKeyMemberColumns)
			{
				_ForeignKeyMemberColumns.Add((TableColumnSchema)_Database.Tables.First(p => p.Name == foreignKeyTable).Columns.First(p => p.Name == str));
			}

			foreach (string str2 in primaryKeyMemberColumns)
			{
				_PrimaryKeyMemberColumns.Add((TableColumnSchema)_Database.Tables.First(p => p.Name == primaryKeyTable).Columns.First(p => p.Name == str2));
			}

			_PrimaryKeyTable = _Database.Tables.First( p => p.Name == primaryKeyTable);
		}

		public TableKeySchema(DatabaseSchema database, string name, string[] foreignKeyMemberColumns, string foreignKeyTable, string[] primaryKeyMemberColumns, string primaryKeyTable, ExtendedProperty[] extendedProperties)
			: this(database, name, foreignKeyMemberColumns, foreignKeyTable, primaryKeyMemberColumns, primaryKeyTable)
		{
			base._ExtendedProperties = extendedProperties.ToList<ExtendedProperty>();
		}

		

		// Properties
		private List<TableColumnSchema> _ForeignKeyMemberColumns = new List<TableColumnSchema>();
		[Browsable(false)]
		public List<TableColumnSchema> ForeignKeyMemberColumns
		{
			get
			{
				return _ForeignKeyMemberColumns;
			}
		}

		private TableSchema _ForeignKeyTable;
		[Browsable(false)]
		public TableSchema ForeignKeyTable
		{
			get
			{
				return _ForeignKeyTable;
			}
		}

		[Browsable(false)]
		public PrimaryKeySchema PrimaryKey
		{
			get
			{
				return PrimaryKeyTable.PrimaryKey;
			}
		}

		private List<TableColumnSchema> _PrimaryKeyMemberColumns = new List<TableColumnSchema>();
		[Browsable(false)]
		public List<TableColumnSchema> PrimaryKeyMemberColumns
		{
			get
			{
				return _PrimaryKeyMemberColumns;
			}
		}

		private TableSchema _PrimaryKeyTable;
		[Browsable(false)]
		public TableSchema PrimaryKeyTable
		{
			get
			{
				return _PrimaryKeyTable;
			}
		}


		public override bool Equals(object obj)
		{
			TableKeySchema schema = obj as TableKeySchema;
			return (((schema != null) && schema.PrimaryKeyTable.Equals(_PrimaryKeyTable)) && (schema.Name == _Name));
		}

		public override int GetHashCode()
		{
			return (_PrimaryKeyTable.GetHashCode() ^ _Name.GetHashCode());
		}

		public override void Refresh()
		{
			_Database.Refresh();
		}
	}
}
