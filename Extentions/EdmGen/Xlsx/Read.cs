using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GemBox.Spreadsheet;
using ICSharpCode.SharpZipLib.Zip;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using Server.Core.Context;
using Server.Core.Model;
using Tsb.Model;

namespace EdmGen
{
    public class XlsxHelper
    {
        public static void ReadXlsx()
        {
            string path_name = @"c:\Disk_D\_Dot_Net\DotNet\Extentions\EdmGen\Xlsx\Data\";
            DataSourceConfiguration conf = Configuration.GetDataSourceConfiguration("EdmGen", "config.json", "MsSqlConfiguration");
            if (conf == null)
                return;

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
        private static void readXlsx(string path_name, string file_name, EntityContext context)
        {
            //DataSourceConfiguration conf = Configuration.GetDataSourceConfiguration("EdmGen", "config.json", "MsSqlConfiguration");
            //if (conf == null)
            //    return;

            //string path_name = @"c:\Disk_D\_Dot_Net\DotNet\Extentions\EdmGen\Xlsx\Data\";
            //string file_name = "Алексеевский.xlsx";

            //HSSFWorkbook xls;
            IWorkbook xlsx;
            using (FileStream file = new FileStream(path_name + file_name, FileMode.Open, FileAccess.Read))
            {
                //xls = new HSSFWorkbook(file);
                xlsx = new XSSFWorkbook(file);
            }

            //ISheet sheet = xls.GetSheet("Лист1");
            ISheet sheet = xlsx.GetSheetAt(0);
            for (int row = 6; row <= sheet.LastRowNum; row++)
            {
                if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                {
                    zzExcel item = new zzExcel
                    {
                        name = Path.GetFileNameWithoutExtension(file_name),
                    };
                    if (sheet.GetRow(row).GetCell(1) != null)
                    {
                        if (sheet.GetRow(row).GetCell(1).CellType == CellType.String)
                            item.val1 = sheet.GetRow(row).GetCell(1).StringCellValue;
                        else if (sheet.GetRow(row).GetCell(1).CellType == CellType.Numeric)
                            item.val1 = sheet.GetRow(row).GetCell(1).NumericCellValue.ToString();
                    }
                    if (sheet.GetRow(row).GetCell(2) != null)
                        item.val2 = sheet.GetRow(row).GetCell(2).StringCellValue;
                    if (sheet.GetRow(row).GetCell(3) != null)
                        item.val3 = sheet.GetRow(row).GetCell(3).StringCellValue;
                    if (sheet.GetRow(row).GetCell(4) != null)
                        item.val4 = sheet.GetRow(row).GetCell(4).StringCellValue;

                    context.zzExcel.Add(item);
                }
            }
        }

        private static string readOds(Stream fileStream)
        {
            var contentXml = "";

            using (var zipInputStream = new ZipInputStream(fileStream))
            {
                ZipEntry contentEntry = null;
                while ((contentEntry = zipInputStream.GetNextEntry()) != null)
                {
                    if (!contentEntry.IsFile)
                        continue;
                    if (contentEntry.Name.ToLower() == "content.xml")
                        break;
                }

                if (contentEntry.Name.ToLower() != "content.xml")
                {
                    throw new Exception("Cannot find content.xml");
                }

                var bytesResult = new byte[] { };
                var bytes = new byte[2000];
                var i = 0;

                while ((i = zipInputStream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    var arrayLength = bytesResult.Length;
                    Array.Resize<byte>(ref bytesResult, arrayLength + i);
                    Array.Copy(bytes, 0, bytesResult, arrayLength, i);
                }
                contentXml = Encoding.UTF8.GetString(bytesResult);
            }
            return contentXml;
        }
        private static void readOd1s(string file_name)
        {
            DataSourceConfiguration conf = Configuration.GetDataSourceConfiguration("EdmGen", "config.json", "MsSqlConfiguration");
            if (conf == null)
                return;

            //string path_name = @"c:\Disk_D\_Dot_Net\DotNet\Extentions\EdmGen\Xlsx\Data\";
            //string file_name = "Алексеевский район.ods";

            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            var workbook = ExcelFile.Load(file_name);

            var sb = new StringBuilder();

            // Iterate through all worksheets in an Excel workbook.
            foreach (var worksheet in workbook.Worksheets)
            {
                //sb.AppendLine();
                //sb.AppendFormat("{0} {1} {0}", new string('-', 25), worksheet.Name);

                // Iterate through all rows in an Excel worksheet.
                using (EntityContext context = new EntityContext(conf.ConnectionString))
                {
                    foreach (var row in worksheet.Rows)
                    {
                        zzExcel item = new zzExcel();
                        //sb.AppendLine();

                        // Iterate through all allocated cells in an Excel row.
                        int i = 0;
                        foreach (var cell in row.AllocatedCells)
                        {
                            if (cell.Value == null)
                                break;
                            string val = cell.Value.ToString();
                            i++;
                            if (i == 1)
                            {
                                int nom = 0;
                                if (int.TryParse(val, out nom) == false)
                                    break;
                                continue;
                            }
                            switch (i)
                            {
                                case 1: item.val1 = val; break;
                                case 2: item.val2 = val; break;
                                case 3: item.val3 = val; break;
                                case 4: item.val4 = val; break;
                                case 5: item.val5 = val; break;
                            }
                            context.zzExcel.Add(item);
                            //if (cell.ValueType != CellValueType.Null)
                            //    sb.Append(string.Format("{0} [{1}]", cell.Value, cell.ValueType).PadRight(25));
                            //else
                            //    sb.Append(new string(' ', 25));
                        }
                    }
                    context.SaveChanges();
                }
            }

            Console.WriteLine(sb.ToString());
        }
    }
}
