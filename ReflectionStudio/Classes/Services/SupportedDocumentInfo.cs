using System;

namespace ReflectionStudio.Classes
{
	/// <summary>
	/// SupportedDocumentInfo describe the managed documents
	/// </summary>
	public class SupportedDocumentInfo
	{
		/// <summary>
		/// Display or not in new doc list
		/// </summary>
		public bool CanCreate { get; set; }

		/// <summary>
		/// Icon to display in the list
		/// </summary>
		public string Image { get; set; }

		/// <summary>
		/// Document type
		/// </summary>
		public string ShortDescription { get; set; }

		/// <summary>
		/// Document type description
		/// </summary>
		public string LongDescription { get; set; }

		/// <summary>
		/// Net Document type for creation
		/// </summary>
		public Type DocumentContentType { get; set; }

		/// <summary>
		/// Net IDocumentDataContext type for creation
		/// </summary>
		public Type DocumentDataType { get; set; }

		/// <summary>
		/// Name of the user control / unique Id
		/// </summary>
		public string DocumentContentGUID { get; set; }

		/// <summary>
		/// Counter used to increment new document without title 
		/// </summary>
		public int Counter { get; set; }

		/// <summary>
		/// default document extension
		/// </summary>
		public string Extension { get; set; }

		/// <summary>
		/// Dialog default document extension
		/// </summary>
		public string DialogExtension { get; set; }

		/// <summary>
		/// Dialog document multiple extension
		/// </summary>
		public string DialogFilter { get; set; }

		/// <summary>
		/// Default title or filename
		/// </summary>
		public string DefaultTitle { get; set; }
	}
}
