using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Collections.ObjectModel;

namespace ReflectionStudio.Core.Database.Schema
{
	//[Editor(typeof(TableSchemaPicker), typeof(UITypeEditor)), TypeConverter(typeof(TableSchemaTypeConverter))]
	public class TableSchema : SchemaObjectBase, ITabularObjectBase
	{
		// Methods
		public TableSchema(DatabaseSchema database, string name, string owner, DateTime dateCreated)
		{
			base._Database = database;
			base._Name = name;

			_DateCreated = DateTime.MinValue;
			_Owner = owner;
			_DateCreated = dateCreated;

			_CanScript = true;
		}

		public TableSchema(DatabaseSchema database, string name, string owner, DateTime dateCreated, List<ExtendedProperty> extendedProperties)
			: this(database, name, owner, dateCreated)
		{
			base._ExtendedProperties = extendedProperties;
		}

		public override bool Equals(object obj)
		{
			TableSchema schema = obj as TableSchema;
			return (((schema != null) && schema.Database.Equals(this.Database)) && ((schema.Owner == this.Owner) && (schema.Name == this.Name)));
		}

		public override int GetHashCode()
		{
			return ((_Database.GetHashCode() ^ _Owner.GetHashCode()) ^ _Name.GetHashCode());
		}

		public bool IsDependantOf(TableSchema table)
		{
			List<TableSchema> schemas = new List<TableSchema>();
			return IsDependantOf(table, schemas);
		}

		private bool IsDependantOf(TableSchema schema1, List<TableSchema> collection1)
		{
			for (int i = 0; i < schema1.ForeignKeys.Count; i++)
			{
				TableSchema primaryKeyTable = schema1.ForeignKeys[i].PrimaryKeyTable;
				if (primaryKeyTable.Equals(this))
				{
					return true;
				}
				if (!collection1.Contains(primaryKeyTable))
				{
					collection1.Add(primaryKeyTable);
					if (IsDependantOf(primaryKeyTable, collection1))
					{
						return true;
					}
				}
			}
			return false;
		}

		public override void Refresh()
		{
			_PrimaryKey = null;
			_PrimaryKeys = null;
			_Keys = null;
			_Indexes = null;
			_Columns = null;
			_NonKeyColumns = null;
			_NonPrimaryKeyColumns = null;
			_ForeignKeys = null;
		}

		public override string ToString()
		{
			return FullName;
		}

		// Properties
		private List<TableColumnSchema> _Columns;
		[Browsable(false)]
		public List<ColumnBaseSchema> Columns
		{
			get
			{
				if (this._Columns == null)
				{
					_Database.Check();
					_Columns = _Database.Provider.GetTableColumns(_Database.ConnectionString, this);
				}

				return _Columns.Cast<ColumnBaseSchema>().ToList();
			}
		}

		private DateTime _DateCreated;
		public DateTime DateCreated
		{
			get
			{
				return _DateCreated;
			}
		}

		private List<TableKeySchema> _ForeignKeys;
		[Browsable(false)]
		public ReadOnlyCollection<TableKeySchema> ForeignKeys
		{
			get
			{
				if (_ForeignKeys == null)
				{
					_ForeignKeys = Keys.Where(p => p.ForeignKeyTable == this).ToList<TableKeySchema>();
				}
				return _ForeignKeys.AsReadOnly();
			}
		}

		public string FullName
		{
			get
			{
				return _Owner + "." + _Name;
			}
		}

		private List<IndexSchema> _Indexes;
		[Browsable(false)]
		public ReadOnlyCollection<IndexSchema> Indexes
		{
			get
			{
				if (_Indexes == null)
				{
					_Database.Check();
					_Indexes = _Database.Provider.GetTableIndexes(_Database.ConnectionString, this);
				}
				return _Indexes.AsReadOnly();
			}
		}

		private List<TableKeySchema> _Keys;
		[Browsable(false)]
		public ReadOnlyCollection<TableKeySchema> Keys
		{
			get
			{
				if (_Keys == null)
				{
					_Database.Check();
					_Keys = _Database.Provider.GetTableKeys(_Database.ConnectionString, this);
				}
				return _Keys.AsReadOnly();
			}
		}

		private List<TableColumnSchema> _NonKeyColumns;
		[Browsable(false)]
		public ReadOnlyCollection<TableColumnSchema> NonKeyColumns
		{
			get
			{
				if (_NonKeyColumns == null)
				{
					_NonKeyColumns = _Columns.Where(p => !p.IsPrimaryKeyMember && !p.IsForeignKeyMember).ToList<TableColumnSchema>();
				}
				return _NonKeyColumns.AsReadOnly();
			}
		}

		private List<TableColumnSchema> _NonPrimaryKeyColumns;
		[Browsable(false)]
		public ReadOnlyCollection<TableColumnSchema> NonPrimaryKeyColumns
		{
			get
			{
				if (_NonPrimaryKeyColumns == null)
				{
					_NonPrimaryKeyColumns = _Columns.Where(p => !p.IsPrimaryKeyMember).ToList<TableColumnSchema>();
				}
				return _NonPrimaryKeyColumns.AsReadOnly();
			}
		}

		private string _Text;
		[Browsable(false)]
		public string Text
		{
			get
			{
				if (_Text == string.Empty)
				{
					_Database.Check();
					//_Text = _Database.Provider.GetWriter().Create( this );
					_Text = "";
				}
				return _Text;
			}
		}

		private PrimaryKeySchema _PrimaryKey;
		[Browsable(false)]
		public PrimaryKeySchema PrimaryKey
		{
			get
			{
				if (_PrimaryKey == null)
				{
					_Database.Check();
					_PrimaryKey = _Database.Provider.GetTablePrimaryKey(_Database.ConnectionString, this);
				}
				return _PrimaryKey;
			}
		}

		private List<TableKeySchema> _PrimaryKeys;
		[Browsable(false)]
		public ReadOnlyCollection<TableKeySchema> PrimaryKeys
		{
			get
			{
				if (_PrimaryKeys == null)
				{
					_PrimaryKeys = _Keys.Where(p => p.ForeignKeyTable != this).ToList<TableKeySchema>();
				}
				return _PrimaryKeys.AsReadOnly();
			}
		}
	} 
}
