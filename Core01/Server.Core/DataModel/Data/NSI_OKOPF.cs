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
    
    
    public partial class NSI_OKOPF : IEntityObject, IEntityLog
    {
        
        #region Columns
        long IEntityObject.Id { get { return NOKOPF_ID; } }//;
        
        [KeyAttribute()]
        public int NOKOPF_ID { get; set; }//;
        
        public string NOKOPF_NAME { get; set; }//;
        
        public System.Nullable<int> OKOPF { get; set; }//;
        
        public System.Nullable<int> SECTION { get; set; }//;
        #endregion
        
        #region Navigation - children
        // FK_PARTNER_EXT_NSI_OKOPF
        public virtual ICollection<PARTNER_EXT> PARTNER_EXT { get; set; }//;
        #endregion
        
        #region Constructor
        public NSI_OKOPF()
        {
            this.PARTNER_EXT = new HashSet<PARTNER_EXT>();
        }
        #endregion
    }
}