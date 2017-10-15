using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using ReflectionStudio.Core.Events;
using ReflectionStudio.Core.Helpers;

namespace ReflectionStudio.Components.UserControls
{
	/// <summary>
	/// Background worker for excuting sql queries and retreive data in assynchronous mode without blocking the UI
	/// </summary>
	internal class QueryWorker : WorkerBase
	{
		#region ----------------------INTERNALS----------------------

		/// <summary>
		/// The current query context. See also <seealso cref="QueryContext"/>
		/// </summary>
		public QueryContext Context
		{
			get;
			set;
		}

		#endregion

		#region ----------------------CONSTRUCTORS----------------------

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="worker"></param>
		/// <param name="e"></param>
		public QueryWorker(BackgroundWorker worker, DoWorkEventArgs e)
			: base(worker, e)
		{
			Context = (QueryContext)e.Argument;
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		public void ExecuteQuery()
		{
			Tracer.Verbose("QueryWorker.ExecuteQuery", "START");
			IDataReader reader = null;

			try
			{
				reader = Context.Source.Database.Provider.GetSQLQueryInterface().ExecuteReader(Context.Source.ConnectionString, Context.Command);

				for (int i = 0; i < reader.FieldCount; i++)
					Context.UIDispatcher.Invoke(Context.AddColumn, reader.GetName(i));

				int counter = 0;
				while (!CancelPending() && reader.Read() )
				{
					dynamic data = new ExpandoObject();

					for (int i = 0; i < reader.FieldCount; i++)
						((IDictionary<String, Object>)data).Add(reader.GetName(i), reader.GetValue(i));

					Context.UIDispatcher.Invoke(Context.AddData, data);
					counter++;
				}

				Context.LineCount = counter;
				Context.Message = "Query ok";
			}
			catch (Exception error)
			{
				Tracer.Error("QueryWorker.ExecuteQuery", error);
				this.Context = null;
			}
			finally
			{
				if (reader != null)
				{
					reader.Close();
				}

				Tracer.Verbose("QueryWorker.ExecuteQuery", "END");
			}
		}
	}
}
