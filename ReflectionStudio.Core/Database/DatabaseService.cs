using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;

namespace ReflectionStudio.Core.Database
{
	public class DatabaseService
	{
		#region ----------------SINGLETON----------------
		public static readonly DatabaseService Instance = new DatabaseService();

		/// <summary>
		/// Private constructor for singleton pattern
		/// </summary>
		private DatabaseService()
		{
			ScanAssemblyFolder();
		}
		#endregion

		#region ----------------PROVIDERS----------------

		private List<String> _ProviderSearchDirectories = new List<String>();
		/// <summary>
		/// Add folder in it to get scan of assemblies
		/// </summary>
		public List<String> ProviderSearchDirectories
		{
			get
			{
				return this._ProviderSearchDirectories;
			}
			set
			{
				this._ProviderSearchDirectories = value;
			}
		}

		List<IDbSchemaProvider> _Providers;
		public ReadOnlyCollection<IDbSchemaProvider> Providers
		{
			get
			{
				if (_Providers == null)
					_Providers = LoadProviders();

				return _Providers.AsReadOnly();
			}
		}

		public IDbSchemaProvider GetProvider(string name)
		{
			return _Providers.Cast<IDbSchemaProvider>().Where(p => p.Name == name).Single();
		}

		#endregion

		public DataSource CreateSource(string sourceName, IDbSchemaProvider provider, string connectionString)
		{
			return new DataSource(sourceName, connectionString, provider.GetType());
		}

		#region ----------------INTERNALS----------------

		private List<IDbSchemaProvider> LoadProviders()
		{
			List<IDbSchemaProvider> list = new List<IDbSchemaProvider>();

			List<String> fileProvider = new List<String>();
			foreach (string folder in this.ProviderSearchDirectories)
			{
				fileProvider.AddRange(Directory.GetFiles(folder, "*Provider.dll"));
			}

			foreach (string str2 in fileProvider)
			{
				try
				{
					foreach (Type type in Assembly.LoadFrom(str2).GetTypes())
					{
						if (type.GetInterface(typeof(IDbSchemaProvider).FullName) != null)
						{
							IDbSchemaProvider provider = (IDbSchemaProvider)Activator.CreateInstance(type);
							if (list.Where(p => p.Name == provider.Name).Count() == 0)
							{
								list.Add(provider);
							}
						}
					}
					continue;
				}
				catch (Exception)
				{
					continue;
				}
			}
			return list;
		}

		private void ScanAssemblyFolder()
		{
			this.ProviderSearchDirectories.Add(AppDomain.CurrentDomain.BaseDirectory);
			FileInfo info = new FileInfo(Assembly.GetExecutingAssembly().Location);
			if (info != null)
			{
				this.ProviderSearchDirectories.Add(info.DirectoryName);
			}
			if (Assembly.GetEntryAssembly() != null)
			{
				info = new FileInfo(Assembly.GetEntryAssembly().Location);
				this.ProviderSearchDirectories.Add(info.DirectoryName);
			}
		}
		#endregion
	}
}
