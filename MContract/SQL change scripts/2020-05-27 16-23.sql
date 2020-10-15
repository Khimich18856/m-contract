USE [MContract]
GO

ALTER TABLE dbo.Offers
ADD OfferStatusId int NULL
GO

UPDATE dbo.Offers
SET OfferStatusId=2 WHERE 1=1
GO

ALTER TABLE dbo.Offers
ALTER COLUMN OfferStatusId int NOT NULL
GO