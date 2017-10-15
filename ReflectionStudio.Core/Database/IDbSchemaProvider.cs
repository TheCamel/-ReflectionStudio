using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ReflectionStudio.Core.Database.Schema;

namespace ReflectionStudio.Core.Database
{
	public interface IDbSchemaProvider
	{
		#region ----------------PROPERTIES----------------
		/// <summary>
		/// Provider description
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Provider name
		/// </summary>
		string Name { get; }

		#endregion

		#region ----------------METHODS----------------
		/// <summary>
		/// Return the database name
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns>Return the database name as string</returns>
		string GetDatabaseName(string connectionString);

		// Commands
		List<CommandSchema> GetCommands(string connectionString, DatabaseSchema database);
		List<ParameterSchema> GetCommandParameters(string connectionString, CommandSchema command);
		string GetCommandText(string connectionString, CommandSchema command);
		List<CommandResultSchema> GetCommandResultSchemas(string connectionString, CommandSchema command);

		// extended properties
		List<ExtendedProperty> GetExtendedProperties(string connectionString, SchemaObjectBase schemaObject);

		// Tables
		List<TableColumnSchema> GetTableColumns(string connectionString, TableSchema table);
		List<IndexSchema> GetTableIndexes(string connectionString, TableSchema table);
		List<TableKeySchema> GetTableKeys(string connectionString, TableSchema table);
		PrimaryKeySchema GetTablePrimaryKey(string connectionString, TableSchema table);
		List<TableSchema> GetTables(string connectionString, DatabaseSchema database);

		// Views
		List<ViewColumnSchema> GetViewColumns(string connectionString, ViewSchema view);
		List<ViewSchema> GetViews(string connectionString, DatabaseSchema database);
		string GetViewText(string connectionString, ViewSchema view);

		// Aditional interfaces
		DataTable GetDependencies(string connectionString, DatabaseSchema database, SchemaObjectBase reference, bool getFrom);

		/// <summary>
		/// Return the IDbSourceDialog interface for displaying specific provider connection dialog
		/// </summary>
		/// <returns>IDbSourceDialog</returns>
		IDbSourcePanel GetSourcePanelInterface();

		/// <summary>
		/// Return the IDbSQLWriter interface for SQL script creation
		/// </summary>
		/// <returns></returns>
		IDbScriptWriter GetSQLWriterInterface();

		/// <summary>
		/// Return the IDbSQLVerifier interface for SQL quality controls
		/// </summary>
		/// <returns>IDbSQLVerifier</returns>
		IDbVerifier GetSQLVerifierInterface(string connectionString, IDbVerifierOutputMode outputMode);

		/// <summary>
		/// Return the IDbSQLQuery interface for excuting SQL queries
		/// </summary>
		/// <returns>IDbSQLQuery</returns>
		IDbQuery GetSQLQueryInterface();

		#endregion
	}
}
