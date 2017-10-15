using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ReflectionStudio.Core.Database.Schema
{
	public class SchemaUtility
	{
		// Methods
		public static Type GetSystemType(DbType type)
		{
			switch (type)
			{
				case DbType.AnsiString:
					return typeof(string);

				case DbType.Binary:
					return typeof(byte[]);

				case DbType.Byte:
					return typeof(byte);

				case DbType.Boolean:
					return typeof(bool);

				case DbType.Currency:
					return typeof(decimal);

				case DbType.Date:
					return typeof(DateTime);

				case DbType.DateTime:
					return typeof(DateTime);

				case DbType.Decimal:
					return typeof(decimal);

				case DbType.Double:
					return typeof(double);

				case DbType.Guid:
					return typeof(Guid);

				case DbType.Int16:
					return typeof(short);

				case DbType.Int32:
					return typeof(int);

				case DbType.Int64:
					return typeof(long);

				case DbType.Object:
					return typeof(object);

				case DbType.SByte:
					return typeof(sbyte);

				case DbType.Single:
					return typeof(float);

				case DbType.String:
					return typeof(string);

				case DbType.Time:
					return typeof(TimeSpan);

				case DbType.UInt16:
					return typeof(ushort);

				case DbType.UInt32:
					return typeof(uint);

				case DbType.UInt64:
					return typeof(ulong);

				case DbType.VarNumeric:
					return typeof(decimal);

				case DbType.AnsiStringFixedLength:
					return typeof(string);

				case DbType.StringFixedLength:
					return typeof(string);
			}
			return typeof(object);
		}

		public static Type GetSystemType(SqlDbType type)
		{
			switch (type)
			{
				case SqlDbType.BigInt:
					return typeof(long);

				case SqlDbType.Binary:
					return typeof(byte[]);

				case SqlDbType.Bit:
					return typeof(bool);

				case SqlDbType.Char:
					return typeof(string);

				case SqlDbType.DateTime:
					return typeof(DateTime);

				case SqlDbType.Decimal:
					return typeof(decimal);

				case SqlDbType.Float:
					return typeof(double);

				case SqlDbType.Image:
					return typeof(byte[]);

				case SqlDbType.Int:
					return typeof(int);

				case SqlDbType.Money:
					return typeof(decimal);

				case SqlDbType.NChar:
					return typeof(string);

				case SqlDbType.NText:
					return typeof(string);

				case SqlDbType.NVarChar:
					return typeof(string);

				case SqlDbType.Real:
					return typeof(float);

				case SqlDbType.UniqueIdentifier:
					return typeof(Guid);

				case SqlDbType.SmallDateTime:
					return typeof(DateTime);

				case SqlDbType.SmallInt:
					return typeof(short);

				case SqlDbType.SmallMoney:
					return typeof(decimal);

				case SqlDbType.Text:
					return typeof(string);

				case SqlDbType.Timestamp:
					return typeof(DateTime);

				case SqlDbType.TinyInt:
					return typeof(byte);

				case SqlDbType.VarBinary:
					return typeof(byte[]);

				case SqlDbType.VarChar:
					return typeof(string);

				case SqlDbType.Variant:
					return typeof(object);
			}
			return typeof(object);
		}
	}


}
