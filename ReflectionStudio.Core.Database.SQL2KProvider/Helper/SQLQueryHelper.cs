using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace ReflectionStudio.Core.Database.SQL2KProvider
{
	internal class SQLQueryHelper : IDisposable
	{
		private SqlConnection Connection { get; set; }
		private SqlTransaction Transaction { get; set; }

		public SQLQueryHelper(string connectionString)
		{
			Connection = new SqlConnection(connectionString);
			Connection.Open();
		}

		public DataTable ExecuteData(string commandText)
		{
			SqlCommand command = new SqlCommand(commandText, Connection);
			SqlDataAdapter adapter = new SqlDataAdapter();
			DataSet set = new DataSet();
			command.CommandType = CommandType.Text;
			adapter.SelectCommand = command;
			adapter.Fill(set);

			adapter.Dispose();
			command.Dispose();

			return set.Tables[0];
		}

		public SqlDataReader ExecuteReader(string commandText)
		{
			SqlCommand command = new SqlCommand(commandText, Connection);
			command.CommandType = CommandType.Text;

			SqlDataReader reader = command.ExecuteReader();
			command.Dispose();

			return reader;
		}

		#region ---------------------IDisposable---------------------

		public void Dispose()
		{
			if (Connection != null)
			{
				Connection.Close();
				Connection.Dispose();
			}	
			//_Transaction
		}

		#endregion
	}
}
