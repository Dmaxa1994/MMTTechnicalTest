USE [MMTShop]
GO

create table dbo.tbl_Featured_Items
(
	Id int primary key not null identity(1,1),
	Item_Category_ID unique int not null
);