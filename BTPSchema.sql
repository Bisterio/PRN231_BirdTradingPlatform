USE [BirdTradingPlatform]
GO
/****** Object:  Table [dbo].[cart_item]    Script Date: 6/19/2023 1:17:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cart_item](
	[product_id] [bigint] NOT NULL,
	[user_id] [bigint] NOT NULL,
	[price] [decimal](10, 2) NOT NULL,
	[quantity] [int] NOT NULL,
	[total] [decimal](10, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[product_id] ASC,
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[category]    Script Date: 6/19/2023 1:17:51 PM ******/
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
/****** Object:  Table [dbo].[order]    Script Date: 6/19/2023 1:17:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[order](
	[order_id] [bigint] IDENTITY(1,1) NOT NULL,
	[address] [nvarchar](255) NOT NULL,
	[created_at] [datetime2](6) NULL,
	[phone] [nvarchar](25) NULL,
	[status] [tinyint] NOT NULL,
	[total_item] [int] NOT NULL,
	[total_price] [decimal](10, 2) NOT NULL,
	[updated_at] [datetime2](6) NULL,
	[user_id] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[order_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[order_item]    Script Date: 6/19/2023 1:17:51 PM ******/
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
/****** Object:  Table [dbo].[product]    Script Date: 6/19/2023 1:17:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[product](
	[product_id] [bigint] IDENTITY(1,1) NOT NULL,
	[created_at] [datetime2](6) NULL,
	[description] [nvarchar](2500) NULL,
	[image] [nvarchar](255) NULL,
	[name] [nvarchar](255) NOT NULL,
	[status] [tinyint] NOT NULL,
	[stock] [int] NOT NULL,
	[updated_at] [datetime2](6) NULL,
	[category_id] [bigint] NULL,
	[store_id] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[product_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[store]    Script Date: 6/19/2023 1:17:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[store](
	[store_id] [bigint] IDENTITY(1,1) NOT NULL,
	[address] [nvarchar](255) NOT NULL,
	[created_at] [datetime2](6) NULL,
	[name] [nvarchar](255) NOT NULL,
	[status] [tinyint] NOT NULL,
	[updated_at] [datetime2](6) NULL,
PRIMARY KEY CLUSTERED 
(
	[store_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[transaction_record]    Script Date: 6/19/2023 1:17:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[transaction_record](
	[transaction_id] [bigint] IDENTITY(1,1) NOT NULL,
	[amount] [decimal](10, 2) NOT NULL,
	[content] [nvarchar](255) NULL,
	[created_at] [datetime2](6) NULL,
	[mode] [nvarchar](255) NOT NULL,
	[status] [tinyint] NOT NULL,
	[type] [nvarchar](255) NOT NULL,
	[updated_at] [datetime2](6) NULL,
	[order_id] [bigint] NULL,
	[product_id] [bigint] NULL,
	[store_id] [bigint] NULL,
	[user_id] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[transaction_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[user_account]    Script Date: 6/19/2023 1:17:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_account](
	[user_id] [bigint] IDENTITY(1,1) NOT NULL,
	[created_at] [datetime2](6) NULL,
	[email] [nvarchar](255) NOT NULL,
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
ALTER TABLE [dbo].[cart_item]  WITH CHECK ADD  CONSTRAINT [FKjcyd5wv4igqnw413rgxbfu4nv] FOREIGN KEY([product_id])
REFERENCES [dbo].[product] ([product_id])
GO
ALTER TABLE [dbo].[cart_item] CHECK CONSTRAINT [FKjcyd5wv4igqnw413rgxbfu4nv]
GO
ALTER TABLE [dbo].[cart_item]  WITH CHECK ADD  CONSTRAINT [FKr3njynly810ab6s4hmhnpkdkj] FOREIGN KEY([user_id])
REFERENCES [dbo].[user_account] ([user_id])
GO
ALTER TABLE [dbo].[cart_item] CHECK CONSTRAINT [FKr3njynly810ab6s4hmhnpkdkj]
GO
ALTER TABLE [dbo].[order]  WITH CHECK ADD  CONSTRAINT [FKn46c37756vcjh8mq42bv4dbfp] FOREIGN KEY([user_id])
REFERENCES [dbo].[user_account] ([user_id])
GO
ALTER TABLE [dbo].[order] CHECK CONSTRAINT [FKn46c37756vcjh8mq42bv4dbfp]
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
ALTER TABLE [dbo].[transaction_record]  WITH CHECK ADD  CONSTRAINT [FK1pj8aa5t4hvasqnvpm1bh1wvy] FOREIGN KEY([product_id])
REFERENCES [dbo].[product] ([product_id])
GO
ALTER TABLE [dbo].[transaction_record] CHECK CONSTRAINT [FK1pj8aa5t4hvasqnvpm1bh1wvy]
GO
ALTER TABLE [dbo].[transaction_record]  WITH CHECK ADD  CONSTRAINT [FK85lpu6l5y8txhr8l8qu1figfo] FOREIGN KEY([store_id])
REFERENCES [dbo].[store] ([store_id])
GO
ALTER TABLE [dbo].[transaction_record] CHECK CONSTRAINT [FK85lpu6l5y8txhr8l8qu1figfo]
GO
ALTER TABLE [dbo].[transaction_record]  WITH CHECK ADD  CONSTRAINT [FKhpkxkiawsljieien9y6l0v2bn] FOREIGN KEY([order_id])
REFERENCES [dbo].[order] ([order_id])
GO
ALTER TABLE [dbo].[transaction_record] CHECK CONSTRAINT [FKhpkxkiawsljieien9y6l0v2bn]
GO
ALTER TABLE [dbo].[transaction_record]  WITH CHECK ADD  CONSTRAINT [FKlp3uuij45qo3rdtqd983pika] FOREIGN KEY([user_id])
REFERENCES [dbo].[user_account] ([user_id])
GO
ALTER TABLE [dbo].[transaction_record] CHECK CONSTRAINT [FKlp3uuij45qo3rdtqd983pika]
GO
ALTER TABLE [dbo].[user_account]  WITH CHECK ADD  CONSTRAINT [FK7vqhqxt45ua0j213qal23oqd5] FOREIGN KEY([store_id])
REFERENCES [dbo].[store] ([store_id])
GO
ALTER TABLE [dbo].[user_account] CHECK CONSTRAINT [FK7vqhqxt45ua0j213qal23oqd5]
GO
