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
    
    
    public partial class NSI_BANK : IEntityObject, IEntityLog
    {
        
        #region Columns
        long IEntityObject.Id { get { return NBANK_ID; } }//;
        
        [KeyAttribute()]
        public int NBANK_ID { get; set; }//;
        
        public string BIK { get; set; }//;
        
        public string IND { get; set; }//;
        
        public string TNP { get; set; }//;
        
        public string NNP { get; set; }//;
        
        public string ADRES { get; set; }//;
        
        public string RKS { get; set; }//;
        
        public string NAMEP { get; set; }//;
        
        public string NAMEN { get; set; }//;
        
        public string NEWKS { get; set; }//;
        
        public string PERMFO { get; set; }//;
        
        public string PERNUM { get; set; }//;
        
        public string PERKS { get; set; }//;
        
        public string AT1 { get; set; }//;
        
        public string AT2 { get; set; }//;
        
        public string TELEF { get; set; }//;
        
        public string REGN { get; set; }//;
        
        public string OKPO { get; set; }//;
        
        public string DT_IZM { get; set; }//;
        
        public string P { get; set; }//;
        
        public string KSNP { get; set; }//;
        
        public System.Nullable<System.DateTime> CRT_DATE { get; set; }//;
        
        public System.Nullable<System.DateTime> MFY_DATE { get; set; }//;
        
        public System.Nullable<int> MFY_SUSER_ID { get; set; }//;
        #endregion
    }
}
