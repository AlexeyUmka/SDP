USE DeliveryDB
DELETE FROM delivery.Route WHERE OriginWarehouseId = 5 AND DestinationWarehouseId = 7;
/*Run this part as a separate request to test paralel work of two operations and to test that isolation level was chosen properly*/
SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;
BEGIN TRANSACTION Connection1;
	SELECT *
	FROM delivery.Route 
	WHERE OriginWarehouseId = 5 AND DestinationWarehouseId = 6
	WAITFOR DELAY '00:00:10'
COMMIT TRANSACTION;
/*End of the part*/
USE DeliveryDB
BEGIN TRANSACTION Connection2;
	UPDATE delivery.Route
	SET DestinationWarehouseId = 7
	WHERE OriginWarehouseId = 5 AND DestinationWarehouseId = 6;
COMMIT TRANSACTION;