using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;
using System.Text;

using Tsb.WCF.Web;
using Tsb.WCF.Web.CoreModel;
using System.Xml.Linq;

namespace CreateBase
{
    public class GenerateScript
    {
        public void Start(string[] _args)
        {
            #region args
            string path = Directory.GetCurrentDirectory();
            List<string> table_names = new List<string>();
            XElement xData = new XElement("data");

            if (_args.Count() >= 1)
            {
                string temp = _args[0];
                if (!String.IsNullOrWhiteSpace(temp))
                {
                    path = temp;
                }
                if (!Directory.Exists(path))
                {
                    Console.WriteLine("Не найден путь вывода");
                    return;
                }
            }
            if (_args.Count() >= 2)
            {
                string files = _args[1];
                files = files.Replace("rem", "");
                files = files.Replace("\n", "");
                files = files.Replace("\r", "");

                string[] tables = files.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                table_names.AddRange(tables);
            }
            if (_args.Count() >= 3)
            {
                string dataPath = _args[2];
                try
                {
                    var xDocument = XElement.Load(dataPath);
                    xData = xDocument.Element("data");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
                if (xData == null)
                {
                    Console.WriteLine("Не удалось прочитать файл с данными");
                    return;
                }
            }
            #endregion

            #region Define
            { }
            DateTime date_beg = DateTime.Now;
            string user = "\"BillBerry\"";
            string schem = "\"UAP\"";
            schem = "\"public\"";
            string table_space = "\"BillBerryTS\"";
            string collate = "pg_catalog.\"default\"";
            #endregion

            #region table_types
            List<int?> table_types = new List<int?>();
            table_types.Add((int)SysTableType_Enum.Sys);
            table_types.Add((int)SysTableType_Enum.Nsi);
            table_types.Add((int)SysTableType_Enum.Table);
            table_types.Add((int)SysTableType_Enum.TableExt);
            //table_types.Add((int)SysTableType_Enum.AuthTable);
            #endregion

            #region table_names
            { }
            List<SYS_TABLE> sys_tables = StaticContext.Current.SysTables
                .Where(ss => table_types.Contains(ss.STABLE_TYPE_ID))
                //.Where(ss => ss.STABLE_NAME == "NSI_FACILITY")
                //.Where(ss => table_names.Contains(ss.STABLE_NAME) == false)
                //.Where(ss => table_names.Contains(ss.STABLE_NAME))
                .OrderBy(ss => ss.STABLE_NAME)
                .ToList()
                ;
            { }
            #endregion

            #region string
            string crt = "";
            string ins = "";
            string del = "";
            string trunc = "";

            string crt_ind = "";
            string del_ind = "";

            string crt_fk = "";
            string del_fk = "";

            string script = "";
            #endregion
            Tsb.WCF.Web.SystemSpace.SysTables.TablesServ tablesServ = new Tsb.WCF.Web.SystemSpace.SysTables.TablesServ();
            foreach (SYS_TABLE tbl in sys_tables)
            {
                #region
                #region script
                if (1 == 2)
                {
                    script +=
                        "\n--****************************************************" +
                        "\nIF OBJECT_ID('" + tbl.STABLE_NAME + "') IS NOT NULL AND col_length('" + tbl.STABLE_NAME + "', 'CRT_DATE') IS NULL" +
                        "\n    ALTER TABLE " + tbl.STABLE_NAME + " ADD CRT_DATE DateTime NULL" +
                        "\nGO" +
                        "\nIF OBJECT_ID('" + tbl.STABLE_NAME + "') IS NOT NULL AND col_length('" + tbl.STABLE_NAME + "', 'MFY_DATE') IS NULL" +
                        "\n    ALTER TABLE " + tbl.STABLE_NAME + " ADD MFY_DATE DateTime NULL" +
                        "\nGO" +
                        "\nIF OBJECT_ID('" + tbl.STABLE_NAME + "') IS NOT NULL AND col_length('" + tbl.STABLE_NAME + "', 'MFY_SUSER_ID') IS NULL" +
                        "\n    ALTER TABLE " + tbl.STABLE_NAME + " ADD MFY_SUSER_ID int NULL" +
                        "\nGO"
                        ;
                    continue;
                }
                #endregion

                List<VW_SYS_TABLE_COLUMN> columns = StaticContext.Current.GetSysTableColumns(tbl.STABLE_ID, 0, SysTableColumn_Enum.Table).ToList();
                { }
                string console_output = tbl.STABLE_NAME + " -" + columns.Count() + " кол.;";
                if (tbl.SYS_TYPE != 1)
                    Console.WriteLine(console_output);

                #region CREATE / DROP TABLE
                string table_name = "\"" + tbl.STABLE_NAME + "\"";
                string schem_table_name = schem + "." + table_name;
                //schem_table_name = table_name;
                crt += " \nCREATE TABLE " + schem_table_name;
                crt += " \n(";

                del += " \nDROP TABLE " + schem_table_name + ";";

                bool is_first = true;
                foreach (VW_SYS_TABLE_COLUMN col in columns)
                {
                    if (is_first == false)
                        crt += " ,";
                    crt += " \n   \"" + col.STABLE_COLUMN_NAME+ "\"";
                    #region TypeSQL
                    if (col.STABLE_COLUMN_ID == 1468)
                    { }
                    switch (col.TypeSQL)
                    {
                        case SQLTypes.bigint:
                            crt += " bigint";
                            break;
                        case SQLTypes.int_type:
                            crt += " integer";
                            break;
                        case SQLTypes.smallint:
                            crt += " smallint";
                            break;
                        case SQLTypes.bit:
                            crt += " bool";
                            break;
                        case SQLTypes.numeric:
                            crt += " numeric";
                            break;
                        case SQLTypes.money:
                            crt += " double precision";
                            break;
                        case SQLTypes.decimal_type:
                            crt += " numeric(20,10)";
                            break;

                        case SQLTypes.varchar:
                        case SQLTypes.varbinary:
                        case SQLTypes.char_type:
                            if (col.MAX_LENGTH == -1)
                                crt += " text";
                            else
                                crt += " character varying(" + col.MAX_LENGTH + ")";
                            if (collate != null)
                                crt += " COLLATE " + collate;
                            break;
                        case SQLTypes.text:
                            crt += " text";
                            if (collate != null)
                                crt += " COLLATE " + collate;
                            break;
                        case SQLTypes.datetime:
                            crt += " date";
                            break;
                        case SQLTypes.uniqueidentifier:
                            crt += " uuid";
                            break;
                        default:
                            crt += " unknown field";
                            break;
                    }
                    #endregion

                    if (col.IS_PRIMARY_KEY == 1)
                        crt += " PRIMARY KEY";
                    else if (col.IS_NULLABLE == 0)
                        crt += " NOT NULL";

                    if (col.IS_IDENTITY == 1)
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
                crt += ";\n\n";
                #endregion

                #region CREATE / DROP INDEX
                List<C__sys_indexes> sys_indexes = tablesServ.get_sys_indexes().Where(ss => ss.table_name == tbl.STABLE_NAME).ToList();
                if (sys_indexes.Count() > 0)
                {
                    List<int> index_ids = sys_indexes.Select(ss => ss.index_id).Distinct().ToList();
                    foreach (int index_id in index_ids)
                    {
                        List<C__sys_indexes> sys_index_columns = sys_indexes.Where(ss => ss.index_id == index_id).ToList();
                        bool is_first_ind = true;
                        foreach (C__sys_indexes ind in sys_index_columns)
                        {
                            if (ind.index_name == "IX_DOCUMENT_DETAIL_DOCUMENT_NSI_DOCUMENT_DETAIL")
                            { }
                            if (ind.index_name.Substring(0, 4) == "%_%_")
                                continue;
                            
                            if (is_first_ind)
                            {
                                string is_unique = "";
                                if (ind.is_unique == true)
                                    is_unique = "UNIQUE";

                                crt_ind += " \nCREATE " + is_unique + " INDEX \"" + ind.index_name + "\"" +
                                    " ON " + schem_table_name + " USING btree";
                                crt_ind += "\n(";

                                del_ind += " \nDROP  INDEX " + schem + ".\"" + ind.index_name + "\";";
                            }
                            if (is_first_ind == false)
                                crt_ind += " ,";
                            crt_ind += "\"" + ind.column_name + "\"";

                            is_first_ind = false;
                        }
                        if (is_first_ind == false)
                        {
                            crt_ind += ")";
                            crt_ind += "\nTABLESPACE " + table_space + ";";
                        }
                        #region example
                        /*
CREATE INDEX "PK_BUILD"
    ON "UAP"."BUILD" USING btree
    ("BUILD_ID")
    TABLESPACE "BillBerryTS"; 

CREATE UNIQUE INDEX "PK_BUILD"
    ON "UAP"."BUILD" USING btree
    ("BUILD_ID", "NSTREET_ID" DESC)
    TABLESPACE "BillBerryTS";

DROP INDEX "UAP"."PK_BUILD";

                     * * */
                        #endregion
                    }
                }
                #endregion

                #region CREATE / DROP FK
                List<C__sys_foreign_keys> sys_foreign_keys = tablesServ.get_sys_foreign_keys().Where(ss => ss.child_table == tbl.STABLE_NAME).ToList();
                if (sys_foreign_keys.Count() > 0)
                {
                    foreach (C__sys_foreign_keys fk in sys_foreign_keys)
                    {
                        if (sys_tables.Where(ss => ss.STABLE_NAME == fk.parent_table).Count() == 0)
                            continue;

                        crt_fk += 
                            "\nALTER TABLE " + schem_table_name +
                            "\n     ADD CONSTRAINT \"" + fk.fk_name + "\" FOREIGN KEY (\"" + fk.child_column + "\")" +
                            "\n     REFERENCES " + schem + ".\"" + fk.parent_table + "\" (\"" + fk.parent_column + "\") MATCH SIMPLE" +
                            "\n     ON UPDATE NO ACTION" +
                            "\n     ON DELETE NO ACTION;";

                        del_fk += "\nALTER TABLE " + schem_table_name + " DROP CONSTRAINT \"" + fk.fk_name + "\" CASCADE ;";
                        #region example
                        /*
ALTER TABLE products ADD FOREIGN KEY (product_group_id)
REFERENCES product_groups;

ALTER TABLE "UAP"."CALC_EXT"
    ADD CONSTRAINT fk_1 FOREIGN KEY ("CALC_ID")
    REFERENCES "UAP"."CALC" ("CALC_ID") MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;

                     * * */
                        #endregion
                    }
                }
                #endregion

                //continue;

                #region INSERT INTO
                /*
                if (tbl.STABLE_NAME == "BUILD" || tbl.STABLE_NAME == "NSI_STREET_TYPE")
                { }
                if (tbl.SYS_TYPE != 1 && !table_names.Contains(tbl.STABLE_NAME) && !xData.Elements(tbl.STABLE_NAME).Any())
                {
                    continue;
                }

                trunc += " \nTRUNCATE TABLE " + schem_table_name + ";";

                string ins_fi = "";
                string ins_val = "";
                if (tbl.STABLE_NAME == "NSI_ALGORITHM")
                { }
                // в первую очередь ищем данные в файле
                List<XElement> xRows = xData.Elements(tbl.STABLE_NAME).ToList();
                // если данных нет в файле - берем из БД
                if (xRows.Count == 0)
                {
                    Nsi nsi = StaticContext.GetNsiTableItems(tbl.STABLE_NAME, true);
                    using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                    {
                        nsi.TableItems.TableName = tbl.STABLE_NAME;
                        foreach (DataColumn dataColumn in nsi.TableItems.Columns)
                        {
                            dataColumn.ColumnMapping = MappingType.Attribute;
                        }
                        nsi.TableItems.WriteXml(stream);
                        stream.Position = 0;
                        var xDocument = XElement.Load(stream);
                        xRows = xDocument.Elements().ToList();
                    }
                }

                console_output += "    -" + xRows.Count + " строк";
                Console.WriteLine(console_output);

                foreach (XElement xRow in xRows)
                {
                    ins_fi = " \nINSERT INTO " + schem_table_name + " (";
                    ins_val = "VALUES (";

                    is_first = true;

                    foreach (XAttribute xField in xRow.Attributes())
                    {
                        #region
                        string col_name = xField.Name.LocalName;

                        #region TypeSQL

                        VW_SYS_TABLE_COLUMN col = columns
                            .Where(ss => ss.STABLE_COLUMN_NAME == col_name)
                            .FirstOrDefault();
                        string quot = "'";
                        string valuec = xField.Value.Trim();
                        switch (col.TypeSQL)
                        {
                            case SQLTypes.bigint:
                            case SQLTypes.int_type:
                            case SQLTypes.smallint:
                                quot = "";
                                if (valuec == "")
                                    continue;
                                break;
                            case SQLTypes.numeric:
                            case SQLTypes.decimal_type:
                            case SQLTypes.money:
                                quot = "";
                                if (valuec == "")
                                    continue;
                                //valuec = Public.FloatTypeToStr(value);
                                break;
                            case SQLTypes.bit:
                                //quot = "";
                                //if (valuec == "")
                                //    continue;
                                //else if (valuec.ToLower() == "true")
                                //    valuec = "1";
                                //else if (valuec.ToLower() == "false")
                                //    valuec = "0";
                                break;
                            default:
                                break;
                        }
                        #endregion
                        switch (tbl.STABLE_NAME)
                        {
                            case "SYS_CONFIG":
                                #region
                                if (col_name == "VALUE")
                                {
                                    if (valuec.IndexOf("SQL=\"") >= 0)
                                    {
                                        if (valuec.IndexOf("'") >= 0)
                                        {
                                            valuec = valuec.Replace('\'', '\"');
                                        }
                                    }
                                }
                                break;
                                #endregion
                        }
                        if (is_first == false)
                        {
                            ins_fi += ", ";
                            ins_val += ", ";
                        }
                        ins_fi += "\"" + col_name + "\"";
                        ins_val += quot + valuec + quot;
                        is_first = false;
                        #endregion
                    }

                    ins += " " + ins_fi + ") " + ins_val + ");";
                }
                */
                #endregion

                #region INSERT INTO v2
                if (tbl.STABLE_NAME == "BUILD" || tbl.STABLE_NAME == "NSI_STREET_TYPE")
                { }
                trunc += " \nTRUNCATE TABLE " + schem_table_name + ";";

                if (tbl.SYS_TYPE != 1 && !table_names.Contains(tbl.STABLE_NAME) && !xData.Elements(tbl.STABLE_NAME).Any())
                {
                    continue;
                }

                string ins_fi = "";
                string ins_col = "";
                if (tbl.STABLE_NAME == "NSI_ALGORITHM")
                { }
                // в первую очередь ищем данные в файле
                List<XElement> xRows = xData.Elements(tbl.STABLE_NAME).ToList();
                // если данных нет в файле - берем из БД
                if (xRows.Count == 0)
                {
                    Nsi nsi = StaticContext.Current.GetNsiTableItems(tbl.STABLE_NAME, true);
                    using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                    {
                        if (nsi.TableItems == null)
                            continue;
                        nsi.TableItems.TableName = tbl.STABLE_NAME;
                        foreach (DataColumn dataColumn in nsi.TableItems.Columns)
                        {
                            dataColumn.ColumnMapping = MappingType.Attribute;
                        }
                        nsi.TableItems.WriteXml(stream);
                        stream.Position = 0;
                        var xDocument = XElement.Load(stream);
                        xRows = xDocument.Elements().ToList();
                    }
                }

                console_output += "    -" + xRows.Count + " строк";
                Console.WriteLine(console_output);

                    ins_fi = "";
                    ins_col = "";
                
                    is_first = true;

                foreach (VW_SYS_TABLE_COLUMN col in columns)
                {
                    string type;

                    #region
                    switch (col.TypeSQL)
                    {
                        case SQLTypes.bigint:
                            type = "bigint";
                            break;
                        case SQLTypes.int_type:
                            type = "integer";
                            break;
                        case SQLTypes.smallint:
                            type = "smallint";
                            break;
                        case SQLTypes.bit:
                            type = "bool";
                            break;
                        case SQLTypes.numeric:
                            type = "numeric";
                            break;
                        case SQLTypes.money:
                            type = "double precision";
                            break;
                        case SQLTypes.decimal_type:
                            type = "numeric(20,10)";
                            break;

                        case SQLTypes.varchar:
                        case SQLTypes.varbinary:
                        case SQLTypes.char_type:
                            if (col.MAX_LENGTH == -1)
                                type = "text";
                            else
                                type = "character varying(" + col.MAX_LENGTH + ")";
                            break;
                        case SQLTypes.text:
                            type = "text";
                            break;
                        case SQLTypes.datetime:
                            type = "date";
                            break;
                        case SQLTypes.uniqueidentifier:
                            type = "uuid";
                            break;
                        default:
                            type = "unknown field";
                            break;
                    }
                    #endregion

                    #region
                    if (is_first == false)
                    {
                        ins_fi += ", ";
                        ins_col += ", ";
                    }
                    ins_fi += "\"" + col.STABLE_COLUMN_NAME + "\"";
                    ins_col += col.STABLE_COLUMN_NAME + " " + type + " PATH '@" + col.STABLE_COLUMN_NAME + "'";
                    is_first = false;
                    #endregion
                }

                if (tbl.STABLE_NAME == "SYS_CONFIG")
                {
                    #region
                    foreach (XElement xRow in xRows)
                    {
                        var xValueAttribute = xRow.Attribute("VALUE");
                        if (xValueAttribute != null)
                        {
                            string valuec = xValueAttribute.Value;
                            if (valuec.IndexOf("SQL=\"") >= 0 && valuec.IndexOf("'") >= 0)
                            {
                                valuec = valuec.Replace('\'', '\"');
                                xValueAttribute.Value = valuec;
                            }
                        }
                    }
                    #endregion
                }
                string xTableData = new XElement("root", xRows).ToString();
                ins +=
                    "\nWITH xdata(x) AS(VALUES('" + xTableData + "'::xml))" +
                    "\nINSERT INTO " + schem_table_name + "(" + ins_fi + ")" +
                    "\nSELECT t.*" +
                    "\nFROM XMLTABLE('/root/" + tbl.STABLE_NAME + "' PASSING(SELECT x FROM xdata) COLUMNS " + ins_col + ") t;";
                #endregion

                #region INSERT INTO old
                /*
                if (tbl.STABLE_NAME == "BUILD" || tbl.STABLE_NAME == "NSI_STREET_TYPE")
                { }
                if (tbl.SYS_TYPE != 1 && table_names.Contains(tbl.STABLE_NAME) == false)
                    continue;

                trunc += " \nTRUNCATE TABLE " + schem_table_name + ";";

                string ins_fi = "";
                string ins_val = "";
                if (tbl.STABLE_NAME == "NSI_ALGORITHM")
                { }
                Nsi nsi = StaticContext.GetNsiTableItems(tbl.STABLE_NAME, true);

                console_output += "    -" + nsi.TableItems.Rows.Count + " строк";
                Console.WriteLine(console_output);

                foreach (DataRow row in nsi.TableItems.Rows)
                {
                    ins_fi = " \nINSERT INTO " + schem_table_name + " (";
                    ins_val = "VALUES (";

                    is_first = true;
                    List<object> row_list = row.ItemArray.ToList();
                    if (tbl.STABLE_NAME == "NSI_ALGORITHM")
                    { }
                    int i = 0;

                    if (1 == 2)
                    {
                        #region new
                        //string row_str = string.Join(" ,", row_list.Select(x => "\"" + x + "\""));
                        foreach (object value in row_list)
                        {
                            #region
                            string col_name = nsi.TableItems.Columns[i].ToString();
                            i++;
                            if (row.IsNull(col_name))
                                continue;

                            #region TypeSQL
                            VW_SYS_TABLE_COLUMN col = nsi.TableColumn.Columns
                                .Where(ss => ss.STABLE_COLUMN_NAME == col_name)
                                .FirstOrDefault();
                            string quot = "'";
                            string valuec = value.ToString().Trim();
                            switch (col.TypeSQL)
                            {
                                case SQLTypes.bigint:
                                case SQLTypes.int_type:
                                case SQLTypes.smallint:
                                    quot = "";
                                    if (valuec == "")
                                        continue;
                                    break;
                                case SQLTypes.numeric:
                                case SQLTypes.decimal_type:
                                case SQLTypes.money:
                                    quot = "";
                                    if (valuec == "")
                                        continue;
                                    valuec = Public.FloatTypeToStr(value);
                                    break;
                                case SQLTypes.bit:
                                    //quot = "";
                                    //if (valuec == "")
                                    //    continue;
                                    //else if (valuec.ToLower() == "true")
                                    //    valuec = "1";
                                    //else if (valuec.ToLower() == "false")
                                    //    valuec = "0";
                                    break;
                                default:
                                    break;
                            }
                            #endregion
                            switch (tbl.STABLE_NAME)
                            {
                                case "SYS_CONFIG":
                                    #region
                                    if (col_name == "VALUE")
                                    {
                                        if (valuec.IndexOf("SQL=\"") >= 0)
                                        {
                                            if (valuec.IndexOf("'") >= 0)
                                            {
                                                valuec = valuec.Replace('\'', '\"');
                                            }
                                        }
                                    }
                                    break;
                                    #endregion
                            }
                            if (is_first == false)
                            {
                                ins_fi += ", ";
                                ins_val += ", ";
                            }
                            ins_fi += "\"" + col_name + "\"";
                            ins_val += quot + valuec + quot;
                            is_first = false;
                            #endregion
                        }
                        #endregion
                    }

                    if (1 == 1)
                    {
                        #region old
                        foreach (VW_SYS_TABLE_COLUMN col in nsi.TableColumn.BaseColumns)
                        {
                            #region old
                            if (row.IsNull(col.STABLE_COLUMN_NAME))
                                continue;
                            object value = row[col.STABLE_COLUMN_NAME];

                            #region TypeSQL
                            string quot = "'";
                            string valuec = value.ToString().Trim();
                            switch (col.TypeSQL)
                            {
                                case SQLTypes.bigint:
                                case SQLTypes.int_type:
                                case SQLTypes.smallint:
                                    quot = "";
                                    if (valuec == "")
                                        continue;
                                    break;
                                case SQLTypes.numeric:
                                case SQLTypes.decimal_type:
                                case SQLTypes.money:
                                    quot = "";
                                    if (valuec == "")
                                        continue;
                                    valuec = Public.FloatTypeToStr(value);
                                    break;
                                case SQLTypes.bit:
                                    //quot = "";
                                    //if (valuec == "")
                                    //    continue;
                                    //else if (valuec.ToLower() == "true")
                                    //    valuec = "1";
                                    //else if (valuec.ToLower() == "false")
                                    //    valuec = "0";
                                    break;
                                default:
                                    break;
                            }
                            #endregion
                            switch (tbl.STABLE_NAME)
                            {
                                case "SYS_CONFIG":
                                    #region
                                    if (col.STABLE_COLUMN_NAME == "VALUE")
                                    {
                                        if (valuec.IndexOf("SQL=\"") >= 0)
                                        {
                                            if (valuec.IndexOf("'") >= 0)
                                            {
                                                valuec = valuec.Replace('\'', '\"');
                                            }
                                        }
                                    }
                                    break;
                                    #endregion
                            }
                            if (is_first == false)
                            {
                                ins_fi += ", ";
                                ins_val += ", ";
                            }
                            ins_fi += "\"" + col.STABLE_COLUMN_NAME + "\"";
                            ins_val += quot + valuec + quot;
                            is_first = false;
                            #endregion
                        }
                        #endregion
                    }

                    ins += " " + ins_fi + ") " + ins_val + ");";
                }
                */
                #endregion
                #endregion
            }

            #region Save
            { }
            writeToFile(path, "create_table.sql", crt, true);
            writeToFile(path, "insert_data.sql", ins);
            writeToFile(path, "drop_table.sql", del, true);
            writeToFile(path, "trunc_data.sql", trunc);

            writeToFile(path, "create_index.sql", crt_ind);
            writeToFile(path, "drop_index.sql", del_ind);

            writeToFile(path, "create_fk.sql", crt_fk);
            writeToFile(path, "drop_fk.sql", del_fk);

            writeToFile(path, "script.sql", script);
            #endregion

            string date_diff = Public.TimeSpanFormat(DateTime.Now - date_beg);
            Console.WriteLine("Время выполнения " + date_diff);
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

    class TableName
    {
        public string name { get; set; }
        public int rows { get; set; }
    }
}


