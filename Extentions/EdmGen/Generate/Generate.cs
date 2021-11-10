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
            string root = "EdmGen";
            string base_dir = AppDomain.CurrentDomain.BaseDirectory;
            string result_dir = base_dir.Substring(0, base_dir.IndexOf(root)) + root + "\\Result";
            string input_dir = result_dir + "\\Input";
            string output_dir = result_dir + "\\Output";
            string[] files = File.ReadAllLines(input_dir + "//_files.txt");
            { }

            DbInfo info = new DbInfo(conf);
            info.files = files;
            info.GenerateInfo();
            if (info.tables != null)
            {
                foreach (table tbl in info.tables)
                    generateOne(output_dir, tbl);
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

        public static ServiceResult generateOne(string dir, table tbl)
        {
            #region
            CodeCompileUnit classUnit = new CodeCompileUnit();
            CodeNamespace classNamespace = new CodeNamespace("Server.Core.Model");
            classUnit.Namespaces.Add(classNamespace);

            #region uses
            classNamespace.Imports.Add(new CodeNamespaceImport("System"));
            classNamespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            classNamespace.Imports.Add(new CodeNamespaceImport("System.ComponentModel.DataAnnotations"));
            classNamespace.Imports.Add(new CodeNamespaceImport("System.ComponentModel.DataAnnotations.Schema"));
            //servOneNamespace.Imports.Add(new CodeNamespaceImport("Server.Core.Public"));
            #endregion

            #region class
            TsbCodeGenResult classItem = new TsbCodeGenResult
            {
                Class_Serv = new CodeTypeDeclaration
                {
                    Name = tbl.name,
                    IsPartial = true,
                },
                Namespace_Serv = classNamespace, //new CodeNamespace("Server.Core.Model"),
                FilePath_Serv = dir + "//" + tbl.name + ".cs",
            };
            classNamespace.Types.Add(classItem.Class_Serv);
            #endregion

            #region interfaces
            classItem.Class_Serv.BaseTypes.Add("IEntityObject");
            classItem.Class_Serv.BaseTypes.Add("IEntityLog");
            classItem.Class_Serv.BaseTypes.Add("IEntityPeriod");
            #endregion

            #region Constructor
            if (tbl.children.Count() > 0)
            {
                CodeConstructor constructor = new CodeConstructor
                {
                    Attributes = MemberAttributes.Public,
                };
                foreach (table child in tbl.children)
                {
                    CodePropertyReferenceExpression prop = new CodePropertyReferenceExpression(
                        new CodeThisReferenceExpression(),
                        child.fk_name
                        );
                    CodeObjectCreateExpression value = new CodeObjectCreateExpression(
                        "HashSet<" + child.name + ">",
                        new CodeExpression[] { }
                        );
                    constructor.Statements.Add(new CodeAssignStatement(prop, value));
                }
                classItem.Class_Serv.Members.Add(constructor);

                constructor.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Constructor"));
                constructor.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
            }
            #endregion

            #region IEntityObject
            column pk = tbl.columns.Where(ss => ss.is_primary_key == 1).FirstOrDefault();
            if (pk != null)
            {
                CodeMemberField propPk = new CodeMemberField
                {
                    Attributes = MemberAttributes.Final,
                    Type = new CodeTypeReference(typeof(long)),
                    Name = "IEntityObject.Id { get { return " + pk.name + " } }",
                };
                classItem.Class_Serv.Members.Add(propPk);
            }
            #endregion

            #region columns
            if (tbl.columns.Count() > 0)
            {
                int i = 0;
                CodeMemberField prop0 = null;
                CodeMemberField prop1 = null;
                foreach (column col in tbl.columns)
                {
                    col.typeClrSet();
                    CodeMemberField prop = new CodeMemberField
                    {
                        Attributes = MemberAttributes.Public,
                        Type = new CodeTypeReference(col.typeClr),
                        Name = col.name + " { get; set; }",
                    };
                    if (i == 0)
                        prop0 = prop;
                    else if (i == tbl.columns.Count - 1)
                        prop1 = prop;
                    i++;

                    if (col.is_primary_key == 1)
                    {
                        prop.CustomAttributes.Add(new CodeAttributeDeclaration("KeyAttribute"));
                        //prop.CustomAttributes.Add(new CodeAttributeDeclaration(
                        //    "System.ComponentModel.DataAnnotations.KeyAttribute",
                        //    new CodeAttributeArgument(new CodePrimitiveExpression(""))));
                    }
                    classItem.Class_Serv.Members.Add(prop);
                }
                if (tbl.columns.Count() > 1)
                {
                    prop0.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Columns"));
                    prop1.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
                }
            }
            #endregion

            #region navigation props (parents)
            if (tbl.parents.Count() > 0)
            {
                // [ForeignKey()]
                // https://www.entityframeworktutorial.net/code-first/foreignkey-dataannotations-attribute-in-code-first.aspx

                // [InverseProperty("Author")]
                // https://docs.microsoft.com/ru-ru/ef/core/modeling/relationships?tabs=data-annotations%2Cfluent-api-simple-key%2Csimple-key


                int i = 0;
                CodeMemberField prop0 = null;
                CodeMemberField prop1 = null;
                foreach (table parent in tbl.parents.OrderBy(ss => ss.name))
                {
                    CodeMemberField prop = new CodeMemberField
                    {
                        Attributes = MemberAttributes.Public,
                        Type = new CodeTypeReference("virtual " + parent.name),
                        Name = parent.fk_name + " { get; set; }",
                    };
                    if (i == 0)
                        prop0 = prop;
                    else if (i == tbl.parents.Count - 1)
                        prop1 = prop;
                    i++;

                    classItem.Class_Serv.Members.Add(prop);
                }
                if (tbl.parents.Count() > 1)
                {
                    prop0.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Navigation - parents"));
                    prop1.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
                }
            }
            #endregion

            #region navigation props (children)
            if (tbl.children.Count() > 0)
            {
                int i = 0;
                CodeMemberField prop0 = null;
                CodeMemberField prop1 = null;
                foreach (table child in tbl.children.OrderBy(ss => ss.name))
                {
                    CodeMemberField prop = new CodeMemberField
                    {
                        Attributes = MemberAttributes.Public,
                        Type = new CodeTypeReference("virtual ICollection<" + child.name + ">"),
                        Name = child.fk_name + " { get; set; }",
                    };
                    //prop.CustomAttributes.Add(new CodeAttributeDeclaration("InverseProperty"));
                    if (i == 0)
                        prop0 = prop;
                    else if (i == tbl.children.Count - 1)
                        prop1 = prop;
                    i++;

                    classItem.Class_Serv.Members.Add(prop);
                }
                if (tbl.children.Count() > 1)
                {
                    prop0.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Navigation - children"));
                    prop1.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
                }
            }
            #endregion

            #region save classUnit
            string codeFileName_Serv = dir + "//" + tbl.name + ".cs";
            using (var outFile = File.Open(codeFileName_Serv, FileMode.Create))
            using (var fileWriter = new StreamWriter(outFile))
            using (var indentedTextWriter = new IndentedTextWriter(fileWriter, "    "))
            {
                var provider = new Microsoft.CSharp.CSharpCodeProvider();
                provider.GenerateCodeFromCompileUnit(classUnit,
                    indentedTextWriter,
                    new CodeGeneratorOptions() { BracingStyle = "C" });
            }
            #endregion

            return new ServiceResult("Файл сохранен");
            #endregion
        }
    }

}

