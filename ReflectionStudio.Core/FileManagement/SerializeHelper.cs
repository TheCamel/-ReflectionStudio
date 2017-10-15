using System;
using System.IO;

namespace ReflectionStudio.Core.FileManagement
{
	/// <summary>
	/// Serialize/Deserialize object to file or stream
	/// </summary>
	public class SerializeHelper
	{
		/// <summary>
		/// Serialize an object to a given file
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="objToSerialize"></param>
		/// <param name="asXML"></param>
		/// <returns></returns>
		static public bool Serialize(string filePath, object objToSerialize, bool asXML = false)
		{
			if (asXML)
				return XmlHelper.Serialize(filePath, objToSerialize);
			else
				return BinaryHelper.Serialize(filePath, objToSerialize);
		}

		/// <summary>
		/// Deserialize an object from a given file
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="objType"></param>
		/// <param name="asXML"></param>
		/// <returns></returns>
		static public object Deserialize(string filePath, Type objType, bool asXML = false)
		{
			if (asXML)
				return XmlHelper.Deserialize(filePath, objType);
			else
				return BinaryHelper.Deserialize(filePath);
		}

		/// <summary>
		/// Serialize an object to a given stream
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="objToSerialize"></param>
		/// <param name="asXML"></param>
		/// <returns></returns>
		static public bool Serialize(Stream stream, object objToSerialize, bool asXML = false)
		{
			if (asXML)
				return XmlHelper.Serialize(stream, objToSerialize);
			else
				return BinaryHelper.Serialize(stream, objToSerialize);
		}

		/// <summary>
		/// Deserialize an object from a given stream
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="objType"></param>
		/// <param name="asXML"></param>
		/// <returns></returns>
		static public object Deserialize(Stream stream, Type objType, bool asXML = false)
		{
			if (asXML)
				return XmlHelper.Deserialize(stream, objType);
			else
				return BinaryHelper.Deserialize(stream);
		}
	}
}
