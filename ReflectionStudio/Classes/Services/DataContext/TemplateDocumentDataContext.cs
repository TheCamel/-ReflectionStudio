using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReflectionStudio.Classes
{
	internal class TemplateDocumentDataContext : DocumentDataContext
	{
		public TemplateDocumentDataContext()
		{
			base.FileServiceType = typeof(TextFileService);
		}
	}
}
