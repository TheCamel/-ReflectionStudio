using System;
using System.Data;

namespace ReflectionStudio.Core.Database.SQL2KProvider
{
	public class DataTypeHolder
	{
		public string StringType { get; set; }

		public DbType eDbType { get; set; }

		public Type NetType { get; set; }

		public SqlDbType eSqlDbType { get; set; }

		public DataTypeHolder(string string_typ, Type net_typ, DbType db_typ, SqlDbType sql_typ)
		{
			StringType = string_typ;
			NetType = net_typ;
			eDbType = db_typ;
			eSqlDbType = sql_typ;
		}
	}
}
