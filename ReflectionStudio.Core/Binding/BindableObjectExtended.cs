using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace ReflectionStudio.Core
{
	[Serializable]
	public abstract class BindableObjectExtended : BindableObject
	{
		#region ----------------------PROPERTIES----------------------

		private bool? _isChecked = true;
		public bool? IsChecked
		{
			get { return _isChecked; }
			set { this.SetIsChecked(value, true, true); }
		}

		public BindableObjectExtended Parent
		{
			get;
			set;
		}

		private ObservableCollection<BindableObjectExtended> _Children = new ObservableCollection<BindableObjectExtended>();
		public ObservableCollection<BindableObjectExtended> Children
		{
			get { return _Children; }
			set { _Children = value; }
		}

		#endregion

		#region ----------------------CHECK MANAGEMENT----------------------

		public void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
		{
			if (value == _isChecked)
				return;

			_isChecked = value;

			if (updateChildren && _isChecked.HasValue)
			{
				foreach (BindableObjectExtended item in this.Children)
					item.SetIsChecked(_isChecked, true, false);
			}
			if (updateParent && Parent != null)
				Parent.VerifyCheckState();

			this.RaisePropertyChanged("IsChecked");
		}

		public void VerifyCheckState()
		{
			bool? state = null;
			for (int i = 0; i < this.Children.Count; ++i)
			{
				bool? current = this.Children[i].IsChecked;
				if (i == 0)
				{
					state = current;
				}
				else if (state != current)
				{
					state = null;
					break;
				}
			}
			this.SetIsChecked(state, false, true);
		}
		#endregion
	}
}
