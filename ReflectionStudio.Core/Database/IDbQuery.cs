using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ReflectionStudio.Core.Database
{
	public interface IDbQuery
	{
		DataTable ExecuteData(string connectionString, string commandText);

		IDataReader ExecuteReader(string connectionString, string commandText);
	}
}
