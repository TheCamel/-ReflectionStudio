namespace Malls.Metier.Service.User
{
	using System;

	using Coachis.Framework.Business.BO;
	using Coachis.Framework.Business.Service;

	using Malls.Metier.BO.User;
	using Malls.Metier.DAC.User;
	using Malls.Metier.Service.Base;
	
	/// <summary>
	/// The base class for that represents the service for 
	/// <c><#= DBCurrentObject.DBName #></c> <#= DBCurrentObject.Type.ToString() #>.
	/// or the <c><#= GetBOClassNameUser() #></c> business object class
	/// </summary>
	public class <#= GetServiceClassNameUser() #> : <#= GetServiceClassNameBase() #>
	{
		#region CONSTRUCTORS
		/// <summary>
		/// Initializes a new instance of the <see cref="<#= GetServiceClassNameUser() #>"/> class.
		/// </summary>
		public <#= GetServiceClassNameUser() #>() : base()
		{
		}
		
		/// <summary>
		/// Constructeur avec un identifiant
		/// </summary>
		/// <param name="id">Identifiant de l'entite metier</param>
		public <#= GetServiceClassNameUser() #>( string user ) : base( user )
		{
		}
		#endregion
		
	} // End of <#= GetServiceClassNameUser() #> class
} // End
