using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Tsb.Model
{
    public partial class DbInfo
    {
        private void createTables(SqlConnection connection)
        {
            try
            {
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
                    string sql =
                        "SELECT name, id FROM sysobjects" +
                        " WHERE xtype='U' and SUBSTRING(name,1,3) not in ('ZZ_', 'YY_')" +
                        "    and name not in (" + ignore_tables_lst + ")" +
                        "";
                    if (files != null && files.Count() > 0)
                    {
                        string files_lst = String.Join(",", files.Select(ss => "'" + ss + "'"));
                        sql += "    and name in (" + files_lst + ")";
                    }

                    sql += " ORDER BY name";
                    command1.CommandText = sql;
                    command1.Connection = connection;

                    SqlDataReader reader1 = command1.ExecuteReader();
                    #endregion
                    if (reader1.HasRows)
                    {
                        int nom = 0;
                        while (reader1.Read())
                        {
                            #region
                            object table_name = reader1.GetValue(0);
                            if (table_name == null)
                                continue;
                            table tbl = new table
                            {
                                name = table_name.ToString(),
                                fk_name = table_name.ToString(),
                                id = long.Parse(reader1.GetValue(1).ToString()),
                                columns = new List<column>(),
                                indexes = new List<index>(),
                                nom = nom,
                            };
                            nom++;
                            tables.Add(tbl);
                            Console.WriteLine("[table] - " + tbl.nom + " - " + tbl.name);
                            #endregion
                        }
                    }
                    reader1.Close();

                    object_ids = String.Join(",", tables.Select(ss => ss.id));
                    tables_str.AddRange(tables.Select(ss => ss.name));
                }
            }
            catch (Exception ex)
            { }
        }

        private void createColumns(SqlConnection connection)
        {
            try
            {
                using (SqlCommand command2 = new SqlCommand())
                {
                    #region Define-2
                    command2.CommandText =
                        "SELECT col.name, col.object_id, column_id, user_type_id, max_length" +
                        ", is_nullable, is_identity" +
                        ", CASE WHEN sch.COLUMN_NAME IS NOT NULL THEN 1 ELSE 0 END AS IS_PRIMARY_KEY" +
                        " FROM sys.all_columns col" +
                        "    INNER JOIN sysobjects tbl on tbl.id = col.object_id" +
                        "    LEFT JOIN (SELECT sch.TABLE_NAME, sch.COLUMN_NAME FROM sys.key_constraints pk" +
                        "        INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE sch on sch.CONSTRAINT_NAME=pk.name WHERE pk.type='PK'" +
                        "        ) sch on sch.TABLE_NAME = tbl.name and sch.COLUMN_NAME = col.name" +
                        " WHERE col.object_id in (" + object_ids + ")" +
                        " ORDER BY col.object_id, column_id" +
                        "";
                    command2.Connection = connection;

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
                                object_id = long.Parse(reader2.GetValue(1).ToString()),
                                column_id = int.Parse(reader2.GetValue(2).ToString()),
                                user_type_id = int.Parse(reader2.GetValue(3).ToString()),
                                max_length = int.Parse(reader2.GetValue(4).ToString()),
                                is_nullable = bool.Parse(reader2.GetValue(5).ToString()),
                                is_identity = bool.Parse(reader2.GetValue(6).ToString()),
                                is_primary_key = short.Parse(reader2.GetValue(7).ToString()),
                                collate = collate,
                            };

                            col.typePostgresSet();
                            columns.Add(col);
                            //tbl.columns.Add(col);
                            #endregion
                        }
                    }
                    reader2.Close();
                }
            }
            catch (Exception ex)
            { }
        }

        private void createFKs(SqlConnection connection)
        {
            try
            {
                using (SqlCommand command3 = new SqlCommand())
                {
                    #region Define-3
                    command3.CommandText =
                        "SELECT fk.name as fk_name, fk.parent_object_id, tbl_par.name as parent_table, tbl_ch.name as child_table" +
                        ", col_par.name as parent_column, col_ch.name as child_column" +
                        " FROM sys.foreign_keys fk" +
                        "   INNER JOIN sysobjects tbl_par on tbl_par.id = fk.parent_object_id" +
                        "   INNER JOIN sysobjects tbl_ch on tbl_ch.id = fk.referenced_object_id" +
                        "   INNER JOIN sys.foreign_key_columns fk_col on fk_col.constraint_object_id = fk.object_id" +
                        "   INNER JOIN sys.all_columns col_par on col_par.OBJECT_ID = fk_col.parent_object_id and col_par.column_id = fk_col.parent_column_id" +
                        "   INNER JOIN sys.all_columns col_ch on col_ch.OBJECT_ID = fk_col.referenced_object_id and col_ch.column_id = fk_col.referenced_column_id" +
                        " WHERE fk.parent_object_id in (" + object_ids + ")" +
                        " ORDER BY fk.parent_object_id";
                    command3.Connection = connection;

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
                                object_id = long.Parse(reader3.GetValue(1).ToString()),
                                this_table = reader3.GetValue(2).ToString(),
                                ref_table = reader3.GetValue(3).ToString(),
                                this_column = reader3.GetValue(4).ToString(),
                                ref_column = reader3.GetValue(5).ToString(),
                            };
                            if (tables_str.Contains(fk.this_table) && tables_str.Contains(fk.ref_table))
                            {
                                foreign_keys.Add(fk);
                                //table ref_table = tables.Where(ss => ss.name == fk.ref_table).FirstOrDefault();
                                //if (ref_table != null)
                                //    foreign_keys.Add(fk);
                            }
                            //tbl.foreign_keys.Add(fk);
                            #endregion
                        }
                    }
                    reader3.Close();
                }
            }
            catch (Exception ex)
            { }
        }

        private void createIndexes(SqlConnection connection)
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
                        " WHERE ind.object_id in (" + object_ids + ") and ind.name IS NOT NULL" +
                        " ORDER BY ind.object_id, ind.index_id" +
                        //"    INNER JOIN sys.objects ob on ob.object_id=ind.object_id" +
                        //" WHERE ob.name=@name and ind.name IS NOT NULL ORDER BY ind.index_id" +
                        "";
                    command4.Connection = connection;
                    //command4.Parameters.Add(new SqlParameter("@name", tbl.name));

                    SqlDataReader reader4 = command4.ExecuteReader();
                    int nom = 0;
                    #endregion
                    if (reader4.HasRows)
                    {
                        while (reader4.Read())
                        {
                            #region index
                            object ind_name = reader4.GetValue(0);
                            if (ind_name == null)
                                continue;
                            index ind = new index
                            {
                                index_name = ind_name.ToString(),
                                object_id = long.Parse(reader4.GetValue(1).ToString()),
                                index_id = int.Parse(reader4.GetValue(2).ToString()),
                                is_unique = bool.Parse(reader4.GetValue(3).ToString()),
                                is_clustered = int.Parse(reader4.GetValue(4).ToString()),
                                index_columns = new List<index_column>(),
                                nom = nom,
                            };
                            nom++;
                            indexes.Add(ind);
                            //tbl.indexes.Add(ind);
                            #endregion
                        }
                    }
                    reader4.Close();
                }
            }
            catch (Exception ex)
            { }
        }

        private void createIndexColumns(SqlConnection connection)
        {
            try
            {
                using (SqlCommand command41 = new SqlCommand())
                {
                    #region Define-41
                    command41.CommandText =
                        "SELECT col.name, coli.object_id, coli.index_id, col.column_id" +
                        " FROM sys.index_columns coli" +
                        "    INNER JOIN sys.all_columns col on col.object_id=coli.object_id and col.column_id=coli.column_id" +
                        " WHERE coli.object_id in (" + object_ids + ")" +
                        " ORDER BY coli.object_id, coli.index_id, col.column_id" +
                        "";
                    command41.Connection = connection;

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
                                object_id = long.Parse(reader41.GetValue(1).ToString()),
                                index_id = int.Parse(reader41.GetValue(2).ToString()),
                                column_id = int.Parse(reader41.GetValue(3).ToString()),
                            };
                            //ind.index_columns.Add(ind_col);
                            index_columns.Add(ind_col);
                            #endregion
                        }
                    }
                    reader41.Close();
                }
            }
            catch (Exception ex)
            { }
        }
    }
}

