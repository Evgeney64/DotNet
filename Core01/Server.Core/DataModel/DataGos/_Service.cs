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
    }
}