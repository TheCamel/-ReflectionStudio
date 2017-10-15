using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Collections.ObjectModel;

namespace ReflectionStudio.Core.Database.Schema
{
	//[TypeConverter(typeof(ViewSchemaTypeConverter)), Editor(typeof(ViewSchemaPicker), typeof(UITypeEditor))]
	public class ViewSchema : SchemaObjectBase, ITabularObjectBase
	{
		private List<ViewColumnSchema> _Columns;
		[Browsable(false)]
		public List<ColumnBaseSchema> Columns
		{
			get
			{
				if (_Columns == null)
				{
					_Database.Check();
					_Columns = _Database.Provider.GetViewColumns(this.Database.ConnectionString, this);
				}
				return _Columns.Cast<ColumnBaseSchema>().ToList();
			}
		}

		public string FullName
		{
			get
			{
				return (_Owner + "." + _Name);
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

		private string _Text;
		[Browsable(false)]
		public string Text
		{
			get
			{
				if (_Text == string.Empty)
				{
					_Database.Check();
					_Text = _Database.Provider.GetViewText(this.Database.ConnectionString, this);
				}
				return _Text;
			}
		}

		// Methods
		public ViewSchema(DatabaseSchema database, string name, string owner, DateTime dateCreated)
		{
			_DateCreated = DateTime.MinValue;
			base._Database = database;
			base._Name = name;

			_Text = string.Empty;
			_Owner = owner;
			_DateCreated = dateCreated;

			_CanScript = true;
		}

		public ViewSchema(DatabaseSchema database, string name, string owner, DateTime dateCreated, ExtendedProperty[] extendedProperties)
			: this(database, name, owner, dateCreated)
		{
			base._ExtendedProperties = new List<ExtendedProperty>(extendedProperties);
		}

		public override bool Equals(object obj)
		{
			ViewSchema schema = obj as ViewSchema;
			return (((schema != null) && schema.Database.Equals(this.Database)) && ((schema.Owner == this.Owner) && (schema.Name == this.Name)));
		}

		public override int GetHashCode()
		{
			return ((_Database.GetHashCode() ^ _Owner.GetHashCode()) ^ _Name.GetHashCode());
		}

		public override void Refresh()
		{
			_Owner = null;
			_Text = string.Empty;
		}

		public override string ToString()
		{
			return (_Owner + "." + _Name);
		}
	}
}
