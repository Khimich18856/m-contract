USE [MContract]
GO

ALTER TABLE dbo.Messages
ADD IsLoadedToClient bit NULL
GO

UPDATE dbo.Messages
SET IsLoadedToClient=1
GO

ALTER TABLE dbo.Messages
ALTER COLUMN IsLoadedToClient bit NOT NULL
GO