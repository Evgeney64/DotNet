using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Linq;
using System.Reflection;
using System.Web.Configuration;
using System.Web.Security;
using Tsb.Security.Web.Models;

namespace Tsb.Security.Web.Membership
{
    public interface IGroups
    {
        int[] Grant { get; }
        int[] Deny { get; }
    }
    
    public interface IGroupRoleProvider
    {
        IGroups GetGroupsForUserInRole(string username, string roleName);
    }

    #region Exceptions
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

    public class RoleOnPopulatedException : Exception
    {
        private const string MessageStr = @"В роли участвуют пользователи или роли.";

        public RoleOnPopulatedException()
            : base(MessageStr)
        {
        }
    }
    #endregion

    public class GroupsForRole : IGroups
    {
        private int[] grant;
        private int[] deny;

        public int[] Grant
        {
            get
            {
                return this.grant;
            }
        }
        public int[] Deny
        { 
            get
            {
                return this.deny;
            }
        }
        
        public GroupsForRole()
        {
            this.grant = new int[0];
            this.deny = new int[0];
        }
        public GroupsForRole(ICollection<int> grant, ICollection<int> deny)
        {
            if (grant == null)
            {
                throw new ArgumentNullException("grant");
            }
            if (deny == null)
            {
                throw new ArgumentNullException("deny");
            }

            this.grant = grant.ToArray();
            this.deny = deny.ToArray();
        }
    }

    public abstract class SuperRoleProvider : RoleProvider, IGroupRoleProvider
    {
        private const string CConnectionStringFormat = "name={0}";

        private string connectionStringName;

        public abstract string DefaultProviderName { get; }
        public override string ApplicationName { get; set; }
        internal abstract SecurityContext.PrincipalType PrincipalType { get; }

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
        internal SecurityContext CreateContext()
        {
            if (!String.IsNullOrWhiteSpace(connectionStringName))
            {
                return SecurityContext.CreateContext(connectionStringName);
            }
            else
            {
                return new SecurityContext();
            }
        }

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
                            scr_principal principal = store.GetPrincipalByPrincipalName(userName, this.PrincipalType);
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

                if (throwOnPopulatedRole)
                {
                    if (store.GetRolePrincipalsByRoleId(role.role_id).Count() > 0 || store.GetRoleRolesByRoleId(role.role_id).Count() > 0)
                    {
                        throw new RoleOnPopulatedException();
                    }
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
                scr_principal principal = store.GetPrincipalByPrincipalName(username, this.PrincipalType);
                if (principal == null)
                {
                    throw new UserNotFoundException(username);
                }

                var userRoles = store.scr_GetRolesByUser(principal.principal_id);
                // пользователь входит в роль если: нет полного запрета и есть разрешение хотя бы на группу
                var roleNames = userRoles.GroupBy(s => s.role_name).Where(g => !g.Any(s => s.group_id == null && s.is_deny) && g.Any(s => !s.is_deny)).Select(g => g.Key).ToArray();

                return roleNames;
            }
        }
        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
            
            //using (SecurityContext store = this.CreateContext())
            //{
            //    scr_role role = store.GetRoleByRoleName(roleName);
            //    if (role == null)
            //    {
            //        throw new RoleNotFoundException(roleName);
            //    }
            //    return store.GetRolePrincipalsByRole(role).Where(rp => rp.scr_principal.IsType(this.PrincipalType)).Select(rp => rp.scr_principal.PrincipalName).Distinct().ToArray();
            //}
        }
        public override bool IsUserInRole(string username, string roleName)
        {
            try
            {
                string[] users = this.GetUsersInRole(roleName);
                return users.Contains(username);
            }
            catch (RoleNotFoundException e)
            {
                return false;
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
                            scr_principal principal = store.GetPrincipalByPrincipalName(userName, this.PrincipalType);
                            if (principal != null)
                            {
                                var rolePrincipals = store.GetRolePrincipalsByRoleIdAndPrincipalId(role.role_id, principal.principal_id);
                                foreach (scr_role_principal rolePrincipal in rolePrincipals)
                                {
                                    store.DeleteRolePrincipal(rolePrincipal);
                                }
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

        #region IGroupRoleProvider members
        public virtual IGroups GetGroupsForUserInRole(string username, string roleName)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_role role = store.GetRoleByRoleName(roleName);
                if (role == null)
                {
                    throw new RoleNotFoundException(roleName);
                }
                scr_principal principal = store.GetPrincipalByPrincipalName(username, this.PrincipalType);
                if (principal == null)
                {
                    throw new UserNotFoundException(username);
                }

                var userRoles = store.scr_GetRolesByUser(principal.principal_id).Where(s => s.role_id == role.role_id).ToList();
                bool deny = userRoles.Any(s => s.group_id == null && s.is_deny);
                ICollection<int> denyGroups = userRoles.Where(s => s.group_id != null && s.is_deny).Select(s => (int)s.group_id).ToList();
                bool grant = userRoles.Any(s => s.group_id == null && !s.is_deny);
                ICollection<int> grantGroups = userRoles.Where(s => s.group_id != null && !s.is_deny).Select(s => (int)s.group_id).ToList();
                // если есть полный запрет или нет разрешения хотя бы на группу - ошибка
                if (deny || !(grant || grantGroups.Count > 0))
                {
                    throw new InvalidOperationException();
                }
                // если нет полного разрешения - возвращаем разрешенные и запрещенные группы
                if (!grant)
                {
                    return new GroupsForRole(grantGroups, denyGroups);
                }
                // иначе - только запрещенные группы
                else
                {
                    return new GroupsForRole(new int[0], denyGroups);
                }
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
        internal override SecurityContext.PrincipalType PrincipalType
        {
            get 
            {
                return SecurityContext.PrincipalType.User;
            }
        }
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
        internal override SecurityContext.PrincipalType PrincipalType
        {
            get
            {
                return SecurityContext.PrincipalType.WinUser;
            }
        }
    }
}
