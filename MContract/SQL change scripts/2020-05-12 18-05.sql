USE [MContract]
GO

ALTER TABLE dbo.Messages
DROP COLUMN AssociatedAdId
GO

ALTER TABLE dbo.Messages
ADD DialogId int NOT NULL
GO

ALTER TABLE dbo.Messages
DROP COLUMN RecipientId
GO

ALTER TABLE dbo.Messages
ADD IsRead bit NOT NULL
GO