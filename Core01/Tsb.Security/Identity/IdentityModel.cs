//using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;

using Tsb.Security.Web.Models;

namespace Tsb.Security.Web.Models
{
    public interface IPrincipal<out TKey>
    {
        TKey Id { get; }
        int ApplicationId { get; }
    }
    
    public partial class scr_user : IUser<int>, IPrincipal<int>
    {
        public int ApplicationId
        {
            get
            {
                if (this.scr_principal != null)
                {
                    return this.scr_principal.application_id;
                }
                return -1;
            }
        }

        int IPrincipal<int>.Id
        {
            get
            {
                return this.user_id;
            }
        }
        int IUser<int>.Id
        {
            get
            {
                return this.user_id;
            }
        }
        string IUser<int>.UserName
        {
            get
            {
                return this.user_name;
            }
            set
            {
                this.user_name = value;
            }
        }

        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
    }

    public partial class scr_user_group : IPrincipal<int>
    {
        public int ApplicationId
        {
            get
            {
                if (this.scr_principal != null)
                {
                    return this.scr_principal.application_id;
                }
                return -1;
            }
        }

        int IPrincipal<int>.Id
        {
            get
            {
                return this.user_group_id;
            }
        }
    }

    public partial class scr_role : IRole<int>
    {
        int IRole<int>.Id
        {
            get
            {
                return this.role_id;
            }
        }
        string IRole<int>.Name
        {
            get
            {
                return this.role_name;
            }
            set
            {
                this.role_name = value;
            }
        }
    }
}

namespace Tsb.Security.Web.Identity
{
    public enum RegisteredBy
    {
        Password = 1,
        External = 2,
        Windows = 3,
        Admin = 4,
    }

    [Flags]
    public enum LoginBy
    {
        Email = 1,
        PhoneNumber = 2,
    }

    public enum UseAsName
    {
        Email = LoginBy.Email,
        PhoneNumber = LoginBy.PhoneNumber,
    }

    public enum Communication
    {
        Application = 1,
        Api = 2,
    }

    //public enum PermissionMode
    //{
    //    None = 0,
    //    Extension = 1,
    //    Restriction = 2,
    //    Mixed = 3,
    //}

    public static class ClaimTypes
    {
        // todo: совместить с ModuleId
        public const string ApplicationType = "Tsb.Security.Web.Identity.ApplicationType";
        public const string RegisteredBy = "Tsb.Security.Web.Identity.RegisteredBy";
        public const string Patronymic = "Tsb.Security.Web.Identity.Patronymic";
        public const string RoleAndGroup = "Tsb.Security.Web.Identity.RoleAndGroup";
        public const string UserGroup = "Tsb.Security.Web.Identity.UserGroup";
        // перенести бы выше
        public const string PartnerId = "Tsb.Security.Web.Identity.PartnerId";
        public const string DefaultGroupId = "Tsb.Security.Web.Identity.DefaultGroupId";
        public const string SynGuid = "Tsb.Security.Web.Identity.SynGuid";
        public const string ScreenSize = "Tsb.Security.Web.Identity.ScreenSize";
        public const string ConnectionName = "Tsb.Security.Web.Identity.ConnectionName";
        public const string GrantedConnectionName = "Tsb.Security.Web.Identity.GrantedConnectionName";
        public const string DeniedConnectionName = "Tsb.Security.Web.Identity.DeniedConnectionName";
        public const string ClientId = "Tsb.Security.Web.Identity.ClientId";
        public const string GrantedClientId = "Tsb.Security.Web.Identity.GrantedClientId";
        public const string DeniedClientId = "Tsb.Security.Web.Identity.DeniedClientId";
        public const string ModuleId = "Tsb.Security.Web.Identity.ModuleId";
        public const string DeveloperMode = "Tsb.Security.Web.Identity.DeveloperMode";
        public const string Debug = "Tsb.Security.Web.Identity.Debug";

        public const string MobilePhoneVerified = "Tsb.Security.Web.Identity.MobilePhoneVerified";
        public const string EmailVerified = "Tsb.Security.Web.Identity.EmailVerified";

        public const string AccessToken = "Tsb.Security.Web.Identity.AccessToken";
        public const string RefreshToken = "Tsb.Security.Web.Identity.RefreshToken";
        public const string AccessTokenExpires = "Tsb.Security.Web.Identity.AccessTokenExpires";
    }

    public class SecurityStore : IDisposable
    {
        protected SecurityContext context;
        private bool ownContext;

        public SecurityStore(SecurityContext context)
            : this(context, false)
        {
        }
        public SecurityStore(SecurityContext context, bool ownContext)
        {
            this.context = context;
            this.ownContext = ownContext;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.ownContext && disposing && this.context != null)
            {
                this.context.Dispose();
            }
            this.context = null;
        }
        public void Dispose()
        {
            this.Dispose(true);
        }
    }

    #region PrincipalStore
    public class PrincipalStore<TPrincipal> : SecurityStore
        where TPrincipal : IPrincipal<int>
    {
        protected internal readonly int ApplicationType;

        public PrincipalStore(SecurityContext context, int applicationType)
            : base(context)
        {
            this.ApplicationType = applicationType;
        }

        #region PrincipalStore
        //public bool ValidateApplication(int principalId)
        //{
        //    var principal = this.context.GetPrincipalByPrincipalId(principalId);
        //    if (principal != null)
        //    {
        //        return principal.application_id == (int)Application.Type;
        //    }
        //    return false;
        //}
        #endregion

        #region PrincipalRoleStore
        protected virtual void AddToRole(TPrincipal principal, IRole<int> role, bool isDeny)
        {
            if (role != null)
            {
                this.context.InsertRolePrincipal(new scr_role_principal
                {
                    role_id = role.Id,
                    principal_id = principal.Id,
                    is_deny = isDeny ? (short)1 : (short)0,
                });
            }
        }
        protected virtual void AddToRole(TPrincipal principal, IRole<int> role, int groupId, bool isDeny)
        {
            if (role != null)
            {
                this.context.InsertRolePrincipal(new scr_role_principal
                {
                    role_id = role.Id,
                    principal_id = principal.Id,
                    group_id = groupId,
                    is_deny = isDeny ? (short)1 : (short)0,
                });
            }
        }
        public Task AddToRoleAsync(TPrincipal principal, string roleName)
        {
            return this.AddToRoleAsync(principal, roleName, false);
        }
        public Task AddToRoleAsync(TPrincipal principal, string roleName, bool isDeny)
        {
            scr_role role = this.context.GetRoleByRoleName(roleName);
            this.AddToRole(principal, role, isDeny);

            return Task.FromResult<int>(0);
        }
        public Task AddToRoleAsync(TPrincipal principal, string roleName, int groupId)
        {
            return this.AddToRoleAsync(principal, roleName, groupId, false);
        }
        public Task AddToRoleAsync(TPrincipal principal, string roleName, int groupId, bool isDeny)
        {
            scr_role role = this.context.GetRoleByRoleName(roleName);
            this.AddToRole(principal, role, groupId, isDeny);

            return Task.FromResult<int>(0);
        }
        public Task AddToRoleAsync(TPrincipal principal, int roleId)
        {
            return this.AddToRoleAsync(principal, roleId, false);
        }
        public Task AddToRoleAsync(TPrincipal principal, int roleId, bool isDeny)
        {
            scr_role role = this.context.GetRoleByRoleId(roleId);
            this.AddToRole(principal, role, isDeny);

            return Task.FromResult<int>(0);
        }
        public Task AddToRoleAsync(TPrincipal principal, int roleId, int groupId)
        {
            return this.AddToRoleAsync(principal, roleId, groupId, false);
        }
        public Task AddToRoleAsync(TPrincipal principal, int roleId, int groupId, bool isDeny)
        {
            scr_role role = this.context.GetRoleByRoleId(roleId);
            this.AddToRole(principal, role, groupId, isDeny);

            return Task.FromResult<int>(0);
        }
        
        protected virtual void RemoveFromRole(TPrincipal principal, IRole<int> role)
        {
            if (role != null)
            {
                var rolePrincipals = this.context.GetRolePrincipalsByRoleIdAndPrincipalId(role.Id, principal.Id);
                foreach (scr_role_principal rolePrincipal in rolePrincipals)
                {
                    this.context.DeleteRolePrincipal(rolePrincipal);
                }
            }
        }
        public Task RemoveFromRoleAsync(TPrincipal principal, string roleName)
        {
            scr_role role = this.context.GetRoleByRoleName(roleName);
            this.RemoveFromRole(principal, role);

            return Task.FromResult<int>(0);
        }
        public Task RemoveFromRoleAsync(TPrincipal principal, int roleId)
        {
            scr_role role = this.context.GetRoleByRoleId(roleId);
            this.RemoveFromRole(principal, role);
            
            return Task.FromResult<int>(0);
        }
        #endregion
    }
    #endregion

    #region UserStore
    public class AdvancedUserStore : PrincipalStore<scr_user>
        , IQueryableUserStore<scr_user, int>
        , IUserStore<scr_user, int>
        , IUserEmailStore<scr_user, int>
        , IUserLockoutStore<scr_user, int>
        , IUserPasswordStore<scr_user, int>
        , IUserClaimStore<scr_user, int>
        , IUserRoleStore<scr_user, int>
        , IUserTwoFactorStore<scr_user, int>
        , IUserLoginStore<scr_user, int>
        , IUserSecurityStampStore<scr_user, int>
        , IUserPhoneNumberStore<scr_user, int>
    {
        public AdvancedUserStore(SecurityContext context, int applicationType)
            : base(context, applicationType)
        {
        }

        #region IQueryableUserStore
        public IQueryable<scr_user> Users
        {
            get
            {
                return this.context.scr_user;
            }
        }
        #endregion
        
        #region IUserStore
        public Task CreateAsync(scr_user user)
        {
            if (user.password == null)
            {
                user.password = "";
                user.password_salt = "";
            }
            if (user.security_token == null)
            {
                //user.security_token = Guid.NewGuid().ToString();
            }
            user.date_creation = DateTime.Now;
            user.date_last_activity = (DateTime)SqlDateTime.MinValue;
            user.date_last_login = (DateTime)SqlDateTime.MinValue;
            user.date_last_password_invalid = (DateTime)SqlDateTime.MinValue;
            user.date_lock = (DateTime)SqlDateTime.MinValue;
            this.context.InsertUser(user, this.ApplicationType);
            return Task.FromResult(this.context.SaveChanges());
        }
        public Task DeleteAsync(scr_user user)
        {
            this.context.DeleteUser(user);
            return Task.FromResult(this.context.SaveChanges());
        }
        public Task<scr_user> FindByIdAsync(int userId)
        {
            var user = this.context.GetUserByUserId(userId, this.ApplicationType);
            return Task.FromResult(user);
        }
        public Task<scr_user> FindByNameAsync(string userName)
        {
            var user = this.context.GetUserByUserName(userName, this.ApplicationType);
            return Task.FromResult(user);
        }
        public Task UpdateAsync(scr_user user)
        {
            this.context.UpdateUser(user);
            return Task.FromResult(this.context.SaveChanges());
        }
        #endregion

        #region IUserLockoutStore
        public Task<int> GetAccessFailedCountAsync(scr_user user)
        {
            return Task.FromResult(user.password_invalid_count);
        }
        public async Task<bool> GetLockoutEnabledAsync(scr_user user)
        {
            bool isAdmin = await IsInRoleAsync(user, "admins");
            return !isAdmin;
        }
        public Task<DateTimeOffset> GetLockoutEndDateAsync(scr_user user)
        {
            DateTimeOffset dateTimeOffset = new DateTimeOffset(DateTime.SpecifyKind(user.date_lock, DateTimeKind.Local));
            return Task.FromResult<DateTimeOffset>(dateTimeOffset);
        }
        public Task<int> IncrementAccessFailedCountAsync(scr_user user)
        {
            user.password_invalid_count = user.password_invalid_count + 1;
            //this.context.UpdateUser(user);
            return Task.FromResult<int>(user.password_invalid_count);
        }
        public Task ResetAccessFailedCountAsync(scr_user user)
        {
            user.password_invalid_count = 0;
            //this.context.UpdateUser(user);
            return Task.FromResult<int>(user.password_invalid_count);
        }
        public Task SetLockoutEnabledAsync(scr_user user, bool enabled)
        {
            return Task.FromResult<int>(0);
        }
        public Task SetLockoutEndDateAsync(scr_user user, DateTimeOffset lockoutEnd)
        {
            user.date_lock = lockoutEnd.LocalDateTime;
            //this.context.UpdateUser(user);
            return Task.FromResult<int>(0);
        }
        #endregion

        #region IUserPasswordStore
        public Task<string> GetPasswordHashAsync(scr_user user)
        {
            string password = user.password;
            if (String.IsNullOrEmpty(user.password))
            {
                password = null;
            }

            return Task.FromResult(password);
        }
        public Task<bool> HasPasswordAsync(scr_user user)
        {
            return Task.FromResult(!String.IsNullOrEmpty(user.password));
        }
        public Task SetPasswordHashAsync(scr_user user, string passwordHash)
        {
            user.password_format = 1;
            user.password = passwordHash;
            user.password_salt = "";
            return Task.FromResult<int>(0);
        }
        #endregion

        #region IUserClaimStore
        public Task AddClaimAsync(scr_user user, System.Security.Claims.Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }
            
            scr_user_claim userClaim = new scr_user_claim
            {
                user_id = user.user_id,
                claim_type = claim.Type,
                claim_value = claim.Value,
            };
            this.context.scr_user_claim.Add(userClaim);
            return Task.FromResult<int>(0);
        }
        public Task<System.Collections.Generic.IList<System.Security.Claims.Claim>> GetClaimsAsync(scr_user user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            
            IList<System.Security.Claims.Claim> claims = this.context.scr_user_claim
                .Where(ss => ss.user_id == user.user_id)
                .ToList()
                .Select(uc => new System.Security.Claims.Claim(uc.claim_type, uc.claim_value))
                .ToList();
            return Task.FromResult(claims);
        }
        public Task RemoveClaimAsync(scr_user user, System.Security.Claims.Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }
            
            IList<scr_user_claim> userClaimsForRemove = this.context.scr_user_claim
                .Where(uc => uc.claim_type == claim.Type && uc.claim_value == claim.Value)
                .ToList();
            foreach (scr_user_claim item in userClaimsForRemove)
            {
                this.context.scr_user_claim.Remove(item);
            }
            return Task.FromResult<int>(0);
        }
        #endregion

        #region IUserEmailStore
        public Task<scr_user> FindByEmailAsync(string email)
        {
            var user = this.context.GetUsersByEmail(email, this.ApplicationType).FirstOrDefault();
            return Task.FromResult(user);
        }
        public Task<string> GetEmailAsync(scr_user user)
        {
            return Task.FromResult(user.email);
        }
        public Task<bool> GetEmailConfirmedAsync(scr_user user)
        {
            return Task.FromResult(user.is_email_confirmed == 1);
        }
        public Task SetEmailAsync(scr_user user, string email)
        {
            user.email = email;
            // personal
            //if (user.ApplicationId == this.ApplicationType)
            //{
            //    user.user_name = email;
            //}
            return Task.FromResult<int>(0);
        }
        public Task SetEmailConfirmedAsync(scr_user user, bool confirmed)
        {
            user.is_email_confirmed = confirmed ? (short)1 : (short)0;
            return Task.FromResult<int>(0);
        }
        #endregion

        #region IUserRoleStore
        public Task<IList<string>> GetRolesAsync(scr_user user)
        {
            var userRoles = this.context.scr_GetRolesByUser(user.user_id).ToList();
            // пользователь входит в роль если: нет полного запрета и есть разрешение хотя бы на группу
            IList<string> roleNames = userRoles.GroupBy(s => s.role_name)
                .Where(g => !g.Any(s => s.group_id == null && s.is_deny) && g.Any(s => !s.is_deny))
                .Select(g => g.Key)
                .ToList();

            return Task.FromResult(roleNames);
        }
        public Task<bool> IsInRoleAsync(scr_user user, string roleName)
        {
            var userRoles = this.context.scr_GetRolesByUser(user.user_id).Where(ur => String.Compare(ur.role_name, roleName, true) == 0).ToList();
            // пользователь входит в роль если: нет полного запрета и есть разрешение хотя бы на группу
            bool result = userRoles.Any(s => !s.is_deny) && !userRoles.Any(s => s.group_id == null && s.is_deny);

            return Task.FromResult(result);
        }
        #endregion

        #region IUserTwoFactorStore
        public Task<bool> GetTwoFactorEnabledAsync(scr_user user)
        {
            return Task.FromResult(false);
        }
        public Task SetTwoFactorEnabledAsync(scr_user user, bool enabled)
        {
            return Task.FromResult<int>(0);
        }
        #endregion

        #region IUserLoginStore
        public Task AddLoginAsync(scr_user user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            scr_user_login userLogin = new scr_user_login
            {
                user_id = user.user_id,
                login_provider = login.LoginProvider,
                provider_key = login.ProviderKey,
            };
            this.context.scr_user_login.Add(userLogin);
            return Task.FromResult<int>(0);
        }
        public Task<scr_user> FindAsync(UserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            scr_user user = null;
            scr_user_login userLogin = this.context.scr_user_login.FirstOrDefault(l => l.login_provider == login.LoginProvider && l.provider_key == login.ProviderKey);
            if (userLogin != null)
            {
                user = this.context.GetUserByUserId(userLogin.user_id, this.ApplicationType);
            }
            return Task.FromResult(user);
        }
        public Task<IList<UserLoginInfo>> GetLoginsAsync(scr_user user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var userLogins = this.context.scr_user_login.Where(l => l.user_id == user.user_id);
            return Task.FromResult<IList<UserLoginInfo>>(userLogins.AsEnumerable().Select(l => new UserLoginInfo(l.login_provider, l.provider_key)).ToList());
        }
        public Task RemoveLoginAsync(scr_user user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            IList<scr_user_login> userLoginsForRemove = user.scr_user_login.Where(l => l.login_provider == login.LoginProvider && l.provider_key == login.ProviderKey).ToList();
            foreach (scr_user_login item in userLoginsForRemove)
            {
                this.context.scr_user_login.Remove(item);
            }
            return Task.FromResult<int>(0);
        }
        #endregion

        #region IUserSecurityStampStore
        public Task<string> GetSecurityStampAsync(scr_user user)
        {
            return Task.FromResult(user.security_token);
        }
        public Task SetSecurityStampAsync(scr_user user, string stamp)
        {
            user.security_token = stamp;
            return Task.FromResult<int>(0);
        }
        #endregion

        #region IUserPhoneNumberStore
        public Task<string> GetPhoneNumberAsync(scr_user user)
        {
            return Task.FromResult(user.phone_number);
        }
        public Task<bool> GetPhoneNumberConfirmedAsync(scr_user user)
        {
            return Task.FromResult(user.is_phone_number_confirmed == 1);
        }
        public Task SetPhoneNumberAsync(scr_user user, string phoneNumber)
        {
            user.phone_number = phoneNumber;
            return Task.FromResult<int>(0);
        }
        public Task SetPhoneNumberConfirmedAsync(scr_user user, bool confirmed)
        {
            user.is_phone_number_confirmed = confirmed ? (short)1 : (short)0;
            return Task.FromResult<int>(0);
        }
        #endregion

        #region AdvancedUserStore
        public Task<scr_user> FindByPhoneNumberAsync(string phoneNumber)
        {
            var user = this.context.GetUsersByPhoneNumber(phoneNumber, this.ApplicationType).FirstOrDefault();
            return Task.FromResult(user);
        }

        public async Task<System.Collections.Generic.IList<System.Security.Claims.Claim>> GetAllClaimsAsync(scr_user user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            IList<int> userGroups = await this.GetGroups(user);
            IList<System.Security.Claims.Claim> claims = this.context.scr_user_claim
                .Where(ss => userGroups.Contains(ss.user_id))
                .ToList()
                .Select(uc => new System.Security.Claims.Claim(uc.claim_type, uc.claim_value))
                .ToList();
            return claims;
        }

        public Task<IList<string>> GetRolesAsync(scr_user user, int groupId)
        {
            var userRoles = this.context.scr_GetRolesByUser(user.user_id).ToList();
            // пользователь входит в роль если: нет полного запрета, нет запрета на группу и есть разрешение на группу
            IList<string> roleNames = userRoles.GroupBy(s => s.role_name).Where(g => !g.Any(s => (s.group_id == null || s.group_id == groupId) && s.is_deny) && g.Any(s => s.group_id == groupId && !s.is_deny)).Select(g => g.Key).ToList();

            return Task.FromResult(roleNames);
        }
        
        public Task<IList<int>> GetGroups(scr_user user)
        {
            var groupList = new List<int>();
            // пользователь также включается
            // note: пересмотреть это поведение
            groupList.Add(user.user_id);

            int? userGroupId = user.user_group_id;
            while (userGroupId != null)
            {
                var userGroup = this.context.scr_user_group.FirstOrDefault(g => g.user_group_id == userGroupId);
                groupList.Add(userGroup.user_group_id);
                userGroupId = userGroup.parent_id;
            }
            
            return Task.FromResult<IList<int>>(groupList);
        }

        public Task<IList<int>> GetGroupsForRoleAsync(scr_user user, string roleName)
        {
            var userRoles = this.context.scr_GetRolesByUser(user.user_id).Where(s => s.role_name == roleName).ToList();
            bool deny = userRoles.Any(s => s.group_id == null && s.is_deny);
            ICollection<int> denyGroups = userRoles.Where(s => s.group_id != null && s.is_deny).Select(s => -(int)s.group_id).ToList();
            bool grant = userRoles.Any(s => s.group_id == null && !s.is_deny);
            ICollection<int> grantGroups = userRoles.Where(s => s.group_id != null && !s.is_deny).Select(s => (int)s.group_id).ToList();
            // если есть полный запрет или нет разрешения хотя бы на группу - пустой список
            if (deny || !(grant || grantGroups.Count > 0))
            {
                return Task.FromResult<IList<int>>(new int[0]);
            }
            // если нет полного разрешения - возвращаем разрешенные и запрещенные группы
            if (!grant)
            {
                return Task.FromResult<IList<int>>(new List<int>(grantGroups.Concat(denyGroups)));
            }
            // иначе - только запрещенные группы
            else
            {
                return Task.FromResult<IList<int>>(new List<int>(denyGroups));
            }
        }

        public Task<bool> GetUserActivatedAsync(scr_user user)
        {
            return Task.FromResult(user.is_approved == 1);
        }
        public Task SetUserActivatedAsync(scr_user user, bool activated)
        {
            user.is_approved = activated ? (short)1 : (short)0;
            return Task.FromResult<int>(0);
        }

        public Task SetLastLoginDateAsync(scr_user user, DateTimeOffset date)
        {
            user.date_last_login = date.LocalDateTime;
            user.date_last_activity = date.LocalDateTime;
            return Task.FromResult<int>(0);
        }
        public Task SetLastActivityDateAsync(scr_user user, DateTimeOffset date)
        {
            user.date_last_activity = date.LocalDateTime;
            return Task.FromResult<int>(0);
        }

        #region Session
        public Task CreateSessionAsync(scr_session session)
        {
            session.application_id = this.ApplicationType;
            if (session.session_uid == Guid.Empty)
            {
                session.session_uid = Guid.NewGuid();
            }
            session.date_beg = DateTime.Now;
            this.context.scr_session.Add(session);
            return Task.FromResult(this.context.SaveChanges());
        }
        public Task DeleteSessionAsync(scr_session session)
        {
            this.context.scr_session_claim.RemoveRange(this.context.scr_session_claim.Where(ss => ss.session_id == session.session_id));
            this.context.scr_session.Remove(session);
            return Task.FromResult(this.context.SaveChanges());
        }
        public Task<scr_session> FindSessionByIdAsync(long sessionId)
        {
            var session = this.context.scr_session.FirstOrDefault(ss => ss.session_id == sessionId);
            return Task.FromResult(session);
        }
        public Task<scr_session> FindSessionByUidAsync(string sessionUid)
        {
            Guid sessionGuid;
            scr_session session;
            if (!Guid.TryParse(sessionUid, out sessionGuid))
            {
                session = null;
            }
            else
            {
                session = this.context.scr_session.FirstOrDefault(ss => ss.session_uid == sessionGuid);
            }
            return Task.FromResult(session);
        }
        public Task UpdateSessionAsync(scr_session session)
        {
            this.context.Entry(session).State = EntityState.Modified;
            return Task.FromResult(this.context.SaveChanges());
        }

        public Task AddSessionClaimAsync(scr_session session, System.Security.Claims.Claim claim)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            var userClaim = new scr_session_claim
            {
                session_id = session.session_id,
                claim_type = claim.Type,
                claim_value = claim.Value,
            };
            this.context.scr_session_claim.Add(userClaim);
            return Task.FromResult<int>(0);
        }
        public Task<System.Collections.Generic.IList<System.Security.Claims.Claim>> GetSessionClaimsAsync(scr_session session)
        {
            if (session == null)
            {
                throw new ArgumentNullException("user");
            }

            IList<System.Security.Claims.Claim> claims = this.context.scr_session_claim
                .Where(ss => ss.session_id == session.session_id)
                .ToList()
                .Select(uc => new System.Security.Claims.Claim(uc.claim_type, uc.claim_value))
                .ToList();
            return Task.FromResult(claims);
        }
        public Task RemoveSessionClaimAsync(scr_session session, System.Security.Claims.Claim claim)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            var claimsForRemove = this.context.scr_session_claim
                .Where(uc => uc.claim_type == claim.Type && uc.claim_value == claim.Value);
            this.context.scr_session_claim.RemoveRange(claimsForRemove);
            return Task.FromResult<int>(0);
        }

        public Task<string> GetSessionEmailAsync(scr_session session)
        {
            return Task.FromResult(session.email);
        }
        public Task<bool> GetSessionEmailConfirmedAsync(scr_session session)
        {
            return Task.FromResult(session.is_email_confirmed == 1);
        }
        public Task SetSessionEmailAsync(scr_session session, string email)
        {
            session.email = email;
            return Task.FromResult<int>(0);
        }
        public Task SetSessionEmailConfirmedAsync(scr_session session, bool confirmed)
        {
            if (confirmed)
            {
                session.is_email_confirmed = 1;
                session.email_confirmation_date = DateTime.Now;
            }
            else
            {
                session.is_email_confirmed = 0;
                session.email_confirmation_date = null;
            }
            return Task.FromResult<int>(0);
        }

        public Task<string> GetSessionPhoneNumberAsync(scr_session user)
        {
            return Task.FromResult(user.phone_number);
        }
        public Task<bool> GetSessionPhoneNumberConfirmedAsync(scr_session session)
        {
            return Task.FromResult(session.is_phone_number_confirmed == 1);
        }
        public Task SetSessionPhoneNumberAsync(scr_session session, string phoneNumber)
        {
            session.phone_number = phoneNumber;
            return Task.FromResult<int>(0);
        }
        public Task SetSessionPhoneNumberConfirmedAsync(scr_session session, bool confirmed)
        {
            if (confirmed)
            {
                session.is_phone_number_confirmed = 1;
                session.phone_number_confirmation_date = DateTime.Now;
            }
            else
            {
                session.is_phone_number_confirmed = 0;
                session.phone_number_confirmation_date = null;
            }
            return Task.FromResult<int>(0);
        }

        public Task<string> GetSessionSecurityStampAsync(scr_session session)
        {
            return Task.FromResult(session.security_token);
        }
        public Task SetSessionSecurityStampAsync(scr_session session, string stamp)
        {
            session.security_token = stamp;
            return Task.FromResult<int>(0);
        }
        #endregion
        #endregion
    }
    #endregion

    #region UserGroupStore
    public class AdvancedUserGroupStore : PrincipalStore<scr_user_group>
    {
        public AdvancedUserGroupStore(SecurityContext context, int applicationType)
            : base(context, applicationType)
        {
        }

        #region IQueryableUserGroupStore
        public IQueryable<scr_user_group> UserGroups
        {
            get
            {
                return this.context.scr_user_group;
            }
        }
        #endregion

        #region UserGroupStore
        public Task CreateAsync(scr_user_group userGroup)
        {
            this.context.InsertUserGroup(userGroup, this.ApplicationType);
            return Task.FromResult(this.context.SaveChanges());
        }
        public Task DeleteAsync(scr_user_group userGroup)
        {
            this.context.DeleteUserGroup(userGroup);
            return Task.FromResult(this.context.SaveChanges());
        }
        public Task<scr_user_group> FindByIdAsync(int userGroupId)
        {
            return Task.FromResult(this.context.GetUserGroupByGroupId(userGroupId, this.ApplicationType));
        }
        public Task UpdateAsync(scr_user_group userGroup)
        {
            this.context.UpdateUserGroup(userGroup);
            return Task.FromResult(this.context.SaveChanges());
        }
        #endregion

        #region AdvancedUserGroupStore
        public Task<IList<int>> GetUsers(scr_user_group userGroup)
        {
            var userGroupIds = new List<int>();
            userGroupIds.Add(userGroup.user_group_id);
            userGroupIds.AddRange(this.context.GetUserGroupsByParentGroupId(userGroup.user_group_id, this.ApplicationType).Select(g => g.user_group_id));
            
            var users = this.context.scr_user.Where(u => u.scr_principal.application_id == this.ApplicationType && u.user_group_id != null && userGroupIds.Contains((int)u.user_group_id)).Select(u => u.user_id).ToArray();
            return Task.FromResult<IList<int>>(users);
        }

        public Task<IList<int>> GetGroups(scr_user_group userGroup)
        {
            var groupList = new List<int>();
            // текущая группа также включается
            // note: пересмотреть это поведение
            groupList.Add(userGroup.user_group_id);

            while (userGroup.parent_id != null)
            {
                userGroup = this.context.scr_user_group.FirstOrDefault(g => g.scr_principal.application_id == this.ApplicationType && g.user_group_id == userGroup.parent_id);
                groupList.Add(userGroup.user_group_id);
            }
            return Task.FromResult<IList<int>>(groupList);
        }
        #endregion
    }
    #endregion

    #region RoleStore
    public class AdvancedRoleStore : SecurityStore
        , IQueryableRoleStore<scr_role, int>
        , IRoleStore<scr_role, int>
    {
        public AdvancedRoleStore(SecurityContext context)
            : base(context)
        {
        }

        #region IQueryableRoleStore
        public IQueryable<scr_role> Roles
        { 
            get
            {
                return this.context.scr_role;
            }
        }
        #endregion

        #region IRoleStore
        public Task CreateAsync(scr_role role)
        {
            this.context.InsertRole(role);
            return Task.FromResult(this.context.SaveChanges());
        }
        public Task DeleteAsync(scr_role role)
        {
            this.context.DeleteRole(role);
            return Task.FromResult(this.context.SaveChanges());
        }
        public Task<scr_role> FindByIdAsync(int roleId)
        {
            return Task.FromResult(this.context.GetRoleByRoleId(roleId));
        }
        public Task<scr_role> FindByNameAsync(string roleName)
        {
            return Task.FromResult(this.context.GetRoleByRoleName(roleName));
        }
        public Task UpdateAsync(scr_role role)
        {
            this.context.UpdateRole(role);
            return Task.FromResult(this.context.SaveChanges());
        }
        #endregion

        #region AdvancedRoleStore
        private void AddToRole(scr_role role, scr_role parentRole)
        {
            if (parentRole != null)
            {
                this.context.InsertRoleRole(new scr_role_role
                {
                    role_id = parentRole.role_id,
                    role_member_id = role.role_id,
                });
            }
        }
        public Task AddToRoleAsync(scr_role role, int roleId)
        {
            scr_role parentRole = this.context.GetRoleByRoleId(roleId);
            this.AddToRole(role, parentRole);
            return Task.FromResult<int>(0);
        }
        public Task AddToRoleAsync(scr_role role, string roleName)
        {
            scr_role parentRole = this.context.GetRoleByRoleName(roleName);
            this.AddToRole(role, parentRole);
            return Task.FromResult<int>(0);
        }
        // проверить
        public Task<IList<scr_role>> GetRolesAsync(scr_role role)
        {
            IList<scr_role> roleRoles = this.context.GetRoleRolesByRoleMemberId(role.role_id).Select(r => r.scr_role1).ToList();
            return Task.FromResult(roleRoles);
        }
        #endregion
    }
    #endregion

    #region GroupStore
    public class AdvancedGroupStore : SecurityStore
    {
        public AdvancedGroupStore(SecurityContext context)
            : base(context)
        {
        }
        
        #region IQueryableUserGroupStore
        public IQueryable<scr_group> Groups
        {
            get
            {
                return this.context.scr_group;
            }
        }
        #endregion
        
        #region GroupStore
        public Task CreateAsync(scr_group group)
        {
            int id = this.context.scr_group.Select(g => g.group_id).DefaultIfEmpty(0).Max();
            group.group_id = id + 1;
            this.context.scr_group.Add(group);
            return Task.FromResult(this.context.SaveChanges());
        }
        public Task DeleteAsync(scr_group group)
        {
            this.context.scr_group.Remove(group);
            return Task.FromResult(this.context.SaveChanges());
        }
        public Task<scr_group> FindByIdAsync(int groupId)
        {
            scr_group group = this.context.scr_group.FirstOrDefault(g => g.group_id == groupId);
            return Task.FromResult(group);
        }
        public Task UpdateAsync(scr_group group)
        {
            //this.context.scr_group.ApplyCurrentValues(group);
            this.context.Entry(group).State = EntityState.Modified;
            return Task.FromResult(this.context.SaveChanges());
        }
        #endregion
    }
    #endregion

    //#region TmpUserStore
    //public class TmpUserStore : SecurityStore
    //    , IUserStore<tmp_scr_user, int>
    //    , IUserEmailStore<tmp_scr_user, int>
    //    , IUserClaimStore<tmp_scr_user, int>
    //    , IUserSecurityStampStore<tmp_scr_user, int>
    //    , IUserPhoneNumberStore<tmp_scr_user, int>
    //{
    //    public TmpUserStore(SecurityContext context)
    //        : base(context)
    //    {
    //    }

    //    #region IUserStore
    //    public Task CreateAsync(tmp_scr_user user)
    //    {
    //        this.context.tmp_scr_user.Add(user);
    //        return Task.FromResult(this.context.SaveChanges());
    //    }
    //    public Task DeleteAsync(tmp_scr_user user)
    //    {
    //        this.context.tmp_scr_user.Remove(user);
    //        return Task.FromResult(this.context.SaveChanges());
    //    }
    //    public Task<tmp_scr_user> FindByIdAsync(int userId)
    //    {
    //        var user = this.context.tmp_scr_user.FirstOrDefault(ss => ss.user_id == userId);
    //        return Task.FromResult(user);
    //    }
    //    public Task<tmp_scr_user> FindByNameAsync(string userName)
    //    {
    //        throw new InvalidOperationException();
    //    }
    //    public Task UpdateAsync(tmp_scr_user user)
    //    {
    //        this.context.Entry(user).State = EntityState.Modified;
    //        return Task.FromResult(this.context.SaveChanges());
    //    }
    //    #endregion

    //    #region IUserClaimStore
    //    public Task AddClaimAsync(tmp_scr_user user, System.Security.Claims.Claim claim)
    //    {
    //        if (user == null)
    //        {
    //            throw new ArgumentNullException("user");
    //        }
    //        if (claim == null)
    //        {
    //            throw new ArgumentNullException("claim");
    //        }

    //        var userClaim = new tmp_scr_user_claim
    //        {
    //            user_id = user.user_id,
    //            claim_type = claim.Type,
    //            claim_value = claim.Value,
    //        };
    //        this.context.tmp_scr_user_claim.Add(userClaim);
    //        return Task.FromResult<int>(0);
    //    }
    //    public Task<System.Collections.Generic.IList<System.Security.Claims.Claim>> GetClaimsAsync(tmp_scr_user user)
    //    {
    //        if (user == null)
    //        {
    //            throw new ArgumentNullException("user");
    //        }

    //        IList<System.Security.Claims.Claim> claims = this.context.tmp_scr_user_claim
    //            .Where(ss => ss.user_id == user.user_id)
    //            .ToList()
    //            .Select(uc => new System.Security.Claims.Claim(uc.claim_type, uc.claim_value))
    //            .ToList();
    //        return Task.FromResult(claims);
    //    }
    //    public Task RemoveClaimAsync(tmp_scr_user user, System.Security.Claims.Claim claim)
    //    {
    //        if (user == null)
    //        {
    //            throw new ArgumentNullException("user");
    //        }
    //        if (claim == null)
    //        {
    //            throw new ArgumentNullException("claim");
    //        }

    //        var userClaimsForRemove = this.context.tmp_scr_user_claim
    //            .Where(uc => uc.claim_type == claim.Type && uc.claim_value == claim.Value);
    //        this.context.tmp_scr_user_claim.RemoveRange(userClaimsForRemove);
    //        return Task.FromResult<int>(0);
    //    }
    //    #endregion

    //    #region IUserEmailStore
    //    public Task<tmp_scr_user> FindByEmailAsync(string email)
    //    {
    //        throw new InvalidOperationException();
    //    }
    //    public Task<string> GetEmailAsync(tmp_scr_user user)
    //    {
    //        return Task.FromResult(user.email);
    //    }
    //    public Task<bool> GetEmailConfirmedAsync(tmp_scr_user user)
    //    {
    //        return Task.FromResult(user.is_email_confirmed == 1);
    //    }
    //    public Task SetEmailAsync(tmp_scr_user user, string email)
    //    {
    //        user.email = email;
    //        return Task.FromResult<int>(0);
    //    }
    //    public Task SetEmailConfirmedAsync(tmp_scr_user user, bool confirmed)
    //    {
    //        user.is_email_confirmed = confirmed ? (short)1 : (short)0;
    //        return Task.FromResult<int>(0);
    //    }
    //    #endregion

    //    #region IUserSecurityStampStore
    //    public Task<string> GetSecurityStampAsync(tmp_scr_user user)
    //    {
    //        return Task.FromResult(user.security_token);
    //    }
    //    public Task SetSecurityStampAsync(tmp_scr_user user, string stamp)
    //    {
    //        user.security_token = stamp;
    //        return Task.FromResult<int>(0);
    //    }
    //    #endregion

    //    #region IUserPhoneNumberStore
    //    public Task<string> GetPhoneNumberAsync(tmp_scr_user user)
    //    {
    //        return Task.FromResult(user.phone_number);
    //    }
    //    public Task<bool> GetPhoneNumberConfirmedAsync(tmp_scr_user user)
    //    {
    //        return Task.FromResult(user.is_phone_number_confirmed == 1);
    //    }
    //    public Task SetPhoneNumberAsync(tmp_scr_user user, string phoneNumber)
    //    {
    //        user.phone_number = phoneNumber;
    //        return Task.FromResult<int>(0);
    //    }
    //    public Task SetPhoneNumberConfirmedAsync(tmp_scr_user user, bool confirmed)
    //    {
    //        user.is_phone_number_confirmed = confirmed ? (short)1 : (short)0;
    //        return Task.FromResult<int>(0);
    //    }
    //    #endregion
    //}
    //#endregion

}