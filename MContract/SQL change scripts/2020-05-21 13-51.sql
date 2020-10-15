USE [MContract]
GO

DELETE FROM dbo.Photos
WHERE 1=1
GO

ALTER TABLE dbo.Photos
ADD GroupId uniqueidentifier NOT NULL
GO