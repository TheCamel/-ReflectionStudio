using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ReflectionStudio.Core.Database
{
	public interface IDbVerifier
	{
		object Verify();
	}

	public enum IDbVerifierOutputMode
	{
		eXpsReport,
		eTabularText
	}
}
