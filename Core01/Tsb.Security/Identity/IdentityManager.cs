//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.Owin;
//using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

//using Tsb.Security.Web.Configuration;
using Tsb.Security.Web.Models;

namespace Tsb.Security.Web.Identity
{
    /// <summary>
    /// Коды ошибок
    /// </summary>
    public enum AdvancedIdentityResultCode
    {
        /// <summary>
        /// Неизвестная ошибка
        /// </summary>
        None = 0,

        /// <summary>
        /// Недопустимый адрес электронной почты
        /// </summary>
        InvalidEmail = 1,
        /// <summary>
        /// Адрес электронной почты уже используется
        /// </summary>
        DuplicateEmail = 2,
        /// <summary>
        /// Адрес электронной почты уже подтвержден
        /// </summary>
        EmailAlreadyConfirmed = 3,

        /// <summary>
        /// Недопустимый номер телефона
        /// </summary>
        InvalidPhoneNumber = 4,
        /// <summary>
        /// Номер телефона уже используется
        /// </summary>
        DuplicatePhoneNumber = 5,
        /// <summary>
        /// Номер телефона уже подтвержден
        /// </summary>
        PhoneNumberAlreadyConfirmed = 6,

        /// <summary>
        /// Недопустимый пароль
        /// </summary>
        InvalidPassword = 7,

        /// <summary>
        /// Недопустимая фамилия
        /// </summary>
        InvalidLastName = 8,
        /// <summary>
        /// Недопустимое имя
        /// </summary>
        InvalidFirstName = 9,
        /// <summary>
        /// Недопустимое отчество
        /// </summary>
        InvalidMiddleName = 10,

        /// <summary>
        /// Сессия не найдена
        /// </summary>
        SessionNotFound = 11,
        /// <summary>
        /// Пользователь не найден
        /// </summary>
        UserNotFound = 12,
        /// <summary>
        /// Недопустимый токен
        /// </summary>
        InvalidToken = 13,
        /// <summary>
        /// Найдено несколько пользователей
        /// </summary>
        MultipleUsersFound = 14,
        /// <summary>
        /// Пользователь заблокирован
        /// </summary>
        UserLocked = 15,
        /// <summary>
        /// Неправильный пароль
        /// </summary>
        IncorrectPassword = 16,

        /// <summary>
        /// Доступ запрещен
        /// </summary>
        AccessDenied = 21,

        /// <summary>
        /// Недопустимый ввод
        /// </summary>
        InvalidData = 254,
        /// <summary>
        /// Недопустимая операция
        /// </summary>
        InvalidOperation = 255,
    }

    public enum VerificationUserResult
    {
        Succeeded,
        Failed,
        VerifyEmail,
        VerifyPhoneNumber,
        VerifyActivation,
    }

    public interface IIdentityUrlGenerator
    {
        string Generate(NameValueCollection queryParams);
    }

    public class SpecificIdentityResult : IdentityResult
    {
        public long Result { get; private set; }
        public SpecificIdentityResult(long result)
            : base(true)
        {
            this.Result = result;
        }
        public SpecificIdentityResult(params string[] errors)
            : base(errors)
        {
        }
        public SpecificIdentityResult(IEnumerable<string> errors)
            : base(errors)
        {
        }

        public static new SpecificIdentityResult Failed(params string[] errors)
        {
            return new SpecificIdentityResult(errors);
        }
        public static SpecificIdentityResult Successfully(long result)
        {
            return new SpecificIdentityResult(result);
        }
    }

    public class AdvancedIdentityResultItem
    {
        /// <summary>
        /// Код ошибки
        /// </summary>
        public AdvancedIdentityResultCode Code { get; set; }
        /// <summary>
        /// Расшифровка ошибки
        /// </summary>
        public string CodeDescription
        {
            get
            {
                return this.Code.ToString();
            }
        }
        /// <summary>
        /// Комментарии
        /// </summary>
        public IEnumerable<string> Messages { get; set; }

        public AdvancedIdentityResultItem()
        {
        }
        public AdvancedIdentityResultItem(AdvancedIdentityResultCode code, params string[] messages)
        {
            this.Code = code;
            this.Messages = messages ?? new string[0];
        }
    }

    /// <summary>
    /// Результат выполнения операции
    /// </summary>
    public class AdvancedIdentityResult : IdentityResult
    {
        private List<AdvancedIdentityResultItem> errorList = new List<AdvancedIdentityResultItem>();

        /// <summary>
        /// Описание ошибок
        /// </summary>
        public IEnumerable<AdvancedIdentityResultItem> ErrorDescriptions
        {
            get
            {
                return this.errorList.AsReadOnly();
            }
        }
        
        public object Result { get; private set; }

        protected AdvancedIdentityResult(bool success)
            : base(success)
        {
        }
        public AdvancedIdentityResult(int result)
            : base(true)
        {
            this.Result = result;
        }
        public AdvancedIdentityResult(IEnumerable<AdvancedIdentityResultItem> errors)
            : base(errors.SelectMany(e => e.Messages))
        {
            errorList.AddRange(errors);
        }

        public static AdvancedIdentityResult CreateError(AdvancedIdentityResultCode errorCode, params string[] messages)
        {
            return new AdvancedIdentityResult(new AdvancedIdentityResultItem[]
            {
                new AdvancedIdentityResultItem(errorCode, messages),
            });
        }
        public static AdvancedIdentityResult CreateSuccess()
        {
            return new AdvancedIdentityResult(0);

        }
    }
    //public class AdvancedIdentityResult<T> : AdvancedIdentityResult
    //{
    //    public new T Result { get; private set; }

    //    public AdvancedIdentityResult(T result)
    //        : base(true)
    //    {
    //        this.Result = result;
    //    }
    //    public AdvancedIdentityResult(AdvancedIdentityResultItem success)
    //        : base(success)
    //    {
    //    }

    //    public static AdvancedIdentityResult CreateSuccess(T result, AdvancedIdentityResultCode successCode, params string[] messages)
    //    {
    //        var advancedIdentityResult = new AdvancedIdentityResult<T>(new AdvancedIdentityResultItem(successCode, messages));
    //        advancedIdentityResult.Result = result;
    //        return advancedIdentityResult;
    //    }
    //}

    public class AdvancedUserManager : UserManager<scr_user, int>
    {
        protected internal virtual int ApplicationId
        {
            get
            {
                AdvancedUserStore userStore = this.Store as AdvancedUserStore;
                if (userStore == null)
                {
                    throw new NotSupportedException();
                }

                return userStore.ApplicationType;
            }
        }

        // todo: отказаться
        public IIdentityUrlGenerator ConfirmEmailUrlGenerator { get; set; }
        // todo: отказаться
        public IIdentityUrlGenerator ResetPasswordUrlGenerator { get; set; }
        public PasswordGenerator PasswordGenerator { get; set; }
        public LoginBy LoginBy { get; set; }
        public UseAsName UseAsName { get; set; }
        //public PermissionMode ConnectionsPermissionMode { get; set; }
        public bool AutoGeneratePassword { get; set; }
        public bool KeepRegInfo { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public IList<ThirdPartyAuthentication> ThirdPartyAuthentications { get; set; }

        public AdvancedUserManager(IUserStore<scr_user, int> store)
            : base(store)
        {
            this.LoginBy = LoginBy.Email;
            this.UseAsName = UseAsName.Email;
            //this.ConnectionsPermissionMode = PermissionMode.None;
            this.UserValidator = new AdvancedUserValidator(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireEmail = true,
                RequireUniqueEmail = true,
                RequireConfirmedEmail = true,
                RequirePhoneNumber = false,
                RequireUniquePhoneNumber = false,
                RequireConfirmedPhoneNumber = false,

                RequireLastName = true,
                RequireFirstName = true,
                RequireMiddleName = true,
                RequireCyrillicName = true,
            };
            this.PasswordValidator = new AdvancedPasswordValidator
            {
                RequiredLength = 8,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            this.AutoGeneratePassword = false;
            this.KeepRegInfo = true;
            this.UserLockoutEnabledByDefault = true;
            this.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            this.MaxFailedAccessAttemptsBeforeLockout = 5;
            this.PasswordGenerator = new PasswordGenerator((PasswordValidator)this.PasswordValidator)
            {
                MaxLength = 8,
                WithoutNonLetterOrDigit = false,
                WithoutDigit = false,
                WithoutLowercase = false,
                WithoutUppercase = false,
            };
            this.TwoFactorEnabled = false;
            this.ThirdPartyAuthentications = new List<ThirdPartyAuthentication>();
        }
        
        private async Task<ClaimsIdentity> GenerateUserIdentityAsync(scr_user user, string authenticationType)
        {
            var userIdentity = await this.CreateIdentityAsync(user, authenticationType);
            userIdentity.AddClaim(new Claim(ClaimTypes.ApplicationType, user.ApplicationId.ToString()));

            #region email
            userIdentity.RemoveClaims(System.Security.Claims.ClaimTypes.Email);
            if (!String.IsNullOrEmpty(user.email))
            {
                userIdentity.AddClaim(new Claim(System.Security.Claims.ClaimTypes.Email, user.email, System.Security.Claims.ClaimValueTypes.String));
            }
            #endregion

            #region phone
            userIdentity.RemoveClaims(System.Security.Claims.ClaimTypes.MobilePhone);
            if (!String.IsNullOrEmpty(user.phone_number))
            {
                userIdentity.AddClaim(new Claim(System.Security.Claims.ClaimTypes.MobilePhone, user.phone_number, System.Security.Claims.ClaimValueTypes.String));
            }
            #endregion

            #region user groups
            int order = 0;
            foreach (int userGroupId in await this.GetGroups(user.user_id))
            {
                userIdentity.AddClaim(new Claim(ClaimTypes.UserGroup, String.Format("{0};{1}", ++order, userGroupId)));
            }
            #endregion

            #region roles in groups
            var userRoles = userIdentity.FindAll(userIdentity.RoleClaimType).Select(c => c.Value).ToList();
            foreach (string roleName in userRoles)
            {
                foreach (int groupId in await this.GetGroupsForRoleAsync(user.user_id, roleName))
                {
                    userIdentity.AddClaim(new Claim(ClaimTypes.RoleAndGroup, String.Format("{0};{1}", groupId, roleName)));
                }
            }
            #endregion

            return userIdentity;
        }
        public virtual async Task<ClaimsIdentity> GenerateUserIdentityAsync(scr_user user, string authenticationType, UserConnectionParam connectionParam)
        {
            var userIdentity = await this.GenerateUserIdentityAsync(user, authenticationType);
            if (connectionParam != null)
            {
                userIdentity.AddClaims(connectionParam.ToClaims());
            }
            return userIdentity;
        }
        internal AdvancedUserStore GetAdvancedUserStoreStore()
        {
            AdvancedUserStore store = this.Store as AdvancedUserStore;
            if (store == null)
            {
                throw new NotSupportedException("Store is not AdvancedUserStore");
            }

            return store;
        }
        // todo:
        private string GenerateConfirmEmailUrl(NameValueCollection queryParams)
        {
            if (this.ConfirmEmailUrlGenerator != null)
            {
                return this.ConfirmEmailUrlGenerator.Generate(queryParams);
            }
            return null;
        }
        public string GenerateConfirmEmailUrl(string email, string token, string sessionUid)
        {
            if (this.ConfirmEmailUrlGenerator != null)
            {
                var queryParams = new NameValueCollection
                {
                    { "email", email },
                    { "token", token },
                    { "sessionUid", sessionUid },
                };
                return this.ConfirmEmailUrlGenerator.Generate(queryParams);
            }
            return null;
        }
        public string GenerateConfirmEmailUrl(string email, string token, int userId)
        {
            if (this.ConfirmEmailUrlGenerator != null)
            {
                var queryParams = new NameValueCollection
                {
                    { "email", email },
                    { "token", token },
                    { "userId", userId.ToString() },
                };
                return this.ConfirmEmailUrlGenerator.Generate(queryParams);
            }
            return null;
        }
        private string GenerateResetPasswordUrl(NameValueCollection queryParams)
        {
            if (this.ResetPasswordUrlGenerator != null)
            {
                return this.ResetPasswordUrlGenerator.Generate(queryParams);
            }
            return null;
        }
        public string GenerateResetPasswordUrl(string email, string token, int userId)
        {
            if (this.ResetPasswordUrlGenerator != null)
            {
                var queryParams = new NameValueCollection
                {
                    { "email", email },
                    { "token", token },
                    { "userId", userId.ToString() },
                };
                return this.ResetPasswordUrlGenerator.Generate(queryParams);
            }
            return null;
        }


        #region AdvancedUserManager
        public static bool IsEmailAddress(string data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (data == "")
            {
                return false;
            }
            try
            {
                MailAddress mailAddress = new MailAddress(data);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        public static bool IsPhoneNumber(string data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (data == "")
            {
                return false;
            }
            PhoneAttribute phoneValidator = new PhoneAttribute();
            if (phoneValidator.IsValid(data))
            {
                return true;
            }
            return false;
        }
        public static string NormalizePhoneNumber(string phoneNumber)
        {
            phoneNumber = String.Concat(phoneNumber.Where(c => Char.IsDigit(c)).Reverse().Take(10).Reverse());
            return phoneNumber;
        }
        public string GetUserName(string email, string phoneNumber)
        {
            //string name = email;
            //if (this.UseAsName == UseAsName.Email)
            //{
            //    name = email;
            //}
            //else if (this.UseAsName == UseAsName.PhoneNumber)
            //{
            //    name = phoneNumber;
            //}

            //return name;

            string name = null;
            if (this.LoginBy == LoginBy.Email)
            {
                name = email;
            }
            else if (this.LoginBy == LoginBy.PhoneNumber)
            {
                name = phoneNumber;
            }
            else if (this.UserValidator is AdvancedUserValidator)
            {
                if (((AdvancedUserValidator)this.UserValidator).RequireEmail)
                {
                    name = email;
                }
                else if (((AdvancedUserValidator)this.UserValidator).RequirePhoneNumber)
                {
                    name = phoneNumber;
                }
            }
            else
            {
                if (email != null)
                {
                    name = email;
                }
                else if (phoneNumber!= null)
                {
                    name = phoneNumber;
                }
            }

            if (name == null)
            {
                throw new Exception("email and phoneNumber is null");
            }

            return name;
        }

        public virtual void ApplyConfiguration(SystemIdentity configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            this.LoginBy = (LoginBy)configuration.LoginBy;
            this.UseAsName = (UseAsName)configuration.UseAsName;
            if (!this.LoginBy.HasFlag((LoginBy)this.UseAsName))
            {
                foreach (UseAsName item in Enum.GetValues(typeof(UseAsName)))
                {
                    if (this.LoginBy.HasFlag((LoginBy)item))
                    {
                        this.UseAsName = item;
                        break;
                    }
                }
            }
            this.AutoGeneratePassword = configuration.AutoGeneratePassword;
            this.KeepRegInfo = configuration.KeepRegInfo;
            if (configuration.UserValidator != null)
            {
                this.UserValidator = new AdvancedUserValidator(this)
                {
                    AllowOnlyAlphanumericUserNames = configuration.UserValidator.AllowOnlyAlphanumericUserNames,

                    RequireEmail = this.LoginBy == LoginBy.Email || configuration.UserValidator.RequireEmail,
                    RequireUniqueEmail = this.LoginBy.HasFlag(LoginBy.Email) || configuration.UserValidator.RequireUniqueEmail,
                    RequireConfirmedEmail = configuration.UserValidator.RequireConfirmedEmail,
                    RequirePhoneNumber = this.LoginBy == LoginBy.PhoneNumber || configuration.UserValidator.RequirePhoneNumber,
                    RequireUniquePhoneNumber = this.LoginBy.HasFlag(LoginBy.PhoneNumber) || configuration.UserValidator.RequireUniquePhoneNumber,
                    RequireConfirmedPhoneNumber = configuration.UserValidator.RequireConfirmedPhoneNumber,

                    RequireLastName = configuration.UserValidator.RequireFullName,
                    RequireFirstName = configuration.UserValidator.RequireFullName,
                    RequireMiddleName = configuration.UserValidator.RequireFullName,
                    RequireCyrillicName = configuration.UserValidator.RequireCyrillicName,
                };
            }
            if (configuration.PasswordValidator != null)
            {
                this.PasswordValidator = new AdvancedPasswordValidator
                {
                    RequiredLength = configuration.PasswordValidator.RequiredLength,
                    RequireNonLetterOrDigit = configuration.PasswordValidator.RequireNonLetterOrDigit,
                    RequireDigit = configuration.PasswordValidator.RequireDigit,
                    RequireLowercase = configuration.PasswordValidator.RequireLowercase,
                    RequireUppercase = configuration.PasswordValidator.RequireUppercase,
                };
            }
            if (configuration.PasswordGenerator != null)
            {
                this.PasswordGenerator = new PasswordGenerator((PasswordValidator)this.PasswordValidator)
                {
                    MaxLength = configuration.PasswordGenerator.MaxLength,
                    WithoutNonLetterOrDigit = configuration.PasswordGenerator.WithoutNonLetterOrDigit,
                    WithoutDigit = configuration.PasswordGenerator.WithoutDigit,
                    WithoutLowercase = configuration.PasswordGenerator.WithoutLowercase,
                    WithoutUppercase = configuration.PasswordGenerator.WithoutUppercase,
                };
            }
            if (configuration.Lockout != null)
            {
                this.UserLockoutEnabledByDefault = configuration.Lockout.Enabled;
                this.DefaultAccountLockoutTimeSpan = TimeSpan.FromSeconds(configuration.Lockout.LockoutInterval);
                this.MaxFailedAccessAttemptsBeforeLockout = configuration.Lockout.MaxFailedAccessAttempts;
            }
            if (configuration.TwoFactor != null)
            {
                this.TwoFactorEnabled = configuration.TwoFactor.Enabled;
            }
            if (configuration.ThirdPartyAuthentication != null)
            {
                foreach (var item in configuration.ThirdPartyAuthentication.Items)
                {
                    this.ThirdPartyAuthentications.Add(new ThirdPartyAuthentication
                    {
                        AuthenticationType = item.AuthenticationType,
                        Caption = item.Caption,
                        AllowedOn = (Communication)item.AllowedOn,
                        IsWindows = item.IsWindows,
                    });
                }
            }
        }
        
        public virtual async Task<scr_user> FindByPhoneNumberAsync(string phoneNumber)
        {
            AdvancedUserStore userStore = this.Store as AdvancedUserStore;
            if (userStore == null)
            {
                throw new NotSupportedException();
            }

            if (phoneNumber == null)
            {
                throw new ArgumentNullException("phoneNumber");
            }

            phoneNumber = NormalizePhoneNumber(phoneNumber);
            return await userStore.FindByPhoneNumberAsync(phoneNumber);
        }
        public override async Task<scr_user> FindByNameAsync(string userName)
        {
            if (userName == null)
            {
                throw new ArgumentNullException("userName");
            }

            if (this.LoginBy.HasFlag(LoginBy.Email) && IsEmailAddress(userName)) 
            {
                return await this.FindByEmailAsync(userName);
            }

            if (this.LoginBy.HasFlag(LoginBy.PhoneNumber) && IsPhoneNumber(userName))
            {
                return await this.FindByPhoneNumberAsync(userName);
            }

            return await base.FindByNameAsync(userName);
        }
        public override Task<bool> GetTwoFactorEnabledAsync(int userId)
        {
            return Task.FromResult(this.TwoFactorEnabled);
        }

        public virtual async Task<IdentityResult> AddToRoleAsync(int userId, string role, int groupId)
        {
            IdentityResult identityResult;
            AdvancedUserStore userRoleStore = this.Store as AdvancedUserStore;
            if (userRoleStore == null)
            {
                throw new NotSupportedException();
            }

            scr_user user = await this.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException();
            }

            var userRoles = await userRoleStore.GetRolesAsync(user, groupId);
            if (!userRoles.Contains(role))
            {
                await userRoleStore.AddToRoleAsync(user, role, groupId);
                identityResult = await this.UpdateAsync(user);
            }
            else
            {
                identityResult = new IdentityResult(new string[] { "Пользователь уже входит в заданную роль." });
            }
            return identityResult;
        }
        public virtual async Task<IdentityResult> AddToRolesAsync(int userId, int groupId, params string[] roles)
        {
            IdentityResult identityResult;
            AdvancedUserStore userRoleStore = this.Store as AdvancedUserStore;
            if (userRoleStore == null)
            {
                throw new NotSupportedException();
            }

            if (roles == null)
            {
                throw new ArgumentNullException("roles");
            }

            scr_user user = await this.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException();
            }

            var userRoles = await userRoleStore.GetRolesAsync(user, groupId);
            string[] strArrays = roles;
            int num = 0;
            while (num < (int)strArrays.Length)
            {
                string str = strArrays[num];
                if (!userRoles.Contains(str))
                {
                    await userRoleStore.AddToRoleAsync(user, str, groupId);
                    num++;
                }
                else
                {
                    identityResult = new IdentityResult(new string[] { "Пользователь уже входит в заданную роль." });
                    return identityResult;
                }
            }
            identityResult = await this.UpdateAsync(user);

            return identityResult;
        }

        public virtual async Task<IList<int>> GetGroups(int userId)
        {
            AdvancedUserStore userRoleStore = this.Store as AdvancedUserStore;
            if (userRoleStore == null)
            {
                throw new NotSupportedException();
            }

            scr_user user = await this.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException();
            }

            return await userRoleStore.GetGroups(user);
        }

        public virtual async Task<IList<int>> GetGroupsForRoleAsync(int userId, string role)
        {
            AdvancedUserStore userRoleStore = this.Store as AdvancedUserStore;
            if (userRoleStore == null)
            {
                throw new NotSupportedException();
            }

            scr_user user = await this.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException();
            }

            return await userRoleStore.GetGroupsForRoleAsync(user, role);
        }

        public virtual async Task<IdentityResult> AddOrUpdateClaimsAsync(int userId, params Claim[] claims)
        {
            IdentityResult identityResult;
            IUserClaimStore<scr_user, int> userClaimStore = this.Store as IUserClaimStore<scr_user, int>;
            if (userClaimStore == null)
            {
                throw new NotSupportedException();
            }

            if (claims == null)
            {
                throw new ArgumentNullException("claims");
            }

            scr_user user = await this.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException();
            }

            var userClaims = await userClaimStore.GetClaimsAsync(user);
            foreach (var claim in claims)
            {
                IList<Claim> userClaim = userClaims.Where(c => c.Type == claim.Type).ToList();
                if (userClaim.Count == 0)
                {
                    await userClaimStore.AddClaimAsync(user, claim);
                }
                else if (userClaim.Count != 1 || userClaim[0].Value != claim.Value)
                {
                    foreach (Claim item in userClaim)
                    {
                        await userClaimStore.RemoveClaimAsync(user, item);
                    }
                    await userClaimStore.AddClaimAsync(user, claim);
                }
            }
            identityResult = await this.UpdateAsync(user);

            return identityResult;
        }
        public virtual async Task<IdentityResult> RemoveClaimsAsync(int userId, params Claim[] claims)
        {
            IdentityResult identityResult;
            IUserClaimStore<scr_user, int> userClaimStore = this.Store as IUserClaimStore<scr_user, int>;
            if (userClaimStore == null)
            {
                throw new NotSupportedException();
            }

            if (claims == null)
            {
                throw new ArgumentNullException("claims");
            }

            scr_user user = await this.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException();
            }

            foreach (var claim in claims)
            {
                await userClaimStore.RemoveClaimAsync(user, claim);
            }
            identityResult = await this.UpdateAsync(user);

            return identityResult;
        }
        public virtual async Task<IdentityResult> RemoveClaimsAsync(int userId, string claimType)
        {
            IdentityResult identityResult;
            IUserClaimStore<scr_user, int> userClaimStore = this.Store as IUserClaimStore<scr_user, int>;
            if (userClaimStore == null)
            {
                throw new NotSupportedException();
            }

            if (claimType == null)
            {
                throw new ArgumentNullException("claimType");
            }

            scr_user user = await this.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException();
            }

            var userClaims = (await userClaimStore.GetClaimsAsync(user))
                .Where(c => c.Type == claimType)
                .ToList();
            if (userClaims.Count > 0)
            {
                foreach (var userClaim in userClaims)
                {
                    await userClaimStore.RemoveClaimAsync(user, userClaim);
                }
                identityResult = await this.UpdateAsync(user);
            }
            else
            {
                identityResult = IdentityResult.Success;
            }

            return identityResult;
        }
        public virtual async Task<IList<string>> GetConnectionClaimsAsync(int userId)
        {
            AdvancedUserStore userClaimStore = this.Store as AdvancedUserStore;
            if (userClaimStore == null)
            {
                throw new NotSupportedException();
            }

            scr_user user = await this.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException();
            }

            var userClaims = await userClaimStore.GetAllClaimsAsync(user);
            var connectionClaims = userClaims
                .Where(s => s.Type == ClaimTypes.GrantedConnectionName)
                .Select(s => s.Value)
                .ToList();

            return connectionClaims;
        }
        public async Task<bool> IsConnectionAllowedAsync(int userId, string connectionName)
        {
            //if (this.ConnectionsPermissionMode == PermissionMode.None)
            //{
            //    return true;
            //}
            var connections = await this.GetConnectionClaimsAsync(userId);
            return connections.Count == 0 || connections.Contains(connectionName);
        }
        public virtual async Task<IList<string>> GetClientClaimsAsync(int userId)
        {
            AdvancedUserStore userClaimStore = this.Store as AdvancedUserStore;
            if (userClaimStore == null)
            {
                throw new NotSupportedException();
            }

            scr_user user = await this.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException();
            }

            var userClaims = await userClaimStore.GetAllClaimsAsync(user);
            var clientClaims = userClaims
                .Where(s => s.Type == ClaimTypes.GrantedClientId)
                .Select(s => s.Value)
                .ToList();

            return clientClaims;
        }
        public async Task<bool> IsClientAllowedAsync(int userId, int clientId)
        {
            //if (this.ConnectionsPermissionMode == PermissionMode.None)
            //{
            //    return true;
            //}
            var clients = await this.GetClientClaimsAsync(userId);
            return clients.Count == 0 || clients.Contains(clientId.ToString());
        }

        public virtual async Task<bool> IsUserActivatedAsync(int userId)
        {
            AdvancedUserStore userStore = this.Store as AdvancedUserStore;
            if (userStore == null)
            {
                throw new NotSupportedException();
            }

            scr_user user = await this.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException();
            }

            return await userStore.GetUserActivatedAsync(user);
        }
        public virtual async Task<IdentityResult> SetUserActivatedAsync(int userId)
        {
            AdvancedUserStore userStore = this.Store as AdvancedUserStore;
            if (userStore == null)
            {
                throw new NotSupportedException();
            }

            scr_user user = await this.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException();
            }

            await userStore.SetUserActivatedAsync(user, true);
            IdentityResult identityResult = await this.UpdateAsync(user);
            return identityResult;
        }
        public virtual async Task<IdentityResult> ActivateUserAsync(int userId)
        {
            return await this.SetUserActivatedAsync(userId);
        }
        public virtual async Task<VerificationUserResult> VerifyUserAsync(int userId)
        {
            var userManager = this;
            //bool isEmailConfirmed = await userManager.IsEmailConfirmedAsync(userId);
            //bool isPhoneNumberConfirmed = await userManager.IsPhoneNumberConfirmedAsync(userId);
            bool isUserActivated = await userManager.IsUserActivatedAsync(userId);

            //var validator = userManager.UserValidator as AdvancedUserValidator;
            //if (validator != null)
            //{
            //    // проверяем подтверждение электронной почты
            //    if (validator.RequireConfirmedEmail && !isEmailConfirmed)
            //    {
            //        return VerificationUserResult.VerifyEmail;
            //    }
            //    // проверяем подтверждение номера телефона
            //    if (validator.RequireConfirmedPhoneNumber && !isPhoneNumberConfirmed)
            //    {
            //        return VerificationUserResult.VerifyPhoneNumber;
            //    }
            //}
            // проверяем активацию
            if (!isUserActivated)
            {
                return VerificationUserResult.VerifyActivation;
            }

            return VerificationUserResult.Succeeded;
        }

        public virtual async Task<IdentityResult> UpdateSecurityStampAsync(params int[] userIds)
        {
            if (userIds == null || userIds.Length == 0)
            {
                userIds = this.Users.Select(u => u.user_id).ToArray();
            }
            foreach (int userId in userIds)
            {
                var identityResult = await base.UpdateSecurityStampAsync(userId);
                //if (!identityResult.Succeeded)
                //{
                //    return identityResult;
                //}
            }

            return IdentityResult.Success;
        }
        public virtual async Task SendSmsAsync(string phoneNumber, string message)
        {
            if (this.SmsService != null)
            {
                IdentityMessage identityMessage = new IdentityMessage()
                {
                    Destination = phoneNumber,
                    Body = message,
                };
                await this.SmsService.SendAsync(identityMessage).ConfigureAwait(false);
            }
        }
        public virtual async Task SendEmailAsync(string email, string subject, string body)
        {
            if (this.EmailService != null)
            {
                IdentityMessage identityMessage = new IdentityMessage()
                {
                    Destination = email,
                    Subject = subject,
                    Body = body,
                };
                await this.EmailService.SendAsync(identityMessage);
            }
        }
        // todo: прибраться
        public virtual async Task SendConfirmEmail(string email, string token, string sessionUid)
        {
            string message = this.GenerateConfirmEmailUrl(email, token, sessionUid);
            await this.SendEmailAsync(email, "", message);
        }
        public virtual async Task SendConfirmEmail(string email, string code)
        {
            await this.SendEmailAsync(email, "", code);
        }
        public virtual async Task SendConfirmSms(string phoneNumber, string code)
        {
            await this.SendSmsAsync(phoneNumber, code);
        }
        public virtual async Task SendPasswordEmail(int userId, string password)
        {
            await this.SendEmailAsync(userId, "", password);
        }
        public virtual async Task SendPasswordSms(int userId, string password)
        {
            await this.SendSmsAsync(userId, password);
        }
        public virtual async Task SendResetPasswordEmail(int userId, string token)
        {
            IUserEmailStore<scr_user, int> userStore = this.Store as IUserEmailStore<scr_user, int>;
            if (userStore == null)
            {
                throw new NotSupportedException();
            }

            scr_user user = await this.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException();
            }

            string email = await userStore.GetEmailAsync(user);
            string message = this.GenerateResetPasswordUrl(email, token, userId);
            await this.SendEmailAsync(userId, "", message);
        }
        public virtual async Task SendResetPasswordEmailWithCode(int userId, string code)
        {
            IUserEmailStore<scr_user, int> userStore = this.Store as IUserEmailStore<scr_user, int>;
            if (userStore == null)
            {
                throw new NotSupportedException();
            }

            scr_user user = await this.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException();
            }

            string email = await userStore.GetEmailAsync(user);
            await this.SendEmailAsync(email, "", code);
        }
        public virtual async Task SendResetPasswordSms(int userId, string code)
        {
            await this.SendSmsAsync(userId, code);
        }
        public virtual async Task SendChangeEmailEmail(string email, string code)
        {
            await this.SendEmailAsync(email, "", code);
        }

        public virtual async Task<IdentityResult> SetEmailConfirmedAsync(int userId)
        {
            IUserEmailStore<scr_user, int> userEmailStore = this.Store as IUserEmailStore<scr_user, int>;
            if (userEmailStore == null)
            {
                throw new NotSupportedException();
            }
            
            scr_user user = await this.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException();
            }
            
            await userEmailStore.SetEmailConfirmedAsync(user, true);
            IdentityResult identityResult = await this.UpdateAsync(user);
            return identityResult;
        }
        public virtual async Task<IdentityResult> SetPhoneNumberConfirmedAsync(int userId)
        {
            IUserPhoneNumberStore<scr_user, int> userPhoneNumberStore = this.Store as IUserPhoneNumberStore<scr_user, int>;
            if (userPhoneNumberStore == null)
            {
                throw new NotSupportedException();
            }

            scr_user user = await this.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException();
            }

            await userPhoneNumberStore.SetPhoneNumberConfirmedAsync(user, true);
            IdentityResult identityResult = await this.UpdateAsync(user);
            return identityResult;
        }
        public virtual async Task<IdentityResult> SetLastLoginDateAsync(int userId)
        {
            var store = this.GetAdvancedUserStoreStore();
            var user = await store.FindByIdAsync(userId);
            if (user == null)
            {
                return AdvancedIdentityResult.CreateError(AdvancedIdentityResultCode.UserNotFound, "Пользователь не найден.");
            }

            DateTimeOffset date = DateTimeOffset.Now;
            await store.SetLastLoginDateAsync(user, date);
            IdentityResult identityResult = await this.UpdateAsync(user);
            return identityResult;
        }
        public virtual async Task<IdentityResult> SetLastActivityDateAsync(int userId)
        {
            var store = this.GetAdvancedUserStoreStore();
            var user = await store.FindByIdAsync(userId);
            if (user == null)
            {
                return AdvancedIdentityResult.CreateError(AdvancedIdentityResultCode.UserNotFound, "Пользователь не найден.");
            }

            DateTimeOffset date = DateTimeOffset.Now;
            await store.SetLastActivityDateAsync(user, date);
            IdentityResult identityResult = await this.UpdateAsync(user);
            return identityResult;
        }




        protected virtual Task<string> GenerateTotpTokenAsync(string stamp, string purpose)
        {
            SecurityToken securityToken = new SecurityToken(Encoding.Unicode.GetBytes(stamp));
            int num = Rfc6238AuthenticationService.GenerateCode(securityToken, purpose);
            return Task.FromResult(num.ToString("D6", System.Globalization.CultureInfo.InvariantCulture));
        }
        protected virtual Task<bool> VerifyTotpTokenAsync(string stamp, string token, string purpose)
        {
            SecurityToken securityToken = new SecurityToken(Encoding.Unicode.GetBytes(stamp));
            int code;
            bool result = !int.TryParse(token, out code) ? false : Rfc6238AuthenticationService.ValidateCode(securityToken, code, purpose);

            return Task.FromResult(result);
        }
        protected virtual Task<string> GenerateTokenAsync(string stamp, string purpose)
        {
            var dataProtectorTokenProvider = this.UserTokenProvider as DataProtectorTokenProvider<scr_user, int>;
            if (this.UserTokenProvider == null)
            {
                throw new NotSupportedException("TokenProviderIsNotDataProtectorTokenProvider");
            }

            var dataProtectionService = new DataProtectionService(dataProtectorTokenProvider.Protector);
            dataProtectionService.TokenLifespan = dataProtectorTokenProvider.TokenLifespan;
            string token = dataProtectionService.GenerateToken(stamp, purpose);

            return Task.FromResult(token);
        }
        protected virtual Task<bool> VerifyTokenAsync(string stamp, string token, string purpose)
        {
            var dataProtectorTokenProvider = this.UserTokenProvider as DataProtectorTokenProvider<scr_user, int>;
            if (this.UserTokenProvider == null)
            {
                throw new NotSupportedException("TokenProviderIsNotDataProtectorTokenProvider");
            }

            var dataProtectionService = new DataProtectionService(dataProtectorTokenProvider.Protector);
            dataProtectionService.TokenLifespan = dataProtectorTokenProvider.TokenLifespan;
            bool result = dataProtectionService.ValidateToken(stamp, purpose, token);

            return Task.FromResult(result);
        }

        public virtual Task<string> GenerateEmailTokenAsync(string email, string stamp, string purpose = null)
        {
            string modifier = $"{email}";
            if (purpose != null)
            {
                modifier = $"{modifier}:{purpose}";
            }
            return this.GenerateTokenAsync(stamp, modifier);
        }
        public virtual Task<bool> VerifyEmailTokenAsync(string email, string stamp, string token, string purpose = null)
        {
            string modifier = $"{email}";
            if (purpose != null)
            {
                modifier = $"{modifier}:{purpose}";
            }

            return this.VerifyTokenAsync(stamp, token, modifier);
        }
        public virtual Task<string> GeneratePhoneNumberCodeAsync(string phoneNumber, string stamp, string purpose = null)
        {
            string modifier = $"{phoneNumber}";
            if (purpose != null)
            {
                modifier = $"{modifier}:{purpose}";
            }

            return this.GenerateTotpTokenAsync(stamp, modifier);
        }
        public virtual Task<bool> VerifyPhoneNumberCodeAsync(string phoneNumber, string stamp, string code, string purpose = null)
        {
            string modifier = $"{phoneNumber}";
            if (purpose != null)
            {
                modifier = $"{modifier}:{purpose}";
            }

            return this.VerifyTotpTokenAsync(stamp, code, modifier);
        }


        public virtual async Task<string> GenerateTwoFactorCodeAsync(int userId)
        {
            var store = this.GetAdvancedUserStoreStore();
            var user = await store.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("UserNotFound");
            }

            string stamp = await store.GetSecurityStampAsync(user);
            string phone = await store.GetPhoneNumberAsync(user);
            string token = await this.GeneratePhoneNumberCodeAsync(phone, stamp, "twoFactor:user");

            return token;
        }
        public virtual async Task<IdentityResult> VerifyTwoFactorCodeAsync(int userId, string code)
        {
            var store = this.GetAdvancedUserStoreStore();
            var user = await store.FindByIdAsync(userId);
            if (user == null)
            {
                return AdvancedIdentityResult.CreateError(AdvancedIdentityResultCode.UserNotFound, "Пользователь не найден.");
            }

            string stamp = await store.GetSecurityStampAsync(user);
            string phone = await store.GetPhoneNumberAsync(user);
            bool result = await this.VerifyPhoneNumberCodeAsync(phone, stamp, code, "twoFactor:user");
            if (!result)
            {
                return AdvancedIdentityResult.CreateError(AdvancedIdentityResultCode.InvalidToken, "Неверный код.");
            }

            return AdvancedIdentityResult.CreateSuccess();
        }

        #region Change Email
        public async Task<string> GenerateChangeEmailCodeAsync(int userId, string email)
        {
            var store = this.GetAdvancedUserStoreStore();
            var user = await store.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("UserNotFound");
            }

            string stamp = await store.GetSecurityStampAsync(user);
            string token = await this.GeneratePhoneNumberCodeAsync(email, stamp, "emailChange:user");

            return token;
        }
        public async Task<bool> VerifyChangeEmailCodeAsync(int userId, string code, string email)
        {
            var store = this.GetAdvancedUserStoreStore();
            var user = await store.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("UserNotFound");
            }

            string stamp = await store.GetSecurityStampAsync(user);
            bool result = await this.VerifyPhoneNumberCodeAsync(email, stamp, code, "emailChange:user");
            return result;
        }
        public async Task<IdentityResult> ChangeEmailAsync(int userId, string email, string code)
        {
            var store = this.GetAdvancedUserStoreStore();
            var user = await store.FindByIdAsync(userId);
            if (user == null)
            {
                return AdvancedIdentityResult.CreateError(AdvancedIdentityResultCode.UserNotFound, "Пользователь не найден.");
            }

            var verifyResult = await this.VerifyChangeEmailCodeAsync(userId, code, email);
            if (!verifyResult)
            {
                return AdvancedIdentityResult.CreateError(AdvancedIdentityResultCode.InvalidToken, "Неверный код.");
            }

            await store.SetEmailAsync(user, email);
            await store.SetEmailConfirmedAsync(user, true);
            await store.SetSecurityStampAsync(user, Guid.NewGuid().ToString());

            var updateResult = await this.UpdateAsync(user);
            return updateResult;
        }
        #endregion

        #region Change PhoneNumber
        public override async Task<string> GenerateChangePhoneNumberTokenAsync(int userId, string phoneNumber)
        {
            var store = this.GetAdvancedUserStoreStore();
            var user = await store.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("UserNotFound");
            }

            string stamp = await store.GetSecurityStampAsync(user);
            string token = await this.GeneratePhoneNumberCodeAsync(phoneNumber, stamp, "phoneNumberChange:user");

            return token;
        }
        public override async Task<bool> VerifyChangePhoneNumberTokenAsync(int userId, string code, string phoneNumber)
        {
            var store = this.GetAdvancedUserStoreStore();
            var user = await store.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("UserNotFound");
            }

            string stamp = await store.GetSecurityStampAsync(user);
            bool result = await this.VerifyPhoneNumberCodeAsync(phoneNumber, stamp, code, "phoneNumberChange:user");
            return result;
        }
        public override async Task<IdentityResult> ChangePhoneNumberAsync(int userId, string phoneNumber, string code)
        {
            var store = this.GetAdvancedUserStoreStore();
            var user = await store.FindByIdAsync(userId);
            if (user == null)
            {
                return AdvancedIdentityResult.CreateError(AdvancedIdentityResultCode.UserNotFound, "Пользователь не найден.");
            }

            var verifyResult = await this.VerifyChangePhoneNumberTokenAsync(userId, code, phoneNumber);
            if (!verifyResult)
            {
                return AdvancedIdentityResult.CreateError(AdvancedIdentityResultCode.InvalidToken, "Неверный код.");
            }

            await store.SetPhoneNumberAsync(user, phoneNumber);
            await store.SetPhoneNumberConfirmedAsync(user, true);
            await store.SetSecurityStampAsync(user, Guid.NewGuid().ToString());

            var updateResult = await this.UpdateAsync(user);
            return updateResult;
        }
        #endregion

        #region Reset Password
        public override async Task<string> GeneratePasswordResetTokenAsync(int userId)
        {
            var store = this.GetAdvancedUserStoreStore();
            var user = await store.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("UserNotFound");
            }

            string stamp = await store.GetSecurityStampAsync(user);
            string email = await store.GetEmailAsync(user);
            string token = await this.GenerateEmailTokenAsync(email, stamp, "passwordReset:user");

            return token;
        }
        public virtual async Task<IdentityResult> VerifyPasswordResetTokenAsync(int userId, string token)
        {
            var store = this.GetAdvancedUserStoreStore();
            var user = await store.FindByIdAsync(userId);
            if (user == null)
            {
                return AdvancedIdentityResult.CreateError(AdvancedIdentityResultCode.UserNotFound, "Пользователь не найден.");
            }

            string stamp = await store.GetSecurityStampAsync(user);
            string email = await store.GetEmailAsync(user);
            bool result = await this.VerifyEmailTokenAsync(email, stamp, token, "passwordReset:user");
            if (!result)
            {
                return AdvancedIdentityResult.CreateError(AdvancedIdentityResultCode.InvalidToken, "Недопустимый маркер.");
            }

            return AdvancedIdentityResult.CreateSuccess();
        }
        public override async Task<IdentityResult> ResetPasswordAsync(int userId, string token, string newPassword)
        {
            var store = this.GetAdvancedUserStoreStore();
            var user = await store.FindByIdAsync(userId);
            if (user == null)
            {
                return AdvancedIdentityResult.CreateError(AdvancedIdentityResultCode.UserNotFound, "Пользователь не найден.");
            }

            var verifyResult = await this.VerifyPasswordResetTokenAsync(userId, token);
            if (!verifyResult.Succeeded)
            {
                return verifyResult;
            }

            var updateResult = await this.UpdatePassword(store, user, newPassword);
            if (updateResult.Succeeded)
            {
                updateResult = await this.UpdateAsync(user);
            }
            return updateResult;
        }

        public virtual async Task<string> GeneratePasswordResetCodeAsync(int userId)
        {
            var store = this.GetAdvancedUserStoreStore();
            var user = await store.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("UserNotFound");
            }

            string stamp = await store.GetSecurityStampAsync(user);
            string phone = await store.GetPhoneNumberAsync(user);
            string token = await this.GeneratePhoneNumberCodeAsync(phone, stamp, "passwordReset:user");

            return token;
        }
        public virtual async Task<IdentityResult> VerifyPasswordResetCodeAsync(int userId, string code)
        {
            var store = this.GetAdvancedUserStoreStore();
            var user = await store.FindByIdAsync(userId);
            if (user == null)
            {
                return AdvancedIdentityResult.CreateError(AdvancedIdentityResultCode.UserNotFound, "Пользователь не найден.");
            }

            string stamp = await store.GetSecurityStampAsync(user);
            string phone = await store.GetPhoneNumberAsync(user);
            bool result = await this.VerifyPhoneNumberCodeAsync(phone, stamp, code, "passwordReset:user");
            if (!result)
            {
                return AdvancedIdentityResult.CreateError(AdvancedIdentityResultCode.InvalidToken, "Неверный код.");
            }

            return AdvancedIdentityResult.CreateSuccess();
        }
        public virtual async Task<IdentityResult> ResetPasswordByCodeAsync(int userId, string code, string newPassword)
        {
            var store = this.GetAdvancedUserStoreStore();
            var user = await store.FindByIdAsync(userId);
            if (user == null)
            {
                return AdvancedIdentityResult.CreateError(AdvancedIdentityResultCode.UserNotFound, "Пользователь не найден.");
            }

            var verifyResult = await this.VerifyPasswordResetCodeAsync(userId, code);
            if (!verifyResult.Succeeded)
            {
                return verifyResult;
            }

            var updateResult = await this.UpdatePassword(store, user, newPassword);
            if (updateResult.Succeeded)
            {
                updateResult = await this.UpdateAsync(user);
            }
            return updateResult;
        }

        public virtual async Task<string> GeneratePasswordResetCodeForEmailAsync(int userId)
        {
            var store = this.GetAdvancedUserStoreStore();
            var user = await store.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("UserNotFound");
            }

            string stamp = await store.GetSecurityStampAsync(user);
            string email = await store.GetEmailAsync(user);
            string token = await this.GeneratePhoneNumberCodeAsync(email, stamp, "passwordReset:user");

            return token;
        }
        public virtual async Task<IdentityResult> VerifyPasswordResetCodeForEmailAsync(int userId, string code)
        {
            var store = this.GetAdvancedUserStoreStore();
            var user = await store.FindByIdAsync(userId);
            if (user == null)
            {
                return AdvancedIdentityResult.CreateError(AdvancedIdentityResultCode.UserNotFound, "Пользователь не найден.");
            }

            string stamp = await store.GetSecurityStampAsync(user);
            string email = await store.GetEmailAsync(user);
            bool result = await this.VerifyPhoneNumberCodeAsync(email, stamp, code, "passwordReset:user");
            if (!result)
            {
                return AdvancedIdentityResult.CreateError(AdvancedIdentityResultCode.InvalidToken, "Неверный код.");
            }

            return AdvancedIdentityResult.CreateSuccess();
        }
        public virtual async Task<IdentityResult> ResetPasswordByCodeForEmailAsync(int userId, string code, string newPassword)
        {
            var store = this.GetAdvancedUserStoreStore();
            var user = await store.FindByIdAsync(userId);
            if (user == null)
            {
                return AdvancedIdentityResult.CreateError(AdvancedIdentityResultCode.UserNotFound, "Пользователь не найден.");
            }

            var verifyResult = await this.VerifyPasswordResetCodeForEmailAsync(userId, code);
            if (!verifyResult.Succeeded)
            {
                return verifyResult;
            }

            var updateResult = await this.UpdatePassword(store, user, newPassword);
            if (updateResult.Succeeded)
            {
                updateResult = await this.UpdateAsync(user);
            }
            return updateResult;
        }
        #endregion

        #region Session
        //public virtual Task<scr_session> FindSessionByIdAsync(long sessionId)
        //{
        //    var store = this.GetAdvancedUserStoreStore();
        //    return store.FindSessionByIdAsync(sessionId);
        //}
        public virtual Task<scr_session> FindSessionByUidAsync(string sessionUid)
        {
            var store = this.GetAdvancedUserStoreStore();
            return store.FindSessionByUidAsync(sessionUid);
        }
        public virtual async Task<IdentityResult> CreateSessionAsync(scr_session session, bool validate = true)
        {
            AdvancedUserStore userStore = this.GetAdvancedUserStoreStore();
            var userValidator = this.UserValidator as IIdentityValidator<scr_session>;
            if (userValidator == null)
            {
                throw new NotSupportedException();
            }
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            await userStore.SetSessionSecurityStampAsync(session, Guid.NewGuid().ToString());
            var identityResult = IdentityResult.Success;
            if (validate)
            {
                identityResult = await userValidator.ValidateAsync(session);
                if (!identityResult.Succeeded)
                {
                    return identityResult;
                }
            }
            await userStore.CreateSessionAsync(session);
            return identityResult;
        }
        public virtual async Task<IdentityResult> UpdateSessionAsync(scr_session session, bool validate = true)
        {
            AdvancedUserStore userStore = this.GetAdvancedUserStoreStore();
            var userValidator = this.UserValidator as IIdentityValidator<scr_session>;
            if (userValidator == null)
            {
                throw new NotSupportedException();
            }
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            var identityResult = IdentityResult.Success;
            if (validate)
            {
                identityResult = await userValidator.ValidateAsync(session);
                if (!identityResult.Succeeded)
                {
                    return identityResult;
                }
            }
            await userStore.UpdateSessionAsync(session);
            return identityResult;
        }
        public virtual async Task<string> GenerateSessionEmailConfirmationTokenAsync(string sessionUid)
        {
            var store = this.GetAdvancedUserStoreStore();
            var session = await store.FindSessionByUidAsync(sessionUid);
            if (session == null)
            {
                throw new InvalidOperationException("SessionNotFound");
            }

            string stamp = await store.GetSessionSecurityStampAsync(session);
            string email = await store.GetSessionEmailAsync(session);
            string token = await this.GenerateEmailTokenAsync(email, stamp, "emailConfirmation:session");

            return token;
        }
        public virtual async Task<IdentityResult> ConfirmSessionEmailAsync(string sessionUid, string token)
        {
            var store = this.GetAdvancedUserStoreStore();
            var session = await store.FindSessionByUidAsync(sessionUid);
            if (session == null)
            {
                return AdvancedIdentityResult.CreateError(AdvancedIdentityResultCode.SessionNotFound, "Сессия не найдена.");
            }
            bool isConfirmed = await store.GetSessionEmailConfirmedAsync(session);
            if (isConfirmed)
            {
                return AdvancedIdentityResult.CreateError(AdvancedIdentityResultCode.EmailAlreadyConfirmed, "Адрес электронной почты уже подтвержден.");
            }
            string stamp = await store.GetSessionSecurityStampAsync(session);
            string email = await store.GetSessionEmailAsync(session);
            bool result = await this.VerifyEmailTokenAsync(email, stamp, token, "emailConfirmation:session");
            if (!result)
            {
                return AdvancedIdentityResult.CreateError(AdvancedIdentityResultCode.InvalidToken, "Недопустимый маркер.");
            }
            await store.SetSessionEmailConfirmedAsync(session, true);
            var updateResult = await this.UpdateSessionAsync(session, false);

            return updateResult;
        }
        public virtual async Task<string> GenerateSessionEmailConfirmationCodeAsync(string sessionId)
        {
            var store = this.GetAdvancedUserStoreStore();
            var session = await store.FindSessionByUidAsync(sessionId);
            if (session == null)
            {
                throw new InvalidOperationException("SessionNotFound");
            }

            string stamp = await store.GetSessionSecurityStampAsync(session);
            string email = await store.GetSessionEmailAsync(session);
            string token = await this.GeneratePhoneNumberCodeAsync(email, stamp, "emailConfirmation:session");

            return token;
        }
        public virtual async Task<IdentityResult> ConfirmSessionEmailCodeAsync(string sessionUid, string code)
        {
            var store = this.GetAdvancedUserStoreStore();
            var session = await store.FindSessionByUidAsync(sessionUid);
            if (session == null)
            {
                return AdvancedIdentityResult.CreateError(AdvancedIdentityResultCode.SessionNotFound, "Сессия не найдена.");
            }
            bool isConfirmed = await store.GetSessionEmailConfirmedAsync(session);
            if (isConfirmed)
            {
                return AdvancedIdentityResult.CreateError(AdvancedIdentityResultCode.EmailAlreadyConfirmed, "Адрес электронной почты уже подтвержден.");
            }
            string stamp = await store.GetSessionSecurityStampAsync(session);
            string email = await store.GetSessionEmailAsync(session);
            bool result = await this.VerifyPhoneNumberCodeAsync(email, stamp, code, "emailConfirmation:session");
            if (!result)
            {
                return AdvancedIdentityResult.CreateError(AdvancedIdentityResultCode.InvalidToken, "Неверный код.");
            }
            await store.SetSessionEmailConfirmedAsync(session, true);
            var updateResult = await this.UpdateSessionAsync(session, false);

            return updateResult;
        }
        public virtual async Task<string> GenerateSessionPhoneNumberConfirmationCodeAsync(string sessionUid)
        {
            var store = this.GetAdvancedUserStoreStore();
            var session = await store.FindSessionByUidAsync(sessionUid);
            if (session == null)
            {
                throw new InvalidOperationException("SessionNotFound");
            }

            string stamp = await store.GetSessionSecurityStampAsync(session);
            string phone = await store.GetSessionPhoneNumberAsync(session);
            string token = await this.GeneratePhoneNumberCodeAsync(phone, stamp, "phoneNumberConfirmation:session");

            return token;
        }
        public virtual async Task<IdentityResult> ConfirmSessionPhoneNumberAsync(string sessionUid, string code)
        {
            var store = this.GetAdvancedUserStoreStore();
            var session = await store.FindSessionByUidAsync(sessionUid);
            if (session == null)
            {
                return AdvancedIdentityResult.CreateError(AdvancedIdentityResultCode.SessionNotFound, "Сессия не найдена.");
            }
            bool isConfirmed = await store.GetSessionPhoneNumberConfirmedAsync(session);
            if (isConfirmed)
            {
                return AdvancedIdentityResult.CreateError(AdvancedIdentityResultCode.PhoneNumberAlreadyConfirmed, "Номер телефона уже подтвержден.");
            }
            string stamp = await store.GetSessionSecurityStampAsync(session);
            string phone = await store.GetSessionPhoneNumberAsync(session);
            bool result = await this.VerifyPhoneNumberCodeAsync(phone, stamp, code, "phoneNumberConfirmation:session");
            if (!result)
            {
                return AdvancedIdentityResult.CreateError(AdvancedIdentityResultCode.InvalidToken, "Неверный код.");
            }
            await store.SetSessionPhoneNumberConfirmedAsync(session, true);
            var updateResult = await this.UpdateSessionAsync(session, false);

            return updateResult;
        }
        //public virtual async Task<IList<Claim>> GetSessionClaimsAsync(string sessionUid)
        //{
        //    var store = this.GetAdvancedUserStoreStore();
        //    var session = await store.FindSessionByUidAsync(sessionUid);
        //    if (session == null)
        //    {
        //        throw new InvalidOperationException("SessionNotFound");
        //    }

        //    return await store.GetSessionClaimsAsync(session);
        //}
        //public virtual async Task<IdentityResult> AddOrUpdateSessionClaimsAsync(string sessionUid, params Claim[] claims)
        //{
        //    var store = this.GetAdvancedUserStoreStore();

        //    if (claims == null)
        //    {
        //        throw new ArgumentNullException(nameof(claims));
        //    }

        //    var session = await store.FindSessionByUidAsync(sessionUid);
        //    if (session == null)
        //    {
        //        throw new InvalidOperationException("SessionNotFound");
        //    }

        //    var userClaims = await store.GetSessionClaimsAsync(session);
        //    foreach (var claim in claims)
        //    {
        //        IList<Claim> userClaim = userClaims.Where(c => c.Type == claim.Type).ToList();
        //        if (userClaim.Count == 0)
        //        {
        //            await store.AddSessionClaimAsync(session, claim);
        //        }
        //        else if (userClaim.Count != 1 || userClaim[0].Value != claim.Value)
        //        {
        //            foreach (Claim item in userClaim)
        //            {
        //                await store.RemoveSessionClaimAsync(session, item);
        //            }
        //            await store.AddSessionClaimAsync(session, claim);
        //        }
        //    }
        //    var identityResult = await this.UpdateSessionAsync(session);

        //    return identityResult;
        //}
        #endregion

        #endregion
    }

    public class AdvancedRoleManager : RoleManager<scr_role, int>
    {
        public AdvancedRoleManager(IRoleStore<scr_role, int> store)
            : base(store)
        {
        }

        #region AdvancedRoleManager
        //public virtual async Task<IdentityResult> AddToRoleAsync(int roleId, int parentRoleId)
        //{
        //    IdentityResult identityResult;
        //    AdvancedUserStore userRoleStore = this.Store as AdvancedUserStore;
        //    if (userRoleStore == null)
        //    {
        //        throw new NotSupportedException();
        //    }

        //    scr_user user = await this.FindByIdAsync(userId);
        //    if (user == null)
        //    {
        //        throw new InvalidOperationException();
        //    }
        //    var userRoles = await userRoleStore.GetRolesAsync(user, groupId);
        //    if (!userRoles.Contains(role))
        //    {
        //        await userRoleStore.AddToRoleAsync(user, role, groupId);
        //        identityResult = await this.UpdateAsync(user);
        //    }
        //    else
        //    {
        //        identityResult = new IdentityResult(new string[] { "Пользователь уже входит в заданную роль." });
        //    }
        //    return identityResult;
        //}
        //public virtual async Task<IdentityResult> AddToRolesAsync(int userId, int groupId, params string[] roles)
        //{
        //    IdentityResult identityResult;
        //    AdvancedUserStore userRoleStore = this.Store as AdvancedUserStore;
        //    if (userRoleStore == null)
        //    {
        //        throw new NotSupportedException();
        //    }

        //    if (roles == null)
        //    {
        //        throw new ArgumentNullException("roles");
        //    }

        //    scr_user user = await this.FindByIdAsync(userId);
        //    if (user == null)
        //    {
        //        throw new InvalidOperationException();
        //    }

        //    var userRoles = await userRoleStore.GetRolesAsync(user, groupId);
        //    string[] strArrays = roles;
        //    int num = 0;
        //    while (num < (int)strArrays.Length)
        //    {
        //        string str = strArrays[num];
        //        if (!userRoles.Contains(str))
        //        {
        //            await userRoleStore.AddToRoleAsync(user, str, groupId);
        //            num++;
        //        }
        //        else
        //        {
        //            identityResult = new IdentityResult(new string[] { "Пользователь уже входит в заданную роль." });
        //            return identityResult;
        //        }
        //    }
        //    identityResult = await this.UpdateAsync(user);

        //    return identityResult;
        //}
        #endregion
    }

    public class AdvancedGroupManager : IDisposable
    {
        private bool _disposed;

        protected AdvancedGroupStore Store
        {
            get;
            private set;
        }

        public AdvancedGroupManager(AdvancedGroupStore store)
        {
            if (store == null)
            {
                throw new ArgumentNullException("store");
            }
            this.Store = store;
        }

        private void ThrowIfDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }
        public void Dispose()
        {
            this.Dispose(true);
            //GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !this._disposed)
            {
                this.Store.Dispose();
            }
            this._disposed = true;
        }

        public virtual async Task<IdentityResult> CreateAsync(scr_group group)
        {
            this.ThrowIfDisposed();
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }
            
            await this.Store.CreateAsync(group);
            return IdentityResult.Success;
        }
        public virtual async Task<IdentityResult> DeleteAsync(scr_group group)
        {
            this.ThrowIfDisposed();
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }

            await this.Store.DeleteAsync(group);
            return IdentityResult.Success;
        }
        public virtual async Task<scr_group> FindByIdAsync(int groupId)
        {
            this.ThrowIfDisposed();
            return await this.Store.FindByIdAsync(groupId);
        }
        public virtual async Task<IdentityResult> UpdateAsync(scr_group group)
        {
            this.ThrowIfDisposed();
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }

            await this.Store.UpdateAsync(group);
            return IdentityResult.Success;
        }
    }

    public class AdvancedUserGroupManager : IDisposable
    {
        private bool _disposed;

        protected AdvancedUserGroupStore Store
        {
            get;
            private set;
        }

        public AdvancedUserGroupManager(AdvancedUserGroupStore store)
        {
            if (store == null)
            {
                throw new ArgumentNullException("store");
            }
            this.Store = store;
        }

        private void ThrowIfDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }
        public void Dispose()
        {
            this.Dispose(true);
            //GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !this._disposed)
            {
                this.Store.Dispose();
            }
            this._disposed = true;
        }

        public virtual async Task<IdentityResult> CreateAsync(scr_user_group userGroup)
        {
            this.ThrowIfDisposed();
            if (userGroup == null)
            {
                throw new ArgumentNullException("userGroup");
            }

            await this.Store.CreateAsync(userGroup);
            return IdentityResult.Success;
        }
        public virtual async Task<IdentityResult> DeleteAsync(scr_user_group userGroup)
        {
            this.ThrowIfDisposed();
            if (userGroup == null)
            {
                throw new ArgumentNullException("userGroup");
            }

            await this.Store.DeleteAsync(userGroup);
            return IdentityResult.Success;
        }
        public virtual async Task<scr_user_group> FindByIdAsync(int userGroupId)
        {
            this.ThrowIfDisposed();
            return await this.Store.FindByIdAsync(userGroupId);
        }
        public virtual async Task<IdentityResult> UpdateAsync(scr_user_group userGroup)
        {
            this.ThrowIfDisposed();
            if (userGroup == null)
            {
                throw new ArgumentNullException("userGroup");
            }

            await this.Store.UpdateAsync(userGroup);
            return IdentityResult.Success;
        }

        public virtual async Task<IList<int>> GetUsers(int userGroupId)
        {
            scr_user_group userGroup = await this.FindByIdAsync(userGroupId);
            if (userGroup == null)
            {
                throw new InvalidOperationException();
            }

            return await this.Store.GetUsers(userGroup);
        }

        public virtual async Task<IList<int>> GetGroups(int userGroupId)
        {
            scr_user_group userGroup = await this.FindByIdAsync(userGroupId);
            if (userGroup == null)
            {
                throw new InvalidOperationException();
            }

            return await this.Store.GetGroups(userGroup);
        }
    }

    #region AdvancedSignInManager
    //public class AdvancedSignInManager : SignInManager<scr_user, int>
    //{
    //    protected internal int ApplicationId
    //    {
    //        get
    //        {
    //            return ((AdvancedUserManager)this.UserManager).ApplicationId;
    //        }
    //    }

    //    public AdvancedSignInManager(AdvancedUserManager userManager, IAuthenticationManager authenticationManager)
    //        : base(userManager, authenticationManager)
    //    {
    //    }

    //    public override Task<ClaimsIdentity> CreateUserIdentityAsync(scr_user user)
    //    {
    //        return ((AdvancedUserManager)UserManager).GenerateUserIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie, null);
    //    }
    //    public override async Task SignInAsync(scr_user user, bool isPersistent, bool rememberBrowser)
    //    {
    //        await base.SignInAsync(user, isPersistent, rememberBrowser);

    //        #region Another Cookie
    //        ClaimsIdentity claimsIdentity = new ClaimsIdentity("ApplicationShadowCookie");
    //        claimsIdentity.AddClaim(new Claim(System.Security.Claims.ClaimTypes.Name, user.user_name));
    //        AuthenticationProperties authenticationProperty = new AuthenticationProperties()
    //        {
    //            IsPersistent = isPersistent
    //        };
    //        this.AuthenticationManager.SignIn(authenticationProperty, new ClaimsIdentity[] { claimsIdentity });
    //        #endregion

    //        // попытаемся обновить данные в БД
    //        await ((AdvancedUserManager)this.UserManager).SetLastLoginDateAsync(user.user_id);
    //    }
    //}
    #endregion

    public class AdvancedUserValidator : UserValidator<scr_user, int>, IIdentityValidator<scr_session>
    {
        private readonly AdvancedUserManager manager;
        public const string CyrillicNamePattern = @"^[а-яА-ЯёЁ\s\-]{1,64}$";
        public const string NamePattern = @"^[\p{L}\s\-]{1,64}$";

        public bool RequireEmail { get; set; }
        public bool RequireConfirmedEmail { get; set; }

        public bool RequirePhoneNumber { get; set; }
        public bool RequireUniquePhoneNumber { get; set; }
        public bool RequireConfirmedPhoneNumber { get; set; }

        public bool RequireLastName { get; set; }
        public bool RequireFirstName { get; set; }
        public bool RequireMiddleName { get; set; }
        public bool RequireCyrillicName { get; set; }

        public AdvancedUserValidator(AdvancedUserManager manager)
            : base(manager)
        {
            this.manager = manager;
        }

        public async Task ValidatePhoneNumberAsync(int userId, string phoneNumber, ICollection<AdvancedIdentityResultItem> errors, bool checkRequirement)
        {
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                PhoneAttribute phoneValidator = new PhoneAttribute();
                if (!phoneValidator.IsValid(phoneNumber))
                {
                    errors.Add(new AdvancedIdentityResultItem(AdvancedIdentityResultCode.InvalidPhoneNumber, $"Номер телефона \"{phoneNumber}\" является недопустимым."));
                    return;
                }
                if (this.RequireUniquePhoneNumber)
                {
                    scr_user currentUser = await this.manager.FindByPhoneNumberAsync(phoneNumber);
                    if (currentUser != null && currentUser.user_id != userId)
                    {
                        errors.Add(new AdvancedIdentityResultItem(AdvancedIdentityResultCode.DuplicatePhoneNumber, $"Номер телефона \"{phoneNumber}\" уже используется."));
                    }
                }
            }
            else if (checkRequirement)
            {
                if (this.RequirePhoneNumber)
                {
                    errors.Add(new AdvancedIdentityResultItem(AdvancedIdentityResultCode.InvalidPhoneNumber, "Номер телефона не может быть пустым."));
                }
            }
        }
        public async Task ValidateEmailAsync(int userId, string email, ICollection<AdvancedIdentityResultItem> errors, bool checkRequirement)
        {
            if (!string.IsNullOrEmpty(email))
            {
                try
                {
                    MailAddress mailAddress = new MailAddress(email);
                }
                catch (FormatException)
                {
                    errors.Add(new AdvancedIdentityResultItem(AdvancedIdentityResultCode.InvalidEmail, $"Адрес электронной почты \"{email}\" является недопустимым."));
                    return;
                }
                if (this.RequireUniqueEmail)
                {
                    scr_user currentUser = await this.manager.FindByEmailAsync(email);
                    if (currentUser != null && currentUser.user_id != userId)
                    {
                        errors.Add(new AdvancedIdentityResultItem(AdvancedIdentityResultCode.DuplicateEmail, $"Адрес электронной почты \"{email}\" уже используется."));
                        return;
                    }
                }
            }
            else if (checkRequirement)
            {
                if (this.RequireEmail)
                {
                    errors.Add(new AdvancedIdentityResultItem(AdvancedIdentityResultCode.InvalidEmail, "Адрес электронной почты не может быть пустым."));
                    return;
                }
            }
        }
        public async Task ValidateFullNameAsync(int userId, UserFullName fullName, ICollection<AdvancedIdentityResultItem> errors, bool checkRequirement)
        {
            string pattern = this.RequireCyrillicName ? CyrillicNamePattern : NamePattern;
            //
            if (fullName.LastNameSpecified)
            {
                if (!Regex.IsMatch(fullName.LastName, pattern))
                {
                    errors.Add(new AdvancedIdentityResultItem(AdvancedIdentityResultCode.InvalidLastName, "Некорректная фамилия."));
                }
            }
            else if (checkRequirement)
            {
                if (this.RequireLastName)
                {
                    errors.Add(new AdvancedIdentityResultItem(AdvancedIdentityResultCode.InvalidLastName, "Фамилия не может быть пустой."));
                }
            }
            //
            if (fullName.FirstNameSpecified)
            {
                if (!Regex.IsMatch(fullName.FirstName, pattern))
                {
                    errors.Add(new AdvancedIdentityResultItem(AdvancedIdentityResultCode.InvalidFirstName, "Некорректное имя."));
                }
            }
            else if (checkRequirement)
            {
                if (this.RequireFirstName)
                {
                    errors.Add(new AdvancedIdentityResultItem(AdvancedIdentityResultCode.InvalidFirstName, "Имя не может быть пустым."));
                }
            }
            //
            if (fullName.MiddleNameSpecified)
            {
                if (!Regex.IsMatch(fullName.MiddleName, pattern))
                {
                    errors.Add(new AdvancedIdentityResultItem(AdvancedIdentityResultCode.InvalidMiddleName, "Некорректное отчество."));
                }
            }
            else if (checkRequirement)
            {
                if (this.RequireMiddleName)
                {
                    errors.Add(new AdvancedIdentityResultItem(AdvancedIdentityResultCode.InvalidMiddleName, "Отчество не может быть пустым."));
                }
            }
        }

        //public override async Task<IdentityResult> ValidateAsync(scr_user item)
        //{
        //    if (item == null)
        //    {
        //        throw new ArgumentNullException("item");
        //    }

        //    IdentityResult identityResult;
        //    //identityResult = await base.ValidateAsync(item);
        //    //List<string> errors = identityResult.Succeeded ? new List<string>() : new List<string>(identityResult.Errors);
        //    List<AdvancedIdentityResultError> errorList = new List<AdvancedIdentityResultError>();
        //    if (this.RequireUniqueEmail)
        //    {
        //        List<string> errors = new List<string>();
        //        await this.ValidateEmailAsync(item, errors);
        //        if (errors.Count > 0)
        //        {
        //            errorList.Add(new AdvancedIdentityResultError(AdvancedIdentityResultCode.Email, errors));
        //        }
        //    }
        //    if (this.RequireUniquePhoneNumber)
        //    {
        //        List<string> errors = new List<string>();
        //        await this.ValidatePhoneNumberAsync(item, errors);
        //        if (errors.Count > 0)
        //        {
        //            errorList.Add(new AdvancedIdentityResultError(AdvancedIdentityResultCode.PhoneNumber, errors));
        //        }
        //    }
        //    identityResult = errorList.Count <= 0 ? IdentityResult.Success : new AdvancedIdentityResult(errorList);

        //    return identityResult;
        //}
        public async override Task<IdentityResult> ValidateAsync(scr_user item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            IdentityResult identityResult;
            List<AdvancedIdentityResultItem> errorList = new List<AdvancedIdentityResultItem>();
            var store = this.manager.GetAdvancedUserStoreStore();

            string email = await store.GetEmailAsync(item);
            await this.ValidateEmailAsync(item.user_id, email, errorList, true);
            
            string phoneNumber = await store.GetPhoneNumberAsync(item);
            await this.ValidatePhoneNumberAsync(item.user_id, phoneNumber, errorList, true);

            // проверяем только при создании, при обновлении данные имени не заполняются
            if (item.user_id == 0)
            {
                var fullName = UserFullName.Create(item.last_name, item.first_name, item.middle_name);
                await this.ValidateFullNameAsync(item.user_id, fullName, errorList, true);
            }

            if (errorList.Count == 0 && String.IsNullOrEmpty(email) && String.IsNullOrEmpty(phoneNumber))
            {
                errorList.Add(new AdvancedIdentityResultItem(AdvancedIdentityResultCode.InvalidData, "Должен быть указан адрес электронной почты или номер телефона."));
            }

            identityResult = errorList.Count == 0 ?
                new AdvancedIdentityResult(0) :
                new AdvancedIdentityResult(errorList);

            return identityResult;
        }
        public async Task<IdentityResult> ValidateAsync(scr_session item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            IdentityResult identityResult;
            List<AdvancedIdentityResultItem> errorList = new List<AdvancedIdentityResultItem>();
            var store = this.manager.GetAdvancedUserStoreStore();
            bool checkRequirement = true; // item.register_type == (int)RegisteredBy.External || item.register_type == (int)RegisteredBy.Windows;

            string email = await store.GetSessionEmailAsync(item);
            await this.ValidateEmailAsync(0, email, errorList, checkRequirement);

            string phoneNumber = await store.GetSessionPhoneNumberAsync(item);
            await this.ValidatePhoneNumberAsync(0, phoneNumber, errorList, checkRequirement);

            var fullName = UserFullName.Create(item.last_name, item.first_name, item.middle_name);
            await this.ValidateFullNameAsync(0, fullName, errorList, checkRequirement);

            if (checkRequirement)
            {
                if (errorList.Count == 0 && String.IsNullOrEmpty(email) && String.IsNullOrEmpty(phoneNumber))
                {
                    errorList.Add(new AdvancedIdentityResultItem(AdvancedIdentityResultCode.InvalidData, "Должен быть указан адрес электронной почты или номер телефона."));
                }
            }

            identityResult = errorList.Count == 0 ?
                new AdvancedIdentityResult(0) :
                new AdvancedIdentityResult(errorList);

            return identityResult;
        }
    }

    public class AdvancedPasswordValidator : PasswordValidator
    {
        public override async Task<IdentityResult> ValidateAsync(string item)
        {
            IdentityResult result = await base.ValidateAsync(item);
            return result.Succeeded ?
                AdvancedIdentityResult.CreateSuccess() :
                AdvancedIdentityResult.CreateError(AdvancedIdentityResultCode.InvalidPassword, result.Errors.ToArray());
        }
    }

    // https://www.ryadel.com/en/c-sharp-random-password-generator-asp-net-core-mvc/
    public class PasswordGenerator
    {
        private readonly PasswordValidator validator;
        private readonly int maxAttempt = 3;

        public int MaxLength { get; set; }
        public bool WithoutDigit { get; set; }
        public bool WithoutLowercase { get; set; }
        public bool WithoutNonLetterOrDigit { get; set; }
        public bool WithoutUppercase { get; set; }

        public PasswordGenerator(PasswordValidator validator)
        {
            if (validator == null)
            {
                throw new ArgumentNullException("validator");
            }
            
            this.validator = validator;
        }

        public async Task<string> GenerateAsync()
        {
            string uppercaseChars = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
            string lowercaseChars = "abcdefghijkmnopqrstuvwxyz";
            string digitChars = "0123456789";
            string nonLetterOrDigitChars = "{}[]&?!*@$_";

            List<string> randomCharsGroups = new List<string>();
            if (!this.WithoutUppercase)
            {
                randomCharsGroups.Add(uppercaseChars);
            }
            if (!this.WithoutLowercase)
            {
                randomCharsGroups.Add(lowercaseChars);
            }
            if (!this.WithoutDigit)
            {
                randomCharsGroups.Add(digitChars);
            }
            if (!this.WithoutNonLetterOrDigit)
            {
                randomCharsGroups.Add(nonLetterOrDigitChars);
            }
            if (randomCharsGroups.Count == 0)
            {
                throw new InvalidOperationException("Invalid parameters");
            }
            
            string result = "";
            for (int j = 0; j < maxAttempt; j++)
            {
                Random rand = new Random(Environment.TickCount);
                List<char> chars = new List<char>();

                if (validator.RequireUppercase)
                    chars.Insert(rand.Next(0, chars.Count), uppercaseChars[rand.Next(0, uppercaseChars.Length)]);

                if (validator.RequireLowercase)
                    chars.Insert(rand.Next(0, chars.Count), lowercaseChars[rand.Next(0, lowercaseChars.Length)]);

                if (validator.RequireDigit)
                    chars.Insert(rand.Next(0, chars.Count), digitChars[rand.Next(0, digitChars.Length)]);

                if (validator.RequireNonLetterOrDigit)
                    chars.Insert(rand.Next(0, chars.Count), nonLetterOrDigitChars[rand.Next(0, nonLetterOrDigitChars.Length)]);

                int requiredLength = Math.Max(this.MaxLength, validator.RequiredLength);
                for (int i = chars.Count; i < requiredLength; i++)
                {
                    string randomChars = randomCharsGroups[rand.Next(0, randomCharsGroups.Count)];
                    chars.Insert(rand.Next(0, chars.Count), randomChars[rand.Next(0, randomChars.Length)]);
                }

                result = new string(chars.ToArray());

                var validateResult = await validator.ValidateAsync(result);
                if (validateResult.Succeeded)
                {
                    break;
                }
            }
            return result;
        }
    }

    public class ThirdPartyAuthentication
    {
        public string AuthenticationType { get; set; }
        public string Caption { get; set; }
        public Communication AllowedOn { get; set; }
        public bool IsWindows { get; set; }
    }

    /// <summary>
    /// Полное имя пользователя
    /// </summary>
    public class UserFullName
    {
        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        public string MiddleName { get; set; }

        public bool LastNameSpecified
        {
            get
            {
                return !String.IsNullOrEmpty(this.LastName);
            }
        }
        public bool FirstNameSpecified
        {
            get
            {
                return !String.IsNullOrEmpty(this.FirstName);
            }
        }
        public bool MiddleNameSpecified
        {
            get
            {
                return !String.IsNullOrEmpty(this.MiddleName);
            }
        }
        public bool FullNameSpecified
        {
            get
            {
                return this.LastNameSpecified && this.FirstNameSpecified && this.MiddleNameSpecified;
            }
        }

        public static UserFullName Create(IEnumerable<Claim> claims)
        {
            var result = new UserFullName();
            if (claims != null && claims.Any())
            {
                result.FirstName = claims
                    .Where(ss => ss.Type == System.Security.Claims.ClaimTypes.GivenName)
                    .Select(ss => ss.Value)
                    .FirstOrDefault();
                result.LastName = claims
                    .Where(ss => ss.Type == System.Security.Claims.ClaimTypes.Surname)
                    .Select(ss => ss.Value)
                    .FirstOrDefault();
                result.MiddleName = claims
                    .Where(ss => ss.Type == Tsb.Security.Web.Identity.ClaimTypes.Patronymic)
                    .Select(ss => ss.Value)
                    .FirstOrDefault();
            }

            return result;
        }
        public static UserFullName Create(string lastName, string firstName, string middleName)
        {
            var result = new UserFullName
            {
                LastName = lastName,
                FirstName = firstName,
                MiddleName = middleName,
            };

            return result;
        }
        public IEnumerable<Claim> ToClaims()
        {
            var claims = new List<Claim>();
            if (this.FirstNameSpecified)
            {
                claims.Add(new Claim(System.Security.Claims.ClaimTypes.GivenName, this.FirstName));
            }
            if (this.LastNameSpecified)
            {
                claims.Add(new Claim(System.Security.Claims.ClaimTypes.Surname, this.LastName));
            }
            if (this.MiddleNameSpecified)
            {
                claims.Add(new Claim(Tsb.Security.Web.Identity.ClaimTypes.Patronymic, this.MiddleName));
            }

            return claims;
        }
    }

    /// <summary>
    /// Параметры подключения
    /// </summary>
    public class UserConnectionParam
    {
        private static readonly string connectionNameField = "connectionName";
        private static readonly string clientIdField = "clientId";
        private static readonly string moduleIdField = "moduleId";
        private static readonly string screenSizeField = "screenSize";
        private static readonly string developerModeField = "developerMode";
        private static readonly string debugField = "debug";

        /// <summary>
        /// Имя подключения
        /// </summary>
        public string ConnectionName { get; set; }

        /// <summary>
        /// Набор конфигураций
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// Модуль
        /// </summary>
        public int ModuleId { get; set; }

        /// <summary>
        /// Ширина экрана
        /// </summary>
        public string ScreenSize { get; set; }

        /// <summary>
        /// Режим разработчика
        /// </summary>
        public bool DeveloperMode { get; set; }

        /// <summary>
        /// Режим отладки
        /// </summary>
        public bool Debug { get; set; }

        public bool ConnectionNameSpecified
        {
            get
            {
                return !String.IsNullOrEmpty(this.ConnectionName);
            }
        }
        public bool ClientIdSpecified
        {
            get
            {
                return this.ClientId > 0;
            }
        }
        public bool ModuleIdSpecified
        {
            get
            {
                return this.ModuleId > 0;
            }
        }
        public bool ScreenSizeSpecified
        {
            get
            {
                return !String.IsNullOrEmpty(this.ScreenSize);
            }
        }

        public static UserConnectionParam Create(IEnumerable<Claim> claims)
        {
            UserConnectionParam result;
            if (claims != null && claims.Any())
            {
                string connectionName = claims
                    .Where(ss => ss.Type.Equals(Tsb.Security.Web.Identity.ClaimTypes.ConnectionName))
                    .Select(ss => ss.Value)
                    .FirstOrDefault();
                string client = claims
                    .Where(ss => ss.Type.Equals(Tsb.Security.Web.Identity.ClaimTypes.ClientId))
                    .Select(ss => ss.Value)
                    .FirstOrDefault();
                string module = claims
                    .Where(ss => ss.Type.Equals(Tsb.Security.Web.Identity.ClaimTypes.ModuleId))
                    .Select(ss => ss.Value)
                    .FirstOrDefault();
                string screenSize = claims
                    .Where(ss => ss.Type.Equals(Tsb.Security.Web.Identity.ClaimTypes.ScreenSize))
                    .Select(ss => ss.Value)
                    .FirstOrDefault();
                string developerMode = claims
                    .Where(ss => ss.Type.Equals(Tsb.Security.Web.Identity.ClaimTypes.DeveloperMode))
                    .Select(ss => ss.Value)
                    .FirstOrDefault();
                string debug = claims
                    .Where(ss => ss.Type.Equals(Tsb.Security.Web.Identity.ClaimTypes.Debug))
                    .Select(ss => ss.Value)
                    .FirstOrDefault();

                result = UserConnectionParam.Create(connectionName, client, module, developerMode, debug, screenSize);
            }
            else
            {
                result = new UserConnectionParam();
            }

            return result;
        }
        public static UserConnectionParam Create(NameValueCollection collection)
        {
            UserConnectionParam result;
            if (collection != null)
            {
                var keyValuePairs = collection.AllKeys
                    .Select(ss => new KeyValuePair<string, string>(ss, collection[ss]));
                result = UserConnectionParam.Create(keyValuePairs);
            }
            else
            {
                result = new UserConnectionParam();
            }

            return result;
        }
        public static UserConnectionParam Create(IEnumerable<KeyValuePair<string, string>> collection)
        {
            UserConnectionParam result;
            if (collection != null && collection.Any())
            {
                string connectionName = collection
                    .Where(ss => ss.Key.Equals(connectionNameField, StringComparison.OrdinalIgnoreCase))
                    .Select(ss => ss.Value)
                    .FirstOrDefault();
                string client = collection
                    .Where(ss => ss.Key.Equals(clientIdField, StringComparison.OrdinalIgnoreCase))
                    .Select(ss => ss.Value)
                    .FirstOrDefault();
                string module = collection
                    .Where(ss => ss.Key.Equals(moduleIdField, StringComparison.OrdinalIgnoreCase))
                    .Select(ss => ss.Value)
                    .FirstOrDefault();
                string screenSize = collection
                    .Where(ss => ss.Key.Equals(screenSizeField, StringComparison.OrdinalIgnoreCase))
                    .Select(ss => ss.Value)
                    .FirstOrDefault();
                string developerMode = collection
                    .Where(ss => ss.Key.Equals(developerModeField, StringComparison.OrdinalIgnoreCase))
                    .Select(ss => ss.Value)
                    .FirstOrDefault();
                string debug = collection
                    .Where(ss => ss.Key.Equals(debugField, StringComparison.OrdinalIgnoreCase))
                    .Select(ss => ss.Value)
                    .FirstOrDefault();

                result = UserConnectionParam.Create(connectionName, client, module, developerMode, debug, screenSize);
            }
            else
            {
                result = new UserConnectionParam();
            }

            return result;
        }
        public static UserConnectionParam Create(IEnumerable<KeyValuePair<string, string[]>> collection)
        {
            UserConnectionParam result;
            if (collection != null && collection.Any())
            {
                var flatCollection = collection
                    .Select(ss => new KeyValuePair<string, string>(ss.Key, String.Join(",", ss.Value)));
                result = UserConnectionParam.Create(flatCollection);
            }
            else
            {
                result = new UserConnectionParam();
            }

            return result;
        }
        public static UserConnectionParam Create(
            string connectionName, 
            string client, 
            string module,
            string developerMode,
            string debug,
            string screenSize)
        {
            int clientId;
            if (!int.TryParse(client, out clientId))
            {
                clientId = 0;
            }
            int moduleId;
            if (!int.TryParse(module, out moduleId))
            {
                moduleId = 0;
            }
            bool developerModeValue;
            developerMode = developerMode?.Split(',')[0];
            if (!bool.TryParse(developerMode, out developerModeValue))
            {
                developerModeValue = false;
            }
            bool debugValue;
            debug = debug?.Split(',')[0];
            if (!bool.TryParse(debug, out debugValue))
            {
                debugValue = false;
            }

            var result = new UserConnectionParam
            {
                ConnectionName = connectionName,
                ClientId = clientId,
                ModuleId = moduleId,
                DeveloperMode = developerModeValue,
                Debug = debugValue,
                ScreenSize = screenSize,
            };

            return result;
        }
        public static UserConnectionParam Create(
            string connectionName,
            int clientId,
            int moduleId,
            bool developerMode,
            bool debug,
            string screenSize)
        {
            var result = new UserConnectionParam
            {
                ConnectionName = connectionName,
                ClientId = clientId,
                ModuleId = moduleId,
                DeveloperMode = developerMode,
                Debug = debug,
                ScreenSize = screenSize,
            };

            return result;
        }

        public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs()
        {
            var keyValuePairs = new List<KeyValuePair<string, string>>();
            if (this.ConnectionNameSpecified)
            {
                keyValuePairs.Add(new KeyValuePair<string, string>(connectionNameField, this.ConnectionName));
            }
            if (this.ClientIdSpecified)
            {
                keyValuePairs.Add(new KeyValuePair<string, string>(clientIdField, this.ClientId.ToString()));
            }
            if (this.ModuleIdSpecified)
            {
                keyValuePairs.Add(new KeyValuePair<string, string>(moduleIdField, this.ModuleId.ToString()));
            }
            if (this.ScreenSizeSpecified)
            {
                keyValuePairs.Add(new KeyValuePair<string, string>(screenSizeField, this.ScreenSize));
            }
            keyValuePairs.Add(new KeyValuePair<string, string>(developerModeField, this.DeveloperMode.ToString()));
            keyValuePairs.Add(new KeyValuePair<string, string>(debugField, this.Debug.ToString()));

            return keyValuePairs;
        }
        public IEnumerable<Claim> ToClaims()
        {
            var claims = new List<Claim>();
            if (this.ConnectionNameSpecified)
            {
                claims.Add(new Claim(Tsb.Security.Web.Identity.ClaimTypes.ConnectionName, this.ConnectionName));
            }
            if (this.ClientIdSpecified)
            {
                claims.Add(new Claim(Tsb.Security.Web.Identity.ClaimTypes.ClientId, this.ClientId.ToString()));
            }
            if (this.ModuleIdSpecified)
            {
                claims.Add(new Claim(Tsb.Security.Web.Identity.ClaimTypes.ModuleId, this.ModuleId.ToString()));
            }
            if (this.ScreenSizeSpecified)
            {
                claims.Add(new Claim(Tsb.Security.Web.Identity.ClaimTypes.ScreenSize, this.ScreenSize));
            }
            claims.Add(new Claim(Tsb.Security.Web.Identity.ClaimTypes.DeveloperMode, this.DeveloperMode.ToString()));
            claims.Add(new Claim(Tsb.Security.Web.Identity.ClaimTypes.Debug, this.Debug.ToString()));

            return claims;
        }
    }
}