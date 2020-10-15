USE [MContract]
GO

ALTER TABLE dbo.Messages
ADD RecipientId int NULL
GO

UPDATE dbo.Messages
SET RecipientId = d.RecipientId
FROM dbo.Messages as m
JOIN dbo.Dialogs as d
ON m.DialogId = d.Id and m.SenderId = d.SenderId
GO

UPDATE dbo.Messages
SET RecipientId = d.SenderId
FROM dbo.Messages as m
JOIN dbo.Dialogs as d
ON m.DialogId = d.Id and m.SenderId = d.RecipientId
GO

ALTER TABLE dbo.Messages
ALTER COLUMN RecipientId int NOT NULL
GO

ALTER TABLE dbo.Messages
DROP COLUMN DialogId
GO