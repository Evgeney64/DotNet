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
        #region Define
        public string[] files = null;

        //string user = "\"gis_hcs\"";
        //string schem = "\"gis_hcs\"";
        //string table_space = "\"pg_default\"";
        string collate = "pg_catalog.\"default\"";

        public List<table> tables = new List<table>();
        public List<string> tables_str = new List<string>();
        string object_ids = "";

        List<column> columns = new List<column>();
        public List<foreign_key> foreign_keys = new List<foreign_key>();
        List<index> indexes = new List<index>();
        List<index_column> index_columns = new List<index_column>();

        DataSourceConfiguration conf;
        #endregion
        public DbInfo(DataSourceConfiguration _conf)
        {
            conf = _conf;
        }

        public void GenerateInfo()
        {
            #region
            using (SqlConnection connection = new SqlConnection(conf.ConnectionString))
            {
                connection.Open();

                #region Read data
                Console.WriteLine("Get table .......................................");
                createTables(connection);

                createColumns(connection);
                createFKs(connection);
                createIndexes(connection);
                createIndexColumns(connection);
                { }
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
                                //SysOperation sop = new SysOperation
                                //{
                                //    OperationId = int.Parse(id.ToString()),
                                //    OperationName = reader4.GetValue(1).ToString(),
                                //    Name = reader4.GetValue(2).ToString(),
                                //    PacketSize = int.Parse(id.ToString()),
                                //};
                                //sysOperations.Add(sop);
                            }
                        }
                    }
                }
                #endregion
            }
            #endregion
        }

        public void GenerateInfo_Index()
        {
            #region
            int count = 1;
            foreach (table tbl in tables)
            {
                List<index> _indexes = indexes
                    .Where(ss => ss.object_id == tbl.id).OrderBy(ss => ss.index_id)
                    .ToList();
                tbl.indexes.AddRange(_indexes);
                foreach (index ind in _indexes)
                {
                    string ind_name_str = ind.index_name;
                    if (ind_name_str.Substring(0, 6) == "%_%_FK")
                        ind_name_str = "IX_FK_" + tbl.name;
                    if (indexes.Where(ss => ss.index_name == ind_name_str && ss.nom != ind.nom).Count() > 0)
                    {
                        ind.index_name += "_" + count;
                        count++;
                    }
                    ind.index_columns.AddRange(index_columns
                        .Where(ss => ss.object_id == ind.object_id && ss.index_id == ind.index_id)
                        .OrderBy(ss => ss.index_id));
                }
                Console.WriteLine("[index] - " + tbl.nom + " - " + tbl.name);
            }
            #endregion
        }

        public void GenerateInfo_Fk()
        {
            #region parents / parents
            foreach (table tbl in tables)
            {
                tbl.columns.AddRange(columns.Where(ss => ss.object_id == tbl.id).OrderBy(ss => ss.column_id));

                foreach (foreign_key fk in foreign_keys.Where(ss => ss.this_table == tbl.name))
                    fk.this_table1 = tbl;
                foreach (foreign_key fk in foreign_keys.Where(ss => ss.ref_table == tbl.name))
                    fk.ref_table1 = tbl;

                Console.WriteLine("[fk] - " + tbl.nom + " - " + tbl.name);
            }
            foreach (table tbl in tables)
            {
                tbl.parents = foreign_keys.Where(ss => ss.this_table == tbl.name).ToList();
                tbl.children = foreign_keys.Where(ss => ss.ref_table == tbl.name).ToList();
            }
            #endregion

            #region fk_nom
            List<table> ref_tables = foreign_keys.Select(ss => ss.ref_table1).Distinct().ToList();
            { }
            foreach (table ref_table in ref_tables)
            {
                List<table> this_tables = foreign_keys
                    .Where(ss => ss.ref_table1 == ref_table)
                    .Select(ss => ss.this_table1).Distinct()
                    .ToList();
                { }
                foreach (table this_table in this_tables)
                {
                    List<foreign_key> children = foreign_keys
                        .Where(ss => ss.ref_table1 == ref_table && ss.this_table1 == this_table)
                        .ToList();
                    if (children.Count == 0)
                        continue;

                    if (children.Count > 1 || ref_table == this_table)
                    {
                        int nom = 0;
                        column col = this_table.columns.Where(ss => ss.name == ref_table.name).FirstOrDefault();
                        if (ref_table == this_table)
                            nom = 1;
                        if (col != null)
                        {
                            col.name += "1";
                            col.attr_name = ref_table.name;
                            nom = 2;
                        }
                        foreach (foreign_key fk in children)
                        {
                            if (nom > 0)
                            {
                                fk.fk_nom = nom;
                                if (ref_table == this_table)
                                {
                                    nom++;
                                    fk.fk_nom1 = nom;
                                }
                            }
                            nom++;
                        }
                    }
                }
            }
            #endregion
        }
    }
}
