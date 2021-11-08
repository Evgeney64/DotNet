using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;

namespace EdmGen
{
    public class Generate
    {
        public void Start()
        {
            string base_dir = AppDomain.CurrentDomain.BaseDirectory;
            string input_dir = base_dir.Substring(0, base_dir.IndexOf("EdmGen")) + "EdmGen\\Result\\Input";
            string output_dir = base_dir.Substring(0, base_dir.IndexOf("EdmGen")) + "EdmGen\\Result\\Output";

            if (Directory.Exists(input_dir))
            {
                string[] path_files = Directory.GetFiles(input_dir);
                string[] files = path_files.Select(ss => Path.GetFileName(ss)).ToArray();
                files = files.Select(ss => ss.Substring(0, ss.Length - 3)).ToArray();

                foreach(string file in files)
                {
                    GenerateOne(output_dir, file);
                }
            }
        }

        public void GenerateOne(string dir, string name)
        {
            #region

            CodeCompileUnit servOneUnit = null;
            CodeNamespace servOneNamespace = null;
            CodeNamespace servOneModelNamespace = null;
            bool is_servFirst = true;
            bool oneFile_Serv = true;
            if (oneFile_Serv)
            {
                servOneUnit = new CodeCompileUnit();
                TsbCodeGenResult item = new TsbCodeGenResult
                {
                    Class_Serv = new CodeTypeDeclaration(name),
                    Namespace_Serv = new CodeNamespace("Server.Core.Model"),
                    Namespace_Serv_Using = new CodeNamespace("Server.Core.Model"),
                };
                List<TsbCodeGenResult> res = new List<TsbCodeGenResult>();
                res.Add(item);
                foreach (var res_item in res)
                {
                    #region
                    if (!res_item.Is_Error)
                    {
                        if ((oneFile_Serv) && (res_item.Class_Serv != null))
                        {
                            if (is_servFirst)
                            {
                                servOneUnit.Namespaces.Add(res_item.Namespace_Serv_Using);
                                servOneNamespace = res_item.Namespace_Serv;
                                // !!!
                                servOneNamespace.Types.Add(res_item.Class_Serv);
                                //
                                is_servFirst = false;
                            }


                            // !!!
                            //servOneNamespace.Types.Add(res_item.Class_Serv);
                            if (res_item.Class_Serv_Entity != null)
                                servOneNamespace.Types.Add(res_item.Class_Serv_Entity);
                            if (res_item.Class_Serv_IContextWithGroup != null)
                                servOneNamespace.Types.Add(res_item.Class_Serv_IContextWithGroup);

                            if (res_item.Class_Serv_Metadata != null)
                            {
                                if (servOneModelNamespace == null)
                                {
                                    servOneModelNamespace = res_item.Namespace_Model;
                                }
                                servOneModelNamespace.Types.Add(res_item.Class_Serv_Metadata);
                            }

                            if (res_item.Class_Serv_Tree != null)
                            {
                                if (servOneModelNamespace == null)
                                {
                                    servOneModelNamespace = res_item.Namespace_Model;
                                }
                                servOneModelNamespace.Types.Add(res_item.Class_Serv_Tree);
                            }

                            if (res_item.Class_Serv_Edm != null)
                            {
                                if (servOneModelNamespace == null)
                                {
                                    servOneModelNamespace = res_item.Namespace_Model;
                                }
                                servOneModelNamespace.Types.Add(res_item.Class_Serv_Edm);
                            }

                            res_item.FilePath_Serv = dir + "//" + name + ".cs";
                        }
                    }
                    #endregion
                }

                if (servOneNamespace != null)
                {
                    servOneUnit.Namespaces.Add(servOneNamespace);
                    if (servOneModelNamespace != null)
                    {
                        servOneUnit.Namespaces.Add(servOneModelNamespace);
                    }
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
                }
            }

            #endregion
        }
    }

}

