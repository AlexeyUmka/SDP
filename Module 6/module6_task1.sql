USE DeliveryDB

GO
ALTER PROCEDURE sp_executesqlVariant @FieldName NVARCHAR(100), @FieldValue NVARCHAR(100)
AS
IF @FieldName NOT IN('FirstName', 'LastName', 'BirthDate')
	THROW 50403, 'Passed field is not allowed', 1;  
DECLARE @sql AS NVARCHAR(500) = N'SELECT FirstName, LastName, BirthDate FROM delivery.Driver WHERE ' + @FieldName + '=@FValue'
EXEC sp_executesql @sql, N'@FValue AS NVARCHAR(100)', @FValue = @FieldValue
GO
EXEC sp_executesqlVariant @FieldName = 'FirstName', @FieldValue = 'John'

GO
ALTER PROCEDURE executeVariant @FieldName NVARCHAR(100), @FieldValue NVARCHAR(100)
AS
IF @FieldName NOT IN('FirstName', 'LastName', 'BirthDate')
THROW 50403, 'Passed field is not allowed', 1;  
DECLARE @sql AS NVARCHAR(500) = 'SELECT FirstName, LastName, BirthDate FROM delivery.Driver WHERE ' + @FieldName + '=' + char(39) + @FieldValue + char(39)
EXEC (@sql)
GO
EXEC executeVariant @FieldName = 'FirstName', @FieldValue = 'John'