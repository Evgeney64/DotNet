using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using Server.Core.Context;
using Server.Core.Model;
using Tsb.Model;

namespace EdmGen
{
    public partial class XlsxHelper
    {
        public static void ReadXlsx()
        {
            string path_name = @"c:\Disk_D\_Dot_Net\DotNet\Extentions\EdmGen\Xlsx\Result\";
            DataSourceConfiguration conf = Configuration.GetDataSourceConfiguration("EdmGen", "config.json", "MsSqlConfiguration");
            if (conf == null)
                return;

            using (SqlConnection connection = new SqlConnection(conf.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("truncate table zzExcel", connection))
                {
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }

            if (Directory.Exists(path_name))
            {
                string[] path_files = Directory.GetFiles(path_name);
                string[] files = path_files.Select(ss => Path.GetFileName(ss)).ToArray();
                //files = files
                //    .Where(ss => ss != "_files.txt")
                //    .Select(ss => ss.Substring(0, ss.Length - 3))
                //    .ToArray();
                foreach (string file in files)
                {
                    using (EntityContext context = new EntityContext(conf.ConnectionString))
                    {
                        if (Path.GetExtension(file) == ".xlsx")
                            readXlsx(path_name , file, context);
                        //else if (Path.GetExtension(file) == ".ods")
                        //    readOds(path_name + file);
                        context.SaveChanges();
                    }
                }
            }
        }
        private static void readXlsx(string path_name, string file_name_ext, EntityContext context)
        {
            string file_name = Path.GetFileNameWithoutExtension(file_name_ext);
            Console.WriteLine("- " + file_name);

            //HSSFWorkbook xls;
            IWorkbook xlsx;
            using (FileStream file = new FileStream(path_name + file_name_ext, FileMode.Open, FileAccess.Read))
            {
                //xls = new HSSFWorkbook(file);
                xlsx = new XSSFWorkbook(file);
            }

            //ISheet sheet = xls.GetSheet("Лист1");
            ISheet sheet = xlsx.GetSheetAt(0);
            IRow row0 = sheet.GetRow(0);
            IRow row1 = sheet.GetRow(1);
            IRow row2 = sheet.GetRow(2);
            { }
            for (int rowi = 0; rowi <= sheet.LastRowNum; rowi++)
            {
                if (sheet.GetRow(rowi) != null) //null is when the row only contains empty cells 
                {
                    zzExcel item = new zzExcel
                    {
                        name = file_name,
                    };
                    if (sheet.GetRow(rowi).GetCell(0) != null)
                    {
                        string nomc = "";
                        if (sheet.GetRow(rowi).GetCell(0).CellType == CellType.String)
                            nomc = sheet.GetRow(rowi).GetCell(0).StringCellValue;
                        else if (sheet.GetRow(rowi).GetCell(0).CellType == CellType.Numeric)
                            nomc = sheet.GetRow(rowi).GetCell(0).NumericCellValue.ToString();
                        int nom = 0;
                        if (int.TryParse(nomc, out nom))
                            item.nom = nom;
                    }
                    if (sheet.GetRow(rowi).GetCell(1) != null)
                        item.val1 = sheet.GetRow(rowi).GetCell(1).StringCellValue;
                    if (sheet.GetRow(rowi).GetCell(2) != null)
                        item.val2 = sheet.GetRow(rowi).GetCell(2).StringCellValue;
                    if (sheet.GetRow(rowi).GetCell(3) != null)
                        item.val3 = sheet.GetRow(rowi).GetCell(3).StringCellValue;
                    if (sheet.GetRow(rowi).GetCell(4) != null)
                        item.val4 = sheet.GetRow(rowi).GetCell(4).StringCellValue;

                    if (!string.IsNullOrEmpty(item.val1)
                        || !string.IsNullOrEmpty(item.val2)
                        || !string.IsNullOrEmpty(item.val3)
                        || !string.IsNullOrEmpty(item.val4)
                        )
                        context.zzExcel.Add(item);
                }
            }
        }
    }
}
