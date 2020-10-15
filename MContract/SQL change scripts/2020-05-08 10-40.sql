USE [MContract]
GO

ALTER TABLE [dbo].[Photos]
ADD UserId int NULL
GO

ALTER TABLE [dbo].[Photos]
ALTER COLUMN AdId int NULL
GO

ALTER TABLE [dbo].[Photos]
ADD PhotoTypeId int NOT NULL
GO