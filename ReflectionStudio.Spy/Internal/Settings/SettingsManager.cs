using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ReflectionStudio.Spy.Internal
{
	internal class SettingsManager
	{
		#region ----------------SINGLETON----------------
		public static readonly SettingsManager Instance = new SettingsManager();

		/// <summary>
		/// Private constructor for singleton pattern
		/// </summary>
		private SettingsManager()
		{
			_Settings = (RuntimeSettings)Deserialize(
				System.Reflection.Assembly.GetExecutingAssembly().Location.Replace(".dll", ".config"));
		}
		#endregion

		private RuntimeSettings _Settings = null;
		internal RuntimeSettings Settings
		{
			get { return _Settings; }
			set { _Settings = value; }
		}

		public object Deserialize(string filePath)
		{
			object objToDeserialize = null;

			XmlTextReader xmlReader = null;
			XmlSerializer xmls = null;

			try
			{
				xmlReader = new XmlTextReader(filePath);
				xmls = new XmlSerializer(typeof(RuntimeSettings));

				objToDeserialize = xmls.Deserialize(xmlReader);
			}
			catch (Exception err)
			{
				RuntimeLogger.Instance.Log(LogType.Error, "SettingsManager:Deserialize" + err.Message);
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
