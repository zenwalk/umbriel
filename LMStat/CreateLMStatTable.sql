USE [working]
GO

/****** Object:  Table [dbo].[LMStat]    Script Date: 11/13/2009 13:20:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LMStat](
	[MachineName] [nvarchar](50) NOT NULL,
	[LicenseName] [nvarchar](30) NOT NULL,
	[TotalLicense] [int] NOT NULL,
	[InUseLicense] [int] NOT NULL,
	[StatusDate] [datetime] NOT NULL,
	[DateCreated] [datetime] NOT NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[LMStat] ADD  CONSTRAINT [DF_LMStats_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO

