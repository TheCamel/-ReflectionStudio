namespace Malls.Metier.BO.Base
{
	using System;
	
	using Coachis.Framework.Business.BO;

	/// <summary>
	/// The base class for that represents a record in the
	/// <c><#= DBCurrentObject.DBName #></c> <#= DBCurrentObject.Type.ToString() #>.
	/// </summary>
	/// <remarks>
	/// Do not change this source code manually.
	/// </remarks>
	public class <#= GetBOClassNameBase() #> : BusinessObject
	{
		#region PROPERTIES
<#		// Write properties
		foreach(DBColumn column in ((DBTable)DBCurrentObject).Columns)
		{
		if (!column.IsPrimaryKey)
		{ ##>
		
		#region <#= column.CodeName.ToUpper() #>
		private <#= column.DotNetType #> _<#= column.CodeName #>;

		/// <summary>
		/// Gets or sets the <c><#= column.DBName #></c> column value.
		/// </summary>
		/// <value>The <c><#= column.DBName #></c> column value.</value>
		public <#= column.DotNetType #> <#= column.CodeName #>
		{
			get { return _<#= column.CodeName #>; }
			set { _<#= column.CodeName #> = value; }
		}
		#endregion
<#		}
		} ##>
		
		#endregion
		
		#region CONSTRUCTORS
		/// <summary>
		/// Initializes a new instance of the <see cref="<#= GetBOClassNameBase() #>"/> class.
		/// </summary>
		public <#= GetBOClassNameBase() #>() : base()
		{
		}
		
		/// <summary>
		/// Constructeur avec un identifiant
		/// </summary>
		/// <param name="id">Identifiant de l'entite metier</param>
		public <#= GetBOClassNameBase() #>(int id) : base(id)
		{
		}
		#endregion

	} // End of <#= GetBOClassNameBase() #> class
} // End
