CREATE PROCEDURE DeleteWarehouse @Id INT
AS
DELETE FROM delivery.ShipmentCargo WHERE EXISTS (SELECT * FROM delivery.Shipment WHERE OriginWarehouseId = @Id OR DestinationWarehouseId = @Id)
DELETE FROM delivery.Shipment WHERE OriginWarehouseId = @Id OR DestinationWarehouseId = @Id
DELETE FROM delivery.Route WHERE OriginWarehouseId = @Id OR DestinationWarehouseId = @Id
DELETE FROM delivery.Warehouse WHERE Id = @Id
GO
BEGIN TRANSACTION;
    EXEC DeleteWarehouse @Id = 8
    SAVE TRANSACTION SavePoint16;
	EXEC DeleteWarehouse @Id = 16
	SAVE TRANSACTION SavePoint24;
	EXEC DeleteWarehouse @Id = 24
    ROLLBACK TRANSACTION SavePoint24;
	ROLLBACK TRANSACTION SavePoint16;
COMMIT TRANSACTION;