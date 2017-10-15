using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ReflectionStudio.Core.Database.Schema
{
	public class CommandResultSchema : SchemaObjectBase
	{
		// Methods
		public CommandResultSchema(CommandSchema command, string name, CommandResultColumnSchema[] columns)
		{
			base._Name = name;
			this._Command = command;
			this._Columns = new List<CommandResultColumnSchema>(columns);
		}

		public CommandResultSchema(CommandSchema command, string name, CommandResultColumnSchema[] columns, ExtendedProperty[] extendedProperties)
			: this(command, name, columns)
		{
			base._ExtendedProperties = extendedProperties.ToList<ExtendedProperty>();
		}

		public override void Refresh()
		{
			this.Command.Refresh();
		}

		// Properties
		private List<CommandResultColumnSchema> _Columns;
		[Browsable(false)]
		public List<CommandResultColumnSchema> Columns
		{
			get
			{
				return this._Columns;
			}
		}

		private CommandSchema _Command;
		[Browsable(false)]
		public CommandSchema Command
		{
			get
			{
				return this._Command;
			}
		}

		[Browsable(false)]
		public CommandResultColumnSchema this[int index]
		{
			get
			{
				return this.Columns[index];
			}
		}
	}
}
