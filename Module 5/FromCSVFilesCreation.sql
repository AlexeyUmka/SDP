use DeliveryDB

-- disable all constraints
EXEC sp_MSforeachtable "ALTER TABLE ? NOCHECK CONSTRAINT all"

--Drivers
DELETE FROM delivery.Driver

BULK INSERT delivery.Driver
FROM 'C:\Users\Oleksii_Yurchenko\RiderProjects\SDPTasks\Module 5\Drivers.csv'
WITH (FIRSTROW = 2, 
	  MAXERRORS = 0,
	  CODEPAGE = '1251',
	  FIELDTERMINATOR = ',')


--Trucks
DELETE FROM delivery.Truck

CREATE TABLE #TrucksCSV(Id int, Brand nvarchar(max), RegNumber nvarchar(max), year date, payload float, fuelConsumption float, volumeCargo float)

BULK INSERT #TrucksCSV
FROM 'C:\Users\Oleksii_Yurchenko\RiderProjects\SDPTasks\Module 5\Trucks.csv'
WITH (FIRSTROW = 2, 
	  MAXERRORS = 0,
	  CODEPAGE = '1251',
	  FIELDTERMINATOR = ',')

INSERT INTO delivery.Truck
SELECT T.Id, T.Brand, T.payload, T.volumeCargo, T.fuelConsumption, T.RegNumber, T.year from #TrucksCSV T

DROP TABLE #TrucksCSV

--TruckDriver
DELETE FROM delivery.TruckDriver

BULK INSERT delivery.TruckDriver
FROM 'C:\Users\Oleksii_Yurchenko\RiderProjects\SDPTasks\Module 5\DriversTrucks.csv'
WITH (FIRSTROW = 2, 
	  MAXERRORS = 0,
	  CODEPAGE = '1251',
	  FIELDTERMINATOR = ',')

--Warehouses
CREATE TABLE #WarehousesCSV(Id int, City nvarchar(50), State nvarchar(50))

BULK INSERT #WarehousesCSV
FROM 'C:\Users\Oleksii_Yurchenko\RiderProjects\SDPTasks\Module 5\Warehouse.csv'
WITH (FIRSTROW = 2, 
	  MAXERRORS = 0,
	  CODEPAGE = '1251',
	  FIELDTERMINATOR = ',')

DELETE FROM delivery.City
DELETE FROM delivery.State
DELETE FROM delivery.Warehouse

DECLARE @ElementName nvarchar(max);
DECLARE @Index int;

--States
DECLARE ElementCursor CURSOR 
  LOCAL STATIC READ_ONLY FORWARD_ONLY
FOR 
SELECT DISTINCT State
FROM #WarehousesCSV

SET @Index = 0

OPEN ElementCursor
FETCH NEXT FROM ElementCursor INTO @ElementName
WHILE @@FETCH_STATUS = 0
BEGIN 
	INSERT INTO delivery.State VALUES(@Index, @ElementName)
    FETCH NEXT FROM ElementCursor INTO @ElementName
	SET @Index = @Index + 1
END

CLOSE ElementCursor
DEALLOCATE ElementCursor

--Cities
DECLARE @StateName nvarchar(max);
DECLARE ElementCursor CURSOR 
  LOCAL STATIC READ_ONLY FORWARD_ONLY
FOR 
SELECT DISTINCT City, State
FROM #WarehousesCSV

SET @Index = 0

OPEN ElementCursor
FETCH NEXT FROM ElementCursor INTO @ElementName, @StateName
WHILE @@FETCH_STATUS = 0
BEGIN 
	INSERT INTO delivery.City VALUES(@Index, (SELECT Id from delivery.State where Name = @StateName), @ElementName)
    FETCH NEXT FROM ElementCursor INTO @ElementName, @StateName
	SET @Index = @Index + 1
END

CLOSE ElementCursor
DEALLOCATE ElementCursor

--Warehouses
Declare @CityName nvarchar(max);
DECLARE ElementCursor CURSOR 
  LOCAL STATIC READ_ONLY FORWARD_ONLY
FOR 
SELECT Id, City
FROM #WarehousesCSV

OPEN ElementCursor
FETCH NEXT FROM ElementCursor INTO @Index, @CityName
WHILE @@FETCH_STATUS = 0
BEGIN 
	INSERT INTO delivery.Warehouse VALUES(@Index, (SELECT Id from delivery.City where Name = @CityName))
    FETCH NEXT FROM ElementCursor INTO @Index, @CityName
END

CLOSE ElementCursor
DEALLOCATE ElementCursor


DROP TABLE #WarehousesCSV

-- enable all constraints
exec sp_MSforeachtable @command1="ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all"