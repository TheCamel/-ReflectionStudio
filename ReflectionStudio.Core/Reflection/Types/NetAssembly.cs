using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.ObjectModel;

namespace ReflectionStudio.Core.Reflection.Types
{
	[Serializable]
	public class NetAssembly : NetBaseType
	{
		#region ----------------------PROPERTIES----------------------

		private ObservableCollection<NetReference> _References;
		public ObservableCollection<NetReference> References
		{
			get
			{
				if (_References == null)
					_References = new ObservableCollection<NetReference>();
				return _References;
			}
			set
			{
				if (_References != value)
					_References = value;
				else return;

				base.RaisePropertyChanged("References");
			}
		}

		private ObservableCollection<NetNamespace> _Namespaces;
		public ObservableCollection<NetNamespace> Namespaces
		{
			get
			{
				if (_Namespaces == null)
					_Namespaces = new ObservableCollection<NetNamespace>();

				return _Namespaces;
			}
			set
			{
				if (_Namespaces != value)
					_Namespaces = value;
				else return;

				base.RaisePropertyChanged("Namespaces");
			}
		}

		//private ObservableCollection<NetComplexType> _Members;
		//public ObservableCollection<NetComplexType> Members
		//{
		//    get { return _Members; }
		//    set
		//    {
		//        if (_Members != value)
		//            _Members = value;
		//        else return;

		//        base.RaisePropertyChanged("Members");
		//    }
		//}

		public string FilePath { get; set; }
		public string Version { get; set; }
		public string Culture { get; set; }
		public string PublicKey { get; set; }
		public string PublicKeyToken { get; set; }
		public string Hash { get; set; }
		public string HashAlgorithm { get; set; }
		public string EntryPoint { get; set; }
		public string TargetRuntime { get; set; }
		public string Kind { get; set; }
		public bool IsProgram { get; set; }

		#endregion

		public NetAssembly(object reflectType)
			: base(reflectType)
		{
		}
	}
}
