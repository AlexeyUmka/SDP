IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'DeliveryDB')
  BEGIN
    CREATE DATABASE [DeliveryDB]
  END
GO

USE [DeliveryDB]

IF NOT EXISTS ( SELECT  *
                FROM    sys.schemas
                WHERE   name = 'delivery' )
    EXEC('CREATE SCHEMA [delivery]');
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Truck' and xtype='U')
BEGIN
	CREATE TABLE [delivery].[Truck]
	(
	 [Id]                 INT NOT NULL ,
	 [Brand]              NVARCHAR(MAX) NOT NULL ,
	 [Payload]            FLOAT NOT NULL ,
	 [Volume]             FLOAT NOT NULL ,
	 [FuelConsumption]    FLOAT NOT NULL ,
	 [RegistrationPlateNumber] NVARCHAR(MAX) NOT NULL ,
	 [Year]               DATE NOT NULL , 
	)
END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TruckDriver' and xtype='U')
BEGIN
	CREATE TABLE [delivery].[TruckDriver]
	(
	 [TruckId]  INT NOT NULL ,
	 [DriverId] INT NOT NULL ,
	);

END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Driver' and xtype='U')
BEGIN
	CREATE TABLE [delivery].[Driver]
	(
	 [Id]        INT NOT NULL ,
	 [FirstName] NVARCHAR(MAX) NOT NULL ,
	 [LastName]  NVARCHAR(MAX) NOT NULL ,
	 [BirthDate] DATE NOT NULL ,
	);
END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='State' and xtype='U')
BEGIN
	CREATE TABLE [delivery].[State]
	(
	 [Id]   INT NOT NULL IDENTITY(1,1),
	 [Name] NVARCHAR(MAX) NOT NULL ,
	);
END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='City' and xtype='U')
BEGIN
	CREATE TABLE [delivery].[City]
	(
	 [Id]      INT NOT NULL,
	 [StateId] INT NOT NULL ,
	 [Name] NVARCHAR(MAX) NOT NULL ,
	);
END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Warehouse' and xtype='U')
BEGIN
	CREATE TABLE [delivery].[Warehouse]
	(
	 [Id]     INT NOT NULL ,
	 [CityId] INT NOT NULL ,
	);
END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Route' and xtype='U')
BEGIN
	CREATE TABLE [delivery].[Route]
	(
	 [OriginWarehouseId]      INT NOT NULL ,
	 [DestinationWarehouseId] INT NOT NULL ,
	 [Distance]               FLOAT NOT NULL ,
	);
END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Shipment' and xtype='U')
BEGIN
	CREATE TABLE [delivery].[Shipment]
	(
	 [Id]					  INT NOT NULL IDENTITY(1,1),
	 [OriginWarehouseId]      INT NOT NULL ,
	 [DestinationWarehouseId] INT NOT NULL ,
	 [TruckId]                INT NOT NULL ,
	 [DriverId]				  INT NOT NULL ,
	);
END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ShipmentCargo' and xtype='U')
BEGIN
	CREATE TABLE [delivery].[ShipmentCargo]
	(
	 [ShipmentId]	INT NOT NULL ,
	 [CargoId]      INT NOT NULL ,
	);
END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Cargo' and xtype='U')
BEGIN
	CREATE TABLE [delivery].[Cargo]
	(
	 [Id]                 INT NOT NULL IDENTITY(1,1),
	 [CustomerSenderId]   INT NOT NULL ,
	 [CustomerRecieverId] INT NOT NULL ,
	 [Volume]             FLOAT NOT NULL ,
	 [Weight]             FLOAT NOT NULL ,
	);
END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Customer' and xtype='U')
BEGIN
	CREATE TABLE [delivery].[Customer]
	(
	 [Id]        INT NOT NULL ,
	 [FirstName] NVARCHAR(MAX) NOT NULL ,
	 [LastName]  NVARCHAR(MAX) NOT NULL ,
	 [Phone]     NVARCHAR(50) NOT NULL ,
	);
END
GO

-- drop all constraints
DECLARE @sql NVARCHAR(MAX);
SET @sql = N'';
SELECT @sql = @sql + N'
  ALTER TABLE ' + QUOTENAME(s.name) + N'.'
  + QUOTENAME(t.name) + N' DROP CONSTRAINT '
  + QUOTENAME(c.name) + ';'
FROM sys.objects AS c
INNER JOIN sys.tables AS t
ON c.parent_object_id = t.[object_id]
INNER JOIN sys.schemas AS s 
ON t.[schema_id] = s.[schema_id]
WHERE c.[type] IN ('D','C','F','PK','UQ')
ORDER BY c.[type];
EXEC sys.sp_executesql @sql;
GO

-- add primary keys
ALTER TABLE [delivery].[Cargo] ADD CONSTRAINT [PK_Cargo] PRIMARY KEY CLUSTERED ([Id] ASC)
ALTER TABLE [delivery].[Driver] ADD CONSTRAINT [PK_Driver] PRIMARY KEY CLUSTERED ([Id] ASC)
ALTER TABLE [delivery].[Truck] ADD CONSTRAINT [PK_Truck] PRIMARY KEY CLUSTERED ([Id] ASC)
ALTER TABLE [delivery].[Route] ADD CONSTRAINT [PK_Route] PRIMARY KEY CLUSTERED ([OriginWarehouseId] ASC, [DestinationWarehouseId])
ALTER TABLE [delivery].[Shipment] ADD CONSTRAINT [PK_Shipment] PRIMARY KEY CLUSTERED ([Id] ASC)
ALTER TABLE [delivery].[State] ADD CONSTRAINT [PK_State] PRIMARY KEY CLUSTERED ([Id] ASC)
ALTER TABLE [delivery].[City] ADD CONSTRAINT [PK_City] PRIMARY KEY CLUSTERED ([Id] ASC)
ALTER TABLE [delivery].[Warehouse] ADD CONSTRAINT [PK_Warehouse] PRIMARY KEY CLUSTERED ([Id] ASC)
ALTER TABLE [delivery].[Customer] ADD CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([Id] ASC)
ALTER TABLE [delivery].[TruckDriver] ADD CONSTRAINT [PK_TruckDriver] PRIMARY KEY CLUSTERED ([TruckId] ASC, [DriverId])
ALTER TABLE [delivery].[ShipmentCargo] ADD CONSTRAINT [PK_ShipmentCargo] PRIMARY KEY CLUSTERED ([ShipmentId] ASC, [CargoId])

GO

-- add foreign keys
ALTER TABLE [delivery].[Cargo] 
ADD CONSTRAINT [FK_Cargo_CustomerSender] FOREIGN KEY ([CustomerSenderId])  REFERENCES [delivery].[Customer]([Id]),
CONSTRAINT [FK_Cargo_CustomerReciever] FOREIGN KEY ([CustomerRecieverId])  REFERENCES [delivery].[Customer]([Id]);

ALTER TABLE [delivery].[Shipment]
ADD CONSTRAINT [FK_Shipment_Truck] FOREIGN KEY ([TruckId])  REFERENCES [delivery].[Truck]([Id]),
	CONSTRAINT [FK_Shipment_Route] FOREIGN KEY ([OriginWarehouseId], [DestinationWarehouseId])  REFERENCES [delivery].[Route]([OriginWarehouseId], [DestinationWarehouseId]),
	CONSTRAINT [FK_Shipment_Driver] FOREIGN KEY ([DriverId])  REFERENCES [delivery].[Driver]([Id]);

ALTER TABLE [delivery].[Route]
ADD CONSTRAINT [FK_Route_WarehouseOrigin] FOREIGN KEY ([OriginWarehouseId])  REFERENCES [delivery].[Warehouse]([Id]),
	CONSTRAINT [FK_Route_WarehouseDestination] FOREIGN KEY ([DestinationWarehouseId])  REFERENCES [delivery].[Warehouse]([Id]);

ALTER TABLE [delivery].[Warehouse]
ADD CONSTRAINT [FK_Warehouse_City] FOREIGN KEY ([CityId])  REFERENCES [delivery].[City]([Id]);

ALTER TABLE [delivery].[City]
ADD CONSTRAINT [FK_City_State] FOREIGN KEY ([StateId])  REFERENCES [delivery].[State]([Id])

ALTER TABLE [delivery].[TruckDriver]
ADD CONSTRAINT [FK_TruckDriver_Truck] FOREIGN KEY ([TruckId])  REFERENCES [delivery].[Truck]([Id]),
    CONSTRAINT [FK_TruckDriver_Driver] FOREIGN KEY ([DriverId])  REFERENCES [delivery].[Driver]([Id])

ALTER TABLE [delivery].[ShipmentCargo]
ADD CONSTRAINT [FK_ShipmentCargo_Shipment] FOREIGN KEY ([ShipmentId])  REFERENCES [delivery].[Shipment]([Id]),
    CONSTRAINT [FK_ShipmentCargo_Cargo] FOREIGN KEY ([CargoId])  REFERENCES [delivery].[Cargo]([Id])
