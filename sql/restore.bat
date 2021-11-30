Sqlcmd -S ROEV\EVGEN -U sa -P 1212 -i “c:\Disk_D\_Dot_Net\Db\restore_0.sql” -o “c:\Disk_D\_Dot_Net\Db\result.txt”

Sqlcmd -S ROEV\EVGEN -i “c:\Disk_D\_Dot_Net\Db\restore_0.sql” -E -o “c:\Disk_D\_Dot_Net\Db\result.txt”