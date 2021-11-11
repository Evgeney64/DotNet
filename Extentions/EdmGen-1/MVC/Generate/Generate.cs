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
        public static ServiceResult CreateResultFile()
        {
            #region
            string base_dir = AppDomain.CurrentDomain.BaseDirectory;
            string input_dir = base_dir.Substring(0, base_dir.IndexOf("EdmGen")) + "EdmGen\\Result\\Input";
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

        public static ServiceResult GenerateEdmClass(DataSourceConfiguration conf)
        {
            #region
            #region _files.txt
            string root = "EdmGen";
            string base_dir = AppDomain.CurrentDomain.BaseDirectory;
            string result_dir = base_dir.Substring(0, base_dir.IndexOf(root)) + root + "\\Result";
            string input_dir = result_dir + "\\Input";
            string output_dir = result_dir + "\\Output";
            string[] files = File.ReadAllLines(input_dir + "//_files.txt");
            { }
            #endregion

            DbInfo info = new DbInfo(conf);
            info.files = files;
            info.GenerateInfo();
            { }

            #region generateOneClass
            string str = "";
            if (info.tables.Count > 0)
            {
                foreach (table tbl in info.tables.OrderBy(ss => ss.name))
                {
                    generateOneClass(output_dir, tbl);
                    str += "<p>" + tbl.name + "</p>";
                }
            }
            #region old
            //if (Directory.Exists(input_dir))
            //{
            //    string[] path_files = Directory.GetFiles(input_dir);
            //    string[] files = path_files.Select(ss => Path.GetFileName(ss)).ToArray();
            //    files = files.Select(ss => ss.Substring(0, ss.Length - 3)).ToArray();
            //}
            #endregion
            #endregion

            return new ServiceResult("<p>Сформированы классы</p><br>" + str);
            #endregion
        }
    }
}

