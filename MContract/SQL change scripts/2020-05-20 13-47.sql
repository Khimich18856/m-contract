USE [MContract]
GO

ALTER TABLE dbo.Offers
ADD ModerateResultId int NULL
GO

UPDATE dbo.Offers
SET ModerateResultId=1 WHERE 1=1
GO

ALTER TABLE dbo.Offers
ALTER COLUMN ModerateResultId int NOT NULL
GO