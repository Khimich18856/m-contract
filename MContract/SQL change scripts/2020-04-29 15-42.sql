USE [MContract]
GO

ALTER TABLE dbo.Offers
ADD ActiveUntilDate datetime NULL
GO

UPDATE dbo.Offers
SET ActiveUntilDate = CURRENT_TIMESTAMP
GO

ALTER TABLE dbo.Offers
ALTER COLUMN ActiveUntilDate datetime NOT NULL
GO
