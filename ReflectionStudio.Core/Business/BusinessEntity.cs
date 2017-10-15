using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReflectionStudio.Core.Business
{
	public abstract class BusinessEntity
	{
		#region ----------------------ISCHANGED----------------------

		[NonSerialized()]
		private bool? _IsChanged = false;
		/// <summary>
		/// Track object changes to optimize saving...
		/// </summary>
		public bool? IsChanged
		{
			get { return _IsChanged; }
			set
			{
				if (_IsChanged != value)
				{
					_IsChanged = value;
				}
			}
		}

		#endregion
	}
}
