using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ReflectionStudio.Core.Database.Schema;
using System.Data.SqlClient;
using System.Collections;
using ReflectionStudio.Core.Database.SQL2KProvider.Helper;
using ReflectionStudio.Core.Database.SQL2KProvider.UI;

namespace ReflectionStudio.Core.Database.SQL2KProvider
{
	/// <summary>
	/// Implement the IDbSchemaProvider interface and is in charge of parsing database and filling
	/// schema objects
	/// </summary>
	public class SqlSchemaProvider : IDbSchemaProvider
	{
		#region ----------------PRIVATE TYPES----------------

		private string _ServerVersion = string.Empty;
		private string _DatabaseName;
		private List<DataTypeHolder> _DataTypes = new List<DataTypeHolder>();
		private EmbeddedQueryManager _Queries = new EmbeddedQueryManager("ReflectionStudio.Core.Database.SQL2KProvider", "Queries.xml");

		#endregion

		#region ----------------PROPERTIES----------------

		/// <summary>
		/// Provider description
		/// </summary>
		public string Description
		{
			get
			{
				return "SQL Server 2K schema provider";
			}
		}

		/// <summary>
		/// Provider name
		/// </summary>
		public string Name
		{
			get
			{
				return "SQLServerSchemaProvider";
			}
		}

		#endregion

		#region ----------------CONSTRUCTOR----------------

		/// <summary>
		/// Constructor, initialize all necessary data
		/// </summary>
		public SqlSchemaProvider()
		{
			_DataTypes.Add(new DataTypeHolder("binary", typeof(byte[]), DbType.Binary, SqlDbType.Binary));
			_DataTypes.Add(new DataTypeHolder("varbinary", typeof(byte[]), DbType.Binary, SqlDbType.VarBinary));

			_DataTypes.Add(new DataTypeHolder("int", typeof(Int32), DbType.Int32, SqlDbType.Int));
			_DataTypes.Add(new DataTypeHolder("smallint", typeof(Int16), DbType.Int16, SqlDbType.SmallInt));
			_DataTypes.Add(new DataTypeHolder("tinyint", typeof(SByte), DbType.UInt16, SqlDbType.TinyInt));
			_DataTypes.Add(new DataTypeHolder("bigint", typeof(Int64), DbType.Int64, SqlDbType.BigInt));

			_DataTypes.Add(new DataTypeHolder("char", typeof(string), DbType.String, SqlDbType.Char));
			_DataTypes.Add(new DataTypeHolder("nchar", typeof(string), DbType.String, SqlDbType.NChar));
			_DataTypes.Add(new DataTypeHolder("text", typeof(string), DbType.String, SqlDbType.Text));
			_DataTypes.Add(new DataTypeHolder("ntext", typeof(string), DbType.String, SqlDbType.NText));
			_DataTypes.Add(new DataTypeHolder("varchar", typeof(string), DbType.String, SqlDbType.VarChar));
			_DataTypes.Add(new DataTypeHolder("nvarchar", typeof(string), DbType.String, SqlDbType.NVarChar));

			_DataTypes.Add(new DataTypeHolder("date", typeof(DateTime), DbType.Date, SqlDbType.Date));
			_DataTypes.Add(new DataTypeHolder("time", typeof(DateTime), DbType.Time, SqlDbType.Time));
			_DataTypes.Add(new DataTypeHolder("datetime", typeof(DateTime), DbType.DateTime, SqlDbType.DateTime));
			_DataTypes.Add(new DataTypeHolder("datetime2", typeof(DateTime), DbType.DateTime2, SqlDbType.DateTime2));
			_DataTypes.Add(new DataTypeHolder("datetimeoffset", typeof(DateTime), DbType.DateTimeOffset, SqlDbType.DateTimeOffset));
			_DataTypes.Add(new DataTypeHolder("smalldatetime", typeof(DateTime), DbType.DateTime, SqlDbType.SmallDateTime));
			_DataTypes.Add(new DataTypeHolder("timestamp", typeof(byte[]), DbType.Binary, SqlDbType.Timestamp));

			_DataTypes.Add(new DataTypeHolder("bit", typeof(bool), DbType.Boolean, SqlDbType.Bit));

			_DataTypes.Add(new DataTypeHolder("float", typeof(double), DbType.Double, SqlDbType.Float));
			_DataTypes.Add(new DataTypeHolder("decimal", typeof(decimal), DbType.Double, SqlDbType.Decimal));
			_DataTypes.Add(new DataTypeHolder("real", typeof(Single), DbType.Single, SqlDbType.Real));

			_DataTypes.Add(new DataTypeHolder("image", typeof(byte[]), DbType.Binary, SqlDbType.Image));
			_DataTypes.Add(new DataTypeHolder("money", typeof(decimal), DbType.Decimal, SqlDbType.Money));
			_DataTypes.Add(new DataTypeHolder("smallmoney", typeof(decimal), DbType.Currency, SqlDbType.SmallMoney));
			
			_DataTypes.Add(new DataTypeHolder("numeric", typeof(decimal), DbType.Decimal, SqlDbType.Decimal));
			_DataTypes.Add(new DataTypeHolder("uniqueidentifier", typeof(Guid), DbType.Guid, SqlDbType.UniqueIdentifier));
			
			_DataTypes.Add(new DataTypeHolder("sql_variant", typeof(object), DbType.Object, SqlDbType.Variant));
			_DataTypes.Add(new DataTypeHolder("xml", typeof(string), DbType.Xml, SqlDbType.Xml));
		}

		#endregion

		#region ----------------DATABASE----------------

		/// <summary>
		/// Return the database name
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns>Return the database name as string</returns>
		public string GetDatabaseName(string connectionString)
		{
			if (string.IsNullOrEmpty(_DatabaseName))
			{
				// Open a connection
				using (SqlConnection conn = new SqlConnection(connectionString))
				{
					conn.Open();
					_DatabaseName = conn.Database;
				}
			}
			return _DatabaseName;
		}

		#endregion

		#region ----------------TABLES----------------

		/// <summary>
		/// Return all tables for the given DatabaseSchema object
		/// </summary>
		/// <param name="connectionString"></param>
		/// <param name="database">DatabaseSchema</param>
		/// <returns></returns>
		public List<TableSchema> GetTables(string connectionString, DatabaseSchema database)
		{
			string str = string.Format(_Queries.GetQuery("TableSchema"), database.Name);

			DataTable table = new SQLQueryHelper(connectionString).ExecuteData(str);
			List<TableSchema> schemaArray = new List<TableSchema>(table.Rows.Count);

			for (int i = 0; i < table.Rows.Count; i++)
			{
				List<ExtendedProperty> list = null;

				string strext = string.Format(_Queries.GetQuery("ExtendedProperty"), (string)table.Rows[i][0]);

				DataTable dtEx = new SQLQueryHelper(connectionString).ExecuteData(strext);
				if (dtEx.Rows.Count == 1)
				{
					list = new List<ExtendedProperty>();
					list.Add(new ExtendedProperty("Description", (string)dtEx.Rows[0]["value"], DbType.String));
				}
				else list = null;

				schemaArray.Add(new TableSchema(database, (string)table.Rows[i][0], (string)table.Rows[i][1], (DateTime)table.Rows[i][2], list) );
			}
			return schemaArray;
		}

		/// <summary>
		/// Return all columns for a given table
		/// </summary>
		/// <param name="connectionString"></param>
		/// <param name="table"></param>
		/// <returns></returns>
		public List<TableColumnSchema> GetTableColumns(string connectionString, TableSchema table)
		{
			DataTable table2 = new SchemaHelper().GetSchemaFromReader(connectionString, table.Name);
			List<TableColumnSchema> schemaArray = new List<TableColumnSchema>(table2.Rows.Count);

			foreach (DataRow columnRow in table2.Rows)
			{
				string name = SafeGetString(columnRow, "ColumnName");
				string nativeType = SafeGetString(columnRow, "DataTypeName");
				DbType dataType = _DataTypes.First(p => p.StringType == SafeGetString(columnRow, "DataTypeName")).eDbType;
				int size = SafeGetInt(columnRow, "ColumnSize");
				byte precision = (byte)SafeGetInt(columnRow, "NumericPrecision");
				int scale = SafeGetInt(columnRow, "NumericScale");
				bool allowDBNull = SafeGetBool(columnRow, "AllowDBNull");

				List<ExtendedProperty> list = new List<ExtendedProperty>();

				

				TableColumnSchema tcs = new TableColumnSchema(table, name, dataType, nativeType, size, precision, scale, allowDBNull, list)
				{
					IsIdentity = SafeGetBool(columnRow, "IsIdentity"),
					IsKey = SafeGetBool(columnRow, "IsKey"),
					IsReadOnly = SafeGetBool(columnRow, "IsReadOnly"),
					IsAutoIncrement = SafeGetBool(columnRow, "IsAutoIncrement")
				};

				schemaArray.Add( tcs );
			}

			//DataTable table2 = new SchemaHelper().GetTableColumns(connectionString, table.Database.Name, table.Owner, table.Name, null );
			//List<TableColumnSchema> schemaArray = new List<TableColumnSchema>(table2.Rows.Count);
			
			//for (int i = 0; i < table2.Rows.Count; i++)
			//{
			//    string name = (string)table2.Rows[i]["COLUMN_NAME"];
			//    string nativeType = (string)table2.Rows[i]["DATA_TYPE"];
			//    DbType dataType = _DataTypes.First(p => p.StringType == ((string)table2.Rows[i]["DATA_TYPE"])).eDbType;
			//    int size = table2.Rows[i].IsNull("CHARACTER_MAXIMUM_LENGTH") ? 0 : ((int)table2.Rows[i]["CHARACTER_MAXIMUM_LENGTH"]);
			//    byte precision = table2.Rows[i].IsNull("NUMERIC_PRECISION") ? ((byte)0) : ((byte)table2.Rows[i]["NUMERIC_PRECISION"]);
			//    int scale = table2.Rows[i].IsNull("NUMERIC_SCALE") ? 0 : ((int)table2.Rows[i]["NUMERIC_SCALE"]);
			//    bool allowDBNull = (bool)(((string)table2.Rows[i]["IS_NULLABLE"]) == "YES" ? true : false);

			//    //bool isIdentity = ((int)table2.Rows[i][6]) == 1;
			//    //bool isComputed = ((int)table2.Rows[i][7]) == 1;
			//    //bool flag4 = ((int)table2.Rows[i][10]) == 1;
			//    //bool flag5 = table2.Rows[i].IsNull(11) || (((int)table2.Rows[i][11]) == 1);
			//    //int num4 = table2.Rows[i].IsNull(12) ? 0 : Convert.ToInt32(table2.Rows[i][12]);
			//    //int num5 = table2.Rows[i].IsNull(13) ? 0 : Convert.ToInt32(table2.Rows[i][13]);
			//    //string str4 = table2.Rows[i].IsNull(7) ? c.a("") : ((string)table2.Rows[i][7]);

			//    //Select 
			//        //COLUMNPROPERTY(OBJECT_ID('Table1'), COLUMN_NAME, 'IsIdentity') as IS_IDENTITY,
			//        //COLUMNPROPERTY(OBJECT_ID('Table1'), COLUMN_NAME, 'IsComputed') as IS_COMPUTED
			//        //from  INFORMATION_SCHEMA.COLUMNS
			//        //Where COLUMN_NAME = 'money'
			//    List<ExtendedProperty> list = new List<ExtendedProperty>();
			//    //list.Add(new ExtendedProperty("IsIdentity", isIdentity, DbType.Boolean));
			//    //list.Add(new ExtendedProperty("isComputed", isComputed, DbType.Boolean));
			//    //list.Add(new ExtendedProperty(c.a("\x0019\t\x0005\x0013)\x001957*/.?>"), flag4, DbType.Boolean));
			//    //list.Add(new ExtendedProperty(c.a("\x0019\t\x0005\x0013)\x001e?.?(7343).39"), flag5, DbType.Boolean));
			//    //list.Add(new ExtendedProperty(c.a("\x0019\t\x0005\x0013>?4.3.#\t??>"), num4, DbType.Int32));
			//    //list.Add(new ExtendedProperty(c.a("\x0019\t\x0005\x0013>?4.3.#\x001349(?7?4."), num5, DbType.Int32));
			//    //list.Add(new ExtendedProperty(c.a("\x0019\t\x0005\x001e?<;/6."), str4, DbType.String));

			//    schemaArray.Add( new TableColumnSchema(table, name, dataType, nativeType, size, precision, scale, allowDBNull, list) );
			//}
			return schemaArray;
		}

		/// <summary>
		/// Return all indexes for given table
		/// </summary>
		/// <param name="connectionString"></param>
		/// <param name="table"></param>
		/// <returns></returns>
		public List<IndexSchema> GetTableIndexes(string connectionString, TableSchema table)
		{
			DataTable tableIndex = new SchemaHelper().GetIndexes(connectionString, table.Database.Name, table.Owner, table.Name, null);
			List<IndexSchema> list = new List<IndexSchema>(tableIndex.Rows.Count);

			for (int i = 0; i < tableIndex.Rows.Count; i++)
			{
				string name = (string)tableIndex.Rows[i]["index_name"];

				DataTable tableCol = new SchemaHelper().GetIndexColumns(connectionString, table.Database.Name, table.Owner, table.Name, name, null);
				List<string> listRelatedCol = new List<string>();
				
				for (int j = 0; j < tableCol.Rows.Count; j++)
					listRelatedCol.Add((string)tableCol.Rows[j]["column_name"]);

				string commandText = string.Format(_Queries.GetQuery("IndexExtended"), name);
				DataTable tableSys = new SQLQueryHelper(connectionString).ExecuteData(commandText);

				bool isPrimaryKey = (bool)tableSys.Rows[0]["is_primary_key"];
				bool isUnique = (bool)tableSys.Rows[0]["is_unique"];
				bool isClustered = (bool)((string)tableSys.Rows[0]["type_desc"] == "CLUSTERED" ? true : false);

				list.Add(new IndexSchema(table, name, isPrimaryKey, isUnique, isClustered, listRelatedCol));
			}

			return list;
		}

		/// <summary>
		/// Return all keys for a given table
		/// </summary>
		/// <param name="connectionString"></param>
		/// <param name="table"></param>
		/// <returns></returns>
		public List<TableKeySchema> GetTableKeys(string connectionString, TableSchema table)
		{
			string commandText = string.Format(_Queries.GetQuery("TableKeys"), table.Owner, table.Name);

			DataTable table2 = new SQLQueryHelper(connectionString).ExecuteData(commandText);

			List<TableKeySchema> schemaArray = new List<TableKeySchema>();

			for (int i = 0; i < table2.Rows.Count; i++)
			{
				string keyName = (string)table2.Rows[i]["ForeignKeyName"];

				string foreignKeyTable = (string) table2.Rows[i]["TableName"];

				List<string> foreignKeyColumnNameList = new List<string>();
				foreignKeyColumnNameList.Add( (string) table2.Rows[i]["ColumnName"] );

				string primaryKeyTable = (string)table2.Rows[i]["ReferenceTableName"];
				List<string> primaryKeyTableColumnNameList = new List<string>();
				primaryKeyTableColumnNameList.Add((string)table2.Rows[i]["ReferenceColumnName"]);

			
				schemaArray.Add( new TableKeySchema(table.Database, keyName, 
				    foreignKeyColumnNameList.ToArray(), foreignKeyTable, primaryKeyTableColumnNameList.ToArray(), primaryKeyTable) );
			}
			return schemaArray;
		}

		/// <summary>
		/// Return all primary keys for a given table
		/// </summary>
		/// <param name="connectionString"></param>
		/// <param name="table"></param>
		/// <returns></returns>
		public PrimaryKeySchema GetTablePrimaryKey(string connectionString, TableSchema table)
		{
			string commandText = string.Format(_Queries.GetQuery("TablePrimaryKey"), table.Name, table.Database.Name, table.Owner);

			DataTable table2 = new SQLQueryHelper(connectionString).ExecuteData(commandText);

			if (table2.Rows.Count == 1)
			{
				string primary = (string)table2.Rows[0]["CONSTRAINT_NAME"];

				string commandText1 = string.Format(_Queries.GetQuery("TablePrimaryKeyColumns"), table.Database.Name, table.Name, primary);

				DataTable table3 = new SQLQueryHelper(connectionString).ExecuteData(commandText1);

				List<string> memberColumns = new List<string>(table3.Rows.Count);

				for (int i = 0; i < table3.Rows.Count; i++)
					memberColumns.Add((string)table3.Rows[i]["COLUMN_NAME"]);

				return new PrimaryKeySchema(table, primary, memberColumns);
			}
			else return null;
		}
		#endregion

		#region ----------------VIEWS----------------

		/// <summary>
		/// Return all views in a given databse
		/// </summary>
		/// <param name="connectionString"></param>
		/// <param name="database"></param>
		/// <returns></returns>
		public List<ViewSchema> GetViews(string connectionString, DatabaseSchema database)
		{
			string str = string.Format(_Queries.GetQuery("ViewSchema"), database.Name); 

			DataTable table = new SQLQueryHelper(connectionString).ExecuteData(str);

			List<ViewSchema> schemaArray = new List<ViewSchema>(table.Rows.Count);
			for (int i = 0; i < table.Rows.Count; i++)
			{
				schemaArray.Add (new ViewSchema(database, (string)table.Rows[i][0], (string)table.Rows[i][1], (DateTime)table.Rows[i][2]) );
			}
			return schemaArray;
		}

		/// <summary>
		/// Return all columns of a given view
		/// </summary>
		/// <param name="connectionString"></param>
		/// <param name="view"></param>
		/// <returns></returns>
		public List<ViewColumnSchema> GetViewColumns(string connectionString, ViewSchema view)
		{
			DataTable table = new SchemaHelper().GetViewColumns(connectionString, view.Database.Name, view.Owner, view.Name, null);
			List<ViewColumnSchema> schemaArray = new List<ViewColumnSchema>(table.Rows.Count);
		
			for (int i = 0; i < table.Rows.Count; i++)
			{
				schemaArray.Add
					(
					GetViewColumnData(connectionString, view, (string)table.Rows[i]["TABLE_NAME"], (string)table.Rows[i]["COLUMN_NAME"])
					);
			}
			return schemaArray;
		}

		/// <summary>
		/// Return the SQL script that compose the given view
		/// </summary>
		/// <param name="connectionString"></param>
		/// <param name="view"></param>
		/// <returns></returns>
		public string GetViewText(string connectionString, ViewSchema view)
		{
			StringBuilder builder = new StringBuilder();
			string str = string.Format(_Queries.GetQuery("ViewText"), view.Name, view.Database.Name, view.Owner);

			using (SQLQueryHelper query = new SQLQueryHelper(connectionString))
			{
				SqlDataReader reader = query.ExecuteReader(str);
				while (reader.Read())
				{
					builder.Append(reader.GetString(0));
				}
				reader.Close();
			}
			return builder.ToString();
		}

		/// <summary>
		/// Retreive information about a view column
		/// </summary>
		/// <param name="connectionString"></param>
		/// <param name="view"></param>
		/// <param name="tableName"></param>
		/// <param name="colName"></param>
		/// <returns></returns>
		internal ViewColumnSchema GetViewColumnData(string connectionString, ViewSchema view, string tableName, string colName)
		{
			DataTable table2 = new SchemaHelper().GetTableColumns(connectionString, view.Database.Name, view.Owner, tableName, colName);

			if (table2.Rows.Count == 1)
			{
				DataRow dr = table2.Rows[0];

				string nativeType = (string)dr["DATA_TYPE"];
				DbType dataType = _DataTypes.First(p => p.StringType == ((string)dr["DATA_TYPE"])).eDbType;
				int size = dr.IsNull("CHARACTER_MAXIMUM_LENGTH") ? 0 : ((int)dr["CHARACTER_MAXIMUM_LENGTH"]);
				byte precision = dr.IsNull("NUMERIC_PRECISION") ? ((byte)0) : ((byte)dr["NUMERIC_PRECISION"]);
				int scale = dr.IsNull("NUMERIC_SCALE") ? 0 : ((int)dr["NUMERIC_SCALE"]);
				bool allowDBNull = (bool)(((string)dr["IS_NULLABLE"]) == "YES" ? true : false);

				return new ViewColumnSchema(view, colName, dataType, nativeType, size, precision, scale, allowDBNull);
			}
			return null;
		}

		#endregion

		#region ----------------COMMANDS----------------

		/// <summary>
		/// Return the parameters of a given command (stored procedure...)
		/// </summary>
		/// <param name="connectionString"></param>
		/// <param name="command"></param>
		/// <returns></returns>
		public List<ParameterSchema> GetCommandParameters(string connectionString, CommandSchema command)
		{
			DataTable table = new SchemaHelper().GetProcedureParameters(connectionString, command.Database.Name, command.Owner, command.Name, null);
			List<ParameterSchema> schemaArray = new List<ParameterSchema>(table.Rows.Count);
			for (int i = 0; i < table.Rows.Count; i++)
			{
				string name = (string)table.Rows[i]["PARAMETER_NAME"];
				ParameterDirection direction = GetParamDirection((string)table.Rows[i]["PARAMETER_MODE"]);
				string nativeType = (string)table.Rows[i]["DATA_TYPE"];
				DbType dataType = _DataTypes.First(p => p.StringType == ((string)table.Rows[i]["DATA_TYPE"])).eDbType;
				int size = table.Rows[i].IsNull("CHARACTER_MAXIMUM_LENGTH") ? 0 : ((int)table.Rows[i]["CHARACTER_MAXIMUM_LENGTH"]);
				byte precision = table.Rows[i].IsNull("NUMERIC_PRECISION") ? ((byte)0) : ((byte)table.Rows[i]["NUMERIC_PRECISION"]);
				int scale = table.Rows[i].IsNull("NUMERIC_SCALE") ? 0 : ((int)table.Rows[i]["NUMERIC_SCALE"]);
				bool allowDBNull = false;

				//default value ?
				//list.Add(new ExtendedProperty(c.a("\x0019\t\x0005\x001e?<;/6."), str4, DbType.String));

				schemaArray.Add( new ParameterSchema(command, name, direction, dataType, nativeType, size, 
					Convert.ToByte(precision), scale, allowDBNull) );
			}
			return schemaArray;
		}

		public List<CommandResultSchema> GetCommandResultSchemas(string connectionString, CommandSchema command)
		{
			return null;
		}

		/// <summary>
		/// Return a list of all command in a given database
		/// </summary>
		/// <param name="connectionString"></param>
		/// <param name="database"></param>
		/// <returns></returns>
		public List<CommandSchema> GetCommands(string connectionString, DatabaseSchema database)
		{
			DataTable table = new SchemaHelper().GetProcedures(connectionString,database.Name, null, null, null);
			List<CommandSchema> schemaArray = new List<CommandSchema>(table.Rows.Count);
			for (int i = 0; i < table.Rows.Count; i++)
			{
				schemaArray.Add( new CommandSchema(database, (string)table.Rows[i]["ROUTINE_NAME"], (string)table.Rows[i]["ROUTINE_SCHEMA"], (DateTime)table.Rows[i]["CREATED"]) );
			}
			return schemaArray;
		}

		/// <summary>
		/// Retreive the sql code of a given command
		/// </summary>
		/// <param name="connectionString"></param>
		/// <param name="command"></param>
		/// <returns></returns>
		public string GetCommandText(string connectionString, CommandSchema command)
		{
			StringBuilder builder = new StringBuilder();
			string str = string.Format(_Queries.GetQuery("CommandText"), command.Database.Name, command.Name);

			using (SQLQueryHelper query = new SQLQueryHelper(connectionString))
			{
				SqlDataReader reader = query.ExecuteReader(str);
				while (reader.Read())
				{
					builder.Append(reader.GetString(0));
				}
				reader.Close();
			}
			return builder.ToString();
		}
		#endregion

		#region ----------------EXTENDED PROPERTIES----------------

		public List<ExtendedProperty> GetExtendedProperties(string connectionString, SchemaObjectBase schemaObject)
		{
			return null;
		}
		#endregion

		#region ----------------CONVERTERS----------------

		private ParameterDirection GetParamDirection(string dir)
		{
			switch (dir)
			{
				case "IN":
					return ParameterDirection.Input;
				case "OUT":
					return ParameterDirection.Output;
				case "INOUT":
					return ParameterDirection.InputOutput;
				case "RETURNVALUE":
					return ParameterDirection.ReturnValue;
			}

			return ParameterDirection.Input;
		}

		#endregion

		public DataTable GetDependencies(string connectionString, DatabaseSchema database, SchemaObjectBase reference, bool getFrom)
		{
			string str = string.Format(_Queries.GetQuery("Dependencies"), reference.Owner, reference.Name, getFrom ? 0 : 1);

			//object_id, object_type, relative_id, relative_type, object_name, object_schema, relative_type_name, relative_type_schema
			return new SQLQueryHelper(connectionString).ExecuteData(str);
		}

		#region ----------------INTERFACES----------------

		/// <summary>
		/// Return the IDbSourceDialog interface for displaying specific provider connection dialog
		/// </summary>
		/// <returns>IDbSourceDialog</returns>
		public IDbSourcePanel GetSourcePanelInterface()
		{
			return (IDbSourcePanel)new WPFNewSource();
		}

		/// <summary>
		/// Return the IDbSQLWriter interface for SQL script creation
		/// </summary>
		/// <returns></returns>
		public IDbScriptWriter GetSQLWriterInterface()
		{
			return (IDbScriptWriter)new SqlScriptWriter();
		}

		/// <summary>
		/// Return the IDbSQLVerifier interface for SQL quality controls
		/// </summary>
		/// <returns>IDbSQLVerifier</returns>
		public IDbVerifier GetSQLVerifierInterface(string connectionString, IDbVerifierOutputMode outputMode)
		{
			return (IDbVerifier)new SqlVerifier(connectionString, outputMode);
		}

		/// <summary>
		/// Return the query interface for excuting SQL queries
		/// </summary>
		/// <returns>IDbSQLQuery</returns>
		public IDbQuery GetSQLQueryInterface()
		{
			return (IDbQuery)new SqlQuery();
		}

		#endregion

		protected string SafeGetString(DataRow row, string columnName)
		{
			string result = string.Empty;

			if (row.Table.Columns.Contains(columnName) && !row.IsNull(columnName))
			{
				result = row[columnName].ToString();
			}

			return result;
		}

		protected int SafeGetInt(DataRow row, string columnName)
		{
			int result = 0;

			if (row.Table.Columns.Contains(columnName) && !row.IsNull(columnName))
			{
				result = Convert.ToInt32(row[columnName]);
			}

			return result;
		}

		protected bool SafeGetBool(DataRow row, string columnName)
		{
			if (row.Table.Columns.Contains(columnName) && !row.IsNull(columnName))
			{
				string value = row[columnName].ToString();
				switch (value.ToLower())
				{
					case "no":
					case "false":
						return false;

					case "yes":
					case "true":
						return true;
				}
			}

			return false;
		}
	}
}
