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
    
    public partial class SHARE_POINT_TASK : IEntityObject, IEntityLog
    {
        #region Columns
        long IEntityObject.Id { get { return SHARE_POINT_TASK_ID; } }//;
        [KeyAttribute()]
        public long SHARE_POINT_TASK_ID { get; set; }//;
        public string NAME { get; set; }//;
        public string OWNER { get; set; }//;
        public string STATUS { get; set; }//;
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        #endregion
    }
}
