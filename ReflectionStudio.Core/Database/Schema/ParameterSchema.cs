using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;

namespace ReflectionStudio.Core.Database.Schema
{
	public class ParameterSchema : DataObjectBase
	{
		// Methods
		public ParameterSchema(CommandSchema command, string name, ParameterDirection direction, DbType dataType, string nativeType, int size, byte precision, int scale, bool allowDBNull)
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
			this._Direction = direction;
		}

		public ParameterSchema(CommandSchema command, string name, ParameterDirection direction, DbType dataType, string nativeType, int size, byte precision, int scale, bool allowDBNull, ExtendedProperty[] extendedProperties)
			: this(command, name, direction, dataType, nativeType, size, precision, scale, allowDBNull)
		{
			base._ExtendedProperties = new List<ExtendedProperty>(extendedProperties);
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

		private ParameterDirection _Direction;
		public ParameterDirection Direction
		{
			get
			{
				return this._Direction;
			}
		}

		public override bool Equals(object obj)
		{
			ParameterSchema schema = obj as ParameterSchema;
			return (((schema != null) && schema.Command.Equals(this.Command)) && (schema.Name == this.Name));
		}

		public override int GetHashCode()
		{
			return (this.Command.GetHashCode() ^ this.Name.GetHashCode());
		}

		public override void Refresh()
		{
			this.Command.Refresh();
		}
	}
}
