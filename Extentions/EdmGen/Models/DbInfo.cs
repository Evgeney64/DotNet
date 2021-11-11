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
        string object_ids = "";

        List<column> columns = new List<column>();
        List<foreign_key> foreign_keys = new List<foreign_key>();
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

                Console.WriteLine("");
                Console.WriteLine("Get metadata .......................................");
                createColumns(connection);
                createFKs(connection);
                createIndexes(connection);
                createIndexColumns(connection);
                { }
                #endregion

                #region Convert data
                int count = 1;
                foreach (table tbl in tables)
                {
                    tbl.columns.AddRange(columns.Where(ss => ss.object_id == tbl.id).OrderBy(ss => ss.column_id));
                    tbl.foreign_keys.AddRange(foreign_keys.Where(ss => ss.object_id == tbl.id));

                    #region index
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
                    #endregion
                    Console.WriteLine("[meta] - " + tbl.nom + " - " + tbl.name);
                }
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

        public void GenerateInfoFk()
        {
            #region
            foreach (table tbl in tables.Where(ss => ss.foreign_keys.Count() > 0))
            {
                foreach (foreign_key fk in tbl.foreign_keys)
                {
                    table ref_table = tables.Where(ss => ss.name == fk.ref_table).FirstOrDefault();
                    if (ref_table != null)
                    {
                        tbl.parents.Add(new table { name = fk.ref_table, fk_name = fk.fk_name });
                        ref_table.children.Add(new table { name = tbl.name, fk_name = fk.fk_name });
                    }
                }
            }
            { }
            foreach (table tbl in tables
                .Where(ss => ss.parents.Count() > 0 || ss.children.Count() > 0)
                .OrderBy(ss => ss.name)
                )
            {
                //List<table> _tables = new List<table>();
                //_tables.AddRange(tbl.parents);
                //_tables.AddRange(tbl.children);
                if (tbl.name == "Partners")
                { }
                foreach (table tbl1 in tbl.parents.OrderBy(ss => ss.name))
                {
                    int i = 0;
                    List<table> _tables = tbl.parents.Where(ss => ss.name == tbl1.name).ToList();
                    foreach (table tbl2 in _tables)
                    {
                        if (i > 0)
                            tbl2.fk_nom = i;
                        i++;
                    }
                }
                if (tbl.name == "Partners")
                { }
                foreach (table tbl1 in tbl.children.OrderBy(ss => ss.name))
                {
                    int i = 0;
                    List<table> _tables = tbl.children.Where(ss => ss.name == tbl1.name).ToList();
                    foreach (table tbl2 in _tables)
                    {
                        if (i > 0)
                            tbl2.fk_nom = i;
                        i++;
                    }
                }
            }
            #endregion
        }
    }
}
