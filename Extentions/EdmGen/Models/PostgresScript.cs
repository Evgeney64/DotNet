using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Tsb.Model
{
    public partial class Postgres
    {
        #region Define
        DbInfo info;
        List<table> tables = new List<table>();
        string crt = "";
        string del = "";
        string crt_fk = "";
        string del_fk = "";
        string crt_ind = "";
        string del_ind = "";
        string ins = "";

        List<SysOperation> sysOperations = new List<SysOperation>();
        #endregion

        public Postgres(DbInfo _info)
        {
            info = _info;
            tables = info.tables;
        }
        public void GeneratePostgresScript(string client_path)
        {
            #region create script
            string user = "\"gis_hcs\"";
            string schem = "\"gis_hcs\"";
            string table_space = "\"pg_default\"";

            Console.WriteLine("");
            Console.WriteLine("Create script.......................................");
            foreach (table tbl in tables)
            {
                #region CREATE TABLE
                string table_name = "\"" + tbl.name + "\"";
                string schem_table_name = schem + "." + table_name;
                crt += " \nCREATE TABLE " + schem_table_name;
                crt += " \n(";
                del += " \nDROP TABLE " + schem_table_name + ";";

                scriptColumn(info, tbl, schem_table_name);

                crt += " \n)";
                crt += " TABLESPACE " + table_space;
                crt += ";";

                crt += " \n";
                crt += " ALTER TABLE " + schem_table_name + " OWNER to " + user;
                crt += ";\n";
                #endregion

                scriptFK(info, tbl, schem_table_name, schem);

                scriptIndex(info, tbl, schem_table_name, schem, table_space);

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

                Console.WriteLine("[script] - " + tbl.nom + " - " + tbl.name);
            }
            { }
            #endregion

            #region Save
            Console.WriteLine("");
            Console.WriteLine("Save script.......................................");
            string base_dir = AppDomain.CurrentDomain.BaseDirectory;
            string path = base_dir.Substring(0, base_dir.IndexOf(client_path)) + client_path + "\\Result\\PostgreScript";
            { }
            writeToFile(path, "create_table.sql", crt);
            writeToFile(path, "drop_table.sql", del);
            writeToFile(path, "create_fk.sql", crt_fk);
            writeToFile(path, "drop_ind.sql", del_ind);
            writeToFile(path, "create_ind.sql", crt_ind);
            writeToFile(path, "drop_fk.sql", del_fk);
            writeToFile(path, "insert_data.sql", ins);
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
                Console.WriteLine("[save] - " + path_mame + " - " + file_name);
            }
            #endregion
        }

        private void scriptColumn(DbInfo info, table tbl, string schem_table_name)
        {
            if (tbl.columns.Count() > 0)
            {
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
            }
        }
        private void scriptFK(DbInfo info, table tbl, string schem_table_name, string schem)
        {
            //if (tbl.foreign_keys.Count() > 0)
            {
                int count = 0;
                //foreach (foreign_key fk in tbl.foreign_keys)
                foreach (foreign_key fk in info.foreign_keys.Where(ss=>ss.this_table1 == tbl))
                {
                    string fk_name = fk.fk_name;
                    if (fk_name.Length > 63)
                    {
                        fk_name = fk_name.Substring(0, 60) + "_" + count.ToString();
                        count++;
                        //fk_name = "FK_" + fk.parent_table + "_" + fk.child_table + "_" + count.ToString();
                    }
                    crt_fk +=
                        "\nALTER TABLE " + schem_table_name +
                        " ADD CONSTRAINT \"" + fk_name + "\"" +
                        " FOREIGN KEY (\"" + fk.this_column + "\")" +
                        " REFERENCES " + schem + ".\"" + fk.ref_table + "\" (\"" + fk.ref_column + "\") MATCH SIMPLE" +
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
        }
        private void scriptIndex(DbInfo info, table tbl, string schem_table_name, string schem, string table_space)
        {
            if (tbl.indexes.Count() > 0)
            {
                foreach (index ind in tbl.indexes)
                {
                    string is_unique = "";
                    if (ind.is_unique)
                        is_unique = "UNIQUE";

                    crt_ind += " \nCREATE " + is_unique + " INDEX \"" + ind.index_name + "\"" +
                        " ON " + schem_table_name + " USING btree";
                    crt_ind += " (";

                    del_ind += " \nDROP  INDEX " + schem + ".\"" + ind.index_name + "\";";
                    bool is_first_ind = true;
                    foreach (index_column ind_col in ind.index_columns)
                    {
                        if (is_first_ind == false)
                            crt_ind += " ,";
                        crt_ind += "\"" + ind_col.column_name + "\"";
                        is_first_ind = false;
                    }
                    crt_ind += ")";
                    crt_ind += " TABLESPACE " + table_space + ";";
                }
            }
        }
    }
}
