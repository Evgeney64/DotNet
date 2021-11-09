using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Model
{
    public partial class DbInfo
    {
        private void createColumn(SqlConnection connection, table tbl)
        {
            try
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
                    SqlParameter nameParam = new SqlParameter("@id", tbl.id);
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
            catch (Exception ex)
            { }
        }
        private void createFK(SqlConnection connection, table tbl)
        {
            try
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
                    SqlParameter nameParam = new SqlParameter("@id", tbl.id);
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
            catch (Exception ex)
            { }
        }
        private void createIndex(SqlConnection connection, table tbl)
        {
            try
            {
                using (SqlCommand command4 = new SqlCommand())
                {
                    #region Define-4
                    command4.CommandText =
                        "SELECT ind.name, ind.object_id, ind.index_id, ind.is_unique" +
                        ", CASE WHEN ind.type_desc='CLUSTERED' THEN 1 ELSE 0 END as is_clustered" +
                        " FROM sys.indexes ind" +
                        "    INNER JOIN sys.objects ob on ob.object_id=ind.object_id" +
                        " WHERE ob.name=@name and ind.name IS NOT NULL ORDER BY ind.index_id" +
                        "";
                    command4.Connection = connection;
                    command4.Parameters.Add(new SqlParameter("@name", tbl.name));

                    SqlDataReader reader4 = command4.ExecuteReader();
                    int count = 1;
                    #endregion
                    if (reader4.HasRows)
                    {
                        while (reader4.Read())
                        {
                            #region index
                            object ind_name = reader4.GetValue(0);
                            if (ind_name == null)
                                continue;
                            string ind_name_str = ind_name.ToString();
                            if (ind_name_str.Substring(0, 6) == "%_%_FK")
                                ind_name_str = "IX_FK_" + tbl.name;
                            if (indexes.Where(ss => ss.index_name == ind_name_str).Count() > 0)
                            {
                                ind_name_str += "_" + count;
                                count++;
                            }
                            index ind = new index
                            {
                                index_name = ind_name_str,
                                object_id = long.Parse(reader4.GetValue(1).ToString()),
                                index_id = int.Parse(reader4.GetValue(2).ToString()),
                                is_unique = bool.Parse(reader4.GetValue(3).ToString()),
                                is_clustered = int.Parse(reader4.GetValue(4).ToString()),
                                index_columns = new List<index_column>(),
                            };
                            indexes.Add(ind);

                            #endregion

                            #region index_column
                            using (SqlCommand command41 = new SqlCommand())
                            {
                                #region Define-41
                                command41.CommandText =
                                    "SELECT col.name, col.column_id" +
                                    " FROM sys.index_columns coli" +
                                    "    INNER JOIN sys.all_columns col on col.object_id=coli.object_id and col.column_id=coli.column_id" +
                                    " WHERE coli.object_id=@object_id and coli.index_id=@index_id ORDER BY col.column_id" +
                                    "";
                                command41.Connection = connection;
                                command41.Parameters.Add(new SqlParameter("@object_id", ind.object_id));
                                command41.Parameters.Add(new SqlParameter("@index_id", ind.index_id));

                                SqlDataReader reader41 = command41.ExecuteReader();
                                #endregion
                                if (reader41.HasRows)
                                {
                                    while (reader41.Read())
                                    {
                                        #region index_column
                                        object col_name = reader41.GetValue(0);
                                        if (col_name == null)
                                            continue;
                                        index_column ind_col = new index_column
                                        {
                                            column_name = col_name.ToString(),
                                            column_id = int.Parse(reader4.GetValue(1).ToString()),
                                        };
                                        ind.index_columns.Add(ind_col);
                                        #endregion
                                    }
                                }
                                reader41.Close();
                            }
                            #endregion

                            tbl.indexes.Add(ind);
                        }
                    }
                    reader4.Close();
                }
            }
            catch (Exception ex)
            { }
        }

        private void scriptColumn(table tbl, string schem_table_name)
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
        private void scriptFK(table tbl, string schem_table_name)
        {
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
        }
        private void scriptIndex(table tbl, string schem_table_name)
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
