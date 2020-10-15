USE [MContract]
GO

ALTER TABLE dbo.Messages
DROP COLUMN Text
GO

ALTER TABLE dbo.Messages
DROP COLUMN Date
GO

ALTER TABLE dbo.Messages
ADD Created datetime NULL
GO

UPDATE dbo.Messages
SET Created = 0
WHERE 1=1

ALTER TABLE dbo.Messages
ALTER COLUMN Created datetime NOT NULL
GO

ALTER TABLE dbo.Messages
ADD Text nchar(500) NULL
GO

UPDATE dbo.Messages
SET Text = 0
WHERE 1=1

ALTER TABLE dbo.Messages
ALTER COLUMN Text nchar(500) NOT NULL
GO