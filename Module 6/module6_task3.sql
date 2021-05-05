USE DeliveryDB

GO
ALTER PROCEDURE sp_executesqlVariant @xml XML
AS
BEGIN
    DECLARE @sqlCommand NVARCHAR(1000);
    DECLARE @idoc INT;
    EXEC sp_xml_preparedocument @idoc OUTPUT, @xml;

   SET @sqlCommand = 'SELECT *
    FROM delivery.Driver 
    WHERE ' + 
    (SELECT STRING_AGG(CONCAT(Name, '=', QUOTENAME(Value, '''')), ' OR ')
    FROM OPENXML(@idoc, '/Dictionary/NameValue', 2)
    WITH (Name VARCHAR(MAX) 'Name',
          Value VARCHAR(MAX) 'Value')) + ';';

  EXEC(@sqlCommand);
END
GO
EXEC sp_executesqlVariant @xml = 
'
<Dictionary>
	<NameValue>
		<Name>FirstName</Name>
		<Value>John</Value>
	</NameValue>
	<NameValue>
		<Name>FirstName</Name>
		<Value>Adam</Value>
	</NameValue>
</Dictionary>
'

GO
ALTER PROCEDURE executeVariant  @xml XML
AS
BEGIN
    DECLARE @sqlCommand NVARCHAR(1000);
    DECLARE @idoc INT;
    EXEC sp_xml_preparedocument @idoc OUTPUT, @xml;

   SET @sqlCommand = 'SELECT *
    FROM delivery.Driver 
    WHERE ' + 
    (SELECT STRING_AGG(CONCAT(Name, '=', QUOTENAME(Value, '''')), ' OR ')
    FROM OPENXML(@idoc, '/Dictionary/NameValue', 2)
    WITH (Name VARCHAR(MAX) 'Name',
          Value VARCHAR(MAX) 'Value')) + ';';

    EXEC sp_executesql
    @stmt=@sqlCommand,
    @params = N'@xml XML',
    @xml=@xml;
END
GO
EXEC executeVariant @xml = 
'
<Dictionary>
	<NameValue>
		<Name>FirstName</Name>
		<Value>John</Value>
	</NameValue>
	<NameValue>
		<Name>FirstName</Name>
		<Value>Adam</Value>
	</NameValue>
</Dictionary>
'
