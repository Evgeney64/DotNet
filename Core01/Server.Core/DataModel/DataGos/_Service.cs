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
    using System.Linq;
    using Server.Core.Context;
    
    public partial class EntityServ : EntityService<EntityContext>
    {
        public IQueryable<Partners>  Get_Partners() => Context.Partners;
        public IQueryable<payerlive>  Get_payerlive() => Context.payerlive;
        public IQueryable<rgn>  Get_rgn() => Context.rgn;
        public IQueryable<street>  Get_street() => Context.street;
        public IQueryable<type_street>  Get_type_street() => Context.type_street;
        public IQueryable<type_village>  Get_type_village() => Context.type_village;
        public IQueryable<village>  Get_village() => Context.village;
    }
}
