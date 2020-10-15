USE [MContract]
GO

ALTER TABLE dbo.Users
ADD CurrentRespondentId int NULL
GO

UPDATE dbo.Users
SET CurrentRespondentId = 0 WHERE 1=1
GO

ALTER TABLE dbo.Users
ALTER COLUMN CurrentRespondentId int NOT NULL
GO