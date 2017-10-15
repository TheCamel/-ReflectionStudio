namespace Malls.Metier.BO.User
{
	using System;
	
	using Coachis.Framework.Business.BO;
	
	using Malls.Metier.BO.Base;

	/// <summary>
	/// The base class for that represents a record in the
	/// <c><#= DBCurrentObject.DBName #></c> <#= DBCurrentObject.Type.ToString() #>.
	/// </summary>
	/// <remarks>
	/// Do not change this source code manually.
	/// </remarks>
	public class <#= GetBOClassNameUser() #> : <#= GetBOClassNameBase() #>
	{
		#region CONSTRUCTORS
		/// <summary>
		/// Initializes a new instance of the <see cref="<#= GetBOClassNameUser() #>"/> class.
		/// </summary>
		public <#= GetBOClassNameUser() #>() : base()
		{
		}
		
		/// <summary>
		/// Constructeur avec un identifiant
		/// </summary>
		/// <param name="id">Identifiant de l'entite metier</param>
		public <#= GetBOClassNameUser() #>(int id) : base(id)
		{
		}
		#endregion	
	} // End of <#= GetBOClassNameUser() #> class
} // End
