USE [MContract]
GO

/****** Object:  Table [dbo].[Offers]    Script Date: 25.04.2020 18:46:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE [dbo].[Offers]
CREATE TABLE [dbo].[Offers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SenderId] [int] NOT NULL,
	[AdId] [int] NOT NULL,
	[ProductsIdAndPricePerWeight] [nchar](1000) NOT NULL,
	[DateOfPosting] [datetime] NOT NULL,
	[CityId] [int] NOT NULL,
	[DeliveryTypeId] [int] NOT NULL,
	[DeliveryAddress] [nchar](500) NULL,
	[DeliveryLoadTypeId] [int] NOT NULL,
	[DeliveryWayId] [int] NOT NULL,
	[TermsOfPaymentsId] [int] NOT NULL,
	[DefermentPeriod] [int] NULL,
	[Nds] [int] NOT NULL,
 CONSTRAINT [PK_Offers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


