BEGIN
	PRINT('Creating Products Table');
		CREATE TABLE [dbo].[Products](
			[Id] [int] NOT NULL,
			[Name] [varchar](200) NOT NULL,
			[Description] [varchar](300) NOT NULL,
			[CurrentPrice] [decimal](16, 2) NULL,
		 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
		(
			[Id] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY];

	PRINT('Creating Customers Table');
	CREATE TABLE [dbo].[Customers](
		[Id] [int] NOT NULL,
		[Type] [int] NOT NULL,
		[FirstName] [nvarchar](100) NULL,
		[LastName] [nvarchar](100) NULL,
		[Email] [nvarchar](200) NULL,
	 CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY];

	PRINT('Creating Addresses Table');
	CREATE TABLE [dbo].[Addresses](
		[Id] [int] NOT NULL,
		[AddressLine1] [nvarchar](100) NOT NULL,
		[AddressLine2] [nvarchar](100) NULL,
		[City] [varchar](100) NOT NULL,
		[State] [varchar](50) NOT NULL,
		[PostalCode] [varchar](50) NOT NULL,
		[Country] [varchar](50) NOT NULL,
		[Type] [int] NOT NULL,
		[CustomerId] [int] NOT NULL,
	 CONSTRAINT [PK_Addresses] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY];

	ALTER TABLE [dbo].[Addresses]  WITH NOCHECK ADD  CONSTRAINT [FK_Addresses_ToCustomers] FOREIGN KEY([CustomerId])
	REFERENCES [dbo].[Customers] ([Id])
	ALTER TABLE [dbo].[OrderItems] CHECK CONSTRAINT [FK_OrderItems_ToOrders]

	PRINT('Creating [Orders] Table');
	CREATE TABLE [dbo].[Orders](
	[Id] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[Date] [datetime] NULL,
	[ShippingAddressId] [int] NOT NULL,
	 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
	ALTER TABLE [dbo].[Orders]  WITH NOCHECK ADD  CONSTRAINT [FK_Orders_ToAddresses] FOREIGN KEY([ShippingAddressId])
	REFERENCES [dbo].[Addresses] ([Id]);
	ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_ToAddresses];
	ALTER TABLE [dbo].[Orders]  WITH NOCHECK ADD  CONSTRAINT [FK_Orders_ToCustomers] FOREIGN KEY([CustomerId])
	REFERENCES [dbo].[Customers] ([Id]);
	ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_ToCustomers];

	PRINT('Creating OrderItems Table');
	CREATE TABLE [dbo].[OrderItems](
		[Id] [int] NOT NULL,
		[ProductId] [int] NOT NULL,
		[PurchasePrice] [decimal](16, 2) NOT NULL,
		[Quantity] [int] NOT NULL,
		[OrderId] [int] NOT NULL,
	 CONSTRAINT [PK_OrderItems] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY];

	ALTER TABLE [dbo].[OrderItems]  WITH NOCHECK ADD  CONSTRAINT [FK_OrderItems_ToOrders] FOREIGN KEY([OrderId])
	REFERENCES [dbo].[Orders] ([Id])
	ALTER TABLE [dbo].[OrderItems] CHECK CONSTRAINT [FK_OrderItems_ToOrders]
	ALTER TABLE [dbo].[OrderItems]  WITH NOCHECK ADD  CONSTRAINT [FK_OrderItems_ToProducts] FOREIGN KEY([ProductId])
	REFERENCES [dbo].[Products] ([Id])
	ALTER TABLE [dbo].[OrderItems] CHECK CONSTRAINT [FK_OrderItems_ToProducts]



	---- Remove Information from Tables
	--DELETE * FROM [dbo].[Products]
	--DELETE * FROM [dbo].[Customers]
	--DELETE * FROM [dbo].[Addresses]
	--DELETE * FROM [dbo].[Orders]
	--DELETE * FROM [dbo].[OrderItems]
END
GO
