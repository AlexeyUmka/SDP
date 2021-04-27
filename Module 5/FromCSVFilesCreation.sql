use DeliveryDB

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
INSERT INTO delivery.State
SELECT DISTINCT State
FROM #WarehousesCSV

--Cities
INSERT INTO delivery.City
SELECT Wr.Id, st.Id, Wr.City
FROM #WarehousesCSV Wr INNER JOIN delivery.State st ON st.Name=Wr.State

--Warehouses
INSERT INTO delivery.Warehouse
SELECT Wr.Id, Ct.Id
FROM #WarehousesCSV Wr INNER JOIN delivery.City Ct ON Ct.Id=Wr.Id

DROP TABLE #WarehousesCSV