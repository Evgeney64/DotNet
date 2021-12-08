using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;

using Tsb.Model;

namespace Tsb.Generate
{
    public partial class EdmGenerator
    {
        public static ServiceResult CreateResultFile(string client_path)
        {
            #region
            string base_dir = AppDomain.CurrentDomain.BaseDirectory;
            string input_dir = base_dir.Substring(0, base_dir.IndexOf(client_path)) + client_path + "\\Result\\Input";
            if (Directory.Exists(input_dir))
            {
                string[] path_files = Directory.GetFiles(input_dir);
                string[] files = path_files.Select(ss => Path.GetFileName(ss)).ToArray();
                files = files
                    .Where(ss => ss != "_files.txt")
                    .Select(ss => ss.Substring(0, ss.Length - 3))
                    .ToArray();

                File.WriteAllLines(input_dir + "//_files.txt", files);
                return new ServiceResult("Файл сохранен");
            }
            return new ServiceResult("Файл не сохранен", true);
            #endregion
        }

        public static ServiceResult CreateKendoIconsFile(string client_path)
        {
            #region
            string base_dir = AppDomain.CurrentDomain.BaseDirectory;
            string input_dir = base_dir.Substring(0, base_dir.IndexOf(client_path)) + client_path + "\\kendo";
            if (Directory.Exists(input_dir))
            {
                string[] path_files = Directory.GetFiles(input_dir);
                List<string> icons = new List<string>();
                foreach (string file in path_files)
                {
                    string[] content = File.ReadAllLines(file);
                    foreach (string str in content)
                    {
                        if (str.Length > 0)
                        { }
                        int ind = 0;
                        while (1 == 1)
                        {
                            string pref = "";
                            //pref = "{background-position:-80px -48px}.";
                            ind = str.IndexOf(pref + "k-i-", ind);
                            //ind = str.IndexOf("k-i-", ind);
                            if (ind == -1)
                                break;

                            ind += pref.Length;
                            int ind1 = str.IndexOf(",", ind + 1);
                            if (ind1 - ind > 20) ind1 = str.IndexOf(":", ind + 1);
                            if (ind1 - ind > 20) ind1 = str.IndexOf(".", ind + 1);
                            if (ind1 - ind > 20) ind1 = str.IndexOf("{", ind + 1);
                            if (ind1 == -1)
                                break;

                            string icon = str.Substring(ind, ind1 - ind);
                            ind1 = icon.IndexOf("{");
                            if (ind1 != -1) icon = icon.Substring(0, ind1 - 1);
                            ind1 = icon.IndexOf(":");
                            if (ind1 != -1) icon = icon.Substring(0, ind1 - 1);
                            ind1 = icon.IndexOf(",");
                            if (ind1 != -1) icon = icon.Substring(0, ind1 - 1);
                            ind1 = icon.IndexOf(".");
                            if (ind1 != -1) icon = icon.Substring(0, ind1 - 1);

                            icon = icon.Trim();
                            if (icon == "k-i-close{display")
                            { }

                            if (icons.Contains(icon) == false)
                                icons.Add(icon);
                            ind += 20;
                        }
                    }
                }

                List<string> items = icons.OrderBy(ss => ss)
                    .Select(ss => "<Item isAuto=\"true\" Title=\"" + ss + "\" Table=\"DEAL_EXT\" Field=\"BUYER_ID\" Control=\"Selector\" Content=\"PARTNER\" IsNavigatable=\"True\" IconNavigate=\"" + ss + " Width=\"Small\" />")
                    .ToList();


                File.WriteAllLines(input_dir + "//icons.txt", icons.ToArray());
                File.WriteAllLines(input_dir + "//items.txt", items.ToArray());

                return new ServiceResult("Файл сохранен");
            }
            return new ServiceResult("Файл не сохранен", true);
            #endregion
        }

        #region Define
        string user = "\"gis_hcs\"";
        string schem = "\"gis_hcs\"";
        string table_space = "\"pg_default\"";
        string collate = "pg_catalog.\"default\"";

        List<table> tables = new List<table>();
        List<column> columns = new List<column>();
        List<foreign_key> foreign_keys = new List<foreign_key>();
        List<index> indexes = new List<index>();
        List<index_column> index_columns = new List<index_column>();


        List<SysOperation> sysOperations = new List<SysOperation>();

        string crt = "";
        string del = "";
        string crt_fk = "";
        string del_fk = "";
        string crt_ind = "";
        string del_ind = "";
        string ins = "";
        int count0 = 0;
        DataSourceConfiguration conf;
        #endregion

        public static ServiceResult GenerateEdmClass(DataSourceConfiguration conf, string client_path)
        {
            #region

            #region _files.txt
            string root = client_path;
            string base_dir = AppDomain.CurrentDomain.BaseDirectory;
            string result_dir = base_dir.Substring(0, base_dir.IndexOf(root)) + root + "\\Result";
            string input_dir = result_dir + "\\Input";
            string output_dir = result_dir + "\\Output";
            string[] files = File.ReadAllLines(input_dir + "//_files.txt");

            #region old
            //if (Directory.Exists(input_dir))
            //{
            //    string[] path_files = Directory.GetFiles(input_dir);
            //    string[] files = path_files.Select(ss => Path.GetFileName(ss)).ToArray();
            //    files = files.Select(ss => ss.Substring(0, ss.Length - 3)).ToArray();
            //}
            #endregion
            #endregion

            #region Clear output dir
            if (Directory.Exists(output_dir))
            {
                string[] path_files = Directory.GetFiles(output_dir);
                string[] output_files = path_files.Select(ss => Path.GetFileName(ss)).ToArray();
                foreach (string file in output_files)
                    File.Delete(output_dir + "//" + file);
            }
            #endregion

            #region DbInfo
            DbInfo info = new DbInfo(conf);
            info.files = files;
            info.GenerateInfo();

            Console.WriteLine("");
            Console.WriteLine("Get indexes .......................................");
            info.GenerateInfo_Index();

            Console.WriteLine("");
            Console.WriteLine("Get foreign_keys .......................................");
            info.GenerateInfo_Fk();
            { }
            #endregion

            #region generateOneClass
            Console.WriteLine("");
            Console.WriteLine("Generate classes .......................................");
            if (info.tables.Count > 0)
            {
                foreach (table tbl in info.tables.OrderBy(ss => ss.name))
                {
                    generateOneClass(output_dir, info, tbl);
                    Console.WriteLine("[gen] - " + tbl.nom + " - " + tbl.name);
                }
            }
            Console.WriteLine("");
            Console.WriteLine("Generate context ...");
            generateContextClass(output_dir, info);

            Console.WriteLine("");
            Console.WriteLine("Generate service ...");
            generateServiceClass(output_dir, info);
            #endregion

            return new ServiceResult("Сформированы классы");
            #endregion
        }
    }
}

