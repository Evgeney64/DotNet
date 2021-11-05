using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using Tsb.Security.Web.Models;

namespace Tsb.Security.Web.Membership
{
    public interface INestedRoleProvider
    {
        string[] GetFixedRoles();
        string[] GetOwnRolesForUser(string username);
        string[] GetRolesForRole(string roleName);
        string[] GetOwnRolesForRole(string roleName);
        string[] GetOwnUsersInRole(string roleName);
        string[] GetRolesInRole(string roleName);
        string[] GetOwnRolesInRole(string roleName);
        void AddRolesToRoles(string[] addedRoleNames, string[] roleNames);
        void RemoveRolesFromRoles(string[] removedRoleNames, string[] roleNames);
        bool IsFixedRole(string roleName);
    }

    public interface IGroupRoleProvider
    {
        int[] GetGroupsForRole(string roleName);
        int[] GetGroupsForUserInRole(string username, string roleName, out bool hasNoGroups);
        void AddRolesToGroups(string[] roleNames, int[] groupIds);
        void RemoveRolesFromGroups(string[] roleNames, int[] groupIds);
    }
    
    public class RoleNotFoundException : Exception
    {
        private const string MessageStr = @"Роль с именем ""{0}"" не найдена.";

        public string RoleName
        {
            get;
            private set;
        }

        public RoleNotFoundException(string RoleName)
            : base(String.Format(MessageStr, RoleName))
        {
            this.RoleName = RoleName;
        }
    }

    public class DuplicateRoleNameException : Exception
    {
        private const string MessageStr = @"Роль с таким именем уже существует.";

        public DuplicateRoleNameException()
            : base(MessageStr)
        {
        }
    }

    public class RoleOnPopulatedNameException : Exception
    {
        private const string MessageStr = @"В роли участвуют пользователи.";

        public RoleOnPopulatedNameException()
            : base(MessageStr)
        {
        }
    }

    public abstract class SuperRoleProvider : RoleProvider, INestedRoleProvider, IGroupRoleProvider
    {
        private const string CConnectionStringFormat = "name={0}";

        private string connectionStringName;

        public abstract string DefaultProviderName { get; }
        public override string ApplicationName { get; set; }

        private bool CheckRoleName(string roleName, bool isFixed)
        {
            using (SecurityContext store = this.CreateContext())
            {
                return store.GetRoleByRoleName(roleName) == null;
            }
        }
        private void CreateRoleInternal(string roleName, bool isFixed)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_role role = new scr_role()
                {
                    role_name = roleName,
                    is_fixed = isFixed
                };
                store.InsertRole(role);
                store.SaveChanges();
            }
        }
        protected SecurityContext CreateContext()
        {
            if (!String.IsNullOrWhiteSpace(connectionStringName))
                return new SecurityContext(String.Format(CConnectionStringFormat, connectionStringName));
            else
                return new SecurityContext();
        }
        protected abstract scr_principal GetPrincipalByUserName(SecurityContext context, string username);

        #region RoleProvider members
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            
            if (String.IsNullOrEmpty(name))
            {
                name = this.DefaultProviderName;
            }
            
            base.Initialize(name, config);
            
            this.connectionStringName = config["connectionStringName"];
            this.ApplicationName = config["applicationName"];
            config.Remove("connectionStringName");
            config.Remove("applicationName");
        }
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            using (SecurityContext store = this.CreateContext())
            {
                foreach (string roleName in roleNames)
                {
                    scr_role role = store.GetRoleByRoleName(roleName);
                    if (role != null)
                    {
                        foreach (string userName in usernames)
                        {
                            scr_principal principal = this.GetPrincipalByUserName(store, userName);
                            if (principal != null)
                            {
                                store.InsertRolePrincipal(new scr_role_principal { role_id = role.role_id, principal_id = principal.principal_id });
                            }
                        }
                    }
                }
                store.SaveChanges();
            }
        }
        public override void CreateRole(string roleName)
        {
            if (!CheckRoleName(roleName, false))
            {
                throw new DuplicateRoleNameException();
            }

            this.CreateRoleInternal(roleName, false);
        }
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_role role = store.GetRoleByRoleName(roleName);
                if (role == null || role.is_fixed)
                {
                    return false;
                }
                
                IQueryable<scr_role_principal> rolePrincipals = store.GetOwnRolePrincipalsByRole(role);
                if (throwOnPopulatedRole && rolePrincipals != null && rolePrincipals.Count() > 0)
                {
                    throw new RoleOnPopulatedNameException();
                }

                store.DeleteRole(role);
                store.SaveChanges();
            }
            return true;
        }
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            return GetUsersInRole(roleName).Where(us => us.Contains(usernameToMatch)).ToArray();
        }
        public override string[] GetAllRoles()
        {
            using (SecurityContext store = this.CreateContext())
            {
                return store.GetRoles(false).Select(rl => rl.role_name).ToArray();
            }
        }
        public override string[] GetRolesForUser(string username)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_principal principal = this.GetPrincipalByUserName(store, username);
                if (principal == null)
                {
                    throw new UserNotFoundException(username);
                }
                return store.GetRolePrincipalsByPrincipal(principal).Select(rp => rp.scr_role.role_name).Distinct().ToArray();
            }
        }
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            using (SecurityContext store = this.CreateContext())
            {
                foreach (string roleName in roleNames)
                {
                    scr_role role = store.GetRoleByRoleName(roleName);
                    if (role != null)
                    {
                        foreach (string userName in usernames)
                        {
                            scr_principal principal = this.GetPrincipalByUserName(store, userName);
                            if (principal != null)
                            {
                                store.DeleteRolePrincipal(store.GetRolePrincipalByRoleIdAndPrincipalId(role.role_id, principal.principal_id));
                            }
                        }
                    }
                }
                store.SaveChanges();
            }
        }
        public override bool RoleExists(string roleName)
        {
            using (SecurityContext store = this.CreateContext())
            {
                return store.GetRoleByRoleName(roleName) != null;
            }
        }
        #endregion

        #region SuperRoleProvider members
        public virtual Tuple<int, string>[] GetAllRoleIds()
        {
            using (SecurityContext store = this.CreateContext())
            {
                return store.GetRoles(false).AsEnumerable().Select(rl => new Tuple<int, string>(rl.role_id, rl.role_name)).ToArray();
            }
        }
        #endregion

        #region INestedRoleProvider members
        public virtual string[] GetFixedRoles()
        {
            using (SecurityContext store = this.CreateContext())
            {
                return store.GetRoles(true).Select(rl => rl.role_name).ToArray();
            }
        }
        public virtual string[] GetOwnRolesForUser(string username)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_principal principal = this.GetPrincipalByUserName(store, username);
                if (principal == null)
                {
                    throw new UserNotFoundException(username);
                }
                return store.GetOwnRolePrincipalsByPrincipal(principal).Select(rp => rp.scr_role.role_name).ToArray();
            }
        }
        public virtual string[] GetRolesForRole(string roleName)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_principal principal = store.GetPrincipalByPrincipalName(roleName, SecurityContext.PrincipalType.Role);
                if (principal == null)
                {
                    throw new RoleNotFoundException(roleName);
                }
                return store.GetRolePrincipalsByPrincipal(principal).Select(rp => rp.scr_role.role_name).ToArray();
            }
        }
        public virtual string[] GetOwnRolesForRole(string roleName)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_principal principal = store.GetPrincipalByPrincipalName(roleName, SecurityContext.PrincipalType.Role);
                if (principal == null)
                {
                    throw new RoleNotFoundException(roleName);
                }
                return store.GetOwnRolePrincipalsByPrincipal(principal).Select(rp => rp.scr_role.role_name).ToArray();
            }
        }
        public abstract string[] GetOwnUsersInRole(string roleName);
        public virtual string[] GetRolesInRole(string roleName)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_role role = store.GetRoleByRoleName(roleName);
                if (role == null)
                {
                    throw new RoleNotFoundException(roleName);
                }
                return store.GetRolePrincipalsByRole(role).Where(rp => !rp.scr_principal.is_user).Select(rp => rp.scr_principal.scr_role.role_name).ToArray();
            }
        }
        public virtual string[] GetOwnRolesInRole(string roleName)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_role role = store.GetRoleByRoleName(roleName);
                if (role == null)
                {
                    throw new RoleNotFoundException(roleName);
                }
                return store.GetOwnRolePrincipalsByRole(role).Where(rp => !rp.scr_principal.is_user).Select(rp => rp.scr_principal.scr_role.role_name).ToArray();
            }
        }
        public virtual void AddRolesToRoles(string[] addedRoleNames, string[] roleNames)
        {
            using (SecurityContext store = this.CreateContext())
            {
                foreach (string roleName in roleNames)
                {
                    scr_role role = store.GetRoleByRoleName(roleName);
                    if (role != null)
                    {
                        IEnumerable<scr_role_principal> rolePrincipals = store.GetRolePrincipalsByPrincipal(role.scr_principal);
                        foreach (string addedRoleName in addedRoleNames)
                        {
                            scr_role addedRole = store.GetRoleByRoleName(addedRoleName);
                            if (addedRole != null && addedRole != role && !rolePrincipals.Any(rp => rp.role_id == addedRole.role_id))
                            {
                                store.InsertRolePrincipal(new scr_role_principal { role_id = role.role_id, principal_id = addedRole.role_id });
                            }
                        }
                    }
                }
                store.SaveChanges();
            }
        }
        public virtual void RemoveRolesFromRoles(string[] removedRoleNames, string[] roleNames)
        {
            using (SecurityContext store = this.CreateContext())
            {
                foreach (string roleName in roleNames)
                {
                    scr_role role = store.GetRoleByRoleName(roleName);
                    if (role != null)
                    {
                        foreach (string removedRoleName in removedRoleNames)
                        {
                            scr_role removedRole = store.GetRoleByRoleName(removedRoleName);
                            if (role != null)
                            {
                                store.DeleteRolePrincipal(store.GetRolePrincipalByRoleIdAndPrincipalId(role.role_id, removedRole.role_id));
                            }
                        }
                    }
                }
                store.SaveChanges();
            }
        }
        public virtual bool IsFixedRole(string roleName)
        {
            using (SecurityContext store = this.CreateContext())
            {
                return store.GetRoles(true).Select(rl => rl.role_name).Contains(roleName);
            }
        }
        #endregion

        #region IGroupRoleProvider members
        public virtual int[] GetGroupsForRole(string roleName)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_principal principal = store.GetPrincipalByPrincipalName(roleName, SecurityContext.PrincipalType.Role);
                if (principal == null)
                {
                    throw new RoleNotFoundException(roleName);
                }
                return store.GetPrincipalGroupsByPrincipal(principal).Select(pg => pg.group_id).ToArray();
            }
        }
        public virtual int[] GetGroupsForUserInRole(string username, string roleName, out bool hasNoGroups)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_role role = store.GetRoleByRoleName(roleName);
                if (role == null)
                {
                    throw new RoleNotFoundException(roleName);
                }
                scr_principal principal = this.GetPrincipalByUserName(store, username);
                if (principal == null)
                {
                    throw new UserNotFoundException(username);
                }
                
                IEnumerable<scr_role_principal> rolePrincipals = store.GetRolePrincipalsByRoleIntersectPrincipal(role, principal);
                if (rolePrincipals.Count() == 0)
                {
                    hasNoGroups = false;
                    return new int[0];
                }
                hasNoGroups = !rolePrincipals.Where(rp => rp.principal_id == principal.principal_id).All(rp => rp.scr_role.scr_principal.scr_principal_group.Any());
                return rolePrincipals.SelectMany(rp => rp.scr_role.scr_principal.scr_principal_group).Select(pg => pg.group_id).Distinct().ToArray();
            }
        }
        public virtual void AddRolesToGroups(string[] roleNames, int[] groupIds)
        {
            using (SecurityContext store = this.CreateContext())
            {
                foreach (string roleName in roleNames)
                {
                    scr_role role = store.GetRoleByRoleName(roleName);
                    if (role != null)
                    {
                        foreach (int groupId in groupIds)
                        {
                            store.InsertPrincipalGroup(new scr_principal_group { principal_id = role.role_id, group_id = groupId });
                        }
                    }
                }
                store.SaveChanges();
            }
        }
        public virtual void RemoveRolesFromGroups(string[] roleNames, int[] groupIds)
        {
            using (SecurityContext store = this.CreateContext())
            {
                foreach (string roleName in roleNames)
                {
                    scr_role role = store.GetRoleByRoleName(roleName);
                    if (role != null)
                    {
                        foreach (int groupId in groupIds)
                        {
                            store.DeletePrincipalGroup(store.GetPrincipalGroupByPrincipalIdAndGroupId(role.role_id, groupId));
                        }
                    }
                }
                store.SaveChanges();
            }
        }
        #endregion
    }

    public class NetRoleProvider : SuperRoleProvider
    {
        public override string DefaultProviderName
        {
            get
            {
                return "NetRoleProvider";
            }
        }

        protected override scr_principal GetPrincipalByUserName(SecurityContext context, string username)
        {
            return context.GetPrincipalByPrincipalName(username, SecurityContext.PrincipalType.User);
        }

        #region RoleProvider members
        public override string[] GetUsersInRole(string roleName)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_role role = store.GetRoleByRoleName(roleName);
                if (role == null)
                {
                    throw new RoleNotFoundException(roleName);
                }
                return store.GetRolePrincipalsByRole(role).Where(rp => rp.scr_principal.scr_user != null).Select(rp => rp.scr_principal.scr_user.user_name).Distinct().ToArray();
            }
        }
        public override bool IsUserInRole(string username, string roleName)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_role role = store.GetRoleByRoleName(roleName);
                if (role == null)
                {
                    return false;
                }
                return store.GetRolePrincipalsByRole(role).Any(rp => rp.scr_principal.scr_user != null && String.Compare(rp.scr_principal.scr_user.user_name, username, true) == 0);
            }
        }
        #endregion

        #region INestedRoleProvider members
        public override string[] GetOwnUsersInRole(string roleName)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_role role = store.GetRoleByRoleName(roleName);
                if (role == null)
                {
                    throw new RoleNotFoundException(roleName);
                }
                return store.GetOwnRolePrincipalsByRole(role).Where(rp => rp.scr_principal.scr_user != null).Select(rp => rp.scr_principal.scr_user.user_name).ToArray();
            }
        }
        #endregion
    }

    public class WindowsRoleProvider : SuperRoleProvider
    {
        public override string DefaultProviderName
        {
            get
            {
                return "WindowsRoleProvider";
            }
        }

        protected override scr_principal GetPrincipalByUserName(SecurityContext context, string username)
        {
            return context.GetPrincipalByPrincipalName(username, SecurityContext.PrincipalType.WinUser);
        }

        #region RoleProvider members
        public override string[] GetUsersInRole(string roleName)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_role role = store.GetRoleByRoleName(roleName);
                if (role == null)
                {
                    throw new RoleNotFoundException(roleName);
                }
                return store.GetRolePrincipalsByRole(role).Where(rp => rp.scr_principal.scr_win_user != null).Select(rp => rp.scr_principal.scr_win_user.user_name).Distinct().ToArray();
            }
        }
        public override bool IsUserInRole(string username, string roleName)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_role role = store.GetRoleByRoleName(roleName);
                if (role == null)
                {
                    return false;
                }
                return store.GetRolePrincipalsByRole(role).Any(rp => rp.scr_principal.scr_win_user != null && String.Compare(rp.scr_principal.scr_win_user.user_name, username, true) == 0);
            }
        }
        #endregion

        #region INestedRoleProvider members
        public override string[] GetOwnUsersInRole(string roleName)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_role role = store.GetRoleByRoleName(roleName);
                if (role == null)
                {
                    throw new RoleNotFoundException(roleName);
                }
                return store.GetOwnRolePrincipalsByRole(role).Where(rp => rp.scr_principal.scr_win_user != null).Select(rp => rp.scr_principal.scr_win_user.user_name).ToArray();
            }
        }
        #endregion
    }
}
