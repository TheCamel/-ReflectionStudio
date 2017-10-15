using System.Reflection;

namespace ReflectionStudio.Core
{
	static public class PathHelper
	{
		static public string ApplicationPath
		{
			get { return System.IO.Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ); }
		}

		static public string GetFullQualifedFileName(string fileName)
		{
			if (fileName.Contains(".\\"))
				return System.IO.Path.Combine(PathHelper.ApplicationPath, fileName.Remove(0,2));
			else
				return fileName;
		}
	}
}
