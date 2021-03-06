USE [$DBNAME]

INSERT [dbo].[intervals] ([standard], [header], [interval]) VALUES (1, N'<нет интервала>', N'0')

INSERT [dbo].[intervals] ([standard], [header], [interval]) VALUES (1, N'секунда', N'=1s')

INSERT [dbo].[intervals] ([standard], [header], [interval]) VALUES (1, N'минута', N'=1m')

INSERT [dbo].[intervals] ([standard], [header], [interval]) VALUES (1, N'час', N'=1h')

INSERT [dbo].[intervals] ([standard], [header], [interval]) VALUES (1, N'вахта ручной ввод', N'[UTC+04]=1d-5h-12h-7h')

INSERT [dbo].[intervals] ([standard], [header], [interval]) VALUES (1, N'вахта', N'[local]8h=12h')

INSERT [dbo].[intervals] ([standard], [header], [interval]) VALUES (0, N'сутки', N'[local]=1d')

INSERT [dbo].[intervals] ([standard], [header], [interval]) VALUES (0, N'сутки UTC', N'=1d')

INSERT [dbo].[intervals] ([standard], [header], [interval]) VALUES (1, N'сутки (Москва)', N'[UTC+04]=1d')

INSERT [dbo].[intervals] ([standard], [header], [interval]) VALUES (0, N'10 дней', N'[local]=1M-10d-10d')

INSERT [dbo].[intervals] ([standard], [header], [interval]) VALUES (1, N'10 дней (Москва)', N'[UTC+04]=1M-10d-10d')

INSERT [dbo].[intervals] ([standard], [header], [interval]) VALUES (0, N'месяц', N'[local]=1M')

INSERT [dbo].[intervals] ([standard], [header], [interval]) VALUES (0, N'месяц UTC', N'=1M')

INSERT [dbo].[intervals] ([standard], [header], [interval]) VALUES (1, N'месяц (Москва)', N'[UTC+04]=1M')

INSERT [dbo].[intervals] ([standard], [header], [interval]) VALUES (0, N'квартал', N'[local]=3M')

INSERT [dbo].[intervals] ([standard], [header], [interval]) VALUES (1, N'квартал (Москва)', N'[UTC+04]=3M')

INSERT [dbo].[intervals] ([standard], [header], [interval]) VALUES (0, N'год', N'[local]=1Y')

INSERT [dbo].[intervals] ([standard], [header], [interval]) VALUES (1, N'год (Москва)', N'[UTC+04]=1Y')
