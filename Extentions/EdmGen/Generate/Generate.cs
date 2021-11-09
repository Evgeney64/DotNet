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
    public class EdmGenerator
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
            string base_dir = AppDomain.CurrentDomain.BaseDirectory;
            string input_dir = base_dir.Substring(0, base_dir.IndexOf("EdmGen")) + "EdmGen\\Result\\Input";
            string output_dir = base_dir.Substring(0, base_dir.IndexOf("EdmGen")) + "EdmGen\\Result\\Output";
            string[] files = File.ReadAllLines(input_dir + "//_files.txt");
            { }

            //Test test = new Test();
            //test.Execute();

            DbInfo info = new DbInfo(conf);
            info.files = files;
            info.GenerateInfo();
            if (info.tables != null)
            { }
            foreach (string file in files)
            {
                generateOne(output_dir, file, info);
            }
            #region old
            //if (Directory.Exists(input_dir))
            //{
            //    string[] path_files = Directory.GetFiles(input_dir);
            //    string[] files = path_files.Select(ss => Path.GetFileName(ss)).ToArray();
            //    files = files.Select(ss => ss.Substring(0, ss.Length - 3)).ToArray();
            //}
            #endregion
            return new ServiceResult("Файл сохранен");
            #endregion
        }

        public static ServiceResult generateOne(string dir, string name, DbInfo info)
        {
            #region
            table tbl = info.tables.Where(ss => ss.name == name).FirstOrDefault();
            if (tbl == null)
                return new ServiceResult("Недопустимая таблица", true);

            CodeCompileUnit servOneUnit = null;
            CodeNamespace servOneNamespace = null;
            CodeNamespace servOneModelNamespace = null;
            bool is_servFirst = true;
            bool oneFile_Serv = true;
            if (oneFile_Serv)
            {
                servOneUnit = new CodeCompileUnit();
                TsbCodeGenResult item_class = new TsbCodeGenResult
                {
                    Class_Serv = new CodeTypeDeclaration
                    {
                        Name = name,
                        IsPartial = true,
                    },
                    Namespace_Serv = new CodeNamespace("Server.Core.Model"),
                };
                foreach(column col in tbl.columns)
                {
                    col.typeClrSet();
                    CodeMemberField prop = new CodeMemberField
                    {
                        Name = col.name + " { get; set; }",
                        Attributes = MemberAttributes.Public | MemberAttributes.Final,
                        Type = new CodeTypeReference(col.typeClr),
                    };
                    if (col.is_primary_key == 1)
                    {
                        prop.CustomAttributes.Add(new CodeAttributeDeclaration("KeyAttribute"));
                        //prop.CustomAttributes.Add(new CodeAttributeDeclaration(
                        //    "System.ComponentModel.DataAnnotations.KeyAttribute",
                        //    new CodeAttributeArgument(new CodePrimitiveExpression(""))));
                    }
                    item_class.Class_Serv.Members.Add(prop);
                }
                //List<TsbCodeGenResult> res = new List<TsbCodeGenResult>();
                //res.Add(item);
                //foreach (var res_item in res)
                {
                    #region
                    #region interfaces
                    item_class.Class_Serv.BaseTypes.Add("IEntityObject");
                    item_class.Class_Serv.BaseTypes.Add("IEntityLog");
                    item_class.Class_Serv.BaseTypes.Add("IEntityPeriod");
                    #endregion

                    if (!item_class.Is_Error)
                    {
                        if ((oneFile_Serv) && (item_class.Class_Serv != null))
                        {
                            if (is_servFirst)
                            {
                                //servOneUnit.Namespaces.Add(res_item.Namespace_Serv_Using);
                                servOneNamespace = item_class.Namespace_Serv;
                                // !!!
                                servOneNamespace.Types.Add(item_class.Class_Serv);
                                //
                                is_servFirst = false;
                            }


                            // !!!
                            ////servOneNamespace.Types.Add(res_item.Class_Serv);
                            //if (item_class.Class_Serv_Entity != null)
                            //    servOneNamespace.Types.Add(item_class.Class_Serv_Entity);
                            //if (item_class.Class_Serv_IContextWithGroup != null)
                            //    servOneNamespace.Types.Add(item_class.Class_Serv_IContextWithGroup);

                            //if (item_class.Class_Serv_Metadata != null)
                            //{
                            //    if (servOneModelNamespace == null)
                            //    {
                            //        servOneModelNamespace = item_class.Namespace_Model;
                            //    }
                            //    servOneModelNamespace.Types.Add(item_class.Class_Serv_Metadata);
                            //}

                            //if (item_class.Class_Serv_Tree != null)
                            //{
                            //    if (servOneModelNamespace == null)
                            //    {
                            //        servOneModelNamespace = item_class.Namespace_Model;
                            //    }
                            //    servOneModelNamespace.Types.Add(item_class.Class_Serv_Tree);
                            //}

                            //if (item_class.Class_Serv_Edm != null)
                            //{
                            //    if (servOneModelNamespace == null)
                            //    {
                            //        servOneModelNamespace = item_class.Namespace_Model;
                            //    }
                            //    servOneModelNamespace.Types.Add(item_class.Class_Serv_Edm);
                            //}

                            item_class.FilePath_Serv = dir + "//" + name + ".cs";
                        }
                    }
                    #endregion
                }

                if (servOneNamespace != null)
                {
                    servOneNamespace.Imports.Add(new CodeNamespaceImport("System"));
                    servOneNamespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
                    servOneNamespace.Imports.Add(new CodeNamespaceImport("System.ComponentModel.DataAnnotations"));
                    servOneNamespace.Imports.Add(new CodeNamespaceImport("System.ComponentModel.DataAnnotations.Schema"));
                    servOneNamespace.Imports.Add(new CodeNamespaceImport("Server.Core.Public"));

                    servOneUnit.Namespaces.Add(servOneNamespace);
                    //CodeTypeReference typeRef1 = new CodeTypeReference("System.DateTime");

                    if (servOneModelNamespace != null)
                    {
                        servOneUnit.Namespaces.Add(servOneModelNamespace);
                    }

                    #region save Edm class
                    string codeFileName_Serv = dir + "//" + name + ".cs";
                    using (var outFile = File.Open(codeFileName_Serv, FileMode.Create))
                    using (var fileWriter = new StreamWriter(outFile))
                    using (var indentedTextWriter = new IndentedTextWriter(fileWriter, "    "))
                    {
                        var provider = new Microsoft.CSharp.CSharpCodeProvider();
                        provider.GenerateCodeFromCompileUnit(servOneUnit,
                            indentedTextWriter,
                            new CodeGeneratorOptions() { BracingStyle = "C" });
                    }
                    #endregion
                }
            }

            return new ServiceResult("Файл сохранен");
            #endregion
        }
    }

}

