using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using ReflectionStudio.Core.Events;
using ReflectionStudio.Core.FileManagement;
using ReflectionStudio.Core;

namespace ReflectionStudio.Controls.Helpers
{
	/// <summary>
	/// ThemeElementCollection is used to serialize only
	/// </summary>
	[Serializable]
	public class ThemeElementCollection
	{
		[XmlElement("ThemeElement")]
		public List<ThemeElement> ThemeElement { get; set; }
	}
	/// <summary>
	/// ThemeElement is used to serialize and maintain the configuration
	/// </summary>
	[Serializable]
	public class ThemeElement
	{
		[XmlAttribute]
		public string Name { get; set; }

		[XmlAttribute]
		public string Group { get; set; }

		[XmlAttribute]
		public bool IsSelected { get; set; }

		[XmlAttribute]
		public bool IsDefault { get; set; }

		[XmlAttribute]
		public string Image { get; set; }

		[XmlElement("Dictionary")]
		public List<string> Dictionaries { get; set; }
	}

	/// <summary>
	/// ThemeManager helps managing themes as an association of a skin and a color. It discover resources in a configuration file as a collection 
	/// of ThemeElement that contains skins and colors.
	/// </summary>
	public class ThemeManager
	{
		#region ----------------SINGLETON----------------
		/// <summary>
		/// Singleton instance
		/// </summary>
		public static readonly ThemeManager Instance = new ThemeManager();

		/// <summary>
		/// Private constructor for singleton pattern
		/// </summary>
		private ThemeManager()
		{
		}
		#endregion

		#region ----------------PROPERTIES----------------

		private List<ThemeElement> _ThemeElements = null;
		/// <summary>
		/// All the theme elements available in the embedded configuration  file
		/// </summary>
		public List<ThemeElement> ThemeElements
		{
			get
			{
				if (_ThemeElements == null)
					_ThemeElements = new List<ThemeElement>();
				return _ThemeElements;
			}
		}

		/// <summary>
		/// get/set the selected skin in the collection
		/// </summary>
		public ThemeElement CurrentSkin
		{
			get { return ThemeElements.Where(p => p.IsSelected == true && p.Group == "Skins").First(); }
			set { LoadThemeResource(value); }
		}

		/// <summary>
		/// get/set the selected color in the collection
		/// </summary>
		public ThemeElement CurrentColor
		{
			get { return ThemeElements.Where(p => p.IsSelected == true && p.Group == "Colors").First(); }
			set { LoadThemeResource(value); }
		}

		#endregion

		#region ----------------METHODS----------------
		/// <summary>
		/// Load a ThemeElement into the application dictionnaries
		/// </summary>
		/// <param name="themeResource"></param>
		public void LoadThemeResource(ThemeElement themeResource)
		{
			Tracer.Verbose("ThemeManager:LoadThemeResource", "START");

			//TraceDictionnaries();

			Application.Current.Resources.BeginInit();
			
			try
			{
				//unload the previous same resource type, load the new one
				LoadDictionaries(ThemeElements.Where(p => p.Group == themeResource.Group && p.IsSelected == true).First(),
					themeResource);
			}
			catch (Exception all)
			{
				Tracer.Error("ThemeManager.LoadThemeResource", all);
			}

			Application.Current.Resources.EndInit();

			//TraceDictionnaries();

			Tracer.Verbose("ThemeManager:LoadThemeResource", "END");
		}

		/// <summary>
		/// Save the configuration to the disk with the current selected resource
		/// </summary>
		/// <returns></returns>
		public bool Save()
		{
			Tracer.Verbose("ThemeManager:Save", "START");

			bool result = false;
			try
			{
				ThemeElementCollection col = new ThemeElementCollection();
				col.ThemeElement = _ThemeElements;
				result = SerializeHelper.Serialize(Path.Combine(PathHelper.ApplicationPath, "ThemeManagerConfig.xml"), col, true);
			}
			catch (Exception all)
			{
				Tracer.Error("ThemeManager.Save", all);
			}
			Tracer.Verbose("ThemeManager:Save", "END");

			return result;
		}

		/// <summary>
		/// Load the configuration file from disk or from resource if it does not exist
		/// </summary>
		public void Load()
		{
			Tracer.Verbose("ThemeManager:Load", "START");
			try
			{
				string file = Path.Combine(PathHelper.ApplicationPath, "ThemeManagerConfig.xml");
				// if found in current directory
				if (File.Exists(file))
				{
					using (Stream s = File.OpenRead(file))
					{
						if (s == null)
							throw new InvalidOperationException("Could not open themes file");
						ThemeElementCollection elem = (ThemeElementCollection)SerializeHelper.Deserialize(s, typeof(ThemeElementCollection), true);
						_ThemeElements = elem.ThemeElement;

						//load selected resources
						InitializeResource(_ThemeElements.Where(p => p.IsSelected == true).ToList());
					}
				}
				else
				{
					//load the config from resources
					using (Stream s = Application.ResourceAssembly.GetManifestResourceStream("ReflectionStudio.Resources.Embedded.ThemeManagerConfig.xml"))
					{
						if (s == null)
							throw new InvalidOperationException("Could not find embedded resource");

						ThemeElementCollection elem = (ThemeElementCollection)SerializeHelper.Deserialize(s, typeof(ThemeElementCollection), true);
						_ThemeElements = elem.ThemeElement;

						//load default resources
						InitializeResource(_ThemeElements.Where(p => p.IsDefault == true).ToList());
					}
				}
			}
			catch (Exception all)
			{
				Tracer.Error("ThemeManager.Load", all);
			}
			Tracer.Verbose("ThemeManager:Load", "END");
		}

		#endregion

		#region ----------------INTERNALS----------------

		/// <summary>
		/// Load the specified ThemeElement liste and remove old ones
		/// </summary>
		/// <param name="themeResourceList"></param>
		private void InitializeResource(List<ThemeElement> themeResourceList)
		{
			Tracer.Verbose("ThemeManager:InitializeResource", "START");
			
			//TraceDictionnaries();
	
			try
			{
				Application.Current.Resources.BeginInit();

				//remove old ones
				List<ResourceDictionary> olds = new List<ResourceDictionary>();
				List<string> newsItems = new List<string>();

				foreach (ThemeElement element in themeResourceList)
					newsItems.AddRange(element.Dictionaries);

				foreach (string dico in newsItems)
				{
					//in existing dictionnary, be sure to remove old ones, in particular the on comming from xaml like App.xaml
					foreach (ResourceDictionary dictionnary in Application.Current.Resources.MergedDictionaries)
					{
						if (newsItems.Find(p => p == dictionnary.Source.OriginalString) == null)
							olds.Add(dictionnary);
					}
				}

				foreach (ResourceDictionary dictionnary in olds)
					Application.Current.Resources.MergedDictionaries.Remove(dictionnary);

				foreach (ThemeElement element in themeResourceList)
					//add new ones
					LoadDictionaries(element);

				Application.Current.Resources.EndInit();
			}
			catch (Exception all)
			{
				Tracer.Error("ThemeManager.InitializeResource", all);
			}

			//TraceDictionnaries();

			Tracer.Verbose("ThemeManager:InitializeResource", "END");
		}

		/// <summary>
		/// Load the specified ThemeElement in the application and set it as IsSelected
		/// </summary>
		/// <param name="oldThemeResource"></param>
		/// <param name="newThemeResource"></param>
		private void LoadDictionaries(ThemeElement oldThemeResource, ThemeElement newThemeResource)
		{
			Tracer.Verbose("ThemeManager:LoadDictionaries", "START");
			try
			{
				try
				{
					foreach (string dictionnary in oldThemeResource.Dictionaries)
					{
						ResourceDictionary dic = Application.Current.Resources.MergedDictionaries.FirstOrDefault(p => p.Source.OriginalString == dictionnary);
						if (dic != null)
						{
							//does not exist in new ones
							if (newThemeResource.Dictionaries.Where(p => p == dic.Source.OriginalString).Count() == 0)
								Application.Current.Resources.MergedDictionaries.Remove(dic);
						}
					}
					oldThemeResource.IsSelected = false;
				}
				catch (Exception all)
				{
					Tracer.Error("ThemeManager.LoadDictionaries", all);
				}

				foreach (string dictionnary in newThemeResource.Dictionaries)
				{
					//if does not exist in application 
					if (Application.Current.Resources.MergedDictionaries.Where(p => p.Source.OriginalString == dictionnary).Count() == 0)
					{
						Uri Source = new Uri(dictionnary, UriKind.Relative);
						ResourceDictionary dico = (ResourceDictionary)Application.LoadComponent(Source);
						dico.Source = Source;
						Application.Current.Resources.MergedDictionaries.Add(dico);
					}
				}
				newThemeResource.IsSelected = true;
			}
			catch (Exception all)
			{
				Tracer.Error("ThemeManager.LoadDictionaries", all);
			}
			Tracer.Verbose("ThemeManager:LoadDictionaries", "END");
		}

		/// <summary>
		/// Load the specified ThemeElement in the application and set it as IsSelected
		/// </summary>
		/// <param name="themeResource"></param>
		private void LoadDictionaries(ThemeElement themeResource)
		{
			Tracer.Verbose("ThemeManager:LoadDictionaries", "START");
			try
			{
				foreach (string dictionnary in themeResource.Dictionaries)
				{
					if (Application.Current.Resources.MergedDictionaries.Where(p => p.Source.OriginalString == dictionnary).Count() == 0)
					{
						Uri Source = new Uri(dictionnary, UriKind.Relative);
						ResourceDictionary dico = (ResourceDictionary)Application.LoadComponent(Source);
						dico.Source = Source;
						Application.Current.Resources.MergedDictionaries.Add(dico);
					}
				}
				themeResource.IsSelected = true;
			}
			catch (Exception all)
			{
				Tracer.Error("ThemeManager.LoadDictionaries", all);
			}
			Tracer.Verbose("ThemeManager:LoadDictionaries", "END");
		}

		private void TraceDictionnaries()
		{
			try
			{
				foreach (ResourceDictionary dictionnary in Application.Current.Resources.MergedDictionaries)
				{
					TraceRecursiveDictionnaries(dictionnary);
				}
			}
			catch (Exception all)
			{
				Tracer.Error("ThemeManager.TraceDictionnaries", all);
			}
		}

		private void TraceRecursiveDictionnaries(ResourceDictionary dico)
		{
			try
			{
				Tracer.Verbose("Dico :", dico.Source.ToString() );

				foreach (ResourceDictionary dictionnary in dico.MergedDictionaries)
				{
					TraceRecursiveDictionnaries(dictionnary);
				}
			}
			catch (Exception all)
			{
				Tracer.Error("ThemeManager.TraceRecursiveDictionnaries", all);
			}
		}

		#endregion
	}
}
