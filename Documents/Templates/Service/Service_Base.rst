<#		int max = ((DBTable)DBCurrentObject).Columns.Count;
		int count = 1;
		foreach(DBColumn column in ((DBTable)DBCurrentObject).Columns)
		{ 
			if ( column.IsPrimaryKey ||
				column.CodeName == "CreeLe" ||
				column.CodeName == "CreePar" ||
				column.CodeName == "ModifieLe" ||
				column.CodeName == "ModifiePar" )
			{
				max--;
			}
		} ##>
namespace Malls.Metier.Service.Base
{
	using System;

	using Coachis.Framework.Business.BO;
	using Coachis.Framework.Business.Service;

	using Malls.Metier.BO.User;
	using Malls.Metier.DAC.User;
	
	/// <summary>
	/// The base class for that represents the service for 
	/// <c><#= DBCurrentObject.DBName #></c> <#= DBCurrentObject.Type.ToString() #>.
	/// or the <c><#= GetBOClassNameUser() #></c> business object class
	/// </summary>
	/// <remarks>
	/// Do not change this source code manually.
	/// </remarks>
	public class <#= GetServiceClassNameBase() #> : ServiceBase
	{
		#region CONSTRUCTORS
		/// <summary>
		/// Initializes a new instance of the <see cref="<#= GetServiceClassNameUser() #>"/> class.
		/// </summary>
		public <#= GetServiceClassNameBase() #>() : base()
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="<#= GetServiceClassNameUser() #>"/> class.
		/// </summary>
		/// <param name="user">Nom de l'utilisateur identifié dans la session active</param>
		public <#= GetServiceClassNameBase() #>( string user ) : base( user )
		{
		}
		#endregion
		
		#region CRUD SERVICE FUNCTIONS
		
		#region GETALL
		/// <summary>
		/// Obtenir la collection des <#= GetBOClassNameUser() #>
		/// </summary>
		/// <returns></returns>
		public BusinessObjectCollection GetAll()
		{
			<#= GetDACClassNameUser() #> DAC = <#= GetDACClassNameUser() #>.GetInstance();
			return DAC.ReadAll();
		}
		#endregion

		#region GET
		/// <summary>
		/// Obtenir un <#= GetBOClassNameUser() #>
		/// </summary>
		/// <returns></returns>
		public <#= GetBOClassNameUser() #> Get( Int64 Id )
		{
			<#= GetDACClassNameUser() #> DAC = <#= GetDACClassNameUser() #>.GetInstance();
			return (<#= GetBOClassNameUser() #>)DAC.Read( Id );
		}
		#endregion
		
		#region CREATE
		/// <summary>
		/// Créer un nouvel enregistrement pour un <#= GetBOClassNameUser() #>
		/// </summary>
		/// <returns> </returns>
		public Int64 Create( 
<#		count = 1;
		foreach(DBColumn column in ((DBTable)DBCurrentObject).Columns)
		{ 
			if ( !column.IsPrimaryKey &&
				column.CodeName != "CreeLe" &&
				column.CodeName != "CreePar" &&
				column.CodeName != "ModifieLe" &&
				column.CodeName != "ModifiePar" )
			{ ##>
<#			if( count == max )
			{  ##>
				<#= column.DotNetType #> <#= column.CodeName.ToLower() #> )
<#
			count++;
			}
			else
			{ ##>
				<#= column.DotNetType #> <#= column.CodeName.ToLower() #>,
<#			count++;
			}
			}
		} ##>
		{
			<#= GetDACClassNameUser() #> DAC = <#= GetDACClassNameUser() #>.GetInstance();

			<#= GetBOClassNameUser() #> bo = new <#= GetBOClassNameUser() #>();
<#		foreach(DBColumn column in ((DBTable)DBCurrentObject).Columns)
		{ 
			if ( !column.IsPrimaryKey &&
				column.CodeName != "CreeLe" &&
				column.CodeName != "CreePar" &&
				column.CodeName != "ModifieLe" &&
				column.CodeName != "ModifiePar" )
			{ ##>
			bo.<#= column.CodeName #> = <#= column.CodeName.ToLower() #>;
<#			}
		} ##>
			
			bo.CreeLe = DateTime.Today;
			bo.CreePar = this.User;

			return DAC.Create( bo, null );
		}
		#endregion

		#region UPDATE
		/// <summary>
		/// Mettre a jour un enregistrement pour un <#= GetBOClassNameUser() #>
		/// </summary>
		public void Update( 
<#		max+=1;
		count = 1;
		foreach(DBColumn column in ((DBTable)DBCurrentObject).Columns)
		{ 
			if( column.CodeName != "CreeLe" &&
				column.CodeName != "CreePar" &&
				column.CodeName != "ModifieLe" &&
				column.CodeName != "ModifiePar" )
			{ ##>
<#			if( count == max )
			{  ##>
				<#= column.DotNetType #> <#= column.CodeName.ToLower() #> )
<#			count++;
			}
			else
			{ ##>
				<#= column.DotNetType #> <#= column.CodeName.ToLower() #>,
<#			count++;
			}
			}

		} ##>
		{
			<#= GetDACClassNameUser() #> DAC = <#= GetDACClassNameUser() #>.GetInstance();

			<#= GetBOClassNameUser() #> bo = new <#= GetBOClassNameUser() #>();
<#		foreach(DBColumn column in ((DBTable)DBCurrentObject).Columns)
		{ 
			if( column.CodeName != "CreeLe" &&
				column.CodeName != "CreePar" &&
				column.CodeName != "ModifieLe" &&
				column.CodeName != "ModifiePar" )
			{ 
			if ( !column.IsPrimaryKey )
			{ ##>
			bo.<#= column.CodeName #> = <#= column.CodeName.ToLower() #>;
<#			}
			else
			{ ##>
			bo.Id = <#= column.CodeName.ToLower() #>;
<#			}
			}
		} ##>			
			
			bo.ModifieLe = DateTime.Today;
			bo.ModifiePar = this.User;

			DAC.Update( bo, null );
		}
		#endregion
		
		#region DELETE
		/// <summary>
		/// Supprimer un enregistrement pour un <#= GetBOClassNameUser() #>
		/// </summary>
		public void Delete( Int64 id )
		{
			<#= GetDACClassNameUser() #> DAC = <#= GetDACClassNameUser() #>.GetInstance();

			DAC.Delete( id, null );
		}
		#endregion
		
		#endregion
		
	} // End of <#= GetServiceClassNameBase() #> class
} // End
