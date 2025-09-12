IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Customers] (
    [CustomerId] int NOT NULL IDENTITY,
    [CustomerType] int NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(450) NOT NULL,
    [Email] nvarchar(450) NOT NULL,
    [IsNew] bit NOT NULL,
    [HasChanges] bit NOT NULL,
    [EntityState] int NOT NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY ([CustomerId])
);
GO

CREATE TABLE [Products] (
    [ProductId] int NOT NULL IDENTITY,
    [ProductName] nvarchar(450) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [CurrentPrice] float NULL,
    [IsNew] bit NOT NULL,
    [HasChanges] bit NOT NULL,
    [EntityState] int NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([ProductId])
);
GO

CREATE TABLE [Addresses] (
    [AddressId] int NOT NULL IDENTITY,
    [CustomerId] int NOT NULL,
    [AddressLine1] nvarchar(max) NOT NULL,
    [AddressLine2] nvarchar(max) NOT NULL,
    [City] nvarchar(450) NOT NULL,
    [State] nvarchar(max) NOT NULL,
    [PostalCode] nvarchar(max) NOT NULL,
    [Country] nvarchar(max) NOT NULL,
    [AddressType] int NOT NULL,
    CONSTRAINT [PK_Addresses] PRIMARY KEY ([AddressId]),
    CONSTRAINT [FK_Addresses_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([CustomerId]) ON DELETE CASCADE
);
GO

CREATE TABLE [Orders] (
    [OrderId] int NOT NULL IDENTITY,
    [CustomerId] int NOT NULL,
    [OrderDate] datetimeoffset NULL,
    [ShippingAddressId] int NOT NULL,
    [IsNew] bit NOT NULL,
    [HasChanges] bit NOT NULL,
    [EntityState] int NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([OrderId]),
    CONSTRAINT [FK_Orders_Addresses_ShippingAddressId] FOREIGN KEY ([ShippingAddressId]) REFERENCES [Addresses] ([AddressId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Orders_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([CustomerId]) ON DELETE CASCADE
);
GO

CREATE TABLE [OrderItems] (
    [OrderItemId] int NOT NULL IDENTITY,
    [OrderId] int NOT NULL,
    [ProductId] int NOT NULL,
    [PurchasePrice] float NULL,
    [Quantity] int NOT NULL,
    CONSTRAINT [PK_OrderItems] PRIMARY KEY ([OrderItemId]),
    CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([OrderId]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([ProductId]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_Addresses_City] ON [Addresses] ([City]);
GO

CREATE INDEX [IX_Addresses_CustomerId] ON [Addresses] ([CustomerId]);
GO

CREATE INDEX [IX_Customers_Email] ON [Customers] ([Email]);
GO

CREATE INDEX [IX_Customers_LastName] ON [Customers] ([LastName]);
GO

CREATE INDEX [IX_OrderItems_OrderId] ON [OrderItems] ([OrderId]);
GO

CREATE INDEX [IX_OrderItems_ProductId] ON [OrderItems] ([ProductId]);
GO

CREATE INDEX [IX_Orders_CustomerId] ON [Orders] ([CustomerId]);
GO

CREATE INDEX [IX_Orders_OrderDate] ON [Orders] ([OrderDate]);
GO

CREATE INDEX [IX_Orders_ShippingAddressId] ON [Orders] ([ShippingAddressId]);
GO

CREATE INDEX [IX_Products_ProductName] ON [Products] ([ProductName]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250911185624_InitialCreate', N'8.0.8');
GO

COMMIT;
GO

