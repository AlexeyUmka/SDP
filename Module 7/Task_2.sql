USE DeliveryDB
DELETE FROM delivery.Route WHERE OriginWarehouseId = 2 AND DestinationWarehouseId = 9;
/*Run this part as a separate request to test paralel work of two operations and to test that isolation level was chosen properly*/
SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;
BEGIN TRANSACTION Connection1;
	SELECT *
	FROM delivery.Route 
	WHERE OriginWarehouseId = 2 AND DestinationWarehouseId = 7;
COMMIT TRANSACTION;
/*End of the part*/
BEGIN TRANSACTION Connection2;
	UPDATE delivery.Route
	SET DestinationWarehouseId = 9
	WHERE OriginWarehouseId = 2 AND DestinationWarehouseId = 7;
COMMIT TRANSACTION;