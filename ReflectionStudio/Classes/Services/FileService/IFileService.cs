
namespace ReflectionStudio.Classes
{
	interface IFileService
	{
		object Load(string fileName);
		bool Save(string fileName, object content);
	}
}
