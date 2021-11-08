using System.Collections.Generic;
using System.Linq;

using ServiceLib;

namespace Server.Core.CoreModel
{
    public partial class EntityServ : ServiceLib.EntityService<EntityContext>
    {
        #region Context
        public EntityServ()
        { }
        public EntityServ(string connectionString)
            : base(connectionString)
        { }

        protected override EntityContext Context
        {
            get
            {
                if (base.Context == null)
                {
                    //base.Context = EntityContext.CreateContext(connectionString);
                    base.Context = new EntityContext(connectionString);
                }
                return base.Context;
            }
        }
        #endregion

        #region Services
        //public IQueryable<NSI_STREET> Get_NSI_STREET_1()
        //{
        //    IQueryable<NSI_STREET> items = Context.NSI_STREET;
        //    List<NSI_STREET> list = items.ToList(); ;
        //    return items;
        //}
        public IQueryable<NSI_STREET> Get_NSI_STREET() => Context.NSI_STREET;

        public IQueryable<NSI_VILLAGE> Get_NSI_VILLAGE() => Context.NSI_VILLAGE;

        #region Gos
        public IQueryable<village> Get_village() => Context.village;
        public IQueryable<rgn> Get_rgn() => Context.rgn;

        public IQueryable<street> GetStreets() => Context.street;
        public IQueryable<type_street> GetTypeStreets() => Context.type_street;
        #endregion
        #endregion
    }
}
