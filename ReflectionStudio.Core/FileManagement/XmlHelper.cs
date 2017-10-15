using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using ReflectionStudio.Core.Events;

namespace ReflectionStudio.Core.FileManagement
{
	/// <summary>
	/// Serialize/Deserialize object to XML file or stream. Internal, use SerializeHelper
	/// </summary>
	internal class XmlHelper
	{
		/// <summary>
		/// Serialize an object to a given file
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="objToSerialize"></param>
		/// <returns></returns>
		static internal bool Serialize(string filePath, object objToSerialize)
		{
			StreamWriter writer = null;
			XmlSerializer xmls = null;

			try
			{
				writer = new StreamWriter(filePath);
				xmls = new XmlSerializer(objToSerialize.GetType());

				xmls.Serialize(writer, objToSerialize);
			}
			catch (Exception err)
			{
				Tracer.Error("XmlHelper:Serialize", err);
				return false;
			}
			finally
			{
				xmls = null;
				writer.Close();
			}
			return true;
		}

		/// <summary>
		/// Deserialize an object from a given file
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="objType"></param>
		/// <returns></returns>
		static internal object Deserialize(string filePath, Type objType)
		{
			object objToDeserialize = null;

			XmlTextReader xmlReader = null;
			XmlSerializer xmls = null;

			try
			{
				xmlReader = new XmlTextReader(filePath);
				xmls = new XmlSerializer(objType);

				objToDeserialize = xmls.Deserialize(xmlReader);
			}
			catch (Exception err)
			{
				Tracer.Error("XmlHelper:Deserialize", err);
				return null;
			}
			finally
			{
				xmls = null;
				xmlReader.Close();
			}

			return objToDeserialize;
		}

		/// <summary>
		/// Serialize an object to a given stream
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="objToSerialize"></param>
		/// <returns></returns>
		static internal bool Serialize(Stream stream, object objToSerialize)
		{
			StreamWriter writer = null;
			XmlSerializer xmls = null;

			try
			{
				writer = new StreamWriter(stream);
				xmls = new XmlSerializer(objToSerialize.GetType());

				xmls.Serialize(writer, objToSerialize);
			}
			catch (Exception err)
			{
				Tracer.Error("XmlHelper:Serialize", err);
				return false;
			}
			finally
			{
				xmls = null;
				writer.Close();
			}
			return true;
		}

		/// <summary>
		/// Deserialize an object from a given stream
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="objType"></param>
		/// <returns></returns>
		static internal object Deserialize(Stream stream, Type objType)
		{
			object objToDeserialize = null;

			XmlTextReader xmlReader = null;
			XmlSerializer xmls = null;

			try
			{
				xmlReader = new XmlTextReader(stream);
				xmls = new XmlSerializer(objType);

				objToDeserialize = xmls.Deserialize(xmlReader);
			}
			catch (Exception err)
			{
				Tracer.Error("XmlHelper:Deserialize", err);
				return null;
			}
			finally
			{
				xmls = null;
				xmlReader.Close();
			}

			return objToDeserialize;
		}
	}
}
