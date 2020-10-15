USE [MContract]
GO

ALTER TABLE dbo.Users
ADD OpenDialogRespondentIds nvarchar(50) NULL
GO

UPDATE dbo.Users
SET OpenDialogRespondentIds = '' WHERE 1=1
GO

ALTER TABLE dbo.Users
ALTER COLUMN OpenDialogRespondentIds nvarchar(50) NOT NULL
GO