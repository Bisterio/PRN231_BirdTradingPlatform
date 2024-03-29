USE [BirdTradingPlatform]
GO
/****** Object:  Table [dbo].[category]    Script Date: 7/3/2023 2:24:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[category](
	[category_id] [bigint] IDENTITY(1,1) NOT NULL,
	[description] [nvarchar](255) NOT NULL,
	[name] [nvarchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[category_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[invoice]    Script Date: 7/3/2023 2:24:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[invoice](
	[invoice_id] [bigint] IDENTITY(1,1) NOT NULL,
	[address] [nvarchar](255) NOT NULL,
	[created_at] [datetime2](6) NULL,
	[email] [nvarchar](255) NULL,
	[is_paid] [tinyint] NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[note] [nvarchar](255) NULL,
	[payment_method] [nvarchar](255) NOT NULL,
	[phone] [nvarchar](25) NULL,
	[total_amount] [decimal](10, 2) NOT NULL,
	[total_amount_pre_shipping] [decimal](10, 2) NOT NULL,
	[total_item] [int] NOT NULL,
	[total_shipping_cost] [decimal](10, 2) NOT NULL,
	[updated_at] [datetime2](6) NULL,
	[user_id] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[invoice_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[order]    Script Date: 7/3/2023 2:24:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[order](
	[order_id] [bigint] IDENTITY(1,1) NOT NULL,
	[created_at] [datetime2](6) NULL,
	[status] [tinyint] NOT NULL,
	[total_amount] [decimal](10, 2) NOT NULL,
	[total_amount_pre_shipping] [decimal](10, 2) NOT NULL,
	[total_item] [int] NOT NULL,
	[total_shipping_cost] [decimal](10, 2) NOT NULL,
	[updated_at] [datetime2](6) NULL,
	[invoice_id] [bigint] NULL,
	[store_id] [bigint] NULL,
	[cancel_reason] [nvarchar](2500) NULL,
	[is_reported] [tinyint] NULL DEFAULT ((0)),
	[refund_duration] [datetime2](6) NULL,
	[refund_reason] [nvarchar](2500) NULL,
	[reported_reason] [nvarchar](2500) NULL,
	[delivered_at] [datetime2](6) NULL,
PRIMARY KEY CLUSTERED 
(
	[order_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[order_item]    Script Date: 7/3/2023 2:24:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[order_item](
	[order_id] [bigint] NOT NULL,
	[product_id] [bigint] NOT NULL,
	[price] [decimal](10, 2) NOT NULL,
	[quantity] [int] NOT NULL,
	[total] [decimal](10, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[order_id] ASC,
	[product_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[product]    Script Date: 7/3/2023 2:24:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[product](
	[product_id] [bigint] IDENTITY(1,1) NOT NULL,
	[created_at] [datetime2](6) NULL,
	[description] [nvarchar](2500) NULL,
	[image] [nvarchar](1024) NULL,
	[name] [nvarchar](255) NOT NULL,
	[status] [tinyint] NOT NULL,
	[stock] [int] NOT NULL,
	[unit_price] [decimal](10, 2) NOT NULL,
	[updated_at] [datetime2](6) NULL,
	[category_id] [bigint] NULL,
	[store_id] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[product_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[store]    Script Date: 7/3/2023 2:24:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[store](
	[store_id] [bigint] IDENTITY(1,1) NOT NULL,
	[address] [nvarchar](255) NOT NULL,
	[cover_image] [nvarchar](1024) NULL,
	[created_at] [datetime2](6) NULL,
	[description] [nvarchar](2500) NULL,
	[logo_image] [nvarchar](1024) NULL,
	[name] [nvarchar](255) NOT NULL,
	[status] [tinyint] NOT NULL,
	[updated_at] [datetime2](6) NULL,
	[user_id] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[store_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[user_account]    Script Date: 7/3/2023 2:24:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_account](
	[user_id] [bigint] IDENTITY(1,1) NOT NULL,
	[created_at] [datetime2](6) NULL,
	[email] [nvarchar](255) NOT NULL,
	[email_verified] [tinyint] NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[password] [nvarchar](255) NULL,
	[phone] [nvarchar](25) NULL,
	[role] [nvarchar](25) NOT NULL,
	[status] [tinyint] NOT NULL,
	[updated_at] [datetime2](6) NULL,
	[store_id] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[invoice]  WITH CHECK ADD  CONSTRAINT [FKfcufjjl5u3n2x82s1ftw01k9f] FOREIGN KEY([user_id])
REFERENCES [dbo].[user_account] ([user_id])
GO
ALTER TABLE [dbo].[invoice] CHECK CONSTRAINT [FKfcufjjl5u3n2x82s1ftw01k9f]
GO
ALTER TABLE [dbo].[order]  WITH CHECK ADD  CONSTRAINT [FK51wrye1n93a8f1j7rxq7cu370] FOREIGN KEY([invoice_id])
REFERENCES [dbo].[invoice] ([invoice_id])
GO
ALTER TABLE [dbo].[order] CHECK CONSTRAINT [FK51wrye1n93a8f1j7rxq7cu370]
GO
ALTER TABLE [dbo].[order]  WITH CHECK ADD  CONSTRAINT [FK90lxxrxlt4chf273vcm9pi8ak] FOREIGN KEY([store_id])
REFERENCES [dbo].[store] ([store_id])
GO
ALTER TABLE [dbo].[order] CHECK CONSTRAINT [FK90lxxrxlt4chf273vcm9pi8ak]
GO
ALTER TABLE [dbo].[order_item]  WITH CHECK ADD  CONSTRAINT [FK551losx9j75ss5d6bfsqvijna] FOREIGN KEY([product_id])
REFERENCES [dbo].[product] ([product_id])
GO
ALTER TABLE [dbo].[order_item] CHECK CONSTRAINT [FK551losx9j75ss5d6bfsqvijna]
GO
ALTER TABLE [dbo].[order_item]  WITH CHECK ADD  CONSTRAINT [FKs234mi6jususbx4b37k44cipy] FOREIGN KEY([order_id])
REFERENCES [dbo].[order] ([order_id])
GO
ALTER TABLE [dbo].[order_item] CHECK CONSTRAINT [FKs234mi6jususbx4b37k44cipy]
GO
ALTER TABLE [dbo].[product]  WITH CHECK ADD  CONSTRAINT [FK1mtsbur82frn64de7balymq9s] FOREIGN KEY([category_id])
REFERENCES [dbo].[category] ([category_id])
GO
ALTER TABLE [dbo].[product] CHECK CONSTRAINT [FK1mtsbur82frn64de7balymq9s]
GO
ALTER TABLE [dbo].[product]  WITH CHECK ADD  CONSTRAINT [FKjlfidudl1gwqem0flrlomvlcl] FOREIGN KEY([store_id])
REFERENCES [dbo].[store] ([store_id])
GO
ALTER TABLE [dbo].[product] CHECK CONSTRAINT [FKjlfidudl1gwqem0flrlomvlcl]
GO
ALTER TABLE [dbo].[store]  WITH CHECK ADD  CONSTRAINT [FKs6piet5tft2wg1tgg2rn3nux2] FOREIGN KEY([user_id])
REFERENCES [dbo].[user_account] ([user_id])
GO
ALTER TABLE [dbo].[store] CHECK CONSTRAINT [FKs6piet5tft2wg1tgg2rn3nux2]
GO
ALTER TABLE [dbo].[user_account]  WITH CHECK ADD  CONSTRAINT [FK7vqhqxt45ua0j213qal23oqd5] FOREIGN KEY([store_id])
REFERENCES [dbo].[store] ([store_id])
GO
ALTER TABLE [dbo].[user_account] CHECK CONSTRAINT [FK7vqhqxt45ua0j213qal23oqd5]
GO
