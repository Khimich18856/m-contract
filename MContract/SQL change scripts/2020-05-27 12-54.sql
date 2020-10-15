USE [MContract]
GO

ALTER TABLE dbo.Messages
ADD AdId int NULL
GO

UPDATE dbo.Messages
SET AdId=0 WHERE 1=1
GO

ALTER TABLE dbo.Messages
ALTER COLUMN AdId int NOT NULL
GO