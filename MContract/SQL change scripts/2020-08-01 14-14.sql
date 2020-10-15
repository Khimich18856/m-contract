USE [MContract]
GO

ALTER TABLE dbo.Ads
ADD ShowInDealsHistory bit NULL
GO

UPDATE dbo.Ads
SET ShowInDealsHistory=1 WHERE 1=1
GO

ALTER TABLE dbo.Ads
ALTER COLUMN ShowInDealsHistory bit NOT NULL
GO

ALTER TABLE dbo.Offers
ADD ShowInDealsHistory bit NULL
GO

UPDATE dbo.Offers
SET ShowInDealsHistory=1 WHERE 1=1
GO

ALTER TABLE dbo.Offers
ALTER COLUMN ShowInDealsHistory bit NOT NULL
GO