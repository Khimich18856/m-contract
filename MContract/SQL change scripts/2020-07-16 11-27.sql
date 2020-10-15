USE [MContract]
GO

/****** Object:  Table [dbo].[Files]    Script Date: 16.07.2020 15:39:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Files](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[MessageId] [int] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Extension] [varchar](10) NOT NULL,
	[Added] [datetime] NOT NULL,
	[Changed] [datetime] NULL,
	[ModerateResult] [int] NOT NULL,
 CONSTRAINT [PK_Files] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Files] ADD  CONSTRAINT [DF_Files_Changed]  DEFAULT ((0)) FOR [Changed]
GO


