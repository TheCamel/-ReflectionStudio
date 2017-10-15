using System.ComponentModel;
using ReflectionStudio.Core.Helpers;

namespace ReflectionStudio.Core.Project
{
	internal class ProjectWorker : WorkerBase
	{
		#region ----------------------INTERNALS----------------------

		/// <summary>
		/// The project we are working on
		/// </summary>
		protected ProjectEntity Project
		{
			get;
			set;
		}

		#endregion

		#region ----------------------CONSTRUCTORS----------------------

		public ProjectWorker(BackgroundWorker worker, DoWorkEventArgs e)
			: base(worker, e)
		{
			Project = (ProjectEntity)e.Argument;
		}

		#endregion
	}
}
