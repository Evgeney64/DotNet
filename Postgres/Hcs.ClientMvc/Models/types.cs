﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hcs.Store
{
    public class DataSourceConfiguration
    {
        public string ConnectionString { get; set; }
        public int CommandTimeout { get; set; }
    }

    public class table
    {
        public string name { get; set; }
        public long id { get; set; }
        public List<column> columns { get; set; }

        public List<foreign_key> foreign_keys { get; set; }

        public override string ToString()
        {
            return name;
        }
    }

    public class column
    {
        public string name { get; set; }
        public int column_id { get; set; }

        public void typePostgresSet()
        {
            switch (this.typeSQL)
            {
                case SQLTypes.bigint:
                    this.typePostgres = "bigint";
                    break;
                case SQLTypes.int_type:
                    this.typePostgres = "integer";
                    break;
                case SQLTypes.smallint:
                    this.typePostgres = "smallint";
                    break;
                case SQLTypes.bit:
                    this.typePostgres = "bool";
                    break;
                case SQLTypes.numeric:
                    this.typePostgres = "numeric";
                    break;
                case SQLTypes.money:
                    this.typePostgres = "double precision";
                    break;
                case SQLTypes.decimal_type:
                    this.typePostgres = "numeric(20,10)";
                    break;

                case SQLTypes.varchar:
                case SQLTypes.varbinary:
                case SQLTypes.char_type:
                    if (this.max_length == -1)
                        this.typePostgres = "text";
                    else
                        this.typePostgres = "character varying(" + this.max_length + ")";
                    if (collate != null)
                        this.typePostgres += " COLLATE " + collate;
                    break;
                case SQLTypes.text:
                    this.typePostgres = "text";
                    if (collate != null)
                        this.typePostgres += " COLLATE " + collate;
                    break;
                case SQLTypes.datetime:
                    this.typePostgres = "date";
                    break;
                case SQLTypes.uniqueidentifier:
                    this.typePostgres = "uuid";
                    break;
                default:
                    this.typePostgres = "unknown field";
                    break;
            }

        }
        public int user_type_id { get; set; }
        public string collate { get; set; }
        public int max_length { get; set; }
        public short is_primary_key { get; set; }
        public bool is_nullable { get; set; }
        public bool is_identity { get; set; }


        public string typePostgres { get; set; }
        public SQLTypes typeSQL
        {
            get
            {
                if (user_type_id != 0)
                    return (SQLTypes)this.user_type_id;
                else
                    return SQLTypes.varchar;
            }
        }

        public override string ToString()
        {
            return name + " [" + typePostgres + " (" + max_length + ")]";
        }
    }

    public class foreign_key
    {
        public string fk_name { get; set; }
        public string parent_table { get; set; }
        public string child_table { get; set; }
        public string parent_column { get; set; }
        public string child_column { get; set; }

        public override string ToString()
        {
            return fk_name;
        }
    }

    public enum SQLTypes
    {
        #region
        // select DISTINCT SYSTEM_TYPE_ID from VW_SYS_TABLE_COLUMN order by SYSTEM_TYPE_ID

        None = 0,
        text = 35,          // *
        tinyint = 48,
        smallint = 52,      // *
        int_type = 56,      // *
        money = 60,
        datetime = 61,      // *
        float_type = 62,
        bit = 104,          // 5
        decimal_type = 106, // *
        numeric = 108,      // 28
        bigint = 127,       // *
        varbinary = 165,    // 1
        varchar = 167,      // *
        char_type = 175,    // 32
        nvarchar = 231,
        nchar = 239,        // 2
        xml = 241,           // 1
        uniqueidentifier = 36,           // 1
        #endregion
    };


    public class SysOperation
    {
        public int OperationId { get; set; }
        public string OperationName { get; set; }
        public string Name { get; set; }
        public int PacketSize { get; set; }

        public override string ToString()
        {
            return OperationId + " - " + OperationName;
        }
    }
}