using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ReflectionStudio.Core;

namespace ReflectionStudio.Classes
{
	internal class TextFileService : IFileService
	{
		public object Load(string fileName)
		{
			if (File.Exists(fileName))
				using (StreamReader sr = File.OpenText(fileName))
				{
					return sr.ReadToEnd();
				}
			else
				return null;
		}

		public bool Save(string fileName, object content)
		{
			throw new NotImplementedException();
		}
	}
}
