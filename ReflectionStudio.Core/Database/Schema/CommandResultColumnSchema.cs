using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;

namespace ReflectionStudio.Core.Database.Schema
{
	public class CommandResultColumnSchema : DataObjectBase
	{
		// Methods
		public CommandResultColumnSchema(CommandSchema command, string name, DbType dataType, string nativeType, int size, byte precision, int scale, bool allowDBNull)
		{
			base._Database = command.Database;
			base._Name = name;
			base._DataType = dataType;
			base._NativeType = nativeType;
			base._Size = size;
			base._Precision = precision;
			base._Scale = scale;
			base._AllowDBNull = allowDBNull;

			this._Command = command;
		}

		public CommandResultColumnSchema(CommandSchema command, string name, DbType dataType, string nativeType, int size, byte precision, int scale, bool allowDBNull, ExtendedProperty[] extendedProperties)
			: this(command, name, dataType, nativeType, size, precision, scale, allowDBNull)
		{
			base._ExtendedProperties = extendedProperties.ToList<ExtendedProperty>();
		}

		public override void Refresh()
		{
			this.Command.Refresh();
		}

		// Properties
		private CommandSchema _Command;
		[Browsable(false)]
		public CommandSchema Command
		{
			get
			{
				return this._Command;
			}
		}
	}


}
