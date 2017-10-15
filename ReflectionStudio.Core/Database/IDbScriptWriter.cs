using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ReflectionStudio.Core.Database.Schema;

namespace ReflectionStudio.Core.Database
{
	public interface IDbScriptWriter
	{
		string Create(SchemaObjectBase schemaObject);
		string Alter(SchemaObjectBase schemaObject);
		string Drop(SchemaObjectBase schemaObject);

		void Create(TextWriter writer, SchemaObjectBase schemaObject);
		void Alter(TextWriter writer, SchemaObjectBase schemaObject);
		void Drop(TextWriter writer, SchemaObjectBase schemaObject);

		string SelectCount(ITabularObjectBase tableOrView);
		string Select(ITabularObjectBase tableOrView);
		string Insert(ITabularObjectBase tableOrView);
		string Update(ITabularObjectBase tableOrView);
		string Delete(ITabularObjectBase tableOrView);

		void SelectCount(TextWriter writer, ITabularObjectBase tableOrView);
		void Select(TextWriter writer, ITabularObjectBase tableOrView);
		void Insert(TextWriter writer, ITabularObjectBase tableOrView);
		void Update(TextWriter writer, ITabularObjectBase tableOrView);
		void Delete(TextWriter writer, ITabularObjectBase tableOrView);
	}
}
