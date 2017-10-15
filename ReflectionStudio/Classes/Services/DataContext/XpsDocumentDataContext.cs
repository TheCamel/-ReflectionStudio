using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReflectionStudio.Classes
{
	internal class XpsDocumentDataContext : DocumentDataContext
	{
		public XpsDocumentDataContext()
		{
			base.FileServiceType = typeof(XpsFileService);
		}
	}
}
