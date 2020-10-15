USE [MContract]
GO

ALTER TABLE dbo.Messages
ADD UserId int NULL
GO

UPDATE dbo.Messages
SET UserId = 0 WHERE 1=1
GO

ALTER TABLE dbo.Messages
ALTER COLUMN UserId int NOT NULL
GO
