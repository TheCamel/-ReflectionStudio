<#		
	int maxcreate = ((DBTable)DBCurrentObject).Columns.Count;
	int maxupdate = maxcreate;
		int count = 1;
		foreach(DBColumn column in ((DBTable)DBCurrentObject).Columns)
		{ 
			if ( column.CodeName == "CreeLe" ||
				column.CodeName == "CreePar" )
			{
				maxcreate--;
			}
			
			if ( column.CodeName == "ModifieLe" ||
				column.CodeName == "ModifiePar" )
			{
				maxupdate--;
			}
		}
//	maxupdate--;
		##>

----------------------------------------------------------
-- Stored procedures for the '<#= DBCurrentObject.DBName #>' table.
----------------------------------------------------------

----------------------------------------------------------
-- GET ALL 
----------------------------------------------------------

-- Drop the 'sp_<#= DBCurrentObject.DBName #>_ReadAll' procedure if it already exists.
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'sp_<#= DBCurrentObject.DBName #>_ReadAll') AND type='P')
	DROP PROCEDURE sp_<#= DBCurrentObject.DBName #>_ReadAll
GO

-- Gets all records from the 'sp_<#= DBCurrentObject.DBName #>_ReadAll' table.
CREATE PROCEDURE sp_<#= DBCurrentObject.DBName #>_ReadAll
AS
	SELECT * FROM <#= DBCurrentObject.DBName #>
GO

----------------------------------------------------------
-- READ ID
----------------------------------------------------------

-- Drop the 'sp_<#= DBCurrentObject.DBName #>_Read' procedure if it already exists.
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'sp_<#= DBCurrentObject.DBName #>_Read') AND type='P')
	DROP PROCEDURE sp_<#= DBCurrentObject.DBName #>_Read
GO

-- Gets a record from the 'DBCurrentObject.DBName' table using the primary key value.
CREATE PROCEDURE sp_<#= DBCurrentObject.DBName #>_Read
<#		foreach(DBColumn column in ((DBTable)DBCurrentObject).Columns)
		{
			if (column.IsPrimaryKey)
			{ ##>
				--@<#= column.DBName #> <#= column.CompleteDataType #>
				@Id bigint
<#			} 
		} ##>
		
AS
	SELECT * FROM <#= DBCurrentObject.DBName #> WHERE
<#		foreach(DBColumn column in ((DBTable)DBCurrentObject).Columns)
		{
			if ( column.IsPrimaryKey )
			{ ##>
				<#= column.DBName #> = @Id
<#			} 
		} ##>

GO

----------------------------------------------------------
-- CREATE/INSERT ID
----------------------------------------------------------

-- Drop the 'sp_<#= DBCurrentObject.DBName #>_Create' procedure if it already exists.
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'sp_<#= DBCurrentObject.DBName #>_Create') AND type='P')
	DROP PROCEDURE sp_<#= DBCurrentObject.DBName #>_Create
GO

-- Inserts a new record into the '<#= DBCurrentObject.DBName #>' table.
CREATE PROCEDURE sp_<#= DBCurrentObject.DBName #>_Create
<#		count = 1;
		foreach(DBColumn column in ((DBTable)DBCurrentObject).Columns)
		{ 
			if ( !column.IsPrimaryKey )
			{
				if ( column.CodeName != "ModifieLe" && column.CodeName != "ModifiePar" )
				{ ##>
<#			if( count >= maxcreate )
			{  ##>
				@<#= column.DBName #> <#= column.CompleteDataType #>
<#			}
			else
			{ ##>
				@<#= column.DBName #> <#= column.CompleteDataType #>,
<#			}
				}
			}
			else
			{ ##>
<#			if( count >= maxcreate )
			{  ##>
				@Id <#= column.CompleteDataType #> OUT
<#			}
			else
			{ ##>
				@Id <#= column.CompleteDataType #> OUT,
<#			}
			}
			count++;
		} ##>
AS
	INSERT INTO <#= DBCurrentObject.DBName #>
	(
<#		count = 1;
		foreach(DBColumn column in ((DBTable)DBCurrentObject).Columns)
		{ 
			if ( !column.IsPrimaryKey && column.CodeName != "ModifieLe" && column.CodeName != "ModifiePar" )
			{ ##>
<#			if( count >= maxcreate )
			{  ##>
				<#= column.DBName #>
<#			}
			else
			{ ##>
				<#= column.DBName #> ,
<#			}
			}
			count++;
		} ##>
	)
	VALUES
	(
<#		count = 1;
		foreach(DBColumn column in ((DBTable)DBCurrentObject).Columns)
		{ 
			if ( !column.IsPrimaryKey && column.CodeName != "ModifieLe" && column.CodeName != "ModifiePar" )
			{ ##>
<#			if( count >= maxcreate )
			{  ##>
				@<#= column.DBName #>
<#			}
			else
			{ ##>
				@<#= column.DBName #> ,
<#			}
			}
			count++;
		} ##>
	)

	SELECT @Id = @@IDENTITY
GO

----------------------------------------------------------
-- UPDATE ID
----------------------------------------------------------

-- Drop the 'sp_<#= DBCurrentObject.DBName #>_Update' procedure if it already exists.
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'sp_<#= DBCurrentObject.DBName #>_Update') AND type='P')
	DROP PROCEDURE sp_<#= DBCurrentObject.DBName #>_Update
GO

-- Updates a record in the '<#= DBCurrentObject.DBName #>' table.
CREATE PROCEDURE sp_<#= DBCurrentObject.DBName #>_Update
<#		count = 1;
		foreach(DBColumn column in ((DBTable)DBCurrentObject).Columns)
		{
			if ( !column.IsPrimaryKey )
			{ 
			if ( column.CodeName != "CreeLe" && column.CodeName != "CreePar" )
			{ ##>
<#				if( count >= maxupdate )
				{  ##>
				@<#= column.DBName #> <#= column.CompleteDataType #>
<#				}
				else
				{ ##>
				@<#= column.DBName #> <#= column.CompleteDataType #> ,
<#				}
			}
			else { count--; }
			} else
			{ ##>
<#				if( count >= maxupdate )
				{  ##>
				@Id <#= column.CompleteDataType #>
<#				}
				else
				{ ##>
				@Id <#= column.CompleteDataType #> ,
<#				}
			}
			count++;
		} ##>
AS
	UPDATE <#= DBCurrentObject.DBName #> SET
<#		count = 1;
		foreach(DBColumn column in ((DBTable)DBCurrentObject).Columns)
		{ 
			if ( !column.IsPrimaryKey )
			{
				if( column.CodeName != "CreeLe" && column.CodeName != "CreePar" )
				{ ##>
<#					if( count >= maxupdate )
					{  ##>
				<#= column.DBName #> = @<#= column.DBName #>
<#					}
					else
					{ ##>
				<#= column.DBName #> = @<#= column.DBName #> ,
<#					}
				}
				else { count--; }
			}
			count++;
		} ##>
	WHERE
<#		foreach(DBColumn column in ((DBTable)DBCurrentObject).Columns)
		{
			if ( column.IsPrimaryKey )
			{ ##>
				<#= column.DBName #> = @Id
<#			} 
		} ##>
GO


----------------------------------------------------------
-- DELETE ID
----------------------------------------------------------

-- Drop the 'sp_<#= DBCurrentObject.DBName #>_Delete' procedure if it already exists.
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'sp_<#= DBCurrentObject.DBName #>_Delete') AND type='P')
	DROP PROCEDURE sp_<#= DBCurrentObject.DBName #>_Delete
GO

-- Deletes a record from the '<#= DBCurrentObject.DBName #>' table using the primary key value.
CREATE PROCEDURE sp_<#= DBCurrentObject.DBName #>_Delete
<#		foreach(DBColumn column in ((DBTable)DBCurrentObject).Columns)
		{
			if (column.IsPrimaryKey)
			{ ##>
				@Id <#= column.CompleteDataType #>
<#			} 
		} ##>
AS
	DELETE FROM <#= DBCurrentObject.DBName #> WHERE
<#		foreach(DBColumn column in ((DBTable)DBCurrentObject).Columns)
		{
			if ( column.IsPrimaryKey )
			{ ##>
				<#= column.DBName #> = @Id
<#			} 
		} ##>
GO