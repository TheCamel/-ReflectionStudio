using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReflectionStudio.Core.Database.Schema;

namespace ReflectionStudio.Core.Database
{
	[Serializable]
	public class DataSource
	{
		#region ----------------------PROPERTIES----------------------

		private string _ConnectionString;
		public string ConnectionString
		{
			get
			{
				return _ConnectionString;
			}
			set
			{
				_ConnectionString = value;
			}
		}

		private string _Name;
		public string Name
		{
			get
			{
				return _Name;
			}
			set
			{
				_Name = value;
			}
		}
		
		private Type _SchemaProviderType;
		public Type SchemaProviderType
		{
			get
			{
				return _SchemaProviderType;
			}
			set
			{
				_SchemaProviderType = value;
			}
		}

		[NonSerialized]
		public DatabaseSchema _Database;
		public DatabaseSchema Database
		{
			get
			{
				if (_Database == null)
				{
					if (this._SchemaProviderType != null)
					{
						return new DatabaseSchema()
						{
							Provider = (IDbSchemaProvider)Activator.CreateInstance(_SchemaProviderType),
							ConnectionString = _ConnectionString
						};
					}
				}
				return _Database;
			}
		}
		#endregion

		#region ----------------------CONSTRUTORS----------------------

		public DataSource()
		{
			_Name = string.Empty;
			_ConnectionString = string.Empty;
			_SchemaProviderType = null;
		}

		public DataSource(string name, string connectionString, string typeName)
		{
			_Name = name;
			_ConnectionString = connectionString;
			_SchemaProviderType = Type.GetType(typeName);
		}

		public DataSource(string name, string connectionString, Type providerType)
		{
			_Name = name;
			_ConnectionString = connectionString;
			_SchemaProviderType = providerType;
		}
		#endregion

		#region ----------------------METHODS----------------------

		public override string ToString()
		{
			return _Name;
		}

		#endregion
	}
}
