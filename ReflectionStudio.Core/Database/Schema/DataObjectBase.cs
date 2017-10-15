using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ReflectionStudio.Core.Database.Schema
{
	public abstract class DataObjectBase : SchemaObjectBase
	{
		protected bool _AllowDBNull;
		public bool AllowDBNull { get { return _AllowDBNull; } }

		protected DbType _DataType;
		public DbType DataType { get { return _DataType; } }

		protected string _NativeType;
		public string NativeType { get { return _NativeType; } }

		protected byte _Precision;
		public byte Precision { get { return _Precision; } }

		protected int _Scale;
		public int Scale { get { return _Scale; } }

		protected int _Size;
		public int Size { get { return _Size; } }

		public Type SystemType
		{
			get
			{
				return SchemaUtility.GetSystemType(_DataType);
			}
		}
	}
}
