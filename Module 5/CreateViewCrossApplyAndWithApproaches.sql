USE DeliveryDB
-- fastest approach
IF OBJECT_ID('ShipmentInfoCrossApply') IS NOT NULL
BEGIN
    DROP VIEW ShipmentInfoCrossApply
END
GO
CREATE VIEW ShipmentInfoCrossApply AS
SELECT 
OriginCity.Name AS 'Origin City',
DestinationCity.Name AS 'Destination City',
Truck.Brand AS 'Truck brand',
CargoTotals.TotalWeight AS 'Total Weight',
CargoTotals.TotalVolume AS 'Total Volume',
Route.Distance * Truck.FuelConsumption / 100 AS 'Total Fuel'
FROM 
delivery.Shipment Shipment INNER JOIN delivery.Route Route ON Shipment.OriginWarehouseId = Route.OriginWarehouseId AND Shipment.DestinationWarehouseId = Route.DestinationWarehouseId
INNER JOIN delivery.Warehouse OriginWarehouse ON OriginWarehouse.Id = Route.OriginWarehouseId
INNER JOIN delivery.Warehouse DestinationWarehouse ON DestinationWarehouse.Id = Route.DestinationWarehouseId
INNER JOIN delivery.City OriginCity ON OriginCity.Id = OriginWarehouse.Id
INNER JOIN delivery.City DestinationCity ON DestinationCity.Id = DestinationWarehouse.Id
INNER JOIN delivery.Truck Truck ON Shipment.TruckId = Truck.Id
CROSS APPLY (SELECT SUM(Cargo.Weight) TotalWeight, SUM(Cargo.Volume) TotalVolume FROM delivery.Cargo Cargo
INNER JOIN delivery.ShipmentCargo ShipmentCargo ON ShipmentCargo.ShipmentId = Shipment.Id AND ShipmentCargo.CargoId = Cargo.Id) CargoTotals
GO
SELECT * FROM ShipmentInfoCrossApply
-- A bit slower
IF OBJECT_ID('ShipmentInfoWith') IS NOT NULL
BEGIN
    DROP VIEW ShipmentInfoWith
END
GO
CREATE VIEW ShipmentInfoWith
  AS
  WITH CargoInfo(TotalWeight, TotalVolume, ShipmentId) as 
  (SELECT 
        SUM(Cargo.Weight) AS TotalWeight,
        SUM(Cargo.Volume) AS TotalVolume,
		Shipment.Id
      FROM
        delivery.Cargo Cargo
		JOIN delivery.ShipmentCargo SC ON Cargo.Id = SC.CargoId
		JOIN delivery.Shipment Shipment ON Shipment.Id = SC.ShipmentId
    GROUP BY Shipment.Id)
  SELECT
    OriginCity.Name AS 'Origin City',
    DestinationCity.Name AS 'Destination City',
    Truck.Brand AS 'Truck Brand',
    CargoInfo.TotalWeight AS 'Total Weight',
    CargoInfo.TotalVolume AS 'Total Volume',
    RouteInfo.Distance * Truck.FuelConsumption / 100 AS 'Total Fuel'
  FROM delivery.Shipment Shipment
	INNER JOIN delivery.Warehouse OriginWarehouse ON OriginWarehouse.Id = Shipment.OriginWarehouseId
	INNER JOIN delivery.Warehouse DestinationWarehouse ON DestinationWarehouse.Id = Shipment.DestinationWarehouseId
	INNER JOIN delivery.City OriginCity ON OriginCity.Id = OriginWarehouse.Id
	INNER JOIN delivery.City DestinationCity ON DestinationCity.Id = DestinationWarehouse.Id
	INNER JOIN delivery.Route RouteInfo ON RouteInfo.OriginWarehouseId = OriginWarehouse.Id AND RouteInfo.DestinationWarehouseId = DestinationWarehouse.Id
	INNER JOIN delivery.Truck Truck ON Shipment.TruckId = Truck.Id
	JOIN CargoInfo ON CargoInfo.ShipmentId = Shipment.Id;
GO
SELECT * FROM ShipmentInfoWith