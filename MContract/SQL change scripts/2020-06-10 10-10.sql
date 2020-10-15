USE [MContract]
GO

ALTER TABLE dbo.Users
ADD ModerateResultId int NULL
GO

UPDATE dbo.Users
SET ModerateResultId = 0
WHERE 1=1
GO

ALTER TABLE dbo.Users
ALTER COLUMN ModerateResultId int NOT NULL
GO
