﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tsb.External.Server.MainModelExt
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.EntityClient;
    
    public partial class MainModelExtEdm : Tsb.WCF.Web.Model.MainEdm
    {
        public MainModelExtEdm()
            : base("name=MainModelExtEdm")
        {
            this.Configuration.LazyLoadingEnabled = false;
    		this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.UseDatabaseNullSemantics = true;
        }
    	
    	public MainModelExtEdm(EntityConnection connection)
            : base(connection)
        {
            this.Configuration.LazyLoadingEnabled = false;
    		this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.UseDatabaseNullSemantics = true;
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
    }
}
