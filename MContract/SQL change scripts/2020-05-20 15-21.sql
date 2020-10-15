USE [MContract]
GO

ALTER TABLE dbo.Messages
ADD IsReviewContractNotification bit NULL
GO

UPDATE dbo.Messages
SET IsReviewContractNotification=0 WHERE 1=1
GO

ALTER TABLE dbo.Messages
ALTER COLUMN IsReviewContractNotification bit NOT NULL
GO

ALTER TABLE dbo.Messages
ADD IsContractReviewed bit NULL
GO

UPDATE dbo.Messages
SET IsContractReviewed=0 WHERE 1=1
GO

ALTER TABLE dbo.Messages
ALTER COLUMN IsContractReviewed bit NOT NULL
GO

ALTER TABLE dbo.Messages
ADD OfferId int NULL
GO

UPDATE dbo.Messages
SET OfferId=0 WHERE 1=1
GO

ALTER TABLE dbo.Messages
ALTER COLUMN OfferId int NOT NULL
GO