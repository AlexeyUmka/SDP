USE DeliveryDB
DELETE FROM delivery.Route WHERE OriginWarehouseId = 2 AND DestinationWarehouseId = 9;
SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;
BEGIN TRANSACTION Connection1;
	SELECT *
	FROM delivery.Route 
	WHERE OriginWarehouseId = 2 AND DestinationWarehouseId = 7;
COMMIT TRANSACTION;
BEGIN TRANSACTION Connection2;
	UPDATE delivery.Route
	SET DestinationWarehouseId = 9
	WHERE OriginWarehouseId = 2 AND DestinationWarehouseId = 7;
COMMIT TRANSACTION;