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
    
    
    public partial class SYS_BASE_COLLECTION : IEntityObject, IEntityLog
    {
        
        #region Columns
        long IEntityObject.Id { get { return SBASE_COLLECTION_ID; } }//;
        
        [KeyAttribute()]
        public long SBASE_COLLECTION_ID { get; set; }//;
        
        public System.Nullable<int> SUSER_ID { get; set; }//;
        
        public System.Nullable<int> STABLE_ID { get; set; }//;
        
        public System.Nullable<long> ID { get; set; }//;
        
        public System.Nullable<long> PARENT_ID { get; set; }//;
        
        public System.Nullable<int> LEVEL_ID { get; set; }//;
        
        public System.Nullable<int> SBASE_COLLECTION_GROUP { get; set; }//;
        
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        #endregion
    }
}
