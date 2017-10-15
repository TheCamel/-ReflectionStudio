namespace Malls.Metier.DAC.User
{
	using System;
	using Microsoft.Practices.EnterpriseLibrary.Data;

	using Coachis.Framework.Data;
	using Coachis.Framework.Business;
	
	using Malls.Metier.DAC.Base;
	using Malls.Metier.BO.User;
	
	/// <summary>
	/// The base class for that represents a record in the
	/// <c><#= DBCurrentObject.DBName #></c> <#= DBCurrentObject.Type.ToString() #>.
	/// </summary>
	/// <remarks>
	/// Do not change this source code manually.
	/// </remarks>
	public class <#= GetDACClassNameUser() #> : <#= GetDACClassNameBase() #>
	{
		#region Mise en oeuvre Singleton
		private static <#= GetDACClassNameUser() #> _singleton;
		/// <summary>
		/// Constructeur privé pour forcer le passage par GetInstance()
		/// </summary>
		private <#= GetDACClassNameUser() #>()
		{
		}
		/// <summary>
		/// Méthode renvoyant un objet <#= GetDACClassNameUser() #>
		/// </summary>
		/// <returns>Instance unique</returns>
		public static <#= GetDACClassNameUser() #> GetInstance()
		{
			if (_singleton == null)
			{
				_singleton = new <#= GetDACClassNameUser() #>();
			}
			return _singleton;
		}
		#endregion
             
	} // End of <#= GetDACClassNameUser() #> class
} // End

