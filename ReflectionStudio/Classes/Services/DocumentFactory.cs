using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AvalonDock;
using ReflectionStudio.Classes.Workspace;
using ReflectionStudio.Components.UserControls;
using ReflectionStudio.Core.Events;
using ReflectionStudio.Core.FileManagement;

namespace ReflectionStudio.Classes
{
	internal class DocumentFactory
	{
		#region ----------------SINGLETON----------------
		/// <summary>
		/// Singleton
		/// </summary>
		public static readonly DocumentFactory Instance = new DocumentFactory();

		/// <summary>
		/// Private constructor for singleton pattern
		/// </summary>
		private DocumentFactory()
		{
		}
		#endregion

		#region ---------------------PRIVATE---------------------

		private DockingManager _DockManager = null;

		#endregion

		#region ----------------PROPERTIES----------------


		public DocumentBase ActiveDocument
		{
			get { return _DockManager.ActiveDocument as DocumentBase; }
		}

		List<SupportedDocumentInfo> _SupportedDocuments = new List<SupportedDocumentInfo>();
		public List<SupportedDocumentInfo> SupportedDocuments
		{
			get { return _SupportedDocuments; }
		}

		#endregion

		#region ----------------METHODS----------------

		public void Initialize(DockingManager dockManager)
		{
			_DockManager = dockManager;

			InitSupportedDocuments();
		}

		internal SupportedDocumentInfo GetSupportedDocumentInfo(DocumentBase docInstance)
		{
			return _SupportedDocuments.Find(p => p.DocumentContentType == docInstance.GetType());
		}

		internal string GetAllSupportedFilter()
		{
			string result = string.Empty;

			foreach( string item in _SupportedDocuments.Where(p => !string.IsNullOrEmpty(p.DialogFilter)).Select(p=>p.DialogFilter).Distinct() )
				result += item + "|";

			return result.Remove( result.Length-1 );
		}


		public DocumentBase CreateDocument(Type docType, IDocumentDataContext context = null)
		{
			return GetDocument(_SupportedDocuments.Find(p => p.DocumentContentType == docType), context, true);
		}

		public DocumentBase CreateDocument(SupportedDocumentInfo info, IDocumentDataContext context = null)
		{
			return GetDocument(info, context, true);
		}


		public void OpenDocument(Type docType, IDocumentDataContext context)
		{
			//WorkspaceService.Instance.AddRecentFile(context.FullName);
			
			GetDocument(_SupportedDocuments.Find(p => p.DocumentContentType == docType), context, true);
		}

		public void OpenDocument( DiskContent dc )
		{
			SupportedDocumentInfo info = SupportedDocuments.Find(p => p.Extension == dc.Extension);
			if (info != null)
			{
				IDocumentDataContext context = (IDocumentDataContext)Activator.CreateInstance(info.DocumentDataType);
				context.FullName = dc.FullName;
				GetDocument(info, context, true);
			}
		}

		public void OpenDocument(SupportedDocumentInfo info, IDocumentDataContext context)
		{
			//WorkspaceService.Instance.AddRecentFile(context.FullName);
			
			GetDocument(info, context, true);
		}

		//public void CloseDocument(SupportedDocumentInfo info, DocumentDataContext context)
		//{
		//    Tracer.Verbose("DocumentFactory:CloseDocument", "docName {0}", docName);

		//    try
		//    {
		//        IEnumerable<DocumentContent> docs = _DockManager.Documents.Where(d => d.Name == docName);

		//        if (docs.Count() == 0)
		//            return;
		//        else
		//        {
		//            foreach (DocumentContent doc in docs)
		//                doc.Close();
		//        }
		//    }
		//    catch (Exception err)
		//    {
		//        Tracer.Error("DocumentFactory.CloseDocument", err);
		//    }
		//    finally
		//    {
		//        Tracer.Verbose("DocumentFactory:CloseDocument", "END");
		//    }
		//}

		#endregion

		#region ---------------------EVENTS---------------------

		private void DocumentClosing(object sender, CancelEventArgs e)
		{
			DocumentBase db = sender as DocumentBase;

			Tracer.Verbose("DocumentFactory.DocumentClosing", db.Title + " closing");

			//manage dirty state ?
		}

		private void DocumentClosed(object sender, EventArgs e)
		{
			DocumentBase db = sender as DocumentBase;

			Tracer.Verbose("DocumentFactory.DocumentClosed", db.Title + " closed");

			if( GetSupportedDocumentInfo(db).CanCreate )
				WorkspaceService.Instance.AddRecentFile(db.Context.FullName);
		}

		#endregion
		
		#region ----------------INTERNALS----------------

		private DocumentBase GetDocument(SupportedDocumentInfo docInfo)
		{
			return GetDocument(docInfo, null, true);
		}

		private DocumentBase GetDocument(SupportedDocumentInfo docInfo, IDocumentDataContext context, bool activate)
		{
			//Tracer.Verbose("DocumentFactory:GetDocument", "docName{0}, docTitle{0}, docType{0}, activate{0}", docName, docTitle, docType, activate);

			DocumentBase doc = FindDocument(docInfo, context);

			try
			{
				if (doc == null)
					doc = CreateNewDocument(docInfo, context);

                if (doc != null && activate)
                    _DockManager.ActiveDocument = doc;
			}
			catch (Exception err)
			{
				Tracer.Error("DocumentFactory.GetDocument", err);
			}
			finally
			{
				Tracer.Verbose("DocumentFactory:GetDocument", "END");
			}

			return doc;
		}

		private DocumentBase CreateNewDocument(SupportedDocumentInfo docInfo, IDocumentDataContext context)
        {
			//Tracer.Verbose("DocumentFactory:CreateDocument", "docName{0}, docTitle{0}, docType{0}, activate{0}", docName, docTitle, docType, activate);

			DocumentBase doc = null;
            
            try
            {
				doc = (DocumentBase)Activator.CreateInstance(docInfo.DocumentContentType);

                if( doc != null )
                {
					if (string.IsNullOrEmpty(context.FullName))
					{
						doc.Title = string.Format(docInfo.DefaultTitle, docInfo.Counter++);
						doc.ToolTip = doc.Title;
					}
					doc.Name = docInfo.DocumentContentGUID;
                    doc.DataContext = context;

                    doc.Closing += new EventHandler<CancelEventArgs>(DocumentClosing);
                    doc.Closed += new EventHandler(DocumentClosed);

                    _DockManager.MainDocumentPane.Items.Add(doc);
                }
            }
            catch (Exception err)
            {
				Tracer.Error("DocumentFactory.CreateDocument", err);
            }
            finally
            {
				Tracer.Verbose("DocumentFactory:CreateDocument", "END");
            }
            return doc;
        }

		private DocumentBase FindDocument(SupportedDocumentInfo docInfo, IDocumentDataContext context)
        {
            IEnumerable<DocumentContent> list = _DockManager.Documents.Where(d => d.Name == docInfo.DocumentContentGUID && d.Title == context.Name);
			if (list.Count() == 1)
				return list.First() as DocumentBase;
			else
				return null;
        }

		internal void InitSupportedDocuments()
		{
			_SupportedDocuments.Add(new SupportedDocumentInfo()
			{
				CanCreate = false,
				ShortDescription = "Home",
				LongDescription = "Home Page",
				Image = "/ReflectionStudio;component/Resources/Images/16x16/template.png",
				DocumentContentType = typeof(HomeDocument),
				DocumentDataType = typeof(HomeDocumentDataContext),
                DocumentContentGUID = "_HomeDocument",
				Counter = 0,
				DefaultTitle = "Home"
			});
			_SupportedDocuments.Add(new SupportedDocumentInfo()
			{
				CanCreate = false,
				ShortDescription = "Help",
				LongDescription = "Help Page",
				Image = "/ReflectionStudio;component/Resources/Images/16x16/template.png",
				DocumentContentType = typeof(HelpDocument),
				DocumentDataType = typeof(HelpDocumentDataContext),
                DocumentContentGUID = "_HelpDocument",
				Counter = 0,
				DefaultTitle = "Help"
			});
			_SupportedDocuments.Add(new SupportedDocumentInfo()
			{
				CanCreate = false,
				ShortDescription = "Project Settings",
				LongDescription = "SQL Query to run against a data source",
				Image = "/ReflectionStudio;component/Resources/Images/16x16/template.png",
				DocumentContentType = typeof(ProjectDocument),
				//DocumentDataType = typeof(),
                DocumentContentGUID = "_DocProject",
				Counter = 0,
				Extension = ".rsp",
				DialogExtension = "*.rsp",
				DialogFilter = "Project files (*.rsp)|*.rsp",
				DefaultTitle = "Project settings"
			});
			_SupportedDocuments.Add(new SupportedDocumentInfo()
			{
				CanCreate = true,
				ShortDescription = "SQL Query",
				LongDescription = "SQL Query to run against a data source",
				Image = "/ReflectionStudio;component/Resources/Images/16x16/template.png",
				DocumentContentType = typeof(QueryDocument),
				DocumentDataType = typeof(QueryDocumentDataContext),
                DocumentContentGUID = "_DocSQLQuery",
				Counter = 0,
				Extension = ".sql",
				DialogExtension = "*.sql",
				DialogFilter = "Query files (*.sql)|*.sql|Text files (*.txt)|*.txt",
				DefaultTitle = "Query{0}"
			});
			_SupportedDocuments.Add(new SupportedDocumentInfo()
			{
				CanCreate = true,
				ShortDescription = "Diagram",
				LongDescription = "Assembly and class diagram",
				Image = "/ReflectionStudio;component/Resources/Images/16x16/template.png",
				//DocumentContentType = typeof(ProjectDocument),
				//DocumentDataType = typeof(),
                DocumentContentGUID = "_DocDiagram",
				Counter = 0,
				Extension = ".rsd",
				DialogExtension = "*.rsd",
				DialogFilter = "Diagram files (*.rsd)|*.rsd",
				DefaultTitle = "Diagram {0}"
			});
			_SupportedDocuments.Add(new SupportedDocumentInfo()
			{
				CanCreate = true,
				ShortDescription = "Template",
				LongDescription = "CSharp template for code generation",
				Image = "/ReflectionStudio;component/Resources/Images/16x16/template.png",
				DocumentContentType = typeof(TemplateDocument),
				DocumentDataType = typeof(TemplateDocumentDataContext),
                DocumentContentGUID = "_DocTemplate",
				Counter = 0,
				Extension = ".rst",
				DialogExtension = "*.rst",
				DialogFilter = "Template files (*.rst;*.txt)|*.rst;*.txt",
				DefaultTitle = "Template {0}"
			});

			_SupportedDocuments.Add(new SupportedDocumentInfo()
			{
				CanCreate = false,
				ShortDescription = "Report",
				LongDescription = "Xps report",
				Image = "/ReflectionStudio;component/Resources/Images/16x16/template.png",
				DocumentContentType = typeof(XpsReportDocument),
				DocumentDataType = typeof(XpsDocumentDataContext),
				DocumentContentGUID = "_DocReport",
				Counter = 0,
				Extension = ".xps",
				DialogExtension = "*.xps",
				DialogFilter = "xps files (*.xps)|*.xps",
				DefaultTitle = "Report {0}"
			});
		}

		#endregion
	}
}
