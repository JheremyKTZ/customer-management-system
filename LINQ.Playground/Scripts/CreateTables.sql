BEGIN
    CREATE TABLE [dbo].[Addresses] (
        [Id]           INT            NOT NULL,
        [AddressLine1] NVARCHAR (100) NOT NULL,
        [AddressLine2] NVARCHAR (100) NULL,
        [City]         VARCHAR (100)  NOT NULL,
        [State]        VARCHAR (50)   NOT NULL,
        [PostalCode]   VARCHAR (50)   NOT NULL,
        [Country]      VARCHAR (50)   NOT NULL,
        [Type]         INT            NOT NULL,
        CONSTRAINT [PK_Addresses] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    CREATE TABLE [dbo].[Products] (
        [Id]           INT             NOT NULL,
        [Name]         VARCHAR (200)   NOT NULL,
        [Description]  VARCHAR (300)   NOT NULL,
        [CurrentPrice] DECIMAL (16, 2) NULL,
        CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    CREATE TABLE [dbo].[Customers] (
        [Id]        INT            NOT NULL,
        [Type]      INT            NOT NULL,
        [FirstName] NVARCHAR (100) NULL,
        [LastName]  NVARCHAR (100) NULL,
        [Email]     NVARCHAR (200) NULL,
        CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    CREATE TABLE [dbo].[Orders] (
        [Id]                INT      NOT NULL,
        [CustomerId]        INT      NOT NULL,
        [Date]              DATETIME NULL,
        [ShippingAddressId] INT      NOT NULL,
        CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Orders_ToCustomers] FOREIGN KEY ([CustomerId]) REFERENCES [Customers]([Id]), 
        CONSTRAINT [FK_Orders_ToAddresses] FOREIGN KEY ([ShippingAddressId]) REFERENCES [Addresses]([Id])
    );
    CREATE TABLE [dbo].[OrderItems]
    (
	    [Id] INT NOT NULL , 
        [ProductId] INT NOT NULL, 
        [PurchasePrice] DECIMAL(16, 2) NOT NULL, 
        [Quantity] INT NOT NULL, 
        [OrderId] INT NOT NULL,
	    CONSTRAINT [PK_OrderItems] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_OrderItems_ToOrders] FOREIGN KEY ([OrderId]) REFERENCES [Orders]([Id]), 
        CONSTRAINT [FK_OrderItems_ToProducts] FOREIGN KEY ([ProductId]) REFERENCES [Products]([Id])
    );
END
GO