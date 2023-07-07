BEGIN
	ALTER TABLE dbo.[OrderItems] NOCHECK CONSTRAINT [FK_OrderItems_ToProducts];
	ALTER TABLE dbo.[OrderItems] NOCHECK CONSTRAINT [FK_OrderItems_ToOrders];
	ALTER TABLE dbo.[Orders] NOCHECK CONSTRAINT [FK_Orders_ToCustomers];
	ALTER TABLE dbo.[Orders] NOCHECK CONSTRAINT [FK_Orders_ToAddresses];

	TRUNCATE TABLE [dbo].[OrderItems];
	TRUNCATE TABLE [dbo].[Orders];
	TRUNCATE TABLE [dbo].[Products];
	TRUNCATE TABLE [dbo].[Customers];
	TRUNCATE TABLE [dbo].[Addresses];

	ALTER TABLE dbo.[OrderItems] CHECK CONSTRAINT [FK_OrderItems_ToProducts];
	ALTER TABLE dbo.[OrderItems] CHECK CONSTRAINT [FK_OrderItems_ToOrders];
	ALTER TABLE dbo.[Orders] CHECK CONSTRAINT [FK_Orders_ToCustomers];
	ALTER TABLE dbo.[Orders] CHECK CONSTRAINT [FK_Orders_ToAddresses];
END
GO
