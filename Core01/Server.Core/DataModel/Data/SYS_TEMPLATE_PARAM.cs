//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Server.Core.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class SYS_TEMPLATE_PARAM : IEntityObject, IEntityLog
    {
        #region Columns
        long IEntityObject.Id { get { return STEMPLATE_PARAM_ID; } }//;
        [KeyAttribute()]
        public int STEMPLATE_PARAM_ID { get; set; }//;
        public int STEMPLATE_TYPE_ID { get; set; }//;
        public string PARAM { get; set; }//;
        public string DESCRIPTION { get; set; }//;
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        #endregion
    }
}
