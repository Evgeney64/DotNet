using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Model
{
    public partial class Public
    {
        #region Define
        DataSourceConfiguration conf;

        string user = "\"gis_hcs\"";
        string schem = "\"gis_hcs\"";
        string table_space = "\"pg_default\"";
        string collate = "pg_catalog.\"default\"";

        List<table> tables = new List<table>();
        List<index> indexes = new List<index>();
        List<SysOperation> sysOperations = new List<SysOperation>();

        string crt = "";
        string del = "";
        string crt_fk = "";
        string del_fk = "";
        string crt_ind = "";
        string del_ind = "";
        string ins = "";
        int count0 = 0;
        #endregion

        public Public(DataSourceConfiguration _conf)
        {
            conf = _conf;
        }
        public string GenerateScript()
        {
            #region
            #region Define
            user = "\"postgres\"";
            schem = "\"tsb\"";
            #endregion

            #region get tables
            using (SqlConnection connection = new SqlConnection(conf.ConnectionString))
            {
                connection.Open();
                #region metadata
                using (SqlCommand command1 = new SqlCommand())
                {
                    #region Define-1
                    List<string> ignore_tables = new List<string>
                    {
                        "__MigrationHistory",
                        "sysdiagrams",
                        "UP_BILLS_DZ",
                    };
                    string ignore_tables_lst = String.Join(",", ignore_tables.Select(ss => "'" + ss + "'"));
                    command1.CommandText =
                        "SELECT name, id FROM sysobjects" +
                        " WHERE xtype='U' and SUBSTRING(name,1,3) not in ('ZZ_', 'YY_')" +
                        "    and name not in (" + ignore_tables_lst + ")" +
                        " ORDER BY name";
                    command1.Connection = connection;

                    SqlDataReader reader1 = command1.ExecuteReader();
                    #endregion
                    if (reader1.HasRows)
                    {
                        while (reader1.Read())
                        {
                            #region table
                            object table_name = reader1.GetValue(0);
                            if (table_name == null)
                                continue;
                            table tbl = new table
                            {
                                name = table_name.ToString(),
                                id = long.Parse(reader1.GetValue(1).ToString()),
                                columns = new List<column>(),
                                foreign_keys = new List<foreign_key>(),
                                indexes = new List<index>(),
                            };
                            tables.Add(tbl);
                            #endregion

                            if (1 == 1) createColumn(connection, tbl);
                            if (1 == 1) createFK(connection, tbl);
                            if (1 == 1) createIndex(connection, tbl);
                        }
                    }
                    reader1.Close();
                }
                #endregion

                #region insert data
                if (1 == 2)
                {
                    using (SqlCommand command4 = new SqlCommand())
                    {
                        command4.CommandText = "SELECT OperationId, OperationName, Name, PacketSize" +
                            " FROM SysOperation ORDER BY OperationId";
                        command4.Connection = connection;

                        SqlDataReader reader4 = command4.ExecuteReader();
                        if (reader4.HasRows)
                        {
                            while (reader4.Read())
                            {
                                object id = reader4.GetValue(0);
                                if (id == null)
                                    continue;
                                SysOperation sop = new SysOperation
                                {
                                    OperationId = int.Parse(id.ToString()),
                                    OperationName = reader4.GetValue(1).ToString(),
                                    Name = reader4.GetValue(2).ToString(),
                                    PacketSize = int.Parse(id.ToString()),
                                };
                                sysOperations.Add(sop);
                            }
                        }
                    }
                }
                #endregion
            }
            { }
            #endregion

            #region create script

            foreach (table tbl in tables)
            {
                #region CREATE TABLE
                string table_name = "\"" + tbl.name + "\"";
                string schem_table_name = schem + "." + table_name;
                crt += " \nCREATE TABLE " + schem_table_name;
                crt += " \n(";
                del += " \nDROP TABLE " + schem_table_name + ";";

                scriptColumn(tbl, schem_table_name);

                crt += " \n)";
                crt += " TABLESPACE " + table_space;
                crt += ";";

                crt += " \n";
                crt += " ALTER TABLE " + schem_table_name + " OWNER to " + user;
                crt += ";\n";
                #endregion

                scriptFK(tbl, schem_table_name);

                scriptIndex(tbl, schem_table_name);

                #region INSERT SysOperation
                if (1 == 2)
                {
                    if (tbl.name == "SysOperation" && sysOperations.Count() > 0)
                    {
                        foreach (SysOperation sop in sysOperations)
                        {
                            ins += "\nINSERT INTO " + schem_table_name + " (\"OperationId\", \"OperationName\", \"Name\", \"PacketSize\")" +
                                " VALUES (" +
                                "'" + sop.OperationId + "'" +
                                ", '" + sop.OperationName + "'" +
                                ", '" + sop.Name + "'" +
                                ", '" + sop.PacketSize + "'" +
                                ");";
                        }
                    }
                }
                #endregion
            }
            { }
            #endregion

            #region Save
            string path = Directory.GetCurrentDirectory() + "//script";
            writeToFile(path, "create_table.sql", crt);
            writeToFile(path, "drop_table.sql", del);
            writeToFile(path, "create_fk.sql", crt_fk);
            writeToFile(path, "drop_ind.sql", del_ind);
            writeToFile(path, "create_ind.sql", crt_ind);
            writeToFile(path, "drop_fk.sql", del_fk);
            writeToFile(path, "insert_data.sql", ins);
            #endregion

            return "GenerateScript - Ok";
            #endregion
        }

        private void writeToFile(string path_mame, string file_name, string src, bool count_tables = false)
        {
            #region
            if (src != "")
            {
                if (count_tables)
                {
                    //src += "\n\n\nSELECT table_name FROM information_schema.tables";
                    src += "\n\n\nSELECT count(*) FROM information_schema.tables WHERE table_schema='UAP'";
                }
                Encoding encoding = Encoding.GetEncoding("UTF-8");
                string[] arr_src = new string[1];
                arr_src[0] = src;
                File.WriteAllLines(Path.Combine(path_mame, file_name), arr_src, encoding);
            }
            #endregion
        }
    }
}
