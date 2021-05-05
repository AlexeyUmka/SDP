USE DeliveryDB

GO
ALTER PROCEDURE sp_executesqlVariant @FieldName NVARCHAR(100), @FieldValue NVARCHAR(100)
AS
IF @FieldName NOT IN('FirstName', 'LastName', 'BirthDate')
	THROW 50403, 'Passed field is not allowed', 1;  
DECLARE @sql AS NVARCHAR(500) = N'SELECT FirstName, LastName, BirthDate FROM delivery.Driver WHERE ' + @FieldName + ' LIKE ''%' + @FieldValue + '%'''
EXEC sp_executesql @sql
GO
EXEC sp_executesqlVariant @FieldName = 'FirstName', @FieldValue = 'John'

GO
ALTER PROCEDURE executeVariant @FieldName NVARCHAR(100), @FieldValue NVARCHAR(100)
AS
IF @FieldName NOT IN('FirstName', 'LastName', 'BirthDate')
THROW 50403, 'Passed field is not allowed', 1;  
DECLARE @sql AS NVARCHAR(500) = 'SELECT FirstName, LastName, BirthDate FROM delivery.Driver WHERE ' + @FieldName + ' LIKE ''%' + @FieldValue + '%'''
EXEC (@sql)
GO
EXEC executeVariant @FieldName = 'FirstName', @FieldValue = 'John'