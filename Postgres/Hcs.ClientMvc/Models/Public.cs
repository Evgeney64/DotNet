using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Hcs.Store
{
    public class Public
    {
        public string GenerateScript(DataSourceConfiguration conf)
        {
            #region Define
            string user = "\"BillBerry\"";
            string schem = "\"UAP\"";
            schem = "\"public\"";
            string table_space = "\"BillBerryTS\"";
            string collate = "pg_catalog.\"default\"";

            List<table> tables = new List<table>();
            List<SysOperation> sysOperations = new List<SysOperation>();
            #endregion

            #region get tables
            using (SqlConnection connection = new SqlConnection(conf.ConnectionString))
            {
                connection.Open();
                #region metadata
                using (SqlCommand command1 = new SqlCommand())
                {
                    #region Define-1
                    command1.CommandText = "SELECT name, id FROM sysobjects WHERE xtype='U' ORDER BY name";
                    command1.Connection = connection;

                    SqlDataReader reader1 = command1.ExecuteReader();
                    #endregion
                    if (reader1.HasRows)
                    {
                        while (reader1.Read())
                        {
                            #region table
                            object table_name = reader1.GetValue(0);
                            object table_id = reader1.GetValue(1);
                            if (table_name == null || table_id == null)
                                continue;
                            long id = long.Parse(table_id.ToString());
                            table tbl = new table
                            {
                                name = table_name.ToString(),
                                id = id,
                                columns = new List<column>(),
                                foreign_keys = new List<foreign_key>(),
                            };
                            tables.Add(tbl);
                            #endregion

                            #region column
                            if (1 == 1)
                            {
                                using (SqlCommand command2 = new SqlCommand())
                                {
                                    #region Define-2
                                    command2.CommandText =
                                        "SELECT name, column_id, user_type_id, max_length" +
                                        //", is_primary_key, is_nullable, is_identity" +
                                        ", is_nullable, is_identity" +
                                        " FROM sys.all_columns WHERE OBJECT_ID=@id ORDER BY column_id";
                                    command2.Connection = connection;
                                    SqlParameter nameParam = new SqlParameter("@id", id);
                                    command2.Parameters.Add(nameParam);

                                    SqlDataReader reader2 = command2.ExecuteReader();
                                    #endregion
                                    if (reader2.HasRows)
                                    {
                                        while (reader2.Read())
                                        {
                                            #region column
                                            object col_name = reader2.GetValue(0);
                                            if (col_name == null)
                                                continue;
                                            column col = new column
                                            {
                                                name = col_name.ToString(),
                                                column_id = int.Parse(reader2.GetValue(1).ToString()),
                                                user_type_id = int.Parse(reader2.GetValue(2).ToString()),
                                                max_length = int.Parse(reader2.GetValue(3).ToString()),
                                                //is_primary_key = short.Parse(reader2.GetValue(4).ToString()),
                                                is_nullable = bool.Parse(reader2.GetValue(4).ToString()),
                                                is_identity = bool.Parse(reader2.GetValue(5).ToString()),
                                                collate = collate,
                                            };
                                            col.typePostgresSet();
                                            tbl.columns.Add(col);
                                            #endregion
                                        }
                                    }
                                    reader2.Close();
                                }
                            }
                            #endregion

                            #region foreign_key
                            if (1 == 1)
                            {
                                using (SqlCommand command3 = new SqlCommand())
                                {
                                    #region Define-3
                                    command3.CommandText =
                                        "SELECT fk.name as fk_name, tbl_par.name as parent_table, tbl_ch.name as child_table" +
                                        ", col_par.name as parent_column, col_ch.name as child_column" +
                                        " FROM sys.foreign_keys fk" +
                                        "   INNER JOIN sysobjects tbl_par on tbl_par.id = fk.parent_object_id" +
                                        "   INNER JOIN sysobjects tbl_ch on tbl_ch.id = fk.referenced_object_id" +
                                        "   INNER JOIN sys.foreign_key_columns fk_col on fk_col.constraint_object_id = fk.object_id" +
                                        "   INNER JOIN sys.all_columns col_par on col_par.OBJECT_ID = fk_col.parent_object_id and col_par.column_id = fk_col.parent_column_id" +
                                        "   INNER JOIN sys.all_columns col_ch on col_ch.OBJECT_ID = fk_col.referenced_object_id and col_ch.column_id = fk_col.referenced_column_id" +
                                        " WHERE fk.parent_object_id=@id";
                                    command3.Connection = connection;
                                    SqlParameter nameParam = new SqlParameter("@id", id);
                                    //if (id == 1737773248)
                                    //{ }
                                    command3.Parameters.Add(nameParam);

                                    SqlDataReader reader3 = command3.ExecuteReader();
                                    #endregion
                                    if (reader3.HasRows)
                                    {
                                        while (reader3.Read())
                                        {
                                            #region foreign_key
                                            object col_name = reader3.GetValue(0);
                                            if (col_name == null)
                                                continue;
                                            foreign_key fk = new foreign_key
                                            {
                                                fk_name = col_name.ToString(),
                                                parent_table = reader3.GetValue(1).ToString(),
                                                child_table = reader3.GetValue(2).ToString(),
                                                parent_column = reader3.GetValue(3).ToString(),
                                                child_column = reader3.GetValue(4).ToString(),
                                            };
                                            tbl.foreign_keys.Add(fk);
                                            #endregion
                                        }
                                    }
                                    reader3.Close();
                                }
                            }
                            #endregion
                        }
                    }
                    reader1.Close();
                }
                #endregion

                #region insert data
                if (1 == 1)
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
            string crt = "";
            string crt_fk = "";
            string ins = "";
            foreach (table tbl in tables)
            {
                string table_name = "\"" + tbl.name + "\"";
                string schem_table_name = schem + "." + table_name;
                crt += " \nCREATE TABLE " + schem_table_name;
                crt += " \n(";

                #region CREATE TABLE
                bool is_first = true;
                foreach (column col in tbl.columns)
                {
                    if (is_first == false)
                        crt += " ,";
                    crt += " \n   \"" + col.name + "\"";
                    crt += " " + col.typePostgres;
                    if (col.is_primary_key == 1 || col.is_identity)
                        crt += " PRIMARY KEY";
                    else if (col.is_nullable == false)
                        crt += " NOT NULL";

                    if (col.is_identity)
                    {
                        //crt += " GENERATED ALWAYS AS IDENTITY";
                        crt += " GENERATED BY DEFAULT AS IDENTITY";
                    }

                    is_first = false;
                }

                crt += " \n)";
                crt += " TABLESPACE " + table_space;
                crt += ";";

                crt += " \n";
                crt += " ALTER TABLE " + tbl.name + " OWNER to " + user;
                crt += ";\n\n";
                #endregion

                #region CREATE FK
                if (tbl.foreign_keys.Count() > 0)
                {
                    foreach (foreign_key fk in tbl.foreign_keys)
                    {
                        crt_fk +=
                            "\nALTER TABLE " + schem_table_name +
                            "\n     ADD CONSTRAINT \"" + fk.fk_name + "\" FOREIGN KEY (\"" + fk.child_column + "\")" +
                            "\n     REFERENCES " + schem + ".\"" + fk.parent_table + "\" (\"" + fk.parent_column + "\") MATCH SIMPLE" +
                            "\n     ON UPDATE NO ACTION" +
                            "\n     ON DELETE NO ACTION;";
                        crt_fk += "\n\n";
                    }
                }
                #endregion

                #region INSERT SysOperation
                if (tbl.name == "SysOperation" && sysOperations.Count() > 0)
                {
                    foreach (SysOperation sop in sysOperations)
                    {
                        ins += "\nINSERT INTO " + schem_table_name + "(\"OperationId\", \"OperationName\", \"Name\", \"PacketSize\")" +
                            "\n VALUES (" +
                            "\"" + sop.OperationId + "\"" +
                            ", \"" + sop.OperationName + "\"" +
                            ", \"" + sop.Name + "\"" +
                            ", \"" + sop.PacketSize + "\"" +
                            ")\n";
                    }
                }
                #endregion
            }
            #endregion

            #region Save
            string path = Directory.GetCurrentDirectory() + "//script";
            writeToFile(path, "create_table.sql", crt);
            writeToFile(path, "create_fk.sql", crt_fk);
            writeToFile(path, "insert_data.sql", ins);
            #endregion

            return "Ok";
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

