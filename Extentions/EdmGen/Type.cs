using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.CodeDom;

namespace EdmGen
{
    public class TsbCodeGenResult
    {
        public TsbCodeGenResult()
        {
            Is_Error = false;
            ErrMess = "";
        }

        public CodeCompileUnit Unit_Vm { get; set; }
        public CodeCompileUnit Unit_Serv { get; set; }
        public CodeNamespace Namespace_Vm { get; set; }
        public CodeNamespace Namespace_Serv { get; set; }
        public CodeNamespace Namespace_Vm_Using { get; set; }
        public CodeNamespace Namespace_Serv_Using { get; set; }
        public CodeNamespace Namespace_Model { get; set; }
        public CodeTypeDeclaration Class_Vm { get; set; }
        public CodeTypeDeclaration Class_Serv { get; set; }
        public CodeTypeDeclaration Class_Serv_Metadata { get; set; }
        public CodeTypeDeclaration Class_Serv_Entity { get; set; }
        public CodeTypeDeclaration Class_Serv_IContextWithGroup { get; set; }
        public CodeTypeDeclaration Class_Vm_Filter { get; set; }
        public CodeTypeDeclaration Class_Serv_Tree { get; set; }
        public CodeTypeDeclaration Class_Serv_Edm { get; set; }
        public string ViewModel { get; set; }
        public string SysTable { get; set; }
        public string FilePath_Vm { get; set; }
        public string FilePath_Serv { get; set; }
        public bool Is_Error { get; set; }
        public string ErrMess { get; set; }
    }
}
