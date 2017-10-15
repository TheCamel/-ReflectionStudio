using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReflectionStudio.Core.Reflection;

namespace ReflectionStudio.Core.Reflection.Types
{
	[Serializable]
	public class NetBaseType : BindableObjectExtended
	{
		#region ----------------------PROPERTIES----------------------
		/// <summary>
		/// Store the reflection base type
		/// </summary>
		[NonSerialized]
		private object _Tag;
		public object Tag
		{
			get { return _Tag; }
			set { _Tag = value; }
		}

		/// <summary>
		/// Global name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Global name
		/// </summary>
		[NonSerialized]
		private string _DisplayName;
		public string DisplayName
		{
			get {
				if( string.IsNullOrEmpty(_DisplayName) )
					return Name;
				else return _DisplayName;
			}
			set { _DisplayName = value; }
		}

		/// <summary>
		/// Namespace of this type
		/// </summary>
		public string NameSpace { get; set; }

		/// <summary>
		/// The type this object derive from
		/// </summary>
		[NonSerialized]
		private NetBaseType _BaseType;
		public NetBaseType BaseType
		{
			get { return _BaseType; }
			set { _BaseType = value; }
		}

		#endregion

		public NetBaseType(object reflectType)
		{
			Tag = reflectType;
		}
	}
}
