using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tsb.Model
{
    public class ServiceResult
    {
        #region
        public ServiceResult(string message) { Message = message; }
        public ServiceResult(string message, bool error)
        {
            Error = error;
            if (error == false)
                Message = message;
            else
                ErrorMessage = message;
        }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
        public bool Error { get; set; }
        #endregion
    }

    public class DataSourceConfiguration
    {
        public string ConnectionString { get; set; }
        public int CommandTimeout { get; set; }
        public bool is_postgres { get; set; }
    }

    public class table
    {
        public string name { get; set; }
        public long id { get; set; }

        public int nom { get; set; }
        public string fk_name { get; set; }
        public string fk_name_nom { get; set; }
        public int? fk_nom { get; set; }


        public List<column> columns { get; set; }
        public List<foreign_key> foreign_keys { get; set; }
        public List<index> indexes { get; set; }

        public List<table> parents { get; set; }
        public List<table> children { get; set; }

        public override string ToString()
        {
            return name;
        }
    }

    public class column
    {
        public long object_id { get; set; }
        public string name { get; set; }
        public string attr_name { get; set; }
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
                case SQLTypes.tinyint:
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
                case SQLTypes.xml:
                case SQLTypes.text:
                    this.typePostgres = "text";
                    if (collate != null)
                        this.typePostgres += " COLLATE " + collate;
                    break;
                case SQLTypes.datetime:
                    this.typePostgres = "timestamp";
                    break;
                case SQLTypes.uniqueidentifier:
                    //this.typePostgres = "uuid UNIQUE";
                    this.typePostgres = "uuid";
                    break;
                default:
                    this.typePostgres = "unknown field";
                    break;
            }

        }
        public void typeClrSet()
        {
            switch (this.typeSQL)
            {
                case SQLTypes.bigint:
                    if (!this.is_nullable)
                        typeClr = typeof(long);
                    else
                        typeClr = typeof(long?);
                    break;
                case SQLTypes.int_type:
                    if (!this.is_nullable)
                        typeClr = typeof(int);
                    else
                        typeClr = typeof(int?);
                    break;
                case SQLTypes.smallint:
                    if (!this.is_nullable)
                        typeClr = typeof(short);
                    else
                        typeClr = typeof(short?);
                    break;
                case SQLTypes.tinyint:
                    if (!this.is_nullable)
                        typeClr = typeof(byte);
                    else
                        typeClr = typeof(byte?);
                    break;
                case SQLTypes.bit:
                    if (!this.is_nullable)
                        typeClr = typeof(bool);
                    else
                        typeClr = typeof(bool?);
                    break;
                case SQLTypes.numeric:
                case SQLTypes.money:
                case SQLTypes.decimal_type:
                    if (!this.is_nullable)
                        typeClr = typeof(decimal);
                    else
                        typeClr = typeof(decimal?);
                    break;
                case SQLTypes.varchar:
                case SQLTypes.varbinary:
                case SQLTypes.char_type:
                    typeClr = typeof(string);
                    break;
                case SQLTypes.xml:
                case SQLTypes.text:
                    typeClr = typeof(string);
                    break;
                case SQLTypes.datetime:
                    if (!this.is_nullable)
                        typeClr = typeof(DateTime);
                    else
                        typeClr = typeof(DateTime?);
                    break;
                case SQLTypes.uniqueidentifier:
                    typeClr = typeof(Guid);
                    break;
                default:
                    typeClr = typeof(string);
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
        public Type typeClr { get; set; }
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

    public class index
    {
        public string index_name { get; set; }
        public long object_id { get; set; }
        public int index_id { get; set; }
        public bool is_unique { get; set; }
        public int is_clustered { get; set; }
        public int nom { get; set; }

        public List<index_column> index_columns { get; set; }
        public override string ToString()
        {
            return index_name;
        }
    }

    public class index_column
    {
        public string column_name { get; set; }
        public long object_id { get; set; }
        public int index_id { get; set; }
        public int column_id { get; set; }
        public override string ToString()
        {
            return column_name + " [" + column_id + "]";
        }
    }
    public class foreign_key
    {
        public string fk_name { get; set; }
        public int? fk_nom { get; set; }
        public long object_id { get; set; }
        public string this_table { get; set; }
        public string ref_table { get; set; }
        public string this_column { get; set; }
        public string ref_column { get; set; }

        public table this_table1 { get; set; }
        public table ref_table1 { get; set; }

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
        [Key]
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