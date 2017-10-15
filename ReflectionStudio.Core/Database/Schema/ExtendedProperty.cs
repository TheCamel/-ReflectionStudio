using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ReflectionStudio.Core.Database.Schema
{
	public class ExtendedProperty
	{
		public ExtendedProperty(string name, object value, DbType dataType)
		{
			_Name = name;
			_Value = value;
			_DataType = dataType;
		}

		private DbType _DataType;
		public DbType DataType
		{
			get
			{
				return _DataType;
			}
		}

		private string _Name;
		public string Name
		{
			get
			{
				return _Name;
			}
		}

		private object _Value;
		public object Value
		{
			get
			{
				return _Value;
			}
		}
	}


}
