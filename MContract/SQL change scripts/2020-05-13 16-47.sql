USE [MContract]
GO

ALTER TABLE dbo.Ads
ADD ModerateResultId int NULL
GO

UPDATE dbo.Ads
SET ModerateResultId = 1
WHERE 1=1
GO

ALTER TABLE dbo.Ads
ALTER COLUMN ModerateResultId int NOT NULL