USE [MMTShop]
GO

create table dbo.tbl_Store_Items
(
	Id int primary key not null identity(1,1),
	Item_SKU int unique not null,
	Item_Name nvarchar(250) not null,
	Item_Description nvarchar(max) not null,
	Item_Price smallmoney not null
);