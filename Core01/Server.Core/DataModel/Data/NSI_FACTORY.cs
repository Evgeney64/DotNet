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
    
    
    public partial class NSI_FACTORY : IEntityObject, IEntityLog
    {
        
        #region Columns
        long IEntityObject.Id { get { return NFACTORY_ID; } }//;
        
        [KeyAttribute()]
        public int NFACTORY_ID { get; set; }//;
        
        public string NFACTORY_NAME { get; set; }//;
        
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        #endregion
        
        #region Navigation - children
        // FK_NSI_METER_NSI_FACTORY
        public virtual ICollection<NSI_METER> NSI_METER { get; set; }//;
        #endregion
        
        #region Constructor
        public NSI_FACTORY()
        {
            this.NSI_METER = new HashSet<NSI_METER>();
        }
        #endregion
    }
}
