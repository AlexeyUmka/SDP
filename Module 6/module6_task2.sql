INSERT INTO delivery.Truck(Id, Brand, RegistrationPlateNumber, Payload, FuelConsumption, Volume, Year)
VALUES(16, 'MAN', '1ABC237', 17500, 20, 95, GETDATE());

-- first approach
WITH TruckDuplicateInfo AS
(SELECT ROW_NUMBER() OVER (
                    PARTITION BY RegistrationPlateNumber 
                    ORDER BY Id
                    ) DuplicateNumber
FROM delivery.Truck)


DELETE 
FROM TruckDuplicateInfo
WHERE TruckDuplicateInfo.DuplicateNumber > 1;

-- second approach
WITH TruckDuplicateInfo AS
(SELECT RANK() OVER (
                    PARTITION BY RegistrationPlateNumber 
                    ORDER BY Id
                    ) Rank
FROM delivery.Truck)


DELETE 
FROM TruckDuplicateInfo
WHERE TruckDuplicateInfo.Rank > 1;
