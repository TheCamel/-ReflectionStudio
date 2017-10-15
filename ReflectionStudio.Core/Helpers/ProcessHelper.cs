using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ReflectionStudio.Core.Helpers
{
	public class ProcessHelper
	{
		static public void LaunchWebUri(Uri url)
		{
			Process.Start( new ProcessStartInfo( url.AbsoluteUri ) );
		}
	}
}
