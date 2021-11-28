using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
//using GemBox.Spreadsheet;
//using ICSharpCode.SharpZipLib.Zip;
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

        #region old
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
        #endregion
    }
}

/*
DROP TABLE[zzExcel]
GO
CREATE TABLE [dbo].[zzExcel](


[id][int] IDENTITY(1, 1) NOT NULL,


[name] [varchar](1000) NULL,
	[nom] [varchar](1000) NULL,
	[val1] [varchar](1000) NULL,
	[val2] [varchar](1000) NULL,
	[val3] [varchar](1000) NULL,
	[val4] [varchar](1000) NULL,
	[municipality] [varchar](1000) NULL,
	[village] [varchar](1000) NULL,
	[village_id] [bigint] NULL,
	[type_village] [varchar](1000) NULL,
	[type_village_id] [bigint] NULL,
	[street] [varchar](1000) NULL,
	[street_id] [bigint] NULL,
	[type_street] [varchar](1000) NULL,
	[type_street_id] [bigint] NULL,
	[region] [varchar](1000) NULL,
	[region_id] [bigint] NULL,
 CONSTRAINT[PK_zzExcel] PRIMARY KEY CLUSTERED 
(

    [id] ASC
)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]
) ON[PRIMARY]
GO 
 * */

//select * from zzExcel

