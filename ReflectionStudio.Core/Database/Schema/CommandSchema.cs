using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Collections.ObjectModel;

namespace ReflectionStudio.Core.Database.Schema
{
	//[TypeConverter(typeof(CommandSchemaTypeConverter)), Editor(typeof(CommandSchemaPicker), typeof(UITypeEditor))]
	public class CommandSchema : SchemaObjectBase
	{
		// Fields
		private bool _flag;

		// Methods
		public CommandSchema(DatabaseSchema database, string name, string owner, DateTime dateCreated)
		{
			base._Database = database;
			base._Name = name;

			_CommandText = string.Empty;
			_flag = false;
			_Owner = owner;
			_DateCreated = dateCreated;

			_CanScript = true;
		}

		public CommandSchema(DatabaseSchema database, string name, string owner, DateTime dateCreated, ExtendedProperty[] extendedProperties)
			: this(database, name, owner, dateCreated)
		{
			base._ExtendedProperties = new List<ExtendedProperty>(extendedProperties);
		}

		public override bool Equals(object obj)
		{
			CommandSchema schema = obj as CommandSchema;
			return (((schema != null) && schema.Database.Equals(this.Database)) && ((schema.Owner == this.Owner) && (schema.Name == this.Name)));
		}

		public override int GetHashCode()
		{
			return ((_Database.GetHashCode() ^ _Owner.GetHashCode()) ^ _Name.GetHashCode());
		}

		public override void Refresh()
		{
			_Parameters = null;
			_CommandText = string.Empty;
			_flag = false;
			_CommandResults = null;
			_AllInputParameters = _AllOutputParameters = _InputOutputParameters = null;
			_InputParameters = _NonReturnValueParameters = _OutputParameters = null;
		}

		public override string ToString()
		{
			return FullName;
		}

		// Properties
		private List<ParameterSchema> _AllInputParameters;
		[Browsable(false)]
		public ReadOnlyCollection<ParameterSchema> AllInputParameters
		{
			get
			{
				if (_AllInputParameters == null)
					_AllInputParameters = _Parameters.Where(p => p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input).ToList<ParameterSchema>();

				return _AllInputParameters.AsReadOnly();
			}
		}

		private List<ParameterSchema> _AllOutputParameters;
		[Browsable(false)]
		public ReadOnlyCollection<ParameterSchema> AllOutputParameters
		{
			get
			{
				if (_AllOutputParameters == null)
					_AllOutputParameters = _Parameters.Where(p => p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output).ToList<ParameterSchema>();
				
				return _AllOutputParameters.AsReadOnly();
			}
		}

		private List<CommandResultSchema> _CommandResults;
		[Browsable(false)]
		public ReadOnlyCollection<CommandResultSchema> CommandResults
		{
			get
			{
				if (_CommandResults == null)
				{
					_Database.Check();
					_CommandResults = _Database.Provider.GetCommandResultSchemas(_Database.ConnectionString, this);
				}
				return _CommandResults.AsReadOnly();
			}
		}

		private string _CommandText;
		[Browsable(false)]
		public string CommandText
		{
			get
			{
				if (_CommandText == string.Empty)
				{
					_Database.Check();
					_CommandText = _Database.Provider.GetCommandText(_Database.ConnectionString, this);
				}
				return _CommandText;
			}
		}

		private DateTime _DateCreated;
		public DateTime DateCreated
		{
			get
			{
				return _DateCreated;
			}
		}

		public string FullName
		{
			get
			{
				return _Owner + "." + _Name;
			}
		}

		private List<ParameterSchema> _InputOutputParameters;
		[Browsable(false)]
		public ReadOnlyCollection<ParameterSchema> InputOutputParameters
		{
			get
			{
				if (_InputOutputParameters == null)
				{
					_InputOutputParameters = _Parameters.Where(p => p.Direction == ParameterDirection.InputOutput).ToList<ParameterSchema>();
				}
				return _InputOutputParameters.AsReadOnly();
			}
		}

		private List<ParameterSchema> _InputParameters;
		[Browsable(false)]
		public ReadOnlyCollection<ParameterSchema> InputParameters
		{
			get
			{
				if (_InputParameters == null)
					_InputParameters = _Parameters.Where(p => p.Direction == ParameterDirection.Input).ToList<ParameterSchema>();
				return _InputParameters.AsReadOnly();
			}
		}

		private List<ParameterSchema> _NonReturnValueParameters;
		[Browsable(false)]
		public ReadOnlyCollection<ParameterSchema> NonReturnValueParameters
		{
			get
			{
				if (_NonReturnValueParameters == null)
				{
					_NonReturnValueParameters = _Parameters.Where(p => p.Direction != ParameterDirection.ReturnValue).ToList<ParameterSchema>();
				}
				return _NonReturnValueParameters.AsReadOnly();
			}
		}

		private List<ParameterSchema> _OutputParameters;
		[Browsable(false)]
		public ReadOnlyCollection<ParameterSchema> OutputParameters
		{
			get
			{
				if (_OutputParameters == null)
					_OutputParameters = _Parameters.Where(p => p.Direction == ParameterDirection.Output).ToList<ParameterSchema>();

				return _OutputParameters.AsReadOnly();
			}
		}

		private List<ParameterSchema> _Parameters;
		[Browsable(false)]
		public ReadOnlyCollection<ParameterSchema> Parameters
		{
			get
			{
				if (_Parameters == null)
				{
					_Database.Check();
					_Parameters = _Database.Provider.GetCommandParameters(_Database.ConnectionString, this);
				}
				return _Parameters.AsReadOnly();
			}
		}

		private ParameterSchema _ReturnValueParameter;
		[Browsable(false)]
		public ParameterSchema ReturnValueParameter
		{
			get
			{
				if (_flag)
				{
					_ReturnValueParameter = _Parameters.First(p => p.Direction == ParameterDirection.ReturnValue);
					_flag = true;
				}
				return _ReturnValueParameter;
			}
		}
	}


}
