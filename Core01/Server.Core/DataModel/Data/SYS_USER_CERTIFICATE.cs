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
    
    
    public partial class SYS_USER_CERTIFICATE : IEntityObject, IEntityLog, IEntityPeriod
    {
        
        #region Columns
        long IEntityObject.Id { get { return SUSER_CERTIFICATE_ID; } }//;
        
        [KeyAttribute()]
        public int SUSER_CERTIFICATE_ID { get; set; }//;
        
        public string SUSER_CERTIFICATE_NAME { get; set; }//;
        
        public string TRUMBPRINT { get; set; }//;
        
        public System.Nullable<System.DateTime> DATE_BEG { get; set; }//;
        
        public System.Nullable<System.DateTime> DATE_END { get; set; }//;
        #endregion
    }
}
