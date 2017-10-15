using System;
using System.Collections.Generic;
using System.Text;
using ReflectionStudio.Core.Database.Schema;

namespace ReflectionStudio.Core.Generator
{
	public class Context
	{
		public DatabaseSchema Database { get; set; }

		public ITabularObjectBase CurrentTable { get; set; }
	}
}
