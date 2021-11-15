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
        private static ServiceResult generateOneClass(string dir, table tbl)
        {
            #region
            #region Define
            bool generate_children = true;
            #endregion

            #region namespace
            CodeCompileUnit classUnit = new CodeCompileUnit();
            CodeNamespace classNamespace = new CodeNamespace("Server.Core.Model");
            classUnit.Namespaces.Add(classNamespace);
            #endregion

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
            if (generate_children && tbl.children.Count() > 0)
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
                    if (col.attr_name != null)
                    {
                        prop.CustomAttributes.Add(new CodeAttributeDeclaration(
                            "Column",
                            new CodeAttributeArgument(new CodePrimitiveExpression(col.attr_name)))
                            );
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
                            "ForeignKey",
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
                    CodeRegionDirective region = new CodeRegionDirective(CodeRegionMode.Start, "Navigation - parents");
                    CodeRegionDirective endregion = new CodeRegionDirective(CodeRegionMode.End, "");
                    prop0.StartDirectives.Add(region);
                    prop1.EndDirectives.Add(endregion);
                }
            }
            #endregion

            #region navigation props (children)
            if (generate_children && tbl.children.Count() > 0)
            {
                int i = 0;
                CodeMemberField prop0 = null;
                CodeMemberField prop1 = null;
                foreach (table child in tbl.children.OrderBy(ss => ss.name))
                {
                    child.fk_name_nom = child.name + child.fk_nom;
                    CodeMemberField prop = new CodeMemberField
                    {
                        Attributes = MemberAttributes.Public,
                        Type = new CodeTypeReference("virtual ICollection<" + child.name + ">"),
                        Name = child.fk_name_nom + " { get; set; }//",
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
                    new CodeGeneratorOptions()
                    {
                        BracingStyle = "C",
                        BlankLinesBetweenMembers = false,
                    });
            }
            #endregion

            return new ServiceResult("Файл сохранен");
            #endregion
        }

        private static ServiceResult generateContextClass(string dir, DbInfo info)
        {
            #region
            #region namespace
            CodeCompileUnit classUnit = new CodeCompileUnit();
            CodeNamespace classNamespace = new CodeNamespace("Server.Core.Context");
            classUnit.Namespaces.Add(classNamespace);
            #endregion

            #region uses
            classNamespace.Imports.Add(new CodeNamespaceImport("Microsoft.EntityFrameworkCore"));
            classNamespace.Imports.Add(new CodeNamespaceImport("Server.Core.Model"));
            #endregion

            #region class
            TsbCodeGenResult classItem = new TsbCodeGenResult
            {
                Class_Serv = new CodeTypeDeclaration
                {
                    Name = "EntityContext",
                    IsPartial = true,
                },
                Namespace_Serv = classNamespace, //new CodeNamespace("Server.Core.Model"),
                //FilePath_Serv = dir + "//" + tbl.name + ".cs",
            };
            classNamespace.Types.Add(classItem.Class_Serv);
            #endregion

            #region interfaces
            classItem.Class_Serv.BaseTypes.Add("DbContext");
            #endregion

            #region DbSet<>
            if (info.tables.Count > 0)
            {
                foreach (table tbl in info.tables.OrderBy(ss => ss.name))
                {
                    CodeMemberField prop = new CodeMemberField
                    {
                        Attributes = MemberAttributes.Public,
                        Type = new CodeTypeReference("virtual DbSet<" + tbl.name + ">"),
                        Name = tbl.name + " { get; set; }//",
                    };
                    classItem.Class_Serv.Members.Add(prop);
                    //Console.WriteLine("[gen-context] - " + tbl.nom + " - " + tbl.name);
                }
            }
            #endregion

            #region creatNavigations
            {
                CodeMemberMethod method = new CodeMemberMethod
                {
                    Name = "creatNavigations",
                };
                method.Parameters.Add(new CodeParameterDeclarationExpression("ModelBuilder", "builder"));

                classItem.Class_Serv.Members.Add(method);

                foreach (table tbl in info.tables.Where(ss => ss.children.Count() > 0))
                {
                    foreach (table tbl1 in tbl.children)
                    {
                        string str = "Entity<"+ tbl1.name + ">()";
                        str += ".HasOne(u => u.Partners2)";
                        str += ".WithMany(t => t." + tbl1.fk_name_nom + ")";
                        str += ".HasForeignKey(t => t.reciever_id)";
                        str += ";//";
                        CodeMethodInvokeExpression expr =
                            new CodeMethodInvokeExpression(
                                new CodeVariableReferenceExpression("builder"), str);

                        method.Statements.Add(expr);
                    }
                }
                //method.Statements.Add(new CodeAssignStatement(prop, value));
                #region examples
                {
                    //// Context.DoSmth();
                    //// .................................
                    //CodeMethodInvokeExpression toStringInvoke =
                    //    new CodeMethodInvokeExpression(
                    //        new CodeVariableReferenceExpression("Context"),
                    //        "DoSmth"
                    //        );


                    //// this.DoSmth();
                    //// .................................
                    //CodeMethodInvokeExpression toStringInvoke =
                    //    new CodeMethodInvokeExpression(
                    //        new CodeThisReferenceExpression(),
                    //        "DoSmth"
                    //        );
                    //method.Statements.Add(toStringInvoke);


                    //// this.widthValue = width;
                    //// .................................
                    //CodeFieldReferenceExpression widthReference =
                    //    new CodeFieldReferenceExpression(
                    //    new CodeThisReferenceExpression(), "widthValue");
                    //method.Statements.Add(new CodeAssignStatement(widthReference,
                    //    new CodeArgumentReferenceExpression("width")));


                    //// Console.Write("Example string");
                    //// .................................
                    //CodeExpression toStringInvoke = new CodeMethodInvokeExpression(
                    //    new CodeTypeReferenceExpression("Console"),
                    //    "Write",
                    //    new CodePrimitiveExpression("Example string")
                    //    );
                    //method.Statements.Add(toStringInvoke);
                }
                #endregion

                method.Comments.Add(new CodeCommentStatement(new CodeComment("", false)));
                method.Comments.Add(new CodeCommentStatement(new CodeComment("Create navigation props", false)));
            }
            #endregion

            #region save classUnit
            string codeFileName_Serv = dir + "//_Context.cs";
            using (var outFile = File.Open(codeFileName_Serv, FileMode.Create))
            using (var fileWriter = new StreamWriter(outFile))
            using (var indentedTextWriter = new IndentedTextWriter(fileWriter, "    "))
            {
                var provider = new Microsoft.CSharp.CSharpCodeProvider();
                provider.GenerateCodeFromCompileUnit(classUnit,
                    indentedTextWriter,
                    new CodeGeneratorOptions() 
                    { 
                        BracingStyle = "C",
                        BlankLinesBetweenMembers = false,
                    });
            }
            #endregion

            return new ServiceResult("Файл сохранен");
            #endregion
        }

        private static ServiceResult generateServiceClass(string dir, DbInfo info)
        {
            #region
            #region namespace
            CodeCompileUnit classUnit = new CodeCompileUnit();
            CodeNamespace classNamespace = new CodeNamespace("Server.Core.Model");
            classUnit.Namespaces.Add(classNamespace);
            #endregion

            #region uses
            classNamespace.Imports.Add(new CodeNamespaceImport("System.Linq"));
            classNamespace.Imports.Add(new CodeNamespaceImport("Server.Core.Context"));
            #endregion

            #region class
            TsbCodeGenResult classItem = new TsbCodeGenResult
            {
                Class_Serv = new CodeTypeDeclaration
                {
                    Name = "EntityServ",
                    IsPartial = true,
                },
                Namespace_Serv = classNamespace, //new CodeNamespace("Server.Core.Model"),
                //FilePath_Serv = dir + "//" + tbl.name + ".cs",
            };
            classNamespace.Types.Add(classItem.Class_Serv);
            #endregion

            #region interfaces
            classItem.Class_Serv.BaseTypes.Add("EntityService<EntityContext>");
            #endregion

            #region IQueryable<>
            if (info.tables.Count > 0)
            {
                foreach (table tbl in info.tables.OrderBy(ss => ss.name))
                {
                    CodeMemberField prop = new CodeMemberField
                    {
                        Attributes = MemberAttributes.Public,
                        Type = new CodeTypeReference("IQueryable<" + tbl.name + ">"),
                        Name = " Get_" + tbl.name + "() => Context." + tbl.name,
                    };
                    classItem.Class_Serv.Members.Add(prop);
                    //Console.WriteLine("[gen-serv] - " + tbl.nom + " - " + tbl.name);
                }
            }
            #endregion

            #region save classUnit
            string codeFileName_Serv = dir + "//_Service.cs";
            using (var outFile = File.Open(codeFileName_Serv, FileMode.Create))
            using (var fileWriter = new StreamWriter(outFile))
            using (var indentedTextWriter = new IndentedTextWriter(fileWriter, "    "))
            {
                var provider = new Microsoft.CSharp.CSharpCodeProvider();
                provider.GenerateCodeFromCompileUnit(classUnit,
                    indentedTextWriter,
                    new CodeGeneratorOptions()
                    {
                        BracingStyle = "C",
                        BlankLinesBetweenMembers = false,
                    });
            }
            #endregion

            return new ServiceResult("Файл сохранен");
            #endregion
        }
    }
}
