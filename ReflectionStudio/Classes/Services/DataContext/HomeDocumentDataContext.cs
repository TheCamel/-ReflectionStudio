using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Xps.Packaging;
using ReflectionStudio.Core;
using ReflectionStudio.Core.Helpers;
using System.IO;
using System.Windows.Documents;
using ReflectionStudio.Core.Events;
using System.Windows.Markup;
using System.Windows;

namespace ReflectionStudio.Classes
{
	internal class HomeDocumentDataContext : DocumentDataContext
	{
		public HomeDocumentDataContext()
		{
			//base.FileServiceType = typeof(XamlFileService);
			base.FullName = ".\\Home.xaml";
		}

		override public bool CanSave
		{
			get { return false; }
		}

		//override public object Load()
		//{
		//    try
		//    {
		//        UrlSaveHelper hlp = new UrlSaveHelper(ReflectionStudio.Properties.Settings.Default.HomeUrl, base.FullName);
		//        hlp.SaveUrlContent();
		//    }
		//    catch (Exception all)
		//    {
		//    }

		//    FlowDocument docFlow = null;

		//    // Finally, handle the case where the operation succeeded.
		//    if (File.Exists(base.FullName))
		//    {
		//        try
		//        {
		//            using (FileStream fs = File.OpenRead(base.FullName))
		//            {
		//                docFlow = (FlowDocument)XamlReader.Load(fs);
		//            }

		//            Tracer.Verbose("HomeDocument:bw_RunWorkerCompleted", "Internet document loaded");
		//        }
		//        catch (Exception)
		//        {
		//            //load the config from resources
		//            using (Stream fs = Application.ResourceAssembly.GetManifestResourceStream("ReflectionStudio.Resources.Embedded.Home.xaml"))
		//            {
		//                if (fs == null)
		//                    throw new InvalidOperationException("Could not find embedded resource");

		//                docFlow = (FlowDocument)XamlReader.Load(fs);

		//                Tracer.Verbose("HomeDocument:bw_RunWorkerCompleted", "Local document loaded");
		//            }
		//        }

		//        Tracer.Verbose("HomeDocument:bw_RunWorkerCompleted", "END");
		//    }

		//    return docFlow;
		//}
	}
}
