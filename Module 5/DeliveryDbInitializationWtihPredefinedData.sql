use DeliveryDB

-- Delete from all upcoming tables to be edited
DELETE FROM delivery.TruckDriver
DELETE FROM delivery.ShipmentCargo
DELETE FROM delivery.Shipment
DELETE FROM delivery.Route
DELETE FROM delivery.Truck
DELETE FROM delivery.Cargo
DELETE FROM delivery.Customer
DELETE FROM delivery.Driver
DELETE FROM delivery.Warehouse
DELETE FROM delivery.City
DELETE FROM delivery.State

-- Reset identity counters
DBCC CHECKIDENT ('delivery.Cargo', RESEED, 1)
DBCC CHECKIDENT ('delivery.Shipment', RESEED, 1)
DBCC CHECKIDENT ('delivery.State', RESEED, 1)

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

-- Customer
INSERT INTO delivery.Customer
VALUES 
(0, 'Sarah ', 'Barthel', '1000-993-986-979'),
(1, 'Josh', 'Carter', '972-965-958-951')


-- Route
INSERT INTO delivery.Route
SELECT OriginWarehouse.Id, DestinationWarehouse.Id, FLOOR(RAND(CHECKSUM(NEWID()))*(1000-100)+100)
FROM
delivery.Warehouse OriginWarehouse INNER JOIN delivery.Warehouse DestinationWarehouse ON OriginWarehouse.Id != DestinationWarehouse.Id

-- Cargo
INSERT INTO delivery.Cargo
SELECT TOP(10000) 
0, 1, RAND(CHECKSUM(NEWID()))*(1000-10)+10, RAND(CHECKSUM(NEWID()))*(12000-100)+20
from delivery.Route

-- Shipment
DECLARE @TruckMinId INT = (SELECT TOP 1 Id FROM delivery.Truck);
DECLARE @TruckMaxId INT = (SELECT TOP 1 Id FROM delivery.Truck ORDER BY Id DESC);
DECLARE @DriverMinId INT = (SELECT TOP 1 Id FROM delivery.Driver);
DECLARE @DriverMaxId INT = (SELECT TOP 1 Id FROM delivery.Driver ORDER BY Id DESC);
DECLARE @OriginMinId INT = (SELECT TOP 1 OriginWarehouseId FROM delivery.Route);
DECLARE @OriginMaxId INT = (SELECT TOP 1 OriginWarehouseId FROM delivery.Route ORDER BY OriginWarehouseId DESC);
DECLARE @DestinationMinId INT = (SELECT TOP 1 DestinationWarehouseId FROM delivery.Route);
DECLARE @DestinationMaxId INT = (SELECT TOP 1 DestinationWarehouseId FROM delivery.Route ORDER BY DestinationWarehouseId DESC);
INSERT INTO delivery.Shipment
SELECT TOP (1000)
W.OriginId,
W.DestinationId,
TD.TruckId,
TD.DriverId
FROM delivery.Route
CROSS APPLY (SELECT
FLOOR(RAND(CHECKSUM(NEWID()))*(@OriginMaxId-@OriginMinId+1))+@OriginMinId AS OriginId,
FLOOR(RAND(CHECKSUM(NEWID()))*(@DestinationMaxId-@DestinationMinId+1))+@DestinationMinId AS DestinationId
) W, delivery.TruckDriver TD
WHERE W.Originid != W.DestinationId
ORDER BY NEWID()

-- ShipmentCargo
INSERT INTO delivery.ShipmentCargo
SELECT Sh.Id, Cg.Id FROM delivery.Shipment Sh INNER JOIN delivery.Cargo Cg ON Cg.Id = Sh.Id