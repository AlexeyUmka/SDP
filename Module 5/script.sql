IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'DeliveryDB')
  BEGIN
    CREATE DATABASE [DeliveryDB]
  END
GO

USE [DeliveryDB]

IF NOT EXISTS ( SELECT  *
                FROM    sys.schemas
                WHERE   name = 'Delivery' )
    EXEC('CREATE SCHEMA [Delivery]');
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Truck' and xtype='U')
BEGIN
	CREATE TABLE [Delivery].[Truck]
	(
	 [Id]                 int NOT NULL ,
	 [Brand]              nvarchar(max) NOT NULL ,
	 [Payload]            float NOT NULL ,
	 [Volume]             float NOT NULL ,
	 [FuelConsumption]    float NOT NULL ,
	 [RegistrationNumber] nvarchar(max) NOT NULL ,
	 [Year]               date NOT NULL , 
	)
END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TruckDriver' and xtype='U')
BEGIN
	CREATE TABLE [Delivery].[TruckDriver]
	(
	 [TruckId]  int NOT NULL ,
	 [DriverId] int NOT NULL ,
	);

END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Driver' and xtype='U')
BEGIN
	CREATE TABLE [Delivery].[Driver]
	(
	 [Id]        int NOT NULL ,
	 [FirstName] nvarchar(max) NOT NULL ,
	 [LastName]  nvarchar(max) NOT NULL ,
	 [Birthdate] date NOT NULL ,
	);
END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='State' and xtype='U')
BEGIN
	CREATE TABLE [Delivery].[State]
	(
	 [Id]   int NOT NULL ,
	 [Name] nvarchar(max) NOT NULL ,
	);
END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='City' and xtype='U')
BEGIN
	CREATE TABLE [Delivery].[City]
	(
	 [Id]      int NOT NULL ,
	 [StateId] int NOT NULL ,
	 [Name] nvarchar(max) NOT NULL ,
	);
END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Warehouse' and xtype='U')
BEGIN
	CREATE TABLE [Delivery].[Warehouse]
	(
	 [Id]     int NOT NULL ,
	 [CityId] int NOT NULL ,
	);
END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Route' and xtype='U')
BEGIN
	CREATE TABLE [Delivery].[Route]
	(
	 [Id]          int            NOT NULL ,
	 [OriginWarehouseId]      int NOT NULL ,
	 [DestinationWarehouseId] int NOT NULL ,
	 [Distance]               float NOT NULL ,
	);
END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Shipment' and xtype='U')
BEGIN
	CREATE TABLE [Delivery].[Shipment]
	(
	 [Id]      int NOT NULL ,
	 [RouteId] int  NOT NULL ,
	 [TruckId] int NOT NULL ,
	 [CargoId] int NOT NULL ,
	 [Price]   float NOT NULL ,
	);
END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Cargo' and xtype='U')
BEGIN
	CREATE TABLE [Delivery].[Cargo]
	(
	 [Id]                 int NOT NULL ,
	 [CustomerSenderId]   int NOT NULL ,
	 [CustomerRecieverId] int NOT NULL ,
	 [Volume]             float NOT NULL ,
	 [Weight]             float NOT NULL ,
	);
END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Customer' and xtype='U')
BEGIN
	CREATE TABLE [Delivery].[Customer]
	(
	 [Id]        int NOT NULL ,
	 [FirstName] nvarchar(max) NOT NULL ,
	 [LastName]  nvarchar(max) NOT NULL ,
	 [Phone]     nvarchar(50) NOT NULL ,
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
ALTER TABLE [Delivery].[Cargo] ADD CONSTRAINT [PK_Cargo] PRIMARY KEY CLUSTERED ([Id] ASC)
ALTER TABLE [Delivery].[Driver] ADD CONSTRAINT [PK_Driver] PRIMARY KEY CLUSTERED ([Id] ASC)
ALTER TABLE [Delivery].[Truck] ADD CONSTRAINT [PK_Truck] PRIMARY KEY CLUSTERED ([Id] ASC)
ALTER TABLE [Delivery].[Route] ADD CONSTRAINT [PK_Route] PRIMARY KEY CLUSTERED ([Id] ASC)
ALTER TABLE [Delivery].[Shipment] ADD CONSTRAINT [PK_Shipment] PRIMARY KEY CLUSTERED ([Id] ASC)
ALTER TABLE [Delivery].[State] ADD CONSTRAINT [PK_State] PRIMARY KEY CLUSTERED ([Id] ASC)
ALTER TABLE [Delivery].[City] ADD CONSTRAINT [PK_City] PRIMARY KEY CLUSTERED ([Id] ASC)
ALTER TABLE [Delivery].[Warehouse] ADD CONSTRAINT [PK_Warehouse] PRIMARY KEY CLUSTERED ([Id] ASC)
ALTER TABLE [Delivery].[Customer] ADD CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([Id] ASC)
ALTER TABLE [Delivery].[TruckDriver] ADD CONSTRAINT [PK_TruckDriver] PRIMARY KEY CLUSTERED ([TruckId] ASC, [DriverId])

-- add foreign keys
ALTER TABLE [Delivery].[Cargo] 
ADD CONSTRAINT [FK_Cargo_CustomerSender] FOREIGN KEY ([CustomerSenderId])  REFERENCES [Delivery].[Customer]([Id]),
CONSTRAINT [FK_Cargo_CustomerReciever] FOREIGN KEY ([CustomerRecieverId])  REFERENCES [Delivery].[Customer]([Id]);

ALTER TABLE [Delivery].[Shipment]
ADD CONSTRAINT [FK_Shipment_Truck] FOREIGN KEY ([TruckId])  REFERENCES [Delivery].[Truck]([Id]),
	CONSTRAINT [FK_Shipment_Cargo] FOREIGN KEY ([CargoId])  REFERENCES [Delivery].[Cargo]([Id]),
	CONSTRAINT [FK_Shipment_Route] FOREIGN KEY ([RouteId])  REFERENCES [Delivery].[Route]([Id]);

ALTER TABLE [Delivery].[Route]
ADD CONSTRAINT [FK_Route_WarehouseOrigin] FOREIGN KEY ([OriginWarehouseId])  REFERENCES [Delivery].[Warehouse]([Id]),
	CONSTRAINT [FK_Route_WarehouseDestination] FOREIGN KEY ([DestinationWarehouseId])  REFERENCES [Delivery].[Warehouse]([Id]);

ALTER TABLE [Delivery].[Warehouse]
ADD CONSTRAINT [FK_Warehouse_City] FOREIGN KEY ([CityId])  REFERENCES [Delivery].[City]([Id]);

ALTER TABLE [Delivery].[City]
ADD CONSTRAINT [FK_City_State] FOREIGN KEY ([StateId])  REFERENCES [Delivery].[State]([Id])

ALTER TABLE [Delivery].[TruckDriver]
ADD CONSTRAINT [FK_TruckDriver_Truck] FOREIGN KEY ([TruckId])  REFERENCES [Delivery].[Truck]([Id]),
    CONSTRAINT [FK_TruckDriver_Driver] FOREIGN KEY ([DriverId])  REFERENCES [Delivery].[Driver]([Id])