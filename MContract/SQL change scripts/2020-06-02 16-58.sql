USE [MContract]
GO

ALTER TABLE dbo.Offers
ADD ContractStatusId int NULL
GO

UPDATE dbo.Offers
SET ContractStatusId=0 WHERE 1=1
GO

ALTER TABLE dbo.Offers
ALTER COLUMN ContractStatusId int NOT NULL
GO