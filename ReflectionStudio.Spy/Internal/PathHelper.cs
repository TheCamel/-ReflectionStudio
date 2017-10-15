
namespace ReflectionStudio.Spy.Internal
{
	static internal class PathHelper
	{
		static internal string SnapPath()
		{
			return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
		}
	}
}
