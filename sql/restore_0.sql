USE [master]
RESTORE DATABASE [renovation_home] FROM  DISK = N'C:\Disk_D\_Dot_Net\Db\BD\renovation_web.bak' WITH  FILE = 2,  
MOVE N'web' TO N'c:\Disk_D\_Dot_Net\Db\renovation_home_new.mdf',  
MOVE N'web_log' TO N'c:\Disk_D\_Dot_Net\Db\renovation_home_new.ldf',  
NOUNLOAD,  STATS = 5

GO

