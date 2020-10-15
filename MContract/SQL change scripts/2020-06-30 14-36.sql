USE [MContract]
GO

ALTER TABLE dbo.Messages
ADD RequestJoinAdFromUserId int NULL
GO

UPDATE dbo.Messages SET RequestJoinAdFromUserId = 0
GO

ALTER TABLE dbo.Messages
ALTER COLUMN RequestJoinAdFromUserId int NOT NULL
GO