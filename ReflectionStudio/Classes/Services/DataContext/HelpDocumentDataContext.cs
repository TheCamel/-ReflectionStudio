using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReflectionStudio.Classes
{
	internal class HelpDocumentDataContext : XpsDocumentDataContext
	{
		override public bool CanSave
		{
			get { return false; }
		}
	}
}
