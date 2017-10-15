using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using ReflectionStudio.Core.Events;
using ReflectionStudio.Core.Database.SQL2KProvider.Helper;

namespace ReflectionStudio.Core.Database.SQL2KProvider
{
	internal class SqlVerifier : IDbVerifier
	{
		public string Connection { get; set; }
		public IDbVerifierOutputMode OutputMode { get; set; }

		public SqlVerifier(string connectionString, IDbVerifierOutputMode outputMode)
		{
			Connection = connectionString;
			OutputMode = outputMode;
		}

		public object Verify()
		{
			Tracer.Verbose("SqlVerifier:Verify", "START");
			try
			{
				List<DataTable> result = VerifyDatabase();

				if (result != null)
				{
					if (OutputMode == IDbVerifierOutputMode.eTabularText)
						return ProcessForTextOutput(result);
					else
						return ProcessForXpsOutput(result);
				}
				else
					return null;
			}
			catch (Exception all)
			{
				Tracer.Error("SqlVerifier.Verify", all);
				return null;
			}
			Tracer.Verbose("SqlVerifier:Verify", "END");
		}

		private object ProcessForTextOutput(List<DataTable> data)
		{
			Tracer.Verbose("SqlVerifier:ProcessForTextOutput", "START");
			try
			{
				return null;
			}
			catch (Exception all)
			{
				Tracer.Error("SqlVerifier.ProcessForTextOutput", all);
				return null;
			}
			finally
			{
				Tracer.Verbose("SqlVerifier:ProcessForTextOutput", "END");
			}
		}

		private object ProcessForXpsOutput(List<DataTable> data)
		{
			Tracer.Verbose("SqlVerifier:ProcessForXpsOutput", "START");
			try
			{
				return new ReportGenerator().Generate("ReflectionStudio.Core.Database.SQL2KProvider.Resources.DBQualityReport.xaml", data);
			}
			catch (Exception all)
			{
				Tracer.Error("SqlVerifier.ProcessForXpsOutput", all);
				return null;
			}
			finally
			{
				Tracer.Verbose("SqlVerifier:ProcessForXpsOutput", "END");
			}
		}

		private List<DataTable> VerifyDatabase()
		{
			List<DataTable> result = new List<DataTable>();

			//obtain all tables, view, stored proc names
			RetreiveParameters(result);

			//obtain all tables, view, stored proc names
			RetreiveAllObjects(result);

			//check against maming conventions

			//performance check aginst proc code and indexes

			//for all proc, check if code contains anti-patterns
			result.Add(RetreiveProcedures());

			return result;
		}

		private void RetreiveParameters(List<DataTable> result)
		{
			DataTable table = new DataTable("PARAM");
			table.Columns.Add("KEY", typeof(string));
			table.Columns.Add("VALUE", typeof(string));

			DataRow dr = table.NewRow();

			table.Rows.Add(new object[] { "SQLSERVER", "2008" });
			table.Rows.Add(new object[] { "DATABASE_NAME", "TEST" });
			table.Rows.Add(new object[] { "ANALYSIS_TIME", DateTime.Now.ToString() });

			table.Rows.Add(new object[] { "COMPANY", "LOGICA" });
			table.Rows.Add(new object[] { "AUTHOR", "G.WASER" });
			table.Rows.Add(new object[] { "VERSION", "1.0" });

			result.Add(table);
		}

		private void RetreiveAllObjects(List<DataTable> result)
		{
			string str = @"SELECT
      o.object_id as ID,
      convert(sysname,o.name) as NAME,
      P.[rows] as ROW_COUNT
FROM
      sys.all_objects o, sys.tables T,sys.partitions P
      
WHERE
		T.object_id = P.object_id
and		o.object_id = T.object_id
and     o.type ='U'
ORDER BY 2
";

			DataTable table = new DataTable("TABLES");
			table.Columns.Add("ID", typeof(int));
			table.Columns.Add("NAME", typeof(string));
			table.Columns.Add("ROW_COUNT", typeof(long));
			table.Columns.Add("NAMING", typeof(bool));

			table.Columns.Add("IX", typeof(int)); //counting
			table.Columns.Add("PK", typeof(int));
			table.Columns.Add("FK", typeof(int));

			table.Columns.Add("IX_LIST", typeof(string)); //list of object in error
			table.Columns.Add("PK_LIST", typeof(string));
			table.Columns.Add("FK_LIST", typeof(string));

			int id;
			string name;

			using (SQLQueryHelper sql = new SQLQueryHelper(Connection))
			{
				IDataReader reader = sql.ExecuteReader(str);
				while (reader.Read())
				{
					DataRow dr = table.NewRow();

					id = (int)reader.GetValue(0);
					dr["ID"] = id;

					name = (string)reader.GetValue(1);
					dr["NAME"] = name;

					dr["ROW_COUNT"] = (long)reader.GetValue(2);

					Match m2 = _TableNameMatch.Match(name);
					dr["NAMING"] = m2.Success;

					table.Rows.Add(dr);
				}
				reader.Close();
			}

			result.Add(table);
		}

		private List<Regex> _PerfPatern = new List<Regex>() { new Regex(@"\b(count)\b"),
			new Regex(@"\b(cursor)\b"),
			new Regex(@"\b(not exist)\b"),
			new Regex(@"\b(distinct)\b"),
			new Regex(@"\b(not in)\b"),
			new Regex(@"\b(case)\b"),
			new Regex(@"\b(goto)\b"),
			new Regex(@"\b(>=)\b"),
			new Regex(@"\b(<=)\b"),
			new Regex(@"\b(top)\b", RegexOptions.Multiline|RegexOptions.IgnoreCase),
			new Regex(@"\b(CREATE TABLE #)\b"),
			new Regex(@"\b(in select)\b")
		};
		private Regex _ProcNameMatch = new Regex("(?'Prefix'usp_)");
		private Regex _TableNameMatch = new Regex("(?'Prefix'T_)");

		private DataTable RetreiveProcedures()
		{
			string str = @"SELECT
      P.object_id,
     convert(sysname,P.name) as NAME,
      c.Text
FROM
      sys.procedures P, sys.syscomments C 
where P.object_id = C.id
";

			DataTable table = new DataTable("Procedures");
			table.Columns.Add("ID", typeof(int));
			table.Columns.Add("NAME", typeof(string));
			table.Columns.Add("LENGTH", typeof(string));
			table.Columns.Add("NAMING", typeof(bool));
			table.Columns.Add("CURSOR", typeof(int));
			table.Columns.Add("NOT_IN", typeof(int));
			table.Columns.Add("NOT_EXIST", typeof(int));
			table.Columns.Add("CASE_WHEN", typeof(int));
			table.Columns.Add("DISTINCT", typeof(int));
			table.Columns.Add("GOTO", typeof(int));
			table.Columns.Add("COUNT", typeof(int));
			table.Columns.Add("SUP_EGAL", typeof(int));
			table.Columns.Add("INF_EGAL", typeof(int));
			table.Columns.Add("TEMP_TABLE", typeof(int));
			table.Columns.Add("TOP", typeof(int));
			table.Columns.Add("IN_SELECT", typeof(int));
			table.Columns.Add("SELECT_ALL", typeof(string));

			int id;
			string name;
			string content;
			using (SQLQueryHelper sql = new SQLQueryHelper(Connection))
			{
				IDataReader reader = sql.ExecuteReader(str);
				while (reader.Read())
				{
					DataRow dr = table.NewRow();
					id = (int)reader.GetValue(0);
					dr["ID"] = id;

					name = (string)reader.GetValue(1);
					dr["NAME"] = name;

					content = (string)reader.GetValue(2);
					dr["LENGTH"] = content.Length;

					dr["COUNT"] = _PerfPatern[0].Match(content).Groups.Count;
					dr["CURSOR"] = _PerfPatern[1].Matches(content).Count;
					dr["NOT_EXIST"] = _PerfPatern[2].Matches(content).Count;
					dr["DISTINCT"] = _PerfPatern[3].Matches(content).Count;
					dr["NOT_IN"] = _PerfPatern[4].Matches(content).Count;
					dr["CASE_WHEN"] = _PerfPatern[5].Matches(content).Count;
					dr["GOTO"] = _PerfPatern[6].Matches(content).Count;
					dr["SUP_EGAL"] = _PerfPatern[7].Matches(content).Count;
					dr["INF_EGAL"] = _PerfPatern[8].Matches(content).Count;
					dr["TEMP_TABLE"] = _PerfPatern[10].Matches(content).Count;
					dr["TOP"] = _PerfPatern[9].Match(content).Groups.Count;
					dr["IN_SELECT"] = _PerfPatern[11].Matches(content).Count;
					
					Match m2 = _ProcNameMatch.Match(name);
					dr["NAMING"] = m2.Success;

					dr["SELECT_ALL"] = RetreiveSelectAll(id);
					table.Rows.Add(dr);
				}
				reader.Close();
			}
			return table;
		}

		private string RetreiveSelectAll(int procId)
		{
			string strSelectAll = string.Format( @"SELECT distinct
      T.name as Ref
FROM
      sys.sql_dependencies D        
LEFT JOIN sys.objects T 
      ON D.referenced_major_id = T.object_id        
LEFT JOIN sys.objects P 
      ON D.object_id = P.object_id
LEFT JOIN sys.columns C
      ON D.referenced_major_id = C.object_id 
      AND D.referenced_minor_id = C.column_id    
WHERE
      1=1        
      and D.is_select_all = 1
      AND    P.type = 'P'
      and P.object_id = '{0}'
", procId);

			string result = string.Empty;

			using (SQLQueryHelper sql = new SQLQueryHelper(Connection))
			{
				IDataReader reader = sql.ExecuteReader(strSelectAll);
				while (reader.Read())
				{
					result += (string)reader.GetValue(0) + ";";
				}
				reader.Close();
			}
			return result;
		}
	}
}
