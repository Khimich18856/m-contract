USE [MContract]
GO

ALTER TABLE dbo.Users
ALTER COLUMN CompanyName nvarchar(200) NOT NULL
GO

ALTER TABLE dbo.Users
ALTER COLUMN Email nvarchar(200) NOT NULL
GO

ALTER TABLE dbo.Users
ALTER COLUMN Password nvarchar(200) NOT NULL
GO

ALTER TABLE dbo.Users
ADD FactualAddress nvarchar(500) NULL