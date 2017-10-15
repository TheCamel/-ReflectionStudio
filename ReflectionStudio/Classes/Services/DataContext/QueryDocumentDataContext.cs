using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReflectionStudio.Classes
{
	internal class QueryDocumentDataContext : DocumentDataContext
	{
		public QueryDocumentDataContext()
		{
			base.FileServiceType = typeof(TextFileService);
		}
	}
}
