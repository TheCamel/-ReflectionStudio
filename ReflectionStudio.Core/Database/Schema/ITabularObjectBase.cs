using System;
using System.Collections.Generic;

namespace ReflectionStudio.Core.Database.Schema
{
	public interface ITabularObjectBase
	{
		DatabaseSchema Database { get; }
		string Name { get; }

		List<ColumnBaseSchema> Columns { get; }
		DateTime DateCreated { get; }
		bool Equals(object obj);
		string FullName { get; }
		int GetHashCode();
		string Owner { get; }
		void Refresh();
		string ToString();
		string Text { get; }
	}
}
