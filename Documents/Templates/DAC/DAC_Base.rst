namespace Malls.Metier.DAC.Base
{
	using System;
	using System.Data;
	using Microsoft.Practices.EnterpriseLibrary.Data;

	using Coachis.Framework.Data;
	using Coachis.Framework.Business.BO;
	
	using Malls.Metier.BO.User;
	
	/// <summary>
	/// The base class for that represents a record in the
	/// <c><#= DBCurrentObject.DBName #></c> <#= DBCurrentObject.Type.ToString() #>.
	/// </summary>
	/// <remarks>
	/// Do not change this source code manually.
	/// </remarks>
	public abstract class <#= GetDACClassNameBase() #> : DacCRUDBase
	{
		#region CONSTRUCTOR
		public <#= GetDACClassNameBase() #>() : base ( "<#= DBCurrentObject.DBName #>" )
		{
		}
		#endregion
		
		#region Methodes de transformations Entites Metier <-> Objets SQL
		
		#region TransformerEnEntite
		protected override BusinessObject TransformToBusinessObject(IDataReader reader)
		{
			<#= GetBOClassNameUser() #> _entite = null;
			try
			{
				_entite = new <#= GetBOClassNameUser() #>();
<#				foreach(DBColumn column in ((DBTable)DBCurrentObject).Columns)
				{
					if (column.IsPrimaryKey)
					{ ##>
				_entite.Id = (Int64) ConvertirPourDotNet(LireColonne(reader, "<#= column.DBName #>"),typeof(Int64));
<#					}
					else
					{ ##>
				_entite.<#= column.CodeName #> = (<#= column.DotNetType #>) ConvertirPourDotNet(LireColonne(reader,"<#= column.DBName #>"),typeof(<#= column.DotNetType #>));
<#					} 
				}##>
			}
			catch (Exception _e)
			{
				ProcessSQLError(_e, _entite);
			}
			return _entite;
		}
		#endregion

		#region TransformerEnParametresPourCreer
		protected override void TransformToParameterForCreate(DBCommandWrapper dbCommandWrapper, BusinessObject entiteBase)
		{
			<#= GetBOClassNameUser() #> _entite = null;
			
			//Add parameters other than the key, because key is mapped by DACCRUDBase.Create at the end
			try
			{
				_entite = entiteBase as <#= GetBOClassNameUser() #>;
				
<# 				foreach(DBColumn column in ((DBTable)DBCurrentObject).Columns)
				{
					if (column.IsPrimaryKey)
					{ ##>
				dbCommandWrapper.AddOutParameter( "@Id", DbType.<#= GetDbType(column.DataType) #>, 1);
<#					}
					else
					{
						if ( column.CodeName != "ModifieLe" && column.CodeName != "ModifiePar" ) 
						{ ##>
				dbCommandWrapper.AddInParameter( "@<#= column.DBName #>", DbType.<#= GetDbType(column.DataType) #>,
													ConvertirPourDB(_entite.<#= column.CodeName #>));
<#					
						}
					}
				} ##>
			}
			catch(Exception _e)
			{
				ProcessSQLError(_e, _entite);
			}
		}
		#endregion
		
		#region TransformerEnParametresPourMAJ
		protected override void TransformToParameterForUpdate(DBCommandWrapper dbCommandWrapper, BusinessObject entiteBase)
		{
			<#= GetBOClassNameUser() #> _entite = null;
			
			try
			{
				_entite = entiteBase as <#= GetBOClassNameUser() #>;
				
<# 				foreach(DBColumn column in ((DBTable)DBCurrentObject).Columns)
				{ 
					if (column.IsPrimaryKey)
					{ ##>
				dbCommandWrapper.AddInParameter( "@Id", DbType.<#= GetDbType(column.DataType) #>, 
													ConvertirPourDB(_entite.Id));
<#					}
					else
					{ 
						if ( column.CodeName != "CreeLe" && column.CodeName != "CreePar" ) 
						{ ##>
				dbCommandWrapper.AddInParameter( "<#= column.DBName #>", DbType.<#= GetDbType(column.DataType) #>,
													ConvertirPourDB(_entite.<#= column.CodeName #>));
<#					
						}
					}
				} ##>
			}
			catch(Exception _e)
			{
				ProcessSQLError(_e, _entite);
			}
		}
		#endregion
		
		#endregion

             
	} // End of <#= GetDACClassNameBase() #> class
} // End
