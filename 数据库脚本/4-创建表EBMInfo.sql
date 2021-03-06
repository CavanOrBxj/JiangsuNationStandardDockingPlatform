USE [volador]
GO
/****** 对象:  Table [dbo].[EBMInfo]    脚本日期: 01/10/2019 09:54:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EBMInfo](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[EBDVersion] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[SEBDID] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[SEDBType] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[SEBRID] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[EBRID] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[SEBBuidTime] [datetime] NULL,
	[EBMID] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[MsaType] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[SenderName] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[SenderCode] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[SendTime] [datetime] NULL,
	[EventType] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[Severity] [varchar](10) COLLATE Chinese_PRC_CI_AS NULL,
	[StartTime] [datetime] NULL,
	[EndTime] [datetime] NULL,
	[LanguageCode] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[MsgTitle] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[msgDesc] [varchar](max) COLLATE Chinese_PRC_CI_AS NULL,
	[AreaCode] [varchar](500) COLLATE Chinese_PRC_CI_AS NULL,
	[AuxiliaryType] [int] NULL,
	[AuxiliaryDesc] [varchar](500) COLLATE Chinese_PRC_CI_AS NULL,
	[Size] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[EBMState] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[TsCmdStoreID] [int] NULL,
 CONSTRAINT [PK__EBMInfo__6E01572D] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF