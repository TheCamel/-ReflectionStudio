using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReflectionStudio.Classes;
using System.Windows.Xps.Packaging;
using ReflectionStudio.Core.Database;

namespace ReflectionStudio.Classes
{
	internal class XpsGeneratedReportDocumentDataContext : DocumentDataContext
	{
		public XpsGeneratedReportDocumentDataContext()
		{
			base.FileServiceType = typeof(XpsFileService);
		}

		override public object Load()
		{
			IDbVerifier verif = Entity as IDbVerifier;

			return verif.Verify();
		}
	}
}
