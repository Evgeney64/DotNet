using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using System.Data.Sql;


namespace Tsb.Security.Web.Models
{
    public partial class SecurityContext : DbContext
    {
        #region DbSet
        //public virtual DbSet<scr_group> scr_group { get; set; }
        //public virtual DbSet<scr_role_role> scr_role_role { get; set; }
        //public virtual DbSet<scr_user_claim> scr_user_claim { get; set; }
        //public virtual DbSet<scr_user_login> scr_user_login { get; set; }
        //public virtual DbSet<scr_principal> scr_principal { get; set; }
        //public virtual DbSet<scr_role> scr_role { get; set; }
        //public virtual DbSet<scr_role_principal> scr_role_principal { get; set; }
        public virtual DbSet<scr_user> scr_user { get; set; }
        public virtual DbSet<scr_user_group> scr_user_group { get; set; }
        //public virtual DbSet<scr_session_claim> scr_session_claim { get; set; }
        //public virtual DbSet<scr_session> scr_session { get; set; }
        #endregion

        //public virtual IEnumerable<scr_GetRolesByUser_Result1> scr_GetRolesByUser(Nullable<int> user_id)
        //{
        //    return from rl in scr_role
        //           select new scr_GetRolesByUser_Result1
        //           { 
        //           };
        //}
        //public virtual ObjectResult<scr_GetRolesByUser_Result1> scr_GetRolesByUser(Nullable<int> user_id)
        //{
        //    var user_idParameter = user_id.HasValue ?
        //        new ObjectParameter("user_id", user_id) :
        //        new ObjectParameter("user_id", typeof(int));

        //    return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<scr_GetRolesByUser_Result1>("scr_GetRolesByUser", user_idParameter);
        //}

        //public virtual ObjectResult<Nullable<int>> scr_GetPrincipalsByPrincipal(Nullable<int> principal_id)
        //{
        //    var principal_idParameter = principal_id.HasValue ?
        //        new ObjectParameter("principal_id", principal_id) :
        //        new ObjectParameter("principal_id", typeof(int));

        //    return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("scr_GetPrincipalsByPrincipal", principal_idParameter);
        //}

        //public virtual ObjectResult<Nullable<int>> scr_GetPrincipalsByUserGroup(Nullable<int> user_group_id)
        //{
        //    var user_group_idParameter = user_group_id.HasValue ?
        //        new ObjectParameter("user_group_id", user_group_id) :
        //        new ObjectParameter("user_group_id", typeof(int));

        //    return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("scr_GetPrincipalsByUserGroup", user_group_idParameter);
        //}

    }
}
