using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace ReflectionStudio.Core.Database.Schema
{
	//[Editor(typeof(DatabaseSchemaPicker), typeof(UITypeEditor)), TypeConverter(typeof(DatabaseSchemaTypeConverter))]
	public class DatabaseSchema : SchemaObjectBase
	{
		// Fields
		private bool _Initialized;
		private string _DatabaseName;

		// Methods
		public DatabaseSchema()
		{
			base._Database = this;

			this._Provider = null;
			this._Commands = null;
			this._ConnectionString = string.Empty;
			this._DatabaseName = string.Empty;
			this._Initialized = false;
		}

		public DatabaseSchema(IDbSchemaProvider provider, string connectionString)
		{
			base._Database = this;

			this._Commands = null;
			this._ConnectionString = string.Empty;
			this._DatabaseName = string.Empty;
			this._Initialized = false;
			
			this._Provider = provider;
			this._ConnectionString = connectionString;
		}

		private void Clear()
		{
			this._Tables = null;
			this._Views = null;
			this._Commands = null;
			this._Initialized = false;
		}

		internal void Check()
		{
			if (this.Provider == null)
			{
				throw new InvalidOperationException("provider must be specified before using this object.");
			}
			if (this.ConnectionString.Trim().Length == 0)
			{
				throw new InvalidOperationException("connection string must be specified before using this object");
			}
		}

		public override bool Equals(object obj)
		{
			DatabaseSchema schema = obj as DatabaseSchema;
			return (((schema != null) && (schema.ConnectionString == this.ConnectionString)) && (schema.Provider.GetType().FullName == this.Provider.GetType().FullName));
		}

		public override int GetHashCode()
		{
			return (this.ConnectionString.GetHashCode() ^ this.Provider.GetType().FullName.GetHashCode());
		}

		public override void Refresh()
		{
			this.Clear();
		}

		public override string ToString()
		{
			return this.Name;
		}

		// Properties
		private List<CommandSchema> _Commands;
		[Browsable(false)]
		public ReadOnlyCollection<CommandSchema> Commands
		{
			get
			{
				if (this._Commands == null)
				{
					this.Check();
					this._Commands = this.Provider.GetCommands(this.ConnectionString, this);
				}
				return this._Commands.AsReadOnly();
			}
		}

		private string _ConnectionString;
		[Browsable(false), ReadOnly(true)]
		public string ConnectionString
		{
			get
			{
				return this._ConnectionString;
			}
			set
			{
				this.Clear();
				this._ConnectionString = value;
			}
		}

		public override string Name
		{
			get
			{
				if (!this._Initialized)
				{
					this.Check();
					this._DatabaseName = this.Provider.GetDatabaseName(this.ConnectionString);
					this._Initialized = true;
				}
				return this._DatabaseName;
			}
		}

		private IDbSchemaProvider _Provider;
		[Browsable(false)]
		public IDbSchemaProvider Provider
		{
			get
			{
				return this._Provider;
			}
			set
			{
				this.Clear();
				this._Provider = value;
			}
		}

		private List<TableSchema> _Tables;
		[Browsable(false)]
		public ReadOnlyCollection<TableSchema> Tables
		{
			get
			{
				if (this._Tables == null)
				{
					this.Check();
					this._Tables = this.Provider.GetTables(this.ConnectionString, this);
				}
				return this._Tables.AsReadOnly();
			}
		}

		private List<ViewSchema> _Views;
		[Browsable(false)]
		public ReadOnlyCollection<ViewSchema> Views
		{
			get
			{
				if (this._Views == null)
				{
					this.Check();
					this._Views = this.Provider.GetViews(this.ConnectionString, this);
				}
				return this._Views.AsReadOnly();
			}
		}
	}
}
