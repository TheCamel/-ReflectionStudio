using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReflectionStudio.Core.Events;
using System.Data;

namespace ReflectionStudio.Core.Database.SQL2KProvider.Helper
{
	internal class TextGenerator
	{
		private object Generate( List<DataTable> list  )
		{
			Tracer.Verbose("TextGenerator:Generate Entry", "param1={0}", list );

			object ReturnValue = null;
            
			try
			{
				return "NOT IMPLEMENTED YET -:(";
			}
			catch (Exception err)
			{
				Tracer.Error("TextGenerator.Generate", err);
				return null;
			}
			finally
			{
				Tracer.Verbose("TextGenerator:Generate output", "returns {0}", ReturnValue );
			}
		}
	}
}
