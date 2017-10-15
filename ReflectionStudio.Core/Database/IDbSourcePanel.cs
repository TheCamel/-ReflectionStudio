using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReflectionStudio.Core.Database
{
	public interface IDbSourcePanel
	{
		string ConnectionString { get; }
		string SourceName { get; }

		bool TestConnection();
	}
}
