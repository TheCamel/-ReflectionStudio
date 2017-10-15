using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ReflectionStudio.Controls.Property
{
	public class PropertyBase : INotifyPropertyChanged, IDisposable
	{
		// Fields
		private bool _disposed = false;

		// Events
		public event PropertyChangedEventHandler PropertyChanged;

		// Methods
		protected PropertyBase()
		{
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.Disposed)
			{
				this._disposed = true;
			}
		}

		~PropertyBase()
		{
			this.Dispose(false);
		}

		protected void NotifyPropertyChanged(string property)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(property));
			}
		}

		// Properties
		protected bool Disposed
		{
			get
			{
				return this._disposed;
			}
		}
	}
}
