using System;

namespace ReflectionStudio.Classes
{
	public interface IDocumentDataContext
	{
		bool CanSave { get; }
		object Entity { get; set; }
		string Extension { get; }
		Type FileServiceType { get; set; }
		string FullName { get; set; }
		string Name { get; }
		string NameWithoutExtension { get; }

		object Load();
		bool Save(object content);
		bool SaveAs(object content, string newFileName);
	}
}
