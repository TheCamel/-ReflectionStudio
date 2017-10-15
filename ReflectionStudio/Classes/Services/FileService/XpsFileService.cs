using System;
using System.IO;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using ReflectionStudio.Core;
using ReflectionStudio.Core.Events;

namespace ReflectionStudio.Classes
{
	internal class XpsFileService : IFileService
	{
		public object Load(string fileName)
		{
			if (File.Exists(fileName))
				return new XpsDocument(fileName, System.IO.FileAccess.Read);
			else
				return null;
		}

		public bool Save(string fileName, object content)
		{
			XpsDocument source = content as XpsDocument;
			XpsDocument dest = null;

			Tracer.Verbose("XpsFileService:Save", "START");
			try
			{
				dest = new XpsDocument(fileName, System.IO.FileAccess.ReadWrite);
				
				XpsDocumentWriter xpsdw = XpsDocument.CreateXpsDocumentWriter(dest);
				xpsdw.Write(source.GetFixedDocumentSequence());

				return true;
			}
			catch (Exception all)
			{
				Tracer.Error("XpsFileService.Save", all);
				return false;
			}
			finally
			{
				if( dest != null )
					dest.Close();

				Tracer.Verbose("XpsFileService:Save", "END");
			}
		}
	}
}
