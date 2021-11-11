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
        public static ServiceResult generateOneClass(string dir, table tbl)
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
            if (tbl.columns.Where(ss => ss.name == "CRT_DATE").Count() == 1
                && tbl.columns.Where(ss => ss.name == "MFY_DATE").Count() == 1
                && tbl.columns.Where(ss => ss.name == "MFY_SUSER_ID").Count() == 1
                )
            {
                classItem.Class_Serv.BaseTypes.Add("IEntityLog");
            }
            column date_beg = tbl.columns.Where(ss => ss.name == "DATE_BEG").FirstOrDefault();
            column date_end = tbl.columns.Where(ss => ss.name == "DATE_END").FirstOrDefault();
            if (date_beg != null && date_beg.is_nullable == false
                && date_end != null && date_end.is_nullable)
            {
                classItem.Class_Serv.BaseTypes.Add("IEntityPeriod");
            }
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
                        child.name + child.fk_nom
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
            CodeMemberField propPk = null;
            if (pk != null)
            {
                propPk = new CodeMemberField
                {
                    Attributes = MemberAttributes.Final,
                    Type = new CodeTypeReference(typeof(long)),
                    Name = "IEntityObject.Id { get { return " + pk.name + "; } }//",
                };
                classItem.Class_Serv.Members.Add(propPk);
            }
            #endregion

            #region columns
            CodeMemberField propLast = null;
            if (tbl.columns.Count() > 0)
            {
                int i = 0;
                foreach (column col in tbl.columns)
                {
                    col.typeClrSet();
                    #region CodeMemberProperty old
                    //CodeMemberProperty prop = new CodeMemberProperty
                    //{
                    //    Attributes = MemberAttributes.Public,
                    //    Type = new CodeTypeReference(col.typeClr),
                    //    Name = col.name,
                    //    HasGet = true,
                    //    HasSet = true,
                    //};
                    //prop.GetStatements.Add(new CodeMethodReturnStatement(
                    //    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName)));
                    //prop.SetStatements.Add(new CodeAssignStatement(
                    //    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName), 
                    //    new CodePropertySetValueReferenceExpression()));
                    #endregion

                    CodeMemberField prop = new CodeMemberField
                    {
                        Attributes = MemberAttributes.Public,
                        Type = new CodeTypeReference(col.typeClr),
                        Name = col.name + " { get; set; }//",
                    };
                    propLast = prop;
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
            }
            if (propPk != null && propLast != null)
            {
                propPk.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Columns"));
                propLast.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
            }
            #endregion

            #region navigation props (parents)
            if (tbl.parents.Count() > 0)
            {
                int i = 0;
                CodeMemberField prop0 = null;
                CodeMemberField prop1 = null;
                foreach (table parent in tbl.parents.OrderBy(ss => ss.name))
                {
                    CodeMemberField prop = new CodeMemberField
                    {
                        Attributes = MemberAttributes.Public,
                        Type = new CodeTypeReference("virtual " + parent.name),
                        Name = parent.name + parent.fk_nom + " { get; set; }//",
                    };
                    prop.Comments.Add(new CodeCommentStatement(new CodeComment(parent.fk_name, false)));

                    #region [InverseProperty]
                    foreign_key fk = tbl.foreign_keys.Where(ss => ss.fk_name == parent.fk_name).FirstOrDefault();
                    if (fk != null)
                    {
                        // [ForeignKey()]
                        // https://www.entityframeworktutorial.net/code-first/foreignkey-dataannotations-attribute-in-code-first.aspx

                        // [InverseProperty("Author")]
                        // https://docs.microsoft.com/ru-ru/ef/core/modeling/relationships?tabs=data-annotations%2Cfluent-api-simple-key%2Csimple-key

                        prop.CustomAttributes.Add(new CodeAttributeDeclaration(
                            "InverseProperty",
                            new CodeAttributeArgument(new CodePrimitiveExpression(fk.this_column))
                            ));
                    }
                    #endregion

                    if (i == 0)
                        prop0 = prop;
                    prop1 = prop;
                    i++;

                    classItem.Class_Serv.Members.Add(prop);
                }
                if (prop0 != null && prop1 != null)
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
                        Name = child.name + child.fk_nom + " { get; set; }//",
                    };
                    prop.Comments.Add(new CodeCommentStatement(new CodeComment(child.fk_name, false)));

                    if (i == 0)
                        prop0 = prop;
                    prop1 = prop;
                    i++;

                    classItem.Class_Serv.Members.Add(prop);
                }
                if (prop0 != null && prop1 != null)
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
