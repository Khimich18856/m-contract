USE [MContract]
GO

/****** Object:  Table [dbo].[UserRatings]    Script Date: 04.08.2020 17:23:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserRatings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ToUserId] [int] NOT NULL,
	[FromUserId] [int] NOT NULL,
	[AdId] [int] NULL,
	[Rating] [int] NOT NULL,
	[Created] [datetime] NOT NULL

 CONSTRAINT [PK_UserRatings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


