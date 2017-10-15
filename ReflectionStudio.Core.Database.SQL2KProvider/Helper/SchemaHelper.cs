using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;

namespace ReflectionStudio.Core.Database.SQL2KProvider.Helper
{
	/// <summary>
	/// Class base on the new SqlConnection.GetSchema method to retreive shema information
	/// </summary>
	internal class SchemaHelper
	{
		#region ----------------BASE----------------

		internal DataTable GetSchema(string connectionString, string schemaType)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				return connection.GetSchema(schemaType);
			}
		}

		internal DataTable GetSchema(string connectionString, string schemaType, string[] restrictions)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				return connection.GetSchema(schemaType, restrictions);
			}
		}
		#endregion

		#region ----------------DATABASE----------------

		/// <summary>
		/// schema : database_name, dbid, create_date
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		public DataTable GetDataBases(string connectionString)
		{
			return GetSchema(connectionString, "Databases");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		public DataTable GetDataSourceInformation(string connectionString)
		{
			return GetSchema(connectionString, "DataSourceInformation");
		}

		/// <summary>
		/// TypeName, ProviderDbType, columnSize, CreateFormat, CreateParameters, DataType
		/// IsAutoIncrementable, IsBestMatch, IsCaseSensitive, IsFiexedLength, IsFixedPrecisionScale, IsLong
		/// IsNullable, IsSearchable, IsSearchableWithLike, IsUnsigned, MaximunScale, MinimiunScale, IsConcurrencyType
		/// IsLiteralSupported, LiteralPrefix, LiteralSuffix
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		public DataTable GetDataTypes(string connectionString)
		{
			return GetSchema(connectionString, "DataTypes");
		}

		/// <summary>
		/// assembly_name, udt_name, version_major, version_minor, version_build, version_revision, culture_info, 
		/// public_key, is_fixed_length, max_length, Create_Date, Permission_set_desc
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		public DataTable GetUserDefinedTypes(string connectionString)
		{
			return GetSchema(connectionString, "UserDefinedTypes");
		}

		public DataTable GetUserDefinedTypes(string connectionString, string assemblyName, string udtName)
		{
			string[] restrictions = new string[2] { assemblyName, udtName };
			return GetSchema(connectionString, "UserDefinedTypes");
		}

		/// <summary>
		/// ReservedWord
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		public DataTable GetReservedWords(string connectionString)
		{
			return GetSchema(connectionString, "ReservedWords");
		}

		/// <summary>
		/// uid, user_name, createdate, updatedate
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		public DataTable GetUsers(string connectionString)
		{
			return GetSchema(connectionString, "Users");
		}
		#endregion

		#region ----------------TABLES----------------
		/// <summary>
		/// TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		public DataTable GetTables(string connectionString)
		{
			return GetSchema(connectionString, "Tables");
		}

		public DataTable GetTables(string connectionString, string database, string owner, string name, string tabletype )
		{
			string[] restrictions = new string[4] { database, owner, name, tabletype };

			return GetSchema(connectionString, "Tables", restrictions);
		}

		/// <summary>
		/// TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME, ORDINAL_POSITION, COLUMN_DEFAULT, IS_NULLABLE
		/// DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, CHARACTER_OCTET_LENGTH, NUMERIC_PRECISION, NUMERIC_PRECISION_RADIX, NUMERIC_SCALE
		/// DATETIME_PRECISION, CHARACTER_SET_CATALOG, CHARACTER_SET_SCHEMA, CHARACTER_SET_NAME, COLLATION_CATALOG
		/// IS_SPARSE, IS_COLUMN_SET, IS_FILESTREAM
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		public DataTable GetTableColumns(string connectionString)
		{
			return GetSchema(connectionString, "Columns");
		}

		public DataTable GetTableColumns(string connectionString, string database, string owner, string tableName, string colName)
		{
			string[] restrictions = new string[4] { database, owner, tableName, colName };

			return GetSchema(connectionString, "Columns", restrictions);
		}

		#endregion

		#region ----------------VIEWS----------------
		/// <summary>
		/// TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, CHECK_OPTION, IS_UPDATEABLE
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		public DataTable GetViews(string connectionString)
		{
			return GetSchema(connectionString, "Views");
		}

		public DataTable GetViews(string connectionString, string database, string owner, string name)
		{
			string[] restrictions = new string[3] { database, owner, name };

			return GetSchema(connectionString, "Views", restrictions);
		}

		/// <summary>
		/// TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME, VIEW_CATALOG, VIEW_SCHEMA, VIEW_NAME
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		public DataTable GetViewColumns(string connectionString)
		{
			return GetSchema(connectionString, "ViewColumns");
		}

		public DataTable GetViewColumns(string connectionString, string database, string owner, string viewName, string colName)
		{
			string[] restrictions = new string[4] { database, owner, viewName, colName };

			return GetSchema(connectionString, "ViewColumns", restrictions);
		}
		#endregion

		#region ----------------PROCEDURES----------------
		/// <summary>
		/// SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME, ROUTINE_CATALOG, ROUTINE_SCHEMA, ROUTINE_NAME
		/// ROUTINE_TYPE, CREATED, LAST_ALTERED
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		public DataTable GetProcedures(string connectionString)
		{
			return GetSchema(connectionString, "Procedures");
		}

		public DataTable GetProcedures(string connectionString, string database, string owner, string name, string type)
		{
			string[] restrictions = new string[4] { database, owner, name, type };

			return GetSchema(connectionString, "Procedures", restrictions);
		}

		/// <summary>
		/// SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME, ORDINAL_POSITION, PARAMETER_MODE, IS_RESULT, AS_LOCATOR,
		/// PARAMETER_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, CHARACTER_OCTET_LENGTH, COLLATION_CATALOG, COLLATION_SCHEMA
		/// COLLATION_NAME, CHARACTER_SET_CATALOG, CHARACTER_SET_SCHEMA, CHARACTER_SET_NAME,
		/// NUMERIC_PRECISION, NUMERIC_PRECISION_RADIX, NUMERIC_SCALE, DATETIME_PRECISION, INTERVAL_TYPE, INTERVAL_PRECISION
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		public DataTable GetProcedureParameters(string connectionString)
		{
			return GetSchema(connectionString, "ProcedureParameters");
		}

		public DataTable GetProcedureParameters(string connectionString, string database, string owner, string procedureName, string parameterName)
		{
			string[] restrictions = new string[4] { database, owner, procedureName, parameterName };

			return GetSchema(connectionString, "ProcedureParameters", restrictions);
		}
		#endregion

		#region ----------------INDEXES----------------
		/// <summary>
		/// constraint_catalog, constraint_schema, constraint_name, table_catalog, table_schema, table_name, index_name, type_desc
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		public DataTable GetIndexes(string connectionString)
		{
			return GetSchema(connectionString, "Indexes");
		}

		public DataTable GetIndexes(string connectionString, string database, string owner, string tableName, string indexName)
		{
			string[] restrictions = new string[4] { database, owner, tableName, indexName };

			return GetSchema(connectionString, "Indexes", restrictions);
		}

		/// <summary>
		/// constraint_catalog, constraint_schema, constraint_name, table_catalog, table_schema, table_name,
		/// column_name, ordinal_position, KeyType, index_name
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		public DataTable GetIndexColumns(string connectionString)
		{
			return GetSchema(connectionString, "IndexColumns");
		}

		public DataTable GetIndexColumns(string connectionString, 
			string database, string owner, string tableName, string constraintName, string colName)
		{
			string[] restrictions = new string[5] { database, owner, tableName, constraintName, colName };

			return GetSchema(connectionString, "IndexColumns", restrictions);
		}
		#endregion

		#region ----------------FOREIGN KEYS----------------
		/// <summary>
		/// constraint_catalog, constraint_schema, constraint_name, table_catalog, table_schema, table_name,
		/// CONSTRAINT_TYPE, IS_DEFERRABLE, INITIALLY_DEFERRED
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		public DataTable GetForeignKeys(string connectionString)
		{
			return GetSchema(connectionString, "ForeignKeys");
		}

		public DataTable GetForeignKeys(string connectionString, string database, string owner, string tableName, string fkName)
		{
			string[] restrictions = new string[4] { database, owner, tableName, fkName };

			return GetSchema(connectionString, "ForeignKeys", restrictions);
		}
		#endregion

		#region ----------------ALL COLUMNS----------------
		/// <summary>
		///  TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME, ORDINAL_POSITION, COLUMN_DEFAULT, IS_NULLABLE
		///  DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, CHARACTER_OCTET_LENGTH, NUMERIC_PRECISION, NUMERIC_PRECISION_RADIX, NUMERIC_SCALE
		/// DATETIME_PRECISION, CHARACTER_SET_CATALOG, CHARACTER_SET_SCHEMA, CHARACTER_SET_NAME, COLLATION_CATALOG
		/// IS_SPARSE, IS_COLUMN_SET, IS_FILESTREAM
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		public DataTable GetAllColumns(string connectionString)
		{
			return GetSchema(connectionString, "AllColumns");
		}

		public DataTable GetAllColumns(string connectionString, string database, string owner, string tableName, string colName)
		{
			string[] restrictions = new string[4] { database, owner, tableName, colName };

			return GetSchema(connectionString, "AllColumns", restrictions);
		}
		#endregion


		/// <summary>
		/// ColumnName, ColumnOrdinal, ColumnSize, NumericPrecision, NumericScale, IsUnique, IsKey, BaseServerName, BaseCatalogName, 
		/// BaseColumnName, BaseSchemaName, BaseTableName, DataType, AllowDBNull, ProviderType, IsAliased, IsExpression, IsIdentity,
		/// IsAutoIncrement, IsRowVersion, IsHidden, IsLong, IsReadOnly, ProviderSpecificDataType, DataTypeName, ..., IsColumnSet
		/// </summary>
		/// <param name="connectionString"></param>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public DataTable GetSchemaFromReader(string connectionString, string sqlObjectName)
		{
			DataTable dataTable = null;

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();

				using (IDbCommand dbCommand = connection.CreateCommand())
				{
					dbCommand.CommandType = CommandType.Text;
					dbCommand.CommandText = "SELECT * FROM " + sqlObjectName;
					using (IDataReader dataReader = dbCommand.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
					{
						dataTable = dataReader.GetSchemaTable();
						dataReader.Close();
					}
				}
			}

			return dataTable;
		}
	}
}
