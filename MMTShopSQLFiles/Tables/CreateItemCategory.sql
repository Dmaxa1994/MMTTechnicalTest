USE [MMTShop]
GO

create table dbo.tbl_Item_Category
(
	Id int primary key not null identity(1,1),
	Category_Name nvarchar(250) unique not null,
	Category_Filter nvarchar(15) unique not null
);