using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Linq;

namespace Tsb.Security.Web.Models
{
    public partial class SecurityContext
    {
        public SecurityContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.UseDatabaseNullSemantics = true;
        }

        #region scr_principal
        public scr_principal GetPrincipalByPrincipalId(int principalId)
        {
            return this.scr_principal.SingleOrDefault(pr => pr.principal_id == principalId);
        }
        public scr_principal GetPrincipalByPrincipalName(string principalName, PrincipalType principalType, int applicationId)
        {
            switch (principalType)
            {
                case PrincipalType.User:
                    scr_user user = GetUserByUserName(principalName, applicationId);
                    if (user != null)
                    {
                        return this.scr_principal.First(pr => pr.principal_id == user.user_id);
                    }
                    break;
                case PrincipalType.UserGroup:
                    scr_user_group userGroup = GetUserGroupByGroupName(principalName, applicationId);
                    if (userGroup != null)
                    {
                        return this.scr_principal.First(pr => pr.principal_id == userGroup.user_group_id);
                    }
                    break;
            }
            return null;
        }

        protected void InsertPrincipal(scr_principal principal, int applicationId)
        {
            principal.application_id = applicationId;
            this.scr_principal.Add(principal);
        }
        protected void InsertPrincipal(scr_user user, int applicationId)
        {
            scr_principal principal = new scr_principal
            {
                is_user = 1,
                scr_user = user,
                user_group_id = user.user_group_id,
            };
            this.InsertPrincipal(principal, applicationId);
        }
        protected void InsertPrincipal(scr_user_group userGroup, int applicationId)
        {
            scr_principal principal = new scr_principal
            {
                is_user = 0,
                scr_user_group = userGroup,
            };
            this.InsertPrincipal(principal, applicationId);
        }
        protected void DeletePrincipal(int principalId)
        {
            scr_principal principal = this.scr_principal.SingleOrDefault(pr => pr.principal_id == principalId);
            if (principal != null)
            {
                foreach (scr_role_principal rolePrincipal in this.scr_role_principal.Where(rp => rp.principal_id == principalId))
                {
                    this.DeleteRolePrincipal(rolePrincipal);
                }
                foreach (scr_user_claim userClaim in this.scr_user_claim.Where(uc => uc.user_id == principalId))
                {
                    this.scr_user_claim.Remove(userClaim);
                }
                this.scr_principal.Remove(principal);
            }
        }
        #endregion

        #region scr_user
        public scr_user GetUserByUserId(int userId, int applicationId)
        {
            return this.scr_user
                .Include(u => u.scr_principal)
                .FirstOrDefault(us => us.user_id == userId && us.scr_principal.application_id == applicationId);
        }
        public scr_user GetUserByUserName(string userName, int applicationId)
        {
            return this.scr_user
                .Include(u => u.scr_principal)
                .FirstOrDefault(us => String.Compare(us.user_name, userName, true) == 0 && us.scr_principal.application_id == applicationId);
        }
        public IEnumerable<scr_user> GetUsersByEmail(string email, int applicationId)
        {
            return this.scr_user
                .Include(u => u.scr_principal)
                .Where(us => String.Compare(us.email, email, true) == 0 && us.scr_principal.application_id == applicationId);
        }
        public IEnumerable<scr_user> GetUsersByPhoneNumber(string phoneNumber, int applicationId)
        {
            return this.scr_user
                .Include(u => u.scr_principal)
                .Where(us => String.Compare(us.phone_number, phoneNumber, true) == 0 && us.scr_principal.application_id == applicationId);
        }
        
        public void InsertUser(scr_user user, int applicationId)
        {
            if (user != null)
            {
                this.InsertPrincipal(user, applicationId);
            }
        }
        public void UpdateUser(scr_user user)
        {
            //this.scr_user.ApplyCurrentValues(user);
            this.Entry(user).State = EntityState.Modified;
        }
        public void DeleteUser(scr_user user)
        {
            if (user != null)
            {
                foreach (scr_user_login userLogin in this.scr_user_login.Where(ul => ul.user_id == user.user_id))
                {
                    this.scr_user_login.Remove(userLogin);
                }
                this.scr_user.Remove(user);
                this.DeletePrincipal(user.user_id);
            }
        }
        #endregion

        #region scr_role
        public IQueryable<scr_role> GetRoles(bool fixedOnly)
        {
            return this.scr_role.Where(rl => (!fixedOnly || rl.is_fixed == 1));
        }
        public scr_role GetRoleByRoleName(string roleName)
        {
            return this.scr_role.SingleOrDefault(rl => String.Compare(rl.role_name, roleName, true) == 0);
        }
        public scr_role GetRoleByRoleId(int roleId)
        {
            return this.scr_role.FirstOrDefault(rl => rl.role_id == roleId);
        }

        public void InsertRole(scr_role role)
        {
            if (role != null)
            {
                this.scr_role.Add(role);
            }
        }
        public void UpdateRole(scr_role role)
        {
            //this.scr_role.ApplyCurrentValues(role);
            this.Entry(role).State = EntityState.Modified;
        }
        public void DeleteRole(scr_role role)
        {
            if (role != null)
            {
                foreach (scr_role_principal rolePrincipal in this.scr_role_principal.Where(rp => rp.role_id == role.role_id))
                {
                    this.DeleteRolePrincipal(rolePrincipal);
                }
                foreach (scr_role_role roleRole in this.scr_role_role.Where(rp => rp.role_id == role.role_id))
                {
                    this.DeleteRoleRole(roleRole);
                }
                foreach (scr_role_role roleRole in this.scr_role_role.Where(rp => rp.role_member_id == role.role_id))
                {
                    this.DeleteRoleRole(roleRole);
                }
                this.scr_role.Remove(role);
            }
        }
        #endregion

        #region scr_role_principal
        public IQueryable<scr_role_principal> GetRolePrincipalsByRoleId(int roleId)
        {
            return this.scr_role_principal.Where(rp => rp.role_id == roleId);
        }
        public IQueryable<scr_role_principal> GetRolePrincipalsByRoleIdAndPrincipalId(int roleId, int principalId)
        {
            return this.scr_role_principal.Where(rp => rp.role_id == roleId && rp.principal_id == principalId);
        }

        public void InsertRolePrincipal(scr_role_principal rolePrincipal)
        {
            if (rolePrincipal != null)
            {
                if (!this.scr_role_principal.Any(rp => rp.role_id == rolePrincipal.role_id && rp.principal_id == rolePrincipal.principal_id && rp.group_id == rolePrincipal.group_id))
                {
                    this.scr_role_principal.Add(rolePrincipal);
                }
            }
        }
        public void DeleteRolePrincipal(scr_role_principal rolePrincipal)
        {
            if (rolePrincipal != null)
            {
                this.scr_role_principal.Remove(rolePrincipal);
            }
        }
        #endregion

        #region scr_role_role
        public scr_role_role GetRoleRoleById(int roleRoleId)
        {
            return this.scr_role_role.FirstOrDefault(rp => rp.role_role_id == roleRoleId);
        }
        public IQueryable<scr_role_role> GetRoleRolesByRoleId(int roleId)
        {
            return this.scr_role_role.Where(rp => rp.role_id == roleId);
        }
        public IQueryable<scr_role_role> GetRoleRolesByRoleMemberId(int roleMemberId)
        {
            return this.scr_role_role.Where(rp => rp.role_member_id == roleMemberId);
        }

        public void InsertRoleRole(scr_role_role roleRole)
        {
            if (roleRole != null)
            {
                if (!this.scr_role_role.Any(rr => rr.role_id == roleRole.role_id && rr.role_member_id == roleRole.role_member_id))
                {
                    this.scr_role_role.Add(roleRole);
                }
            }
        }
        public void DeleteRoleRole(scr_role_role roleRole)
        {
            if (roleRole != null)
            {
                this.scr_role_role.Add(roleRole);
            }
        }
        #endregion

        #region scr_principal_group
        //public IEnumerable<scr_principal_group> GetPrincipalGroupsByPrincipalId(int principalId)
        //{
        //    return this.scr_principal_group.Where(pg => pg.principal_id == principalId);
        //}
        //public IEnumerable<scr_principal_group> GetPrincipalGroupsByPrincipal(scr_principal principal)
        //{
        //    return this.GetPrincipalGroupsByPrincipalId(principal.principal_id);
        //}
        //public IEnumerable<scr_principal_group> GetPrincipalGroupsByPrincipals(IEnumerable<scr_principal> principals)
        //{
        //    return this.scr_principal_group.Join(
        //        principals.Select(p => p.principal_id).Distinct(),
        //        outer => outer.principal_id,
        //        inner => inner,
        //        (outer, inner) => outer);
        //}

        //public scr_principal_group GetPrincipalGroupByPrincipalIdAndGroupId(int principalId, int groupId)
        //{
        //    return this.scr_principal_group.SingleOrDefault(pg => pg.principal_id == principalId && pg.group_id == groupId);
        //}

        //public bool IsAnyPrincipalHasNoGroups(IQueryable<scr_principal> principals)
        //{
        //    return !principals.All(p => p.scr_principal_group.Any());
        //}

        //public void InsertPrincipalGroup(scr_principal_group principalGroup)
        //{
        //    if (principalGroup != null)
        //    {
        //        if (!this.scr_principal_group.Any(pg => pg.principal_id == principalGroup.principal_id && pg.group_id == principalGroup.group_id))
        //        {
        //            this.scr_principal_group.Add(principalGroup);
        //        }
        //    }
        //}
        //public void DeletePrincipalGroup(scr_principal_group principalGroup)
        //{
        //    if (principalGroup != null)
        //    {
        //        this.scr_principal_group.Remove(principalGroup);
        //    }
        //}
        #endregion

        #region scr_user_group
        public scr_user_group GetUserGroupByGroupId(int userGroupId, int applicationId)
        {
            return this.scr_user_group
                .Include(u => u.scr_principal)
                .FirstOrDefault(us => us.user_group_id == userGroupId && us.scr_principal.application_id == applicationId);
        }
        public scr_user_group GetUserGroupByGroupName(string groupName, int applicationId)
        {
            return this.scr_user_group
                .Include(u => u.scr_principal)
                .FirstOrDefault(us => String.Compare(us.user_group_name, groupName, true) == 0 && us.scr_principal.application_id == applicationId);
        }
        public IList<scr_user_group> GetUserGroupsByParentGroupId(int parentGroupId, int applicationId)
        {
            List<scr_user_group> allUserGroups = new List<scr_user_group>();
            
            var userGroups = this.scr_user_group
                .Include(u => u.scr_principal)
                .Where(g => g.parent_id == parentGroupId && g.scr_principal.application_id == applicationId);
            allUserGroups.AddRange(userGroups);
            foreach (var userGroup in userGroups)
            {
                allUserGroups.AddRange(this.GetUserGroupsByParentGroupId(userGroup.user_group_id, applicationId));
            }

            return allUserGroups;
        }

        public void InsertUserGroup(scr_user_group userGroup, int applicationId)
        {
            if (userGroup != null)
            {
                this.InsertPrincipal(userGroup, applicationId);
            }
        }
        public void UpdateUserGroup(scr_user_group userGroup)
        {
            //this.scr_user_group.ApplyCurrentValues(userGroup);
            this.Entry(userGroup).State = EntityState.Modified;
        }
        public void DeleteUserGroup(scr_user_group userGroup)
        {
            if (userGroup != null)
            {
                this.scr_user_group.Remove(userGroup);
                this.DeletePrincipal(userGroup.user_group_id);
            }
        }
        #endregion

        public enum PrincipalType
        {
            User,
            UserGroup,
        }

        public static SecurityContext CreateContext(string sqlConnectionStringName)
        {
            if (sqlConnectionStringName == null)
            {
                throw new ArgumentNullException("sqlConnectionStringName");
            }

            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[sqlConnectionStringName];
            if (connectionStringSettings == null)
            {
                throw new SettingsPropertyNotFoundException();
            }

            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
            entityBuilder.Provider = connectionStringSettings.ProviderName;
            entityBuilder.ProviderConnectionString = connectionStringSettings.ConnectionString;
            entityBuilder.Metadata = "res://*/Models.SecurityContext.csdl|res://*/Models.SecurityContext.ssdl|res://*/Models.SecurityContext.msl";

            SecurityContext context = new SecurityContext(entityBuilder.ConnectionString);
            return context;
        }
    }

    public partial class scr_principal
    {
        public string PrincipalName
        {
            get
            {
                if (this.scr_user != null)
                {
                    return this.scr_user.user_name;
                }
                else if (this.scr_user_group != null)
                {
                    return this.scr_user_group.user_group_name;
                }
                else
                {
                    return null;
                }
            }
        }

        internal bool IsType(SecurityContext.PrincipalType principalType)
        {
            switch (principalType)
            {
                case SecurityContext.PrincipalType.User:
                    return this.scr_user != null;
                case SecurityContext.PrincipalType.UserGroup:
                    return this.scr_user_group != null;
                default:
                    return false;
            }
        }
    }
}
