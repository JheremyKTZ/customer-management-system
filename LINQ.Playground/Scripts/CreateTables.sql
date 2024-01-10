BEGIN
    PRINT('Creating Products Table');
    CREATE TABLE [dbo].[Products] (
        [Id]           INT             NOT NULL,
        [Name]         VARCHAR (200)   NOT NULL,
        [Description]  VARCHAR (300)   NOT NULL,
        [CurrentPrice] DECIMAL (16, 2) NULL,
        CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([Id] ASC) 
            WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY];

    PRINT('Creating Customers Table');
    CREATE TABLE [dbo].[Customers] (
        [Id]        INT            NOT NULL,
        [Type]      INT            NOT NULL,
        [FirstName] NVARCHAR (100) NULL,
        [LastName]  NVARCHAR (100) NULL,
        [Email]     NVARCHAR (200) NULL,
        CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED ([Id] ASC) 
            WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY];

    PRINT('Creating Addresses Table');
    CREATE TABLE [dbo].[Addresses] (
        [Id]           INT            NOT NULL,
        [AddressLine1] NVARCHAR (100) NOT NULL,
        [AddressLine2] NVARCHAR (100) NULL,
        [City]         VARCHAR (100)  NOT NULL,
        [State]        VARCHAR (50)   NOT NULL,
        [PostalCode]   VARCHAR (50)   NOT NULL,
        [Country]      VARCHAR (50)   NOT NULL,
        [Type]         INT            NOT NULL,
        [CustomerId]   INT            NOT NULL,
        CONSTRAINT [PK_Addresses] PRIMARY KEY CLUSTERED ([Id] ASC)
            WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY];

    ALTER TABLE [dbo].[Addresses]  WITH NOCHECK ADD  CONSTRAINT [FK_Addresses_ToCustomers] FOREIGN KEY([CustomerId])
	REFERENCES [dbo].[Customers] ([Id])
    
    PRINT('Creating [Orders] Table');
    CREATE TABLE [dbo].[Orders] (
        [Id]                INT      NOT NULL,
        [CustomerId]        INT      NOT NULL,
        [Date]              DATETIME NULL,
        [ShippingAddressId] INT      NOT NULL,
        CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED ([Id] ASC) 
            WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
        CONSTRAINT [FK_Orders_ToCustomers] FOREIGN KEY ([CustomerId]) REFERENCES [Customers]([Id]), 
        CONSTRAINT [FK_Orders_ToAddresses] FOREIGN KEY ([ShippingAddressId]) REFERENCES [Addresses]([Id])
    );


    PRINT('Creating OrderItems Table');
    CREATE TABLE [dbo].[OrderItems]
    (
	    [Id] INT NOT NULL , 
        [ProductId] INT NOT NULL, 
        [PurchasePrice] DECIMAL(16, 2) NOT NULL, 
        [Quantity] INT NOT NULL, 
        [OrderId] INT NOT NULL,
	    CONSTRAINT [PK_OrderItems] PRIMARY KEY CLUSTERED ([Id] ASC) 
            WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
        CONSTRAINT [FK_OrderItems_ToOrders] FOREIGN KEY ([OrderId]) REFERENCES [Orders]([Id]), 
        CONSTRAINT [FK_OrderItems_ToProducts] FOREIGN KEY ([ProductId]) REFERENCES [Products]([Id])
    );
END
GO