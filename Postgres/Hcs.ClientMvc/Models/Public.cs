﻿using System;
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
        DataSourceConfiguration conf;
        public Public(DataSourceConfiguration _conf)
        {
            conf = _conf;
        }
        public string GenerateScript()
        {
            #region
            #region Define
            string user = "\"gis_hcs\"";
            string schem = "\"gis_hcs\"";
            string table_space = "\"pg_default\"";
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
                                        "SELECT col.name, column_id, user_type_id, max_length" +
                                        //", is_primary_key, is_nullable, is_identity" +
                                        ", is_nullable, is_identity" +
                                        ", CASE WHEN sch.COLUMN_NAME IS NOT NULL THEN 1 ELSE 0 END AS IS_PRIMARY_KEY" +
                                        " FROM sys.all_columns col" +
                                        "    INNER JOIN sysobjects tbl on tbl.id = col.object_id" +
                                        "    LEFT JOIN (SELECT sch.TABLE_NAME, sch.COLUMN_NAME FROM sys.key_constraints pk" +
                                        "        INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE sch on sch.CONSTRAINT_NAME=pk.name WHERE pk.type='PK'" +
                                        "        ) sch on sch.TABLE_NAME = tbl.name and sch.COLUMN_NAME = col.name" +
                                        " WHERE col.object_id=@id ORDER BY column_id" +
                                        "";
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
                                                is_nullable = bool.Parse(reader2.GetValue(4).ToString()),
                                                is_identity = bool.Parse(reader2.GetValue(5).ToString()),
                                                is_primary_key = short.Parse(reader2.GetValue(6).ToString()),
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
                                        //" WHERE fk.referenced_object_id=@id";
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
            string del = "";
            string crt_fk = "";
            string del_fk = "";
            string ins = "";
            int count0 = 0;
            foreach (table tbl in tables)
            {
                string table_name = "\"" + tbl.name + "\"";
                string schem_table_name = schem + "." + table_name;
                crt += " \nCREATE TABLE " + schem_table_name;
                crt += " \n(";

                del += " \nDROP TABLE " + schem_table_name + ";";

                #region CREATE TABLE
                bool is_first = true;
                foreach (column col in tbl.columns)
                {
                    if (is_first == false)
                        crt += " ,";
                    crt += " \n   \"" + col.name + "\"";
                    crt += " " + col.typePostgres;
                    if (col.is_primary_key == 1)
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
                crt += " ALTER TABLE " + schem_table_name + " OWNER to " + user;
                crt += ";\n";
                #endregion

                #region CREATE FK
                if (tbl.foreign_keys.Count() > 0)
                {
                    int count = 0;
                    foreach (foreign_key fk in tbl.foreign_keys)
                    {
                        string fk_name = fk.fk_name;
                        if (fk_name.Length > 63)
                        {
                            fk_name = fk_name.Substring(0, 60) + "_" + count.ToString();
                            count++;
                            count0++;
                            //fk_name = "FK_" + fk.parent_table + "_" + fk.child_table + "_" + count.ToString();
                        }
                        crt_fk +=
                            "\nALTER TABLE " + schem_table_name + 
                            " ADD CONSTRAINT \"" + fk_name + "\"" + 
                            " FOREIGN KEY (\"" + fk.parent_column + "\")" +
                            " REFERENCES " + schem + ".\"" + fk.child_table + "\" (\"" + fk.child_column + "\") MATCH SIMPLE" +
                            " ON UPDATE NO ACTION" +
                            " ON DELETE NO ACTION;";
                        //crt_fk +=
                        //    "\nALTER TABLE " + schem_table_name +
                        //    "\n     ADD CONSTRAINT \"" + fk.fk_name + "\"" +
                        //    "\n     FOREIGN KEY (\"" + fk.parent_column + "\")" +
                        //    "\n     REFERENCES " + schem + ".\"" + fk.child_table + "\" (\"" + fk.child_column + "\") MATCH SIMPLE" +
                        //    "\n     ON UPDATE NO ACTION" +
                        //    "\n     ON DELETE NO ACTION;";
                        //crt_fk += "\n\n";

                        del_fk += "\nALTER TABLE " + schem_table_name + " DROP CONSTRAINT \"" + fk_name + "\";";
                        //del_fk += "\nIF (SELECT * FROM pg_constraint WHERE conname = \"" + fk.fk_name + "\")" + 
                        //    "\n     ALTER TABLE " + schem_table_name + " DROP CONSTRAINT \"" + fk.fk_name + "\";";
                        /*
                        SELECT* FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SysParams'
                        SELECT* FROM pg_constraint WHERE conname = 'FK_AttachmentPostResultCopy_AttachmentPostResult'
                        */
                    }
                }
                #endregion

                #region INSERT SysOperation
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
                #endregion
            }
            { }
            #endregion

            #region Save
            string path = Directory.GetCurrentDirectory() + "//script";
            writeToFile(path, "create_table.sql", crt);
            writeToFile(path, "drop_table.sql", del);
            writeToFile(path, "create_fk.sql", crt_fk);
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

        public string GetData()
        {
            using (ApplicationContext db = new ApplicationContext(conf.ConnectionString))
            {
                // получаем объекты из бд и выводим на консоль
                var sopr = db.SysOperation;
                if (sopr != null)
                {
                    try
                    {
                        List<SysOperation> sysOperations = sopr.ToList();
                        { }
                        //Console.WriteLine("Users list:");
                        foreach (SysOperation sop in sysOperations)
                        {
                            //Console.WriteLine($"{u.Id}.{u.Name} - {u.Age}");
                        }
                    }
                    catch (Exception ex)
                    { }
                }
            }
            return "GetData - Ok";
        }

        public string GetSysOperation()
        {
            //using (ApplicationContext context = new ApplicationContext(conf.ConnectionString, conf.is_postgres))
            using (var context = this.CreateContext())
            {
                string str = "";
                var sopr = context.SysOperation;
                if (sopr != null)
                {
                    try
                    {
                        List<SysOperation> sysOperations = null;
                        try
                        {
                            sysOperations = sopr.ToList();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Ошибка");
                        }
                        foreach (SysOperation sop in sysOperations)
                        {
                            str += "<p>" + sop.OperationId + " - " + sop.OperationName + "</p>";
                        }
                    }
                    catch (Exception ex)
                    { }
                }
                return str;
            }
        }
        public string GetSysTransaction()
        {
            using (ApplicationContext context = new ApplicationContext(conf.ConnectionString, conf.is_postgres))
            //using (var context = this.CreateContext())
            {
                string str = "";
                DateTime date = new DateTime(2021, 11, 1);
                var items = context.SysTransaction.Where(ss => ss.StartDate >= date);
                if (items != null)
                {
                    try
                    {
                        List<SysTransaction> itemsL = items.ToList();
                        { }
                        foreach (SysTransaction sop in itemsL)
                        {
                            str += "<p>" + sop.TransactionGUID + "</p>";
                        }
                    }
                    catch (Exception ex)
                    { }
                }
                return str;
            }
        }

        protected ApplicationContext CreateContext()
        {
            var context = ApplicationContext.CreateContext(conf.ConnectionString, conf.is_postgres);
            //$$$
            //context.ObjectContext.Connection.Open();
            //context.ObjectContext.CommandTimeout = this.Configuration.CommandTimeout;
            AssemblyName assemblyName = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
            string versionNumber = assemblyName.Version.ToString();
            //var sysParams = context.SysParams
            //    .AsNoTracking()
            //    .FirstOrDefault();
            //$$$
            //if (sysParams == null || !String.Equals(sysParams.VersionNumber, versionNumber, StringComparison.Ordinal))
            //{
            //    throw new Exception(String.Format("Текущая версия промежуточной БД не поддерживается. Требуется версия {0}", versionNumber));
            //}

            return context;
        }

    }
}


