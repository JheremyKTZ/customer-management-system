BEGIN
	PRINT('Removing data from Products Table');
	DELETE FROM [dbo].[Products]

	PRINT('Removing data from Customers Table');
	DELETE FROM [dbo].[Customers]

	PRINT('Removing data from Addresses Table');
	DELETE FROM [dbo].[Addresses]

	PRINT('Removing data from Orders Table');
	DELETE FROM [dbo].[Orders]

	PRINT('Removing data from OrderItems Table');
	DELETE FROM [dbo].[OrderItems]
END
GO
