using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Xps.Packaging;
using CodeReason.Reports;
using ReflectionStudio.Core.Events;
using ReflectionStudio.Core.Database;
using ReflectionStudio.Core;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace ReflectionStudio.Core.Database.SQL2KProvider.Helper
{
	internal class ReportGenerator
	{
		internal XpsDocument Generate(string path, List<DataTable> results)
		{
			Tracer.Verbose("ReportGenerator:Generate", "START");

			XpsDocument xpsResult = null;

			try
			{
				EventDispatcher.Instance.RaiseStatus("Generating document", StatusEventType.StartProgress);

				DateTime dateTimeStart = DateTime.Now; // start time measure here

				//create a new report
				ReportDocument reportDocument = new ReportDocument();

				//retreive the template from the resources
				Stream stream = Assembly.GetCallingAssembly().GetManifestResourceStream(path);
				StreamReader reader = new StreamReader(stream);
				reportDocument.XamlData = reader.ReadToEnd();
				reportDocument.XamlImagePath = PathHelper.ApplicationPath;
				stream.Close();

				ReportData rd = new ReportData();

				// set constant document values
				rd.ReportDocumentValues.Add("GenerationTime", dateTimeStart); // print date is now
				
				rd.ReportDocumentValues.Add("Server", "2008");
				rd.ReportDocumentValues.Add("DatabaseName", 1);
				rd.ReportDocumentValues.Add("AnalisysTime", 1);
				rd.ReportDocumentValues.Add("Company", 1);
				rd.ReportDocumentValues.Add("Author", 1);
				rd.ReportDocumentValues.Add("Version", 1);

				rd.DataTables.AddRange(results);

				xpsResult = reportDocument.CreateXpsDocument(rd);

				// show the elapsed time in window title
				EventDispatcher.Instance.RaiseStatus(string.Format("Report generated in {0} sec", (DateTime.Now - dateTimeStart).TotalSeconds),
					StatusEventType.StopProgress);
				
				return xpsResult;
			}
			catch (Exception all)
			{
				Tracer.Error("ReportGenerator.Generate", all);
				return null;
			}
			finally
			{
				Tracer.Verbose("ReportGenerator:Generate", "END");
			}
		}
	}
}
