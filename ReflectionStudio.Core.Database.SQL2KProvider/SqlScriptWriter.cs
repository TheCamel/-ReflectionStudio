using System;
using System.IO;
using ReflectionStudio.Core.Database.Schema;

namespace ReflectionStudio.Core.Database.SQL2KProvider
{
	class SqlScriptWriter : IDbScriptWriter
	{
		#region ---------------------CREATE---------------------

		public string Create(SchemaObjectBase schemaObject)
		{
			StringWriter stringWriter = new StringWriter();

			Create(stringWriter, schemaObject);

			return stringWriter.ToString();
		}

		public void Create(TextWriter writer, SchemaObjectBase schemaObject)
		{
			if (schemaObject is ViewSchema)
				writer.Write(schemaObject.Database.Provider.GetViewText(schemaObject.Database.ConnectionString, (ViewSchema)schemaObject));

			if( schemaObject is CommandSchema )
				writer.Write(schemaObject.Database.Provider.GetCommandText(schemaObject.Database.ConnectionString, (CommandSchema)schemaObject));

			if (schemaObject is TableSchema)
			{
				// TODO
			}
		}

		#endregion

		#region ---------------------ALTER---------------------

		public string Alter(SchemaObjectBase schemaObject)
		{
			StringWriter stringWriter = new StringWriter();

			Alter(stringWriter, schemaObject);

			return stringWriter.ToString();
		}

		public void Alter(TextWriter writer, SchemaObjectBase schemaObject)
		{
			if (schemaObject is ViewSchema)
				writer.Write(
					schemaObject.Database.Provider.GetViewText(schemaObject.Database.ConnectionString, (ViewSchema)schemaObject).Replace("CREATE ", "ALTER ")
					);

			if (schemaObject is CommandSchema)
				writer.Write(
					schemaObject.Database.Provider.GetCommandText(schemaObject.Database.ConnectionString, (CommandSchema)schemaObject).Replace("CREATE ", "ALTER ")
					);

			if (schemaObject is TableSchema)
			{
				// TODO
			}
		}

		#endregion

		#region ---------------------DROP---------------------

		public string Drop(SchemaObjectBase schemaObject)
		{
			StringWriter stringWriter = new StringWriter();

			Drop(stringWriter, schemaObject);

			return stringWriter.ToString();
		}

		public void Drop(TextWriter writer, SchemaObjectBase schemaObject)
		{
			writer.WriteLine( string.Format( "/****** Objet :  [{0}].[{1}]    Date de génération du script : {2} ******/",
													schemaObject.Owner, schemaObject.Name, DateTime.Now.ToLongDateString()) );

			writer.Write( "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'");
			writer.WriteLine(string.Format("[{0}].[{1}]')", schemaObject.Owner, schemaObject.Name));

			if (schemaObject is CommandSchema)
				writer.WriteLine("AND type in (N'P', N'PC'))");
			if (schemaObject is TableSchema)
				writer.WriteLine("AND type in (N'U'))");
			if (schemaObject is ViewSchema)
				writer.WriteLine("AND type in (N'V'))");

			writer.Write("DROP ");
			if( schemaObject is CommandSchema )
				writer.Write("PROCEDURE");
			if (schemaObject is TableSchema)
				writer.Write("TABLE");
			if (schemaObject is ViewSchema)
				writer.Write("VIEW");

			writer.WriteLine(string.Format("[{0}].[{1}])", schemaObject.Owner, schemaObject.Name));
		}

		#endregion

		#region ---------------------SELECT---------------------

		public string Select(ITabularObjectBase tableOrView)
		{
			StringWriter stringWriter = new StringWriter();

			Select(stringWriter, tableOrView);

			return stringWriter.ToString();
		}

		public void Select(TextWriter writer, ITabularObjectBase tableOrView)
		{
			writer.WriteLine("SELECT");

			WriteTableColumns( writer, tableOrView );

			writer.WriteLine("FROM ");
			WriteTableName(writer, tableOrView);
		}

		#endregion

		#region ---------------------INSERT---------------------

		public string Insert(ITabularObjectBase tableOrView)
		{
			StringWriter stringWriter = new StringWriter();

			Insert(stringWriter, tableOrView);

			return stringWriter.ToString();
		}

		public void Insert(TextWriter writer, ITabularObjectBase tableOrView)
		{
			writer.Write("INSERT INTO ");
			WriteTableName(writer, tableOrView);

			writer.Write("(");

			// get all columns that are "writable" including PKs that are not auto generated
			WriteTableColumns(writer, tableOrView);

			writer.WriteLine(")");
			writer.WriteLine("VALUES");
			writer.WriteLine("(");

			WriteTableColumnsForInsert(writer, tableOrView);
			
			writer.WriteLine(")");
		}
		#endregion

		#region ---------------------UPDATE---------------------

		public string Update(ITabularObjectBase tableOrView)
		{
			StringWriter stringWriter = new StringWriter();

			Update(stringWriter, tableOrView);

			return stringWriter.ToString();
		}

		public void Update(TextWriter writer, ITabularObjectBase tableOrView)
		{
			writer.WriteLine("UPDATE ");
			
			WriteTableName(writer, tableOrView);
			
			writer.WriteLine("SET ");
			
			WriteTableColumnsForUpdate(writer, tableOrView);
			
			writer.WriteLine("WHERE <Conditions>");
		}

		#endregion

		#region ---------------------DELETE---------------------

		public string Delete(ITabularObjectBase tableOrView)
		{
			StringWriter stringWriter = new StringWriter();

			Delete(stringWriter, tableOrView);

			return stringWriter.ToString();
		}

		public void Delete(TextWriter writer, ITabularObjectBase tableOrView)
		{
			writer.WriteLine("DELETE FROM ");
			
			WriteTableName( writer, tableOrView );
			
			writer.WriteLine("WHERE <Conditions>");
		}

		#endregion

		#region ---------------------SELECT COUNT---------------------

		public string SelectCount(ITabularObjectBase tableOrView)
		{
			StringWriter stringWriter = new StringWriter();

			SelectCount(stringWriter, tableOrView);

			return stringWriter.ToString();
		}

		public void SelectCount(TextWriter writer, ITabularObjectBase tableOrView)
		{
			writer.WriteLine("SELECT COUNT(*) FROM ");
			
			WriteTableName(writer, tableOrView);
		}
		#endregion

		#region ---------------------INTERNALS---------------------

		internal void WriteTableColumnsForInsert(TextWriter writer, ITabularObjectBase table)
		{
			int maxCol = table.Columns.Count - 1;

			for (int i = 0; i < table.Columns.Count; i++)
			{
				DataObjectBase cs = table.Columns[i];
				writer.WriteLine("\t" + string.Concat("<", cs.Name, cs.DataType , ">"));

				if (i < maxCol)
					writer.Write(",");
			}
		}

		internal void WriteTableColumnsForUpdate(TextWriter writer, ITabularObjectBase table)
		{
			int maxCol = table.Columns.Count - 1;

			for (int i = 0; i < table.Columns.Count; i++)
			{
				DataObjectBase cs = table.Columns[i];
				writer.WriteLine("\t" + string.Concat("[", cs.Name, "] = ", "<", cs.Name, cs.DataType, ">"));
				
				if (i < maxCol)
					writer.Write(",");
			}
		}

		internal void WriteTableColumns(TextWriter writer, ITabularObjectBase table)
		{
			int maxCol = table.Columns.Count - 1;

			for (int i = 0; i < table.Columns.Count; i++)
			{
				writer.WriteLine("\t" + string.Concat("[", table.Columns[i].Name, "]"));

				if (i < maxCol)
					writer.Write(",");
			}
		}

		internal void WriteTableName(TextWriter writer, ITabularObjectBase table)
		{
			writer.WriteLine("\t[{0}].[{1}].[{2}]", table.Database.Name, table.Owner, table.Name);
		}
		#endregion
	}
}
