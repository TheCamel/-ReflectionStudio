using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ReflectionStudio.Core.FileManagement;
using ReflectionStudio.Core.Helpers;

namespace ReflectionStudio.Core.Database
{
	public class EmbeddedQueryManager
	{
		private List<Query> _Queries = null;
		internal List<Query> Queries
		{
		  get { return _Queries; }
		  set { _Queries = value; }
		}

		public EmbeddedQueryManager(string strAssembly, string strResName)
		{
			ReadQueryResources(strAssembly, strResName);
		}

		/// <summary>
		/// When no version specified, return the most recent query
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string GetQuery(string key)
		{
			return (from q in Queries
				   where q.Key == key && q.Version == Queries.Max(v => v.Version)
				   select q).First().Content;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string GetQuery(string key, string version)
		{
			return (from q in Queries
					where q.Key == key && q.Version == version.ToString()
					select q).First().Content;

			//return Queries.Find(p => p.Key == key && p.Version == version.ToString() ).Content;
		}

		private void ReadQueryResources(string strAssembly, string strResName)
		{
			Stream resourceStream = null;
			try
			{
				resourceStream = new ResourceHelper().ReadResourceAsStream(strAssembly, strResName);
				if (resourceStream != null)
					_Queries = ((Queries)SerializeHelper.Deserialize(resourceStream, typeof(Queries), true)).ListOfQuery;
			}
			catch (Exception Error)
			{
			}
			finally
			{
				if( resourceStream != null )
					resourceStream.Close();
			}
		}
	}

	[Serializable]
	public class Queries
	{
		public List<Query> ListOfQuery { get; set; }
	}

	[Serializable]
	public class Query
	{
		[XmlAttribute]
		public string Key { get; set; }

		[XmlAttribute]
		public string Version { get; set; }

		public string Content { get; set; }
	}
}
