USE [$DBNAME]

/****** Object:  Table [dbo].[calc_autonode]    Script Date: 05.12.2013 15:31:18 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[calc_autonode](
	[task_id] [int] NOT NULL,
	[node_id] [int] NOT NULL,
 CONSTRAINT [PK_calc_autonode] PRIMARY KEY CLUSTERED 
(
	[task_id] ASC,
	[node_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


/****** Object:  Table [dbo].[calc_autotask]    Script Date: 05.12.2013 15:31:18 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[calc_autotask](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](256) NOT NULL,
	[schedule_id] [int] NOT NULL,
	[start_time] [datetime] NULL,
 CONSTRAINT [PK_calc_autotask] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


/****** Object:  Table [dbo].[calc_consts]    Script Date: 05.12.2013 15:31:18 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[calc_consts](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](256) NULL,
	[description] [nvarchar](1024) NULL,
	[value] [nvarchar](256) NULL,
 CONSTRAINT [PK_calc_consts] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


/****** Object:  Table [dbo].[calc_function]    Script Date: 05.12.2013 15:31:18 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[calc_function](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](256) NULL,
	[description] [nvarchar](1024) NULL,
	[args] [nvarchar](1024) NULL,
	[text] [nvarchar](max) NULL,
	[group_name] [nvarchar](256) NULL,
 CONSTRAINT [PK_calc_function] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


/****** Object:  Table [dbo].[child_params]    Script Date: 05.12.2013 15:31:18 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[child_params](
	[idnum] [int] IDENTITY(1,1) NOT NULL,
	[parentId] [int] NOT NULL,
	[paramId] [int] NOT NULL,
	[sortIndex] [int] NOT NULL,
	[paramCode] [varchar](100) NULL,
	[username] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[idnum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


SET ANSI_PADDING OFF

/****** Object:  Table [dbo].[child_props]    Script Date: 05.12.2013 15:31:18 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[child_props](
	[idnum] [int] IDENTITY(1,1) NOT NULL,
	[parentid] [int] NOT NULL,
	[name] [varchar](100) NOT NULL,
	[value] [varchar](max) NULL,
 CONSTRAINT [PK_child_props] PRIMARY KEY CLUSTERED 
(
	[idnum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


SET ANSI_PADDING OFF

/****** Object:  Table [dbo].[db_info]    Script Date: 05.12.2013 15:31:18 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[db_info](
	[db_version] [nchar](10) NULL
) ON [PRIMARY]


/****** Object:  Table [dbo].[groups]    Script Date: 05.12.2013 15:31:18 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[groups](
	[idnum] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NOT NULL,
	[description] [varchar](126) NULL,
 CONSTRAINT [PK_groups] PRIMARY KEY CLUSTERED 
(
	[idnum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


SET ANSI_PADDING OFF

/****** Object:  Table [dbo].[intervals]    Script Date: 05.12.2013 15:31:18 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[intervals](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[standard] [bit] NOT NULL,
	[header] [nvarchar](256) NOT NULL,
	[interval] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_intervals] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


/****** Object:  Table [dbo].[lobs]    Script Date: 05.12.2013 15:31:18 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[lobs](
	[idnum] [int] IDENTITY(1,1) NOT NULL,
	[parentid] [int] NOT NULL,
	[name] [varchar](100) NOT NULL,
	[value] [image] NULL,
	[revision] [int] NULL,
 CONSTRAINT [PK_lobs] PRIMARY KEY CLUSTERED 
(
	[idnum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


SET ANSI_PADDING OFF

/****** Object:  Table [dbo].[props]    Script Date: 05.12.2013 15:31:18 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[props](
	[idnum] [int] IDENTITY(1,1) NOT NULL,
	[parentid] [int] NOT NULL,
	[name] [varchar](100) NOT NULL,
	[value] [nvarchar](max) NULL,
	[revision] [int] NULL,
 CONSTRAINT [PK_props] PRIMARY KEY CLUSTERED 
(
	[idnum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


SET ANSI_PADDING OFF

/****** Object:  Table [dbo].[reports]    Script Date: 05.12.2013 15:31:18 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[reports](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[reportId] [int] NOT NULL,
	[userId] [int] NOT NULL,
	[date] [datetime] NOT NULL,
	[dateFrom] [datetime] NULL,
	[dateTo] [datetime] NULL,
	[report] [image] NOT NULL,
	[props] [varbinary](max) NULL,
 CONSTRAINT [PK_reports] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


/****** Object:  Table [dbo].[revision]    Script Date: 05.12.2013 15:31:18 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[revision](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[time_from] [datetime] NOT NULL,
	[brief] [nvarchar](128) NOT NULL,
	[comment] [nvarchar](1024) NULL,
 CONSTRAINT [PK_revision] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


/****** Object:  Table [dbo].[schedules]    Script Date: 05.12.2013 15:31:18 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[schedules](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](256) NOT NULL,
	[rule] [varchar](1024) NOT NULL,
 CONSTRAINT [PK_schedules] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


SET ANSI_PADDING OFF

/****** Object:  Table [dbo].[sprparam]    Script Date: 05.12.2013 15:31:18 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[sprparam](
	[idnum] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NULL,
	[label] [varchar](100) NULL,
	[typ] [int] NULL,
	[nazn] [varchar](254) NULL,
	[chclass] [varchar](100) NULL,
	[sortcod] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[idnum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


SET ANSI_PADDING OFF

/****** Object:  Table [dbo].[unit]    Script Date: 05.12.2013 15:31:18 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[unit](
	[idnum] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](256) NULL,
	[type] [int] NULL,
	[parent] [int] NULL,
 CONSTRAINT [PK_unit] PRIMARY KEY CLUSTERED 
(
	[idnum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


SET ANSI_PADDING OFF

/****** Object:  Table [dbo].[unit_type]    Script Date: 05.12.2013 15:31:18 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[unit_type](
	[idnum] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NULL,
	[tree_visible] [int] NOT NULL,
	[img] [image] NULL,
	[props] [varchar](max) NULL,
	[fkey] [image] NULL,
	[child_filter] [varchar](1024) NULL,
	[ext_guid] [binary](16) NULL,
 CONSTRAINT [PK_unit_type] PRIMARY KEY CLUSTERED 
(
	[idnum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


SET ANSI_PADDING OFF

/****** Object:  Table [dbo].[usergroup]    Script Date: 05.12.2013 15:31:18 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[usergroup](
	[userid] [int] NOT NULL,
	[groupid] [int] NOT NULL,
	[priv] [int] NULL,
 CONSTRAINT [PK_usergroup] PRIMARY KEY CLUSTERED 
(
	[userid] ASC,
	[groupid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


/****** Object:  Table [dbo].[users]    Script Date: 05.12.2013 15:31:18 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[users](
	[idnum] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NOT NULL,
	[pass] [varbinary](100) NULL,
	[roles] [varchar](500) NULL,
	[fullname] [nvarchar](1024) NULL,
	[position] [nvarchar](256) NULL,
 CONSTRAINT [PK__users__0BC6C43E] PRIMARY KEY CLUSTERED 
(
	[idnum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


SET ANSI_PADDING OFF

/****** Object:  Table [dbo].[value_aparam]    Script Date: 05.12.2013 15:31:18 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[value_aparam](
	[idnum] [int] IDENTITY(1,1) NOT NULL,
	[idparam] [int] NOT NULL,
	[time] [datetime] NOT NULL,
	[pack] [varbinary](max) NULL,
	[ch_time] [datetime] NOT NULL,
 CONSTRAINT [PK_value_aparam] PRIMARY KEY CLUSTERED 
(
	[idnum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


SET ANSI_PADDING OFF

/****** Object:  Table [dbo].[value_mparam]    Script Date: 05.12.2013 15:31:18 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[value_mparam](
	[idnum] [int] IDENTITY(1,1) NOT NULL,
	[idparam] [int] NOT NULL,
	[time] [datetime] NOT NULL,
	[value] [decimal](25, 7) NULL,
	[ch_time] [datetime] NULL,
	[value_corr] [decimal](25, 7) NULL,
 CONSTRAINT [PK_value_mparam] PRIMARY KEY CLUSTERED 
(
	[idnum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Index [child_idx12]    Script Date: 05.12.2013 15:34:34 ******/
CREATE NONCLUSTERED INDEX [idx1_child_params] ON [dbo].[child_params]
(
	[parentId] ASC,
	[paramId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

SET ANSI_PADDING ON


/****** Object:  Index [idx1_child_props]    Script Date: 05.12.2013 15:34:34 ******/
CREATE UNIQUE NONCLUSTERED INDEX [idx1_child_props] ON [dbo].[child_props]
(
	[parentid] ASC,
	[name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

SET ANSI_PADDING ON


/****** Object:  Index [NonClusteredIndex-20131205-152319]    Script Date: 05.12.2013 15:34:34 ******/
CREATE UNIQUE NONCLUSTERED INDEX idx1_groups ON [dbo].[groups]
(
	[name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

SET ANSI_PADDING ON


/****** Object:  Index [idx1_lobs]    Script Date: 05.12.2013 15:34:34 ******/
CREATE UNIQUE NONCLUSTERED INDEX [idx1_lobs] ON [dbo].[lobs]
(
	[parentid] ASC,
	[name] ASC,
	[revision] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

SET ANSI_PADDING ON


/****** Object:  Index [idx1_props]    Script Date: 05.12.2013 15:34:34 ******/
CREATE UNIQUE NONCLUSTERED INDEX [idx1_props] ON [dbo].[props]
(
	[parentid] ASC,
	[name] ASC,
	[revision] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

/****** Object:  Index [NonClusteredIndex-20131205-152304]    Script Date: 05.12.2013 15:34:34 ******/
CREATE UNIQUE NONCLUSTERED INDEX [idx1_users] ON [dbo].[users]
(
	[name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

/****** Object:  Index [idx1_aparam]    Script Date: 05.12.2013 15:34:34 ******/
CREATE UNIQUE NONCLUSTERED INDEX [idx1_aparam] ON [dbo].[value_aparam]
(
	[idparam] ASC,
	[time] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

/****** Object:  Index [idx1_mparam]    Script Date: 05.12.2013 15:34:34 ******/
CREATE UNIQUE NONCLUSTERED INDEX [idx1_mparam] ON [dbo].[value_mparam]
(
	[idparam] ASC,
	[time] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

ALTER TABLE [dbo].[child_params] ADD  DEFAULT ((0)) FOR [sortIndex]

ALTER TABLE [dbo].[child_params] ADD  DEFAULT ('') FOR [username]

ALTER TABLE [dbo].[unit_type] ADD  CONSTRAINT [DF_unit_type_tree_visible]  DEFAULT ((1)) FOR [tree_visible]

ALTER TABLE [dbo].[calc_autonode]  WITH CHECK ADD  CONSTRAINT [FK_calc_autonode_calc_autotask] FOREIGN KEY([task_id])
REFERENCES [dbo].[calc_autotask] ([id])

ALTER TABLE [dbo].[calc_autonode] CHECK CONSTRAINT [FK_calc_autonode_calc_autotask]

ALTER TABLE [dbo].[calc_autonode]  WITH CHECK ADD  CONSTRAINT [FK_calc_autonode_unit] FOREIGN KEY([node_id])
REFERENCES [dbo].[unit] ([idnum])
ON DELETE CASCADE

ALTER TABLE [dbo].[calc_autonode] CHECK CONSTRAINT [FK_calc_autonode_unit]

ALTER TABLE [dbo].[calc_autotask]  WITH CHECK ADD  CONSTRAINT [FK_calc_autotask_schedules] FOREIGN KEY([schedule_id])
REFERENCES [dbo].[schedules] ([id])

ALTER TABLE [dbo].[calc_autotask] CHECK CONSTRAINT [FK_calc_autotask_schedules]

ALTER TABLE [dbo].[child_params]  WITH CHECK ADD  CONSTRAINT [FK_child_params_param_unit] FOREIGN KEY([paramId])
REFERENCES [dbo].[unit] ([idnum])
ON DELETE CASCADE

ALTER TABLE [dbo].[child_params] CHECK CONSTRAINT [FK_child_params_param_unit]

ALTER TABLE [dbo].[child_params]  WITH CHECK ADD  CONSTRAINT [FK_child_params_parent_unit] FOREIGN KEY([parentId])
REFERENCES [dbo].[unit] ([idnum])

ALTER TABLE [dbo].[child_params] CHECK CONSTRAINT [FK_child_params_parent_unit]

ALTER TABLE [dbo].[child_props]  WITH CHECK ADD  CONSTRAINT [FK_child_props_child_params] FOREIGN KEY([parentid])
REFERENCES [dbo].[child_params] ([idnum])

ALTER TABLE [dbo].[child_props] CHECK CONSTRAINT [FK_child_props_child_params]

ALTER TABLE [dbo].[lobs]  WITH CHECK ADD  CONSTRAINT [FK_lobs_revision] FOREIGN KEY([revision])
REFERENCES [dbo].[revision] ([id])
ON DELETE CASCADE

ALTER TABLE [dbo].[lobs] CHECK CONSTRAINT [FK_lobs_revision]

ALTER TABLE [dbo].[lobs]  WITH CHECK ADD  CONSTRAINT [FK_lobs_unit] FOREIGN KEY([parentid])
REFERENCES [dbo].[unit] ([idnum])
ON DELETE CASCADE

ALTER TABLE [dbo].[lobs] CHECK CONSTRAINT [FK_lobs_unit]

ALTER TABLE [dbo].[props]  WITH CHECK ADD  CONSTRAINT [FK_props_revision] FOREIGN KEY([revision])
REFERENCES [dbo].[revision] ([id])
ON DELETE CASCADE

ALTER TABLE [dbo].[props] CHECK CONSTRAINT [FK_props_revision]

ALTER TABLE [dbo].[props]  WITH CHECK ADD  CONSTRAINT [FK_props_unit] FOREIGN KEY([parentid])
REFERENCES [dbo].[unit] ([idnum])
ON DELETE CASCADE

ALTER TABLE [dbo].[props] CHECK CONSTRAINT [FK_props_unit]

--ALTER TABLE [dbo].[unit]  WITH CHECK ADD  CONSTRAINT [FK_unit_parent_unit] FOREIGN KEY([parent])
--REFERENCES [dbo].[unit] ([idnum])

--ALTER TABLE [dbo].[unit] CHECK CONSTRAINT [FK_unit_parent_unit]

ALTER TABLE [dbo].[unit]  WITH CHECK ADD  CONSTRAINT [FK_unit_unit_type] FOREIGN KEY([type])
REFERENCES [dbo].[unit_type] ([idnum])
ON DELETE SET NULL

ALTER TABLE [dbo].[unit] CHECK CONSTRAINT [FK_unit_unit_type]

ALTER TABLE [dbo].[usergroup]  WITH CHECK ADD  CONSTRAINT [FK_usergroup_groups] FOREIGN KEY([groupid])
REFERENCES [dbo].[groups] ([idnum])
ON DELETE CASCADE

ALTER TABLE [dbo].[usergroup] CHECK CONSTRAINT [FK_usergroup_groups]

ALTER TABLE [dbo].[usergroup]  WITH CHECK ADD  CONSTRAINT [FK_usergroup_users] FOREIGN KEY([userid])
REFERENCES [dbo].[users] ([idnum])
ON DELETE CASCADE

ALTER TABLE [dbo].[usergroup] CHECK CONSTRAINT [FK_usergroup_users]

ALTER TABLE [dbo].[value_aparam]  WITH CHECK ADD  CONSTRAINT [FK_value_aparam_unit] FOREIGN KEY([idparam])
REFERENCES [dbo].[unit] ([idnum])
ON DELETE CASCADE

ALTER TABLE [dbo].[value_aparam] CHECK CONSTRAINT [FK_value_aparam_unit]

ALTER TABLE [dbo].[value_mparam]  WITH CHECK ADD  CONSTRAINT [FK_value_mparam_unit] FOREIGN KEY([idparam])
REFERENCES [dbo].[unit] ([idnum])
ON DELETE CASCADE

ALTER TABLE [dbo].[value_mparam] CHECK CONSTRAINT [FK_value_mparam_unit]

