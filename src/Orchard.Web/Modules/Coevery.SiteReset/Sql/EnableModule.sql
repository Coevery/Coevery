INSERT [Settings_ShellFeatureRecord] ([Name],[ShellDescriptorRecord_id]) VALUES (N'Coevery.SiteReset',1)
INSERT [Settings_ShellFeatureStateRecord] ([Name],[InstallState],[EnableState],[ShellStateRecord_Id]) VALUES (N'Coevery.SiteReset',N'Up',N'Up',1)
UPDATE [Settings_ShellDescriptorRecord] SET [SerialNumber]=[SerialNumber]+1
