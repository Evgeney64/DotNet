using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using Tsb.Security.Web.Models;
using Base64String = System.String;

namespace Tsb.Security.Web.Membership
{
    public interface IGroupMembershipProvider
    {
        int[] GetGroupsForUser(string userName);
    }
    
    public class UserNotFoundException : Exception
    {
        private const string NameNotFoundString = @"Пользователь с именем ""{0}"" не найден.";
        private const string KeyNotFoundString = @"Пользователь с кодом ""{0}"" не найден.";

        public string UserName
        {
            get;
            private set;
        }
        public object UserKey
        {
            get;
            private set;
        }

        public UserNotFoundException(string userName)
            : base(String.Format(NameNotFoundString, userName))
        {
            this.UserName = userName;
        }
        public UserNotFoundException(object userKey)
            : base(String.Format(KeyNotFoundString, userKey))
        {
            this.UserKey = userKey;
        }
    }

    public class UserIsLockedException : Exception
    {
        private const string MessageStr = @"Пользователь заблокирован.";

        public UserIsLockedException()
            : base(MessageStr)
        {
        }
    }

    public class InvalidNameOrPasswordException : Exception
    {
        private const string MessageStr = @"Неверное имя пользователя и/или пароль.";

        public InvalidNameOrPasswordException()
            : base(MessageStr)
        {
        }
    }

    public class InvalidNameOrAnswerException : Exception
    {
        private const string MessageStr = @"Неверное имя пользователя и/или контрольный ответ.";

        public InvalidNameOrAnswerException()
            : base(MessageStr)
        {
        }
    }

    public class InvalidEmailException : Exception
    {
        private const string MessageStr = @"Неверный формат адреса электронной почты.";

        public InvalidEmailException()
            : base(MessageStr)
        {
        }
    }

    public class DuplicateEmailException : Exception
    {
        private const string MessageStr = @"Электронный почтовый ящик ""{0}"" уже зарегистрирован с системе.";

        public string Email
        {
            get;
            private set;
        }

        public DuplicateEmailException(string Email)
            : base(String.Format(MessageStr, Email))
        {
            this.Email = Email;
        }
    }

    public class SimplePasswordException : Exception
    {
    }

    public class PasswordRetrievalDisabledException : Exception
    {
        private const string MessageStr = @"Восстановление пароля не поддерживается.";

        public PasswordRetrievalDisabledException()
            : base(MessageStr)
        {
        }
    }

    public class PasswordRetrievalFailException : Exception
    {
        private const string MessageStr = @"Не удалось восстановить пароль.";

        public PasswordRetrievalFailException()
            : base(MessageStr)
        {
        }
    }

    public class PasswordResetDisabledException : Exception
    {
        private const string MessageStr = @"Сброс пароля не поддерживается.";

        public PasswordResetDisabledException()
            : base(MessageStr)
        {
        }
    }

    public abstract class SuperMembershipProvider : MembershipProvider, IGroupMembershipProvider
    {
        private const string CConnectionStringFormat = "name={0}";

        private bool requiresUniqueEmail;
        private string connectionStringName;

        protected abstract string DefaultProviderName { get; }
        protected virtual DateTime MinDate { get; private set; }
        public override string ApplicationName { get; set; }
        public override bool RequiresUniqueEmail
        {
            get
            { 
                return requiresUniqueEmail;
            }
        }

        internal System.Security.SecurityContext CreateContext()
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
        internal abstract scr_principal GetPrincipalByUserName(System.Security.SecurityContext context, string username);

        #region MembershipProvider members
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (String.IsNullOrEmpty(name))
            {
                name = DefaultProviderName;
            }

            base.Initialize(name, config);

            requiresUniqueEmail = false;
            connectionStringName = null;
            this.MinDate = DateTime.Parse("1900-01-01T00:00:00");

            foreach (string key in config.Keys)
            {
                switch (key.ToLower())
                {
                    case "applicationname":
                        ApplicationName = config[key];
                        break;
                    case "requiresuniqueemail":
                        requiresUniqueEmail = bool.Parse(config[key]);
                        break;
                    case "connectionstringname":
                        connectionStringName = config[key];
                        break;
                }
            }
        }
        #endregion

        #region SuperMembershipProvider members
        public virtual Tuple<string, string> GetSipInfo(string username)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_principal principal = GetPrincipalByUserName(store, username);
                if (principal == null)
                {
                    throw new UserNotFoundException(username);
                }
                scr_user_sip userSip = store.GetUserSipsByUserId(principal.principal_id);
                if (userSip != null)
                {
                    return new Tuple<string, string>(userSip.sip_uri, userSip.password);
                }
                return null;
            }
        }
        #endregion

        #region IGroupMembershipProvider members
        public int[] GetGroupsForUser(string userName)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_principal principal = GetPrincipalByUserName(store, userName);
                if (principal == null)
                {
                    throw new UserNotFoundException(userName);
                }

                return store.scr_GetPrincipalsByPrincipal(principal.principal_id).Cast<int>().ToArray();
            }
        }
        #endregion
    }

    public class NetMembershipProvider : SuperMembershipProvider
    {
        private const int CMaxInvalidPasswordAttempts = 5;
        private const int CMinRequiredNonAlphanumericCharacters = 0;
        private const int CMinRequiredPasswordLength = 8;
        private const int CPasswordAttemptWindow = 10;
        private const int CPasswordSaltLength = 16;
        private const MembershipPasswordFormat CPasswordFormat = MembershipPasswordFormat.Hashed;

        private bool enablePasswordReset;
        private bool enablePasswordRetrieval;
        private int maxInvalidPasswordAttempts;
        private int minRequiredNonAlphanumericCharacters;
        private int minRequiredPasswordLength;
        private int passwordAttemptWindow;
        private MembershipPasswordFormat passwordFormat;
        private string passwordStrengthRegularExpression;
        private bool requiresQuestionAndAnswer;

        protected override string DefaultProviderName
        {
            get
            {
                return "NetMembershipProvider";
            }
        }
        public override bool EnablePasswordReset
        {
            get { return enablePasswordReset; }
        }
        public override bool EnablePasswordRetrieval
        {
            get { return enablePasswordRetrieval; }
        }
        public override int MaxInvalidPasswordAttempts
        {
            get { return maxInvalidPasswordAttempts; }
        }
        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return minRequiredNonAlphanumericCharacters; }
        }
        public override int MinRequiredPasswordLength
        {
            get { return minRequiredPasswordLength; }
        }
        public override int PasswordAttemptWindow
        {
            get { return passwordAttemptWindow; }
        }
        public override MembershipPasswordFormat PasswordFormat
        {
            get { return passwordFormat; }
        }
        public override string PasswordStrengthRegularExpression
        {
            get { return passwordStrengthRegularExpression; }
        }
        public override bool RequiresQuestionAndAnswer
        {
            get { return requiresQuestionAndAnswer; }
        }

        private bool CheckUserName(string UserName, ref MembershipCreateStatus status)
        {
            bool result = !String.IsNullOrWhiteSpace(UserName);
            if (!result)
            {
                status = MembershipCreateStatus.InvalidUserName;
                return result;
            }

            using (SecurityContext store = this.CreateContext())
            {
                result = !store.scr_user.Any(us => String.Compare(UserName, us.user_name, true) == 0);
            }
            if (!result)
            {
                status = MembershipCreateStatus.DuplicateUserName;
            }
            return result;
        }
        private bool CheckEmail(string UserName, string Email, ref MembershipCreateStatus status)
        {
            bool result = !this.RequiresUniqueEmail || !String.IsNullOrWhiteSpace(Email);
            if (!result)
            {
                status = MembershipCreateStatus.InvalidEmail;
                return result;
            }

            using (SecurityContext store = this.CreateContext())
            {
                result = !this.RequiresUniqueEmail || !store.scr_user.Any(us => us.user_name != UserName && String.Compare(Email, us.email, true) == 0);
            }
            if (!result)
            {
                status = MembershipCreateStatus.DuplicateEmail;
            }
            return result;
        }
        private bool CheckPassword(string Password, ref MembershipCreateStatus status)
        {
            bool result =
                Password.Length >= this.MinRequiredPasswordLength &&
                Password.Count(ch => !char.IsLetterOrDigit(ch)) >= this.MinRequiredNonAlphanumericCharacters &&
                (this.PasswordStrengthRegularExpression == null || Regex.IsMatch(Password, this.PasswordStrengthRegularExpression));
            if (!result)
                status = MembershipCreateStatus.InvalidPassword;

            return result;
        }
        private bool CheckPasswordQuestion(string PasswordQuestion, ref MembershipCreateStatus status)
        {
            bool result = !this.RequiresQuestionAndAnswer || !String.IsNullOrWhiteSpace(PasswordQuestion);
            if (!result)
                status = MembershipCreateStatus.InvalidQuestion;

            return result;
        }
        private bool CheckPasswordAnswer(string PasswordAnswer, ref MembershipCreateStatus status)
        {
            bool result = !this.RequiresQuestionAndAnswer || !String.IsNullOrWhiteSpace(PasswordAnswer);
            if (!result)
                status = MembershipCreateStatus.InvalidAnswer;

            return result;
        }
        private bool ValidateUserPasswordInternal(scr_user User, string Password)
        {
            return
                User != null &&
                String.Compare(User.password, this.EncryptString(Password, User.password_salt, User.password_format)) == 0;
        }
        private bool ValidateUserAnswerInternal(scr_user User, string Answer)
        {
            return
                User != null &&
                String.Compare(User.password_answer, this.EncryptString(Answer, User.password_salt, User.password_format)) == 0;
        }
        private MembershipUser CreateMembershipUser(scr_user User)
        {
            return User != null ?
                new MembershipUser(
                    base.Name,
                    User.user_name,
                    User.user_id,
                    User.email,
                    User.password_question,
                    string.Empty,
                    true,
                    User.is_locked,
                    User.date_creation,
                    User.date_last_login,
                    User.date_last_activity,
                    base.MinDate,
                    User.date_lock) :
                null;
        }
        private MembershipUserCollection CreateMembershipUserCollection(IEnumerable<scr_user> Users)
        {
            MembershipUserCollection membershipUsers = new MembershipUserCollection();
            foreach (scr_user user in Users)
            {
                membershipUsers.Add(CreateMembershipUser(user));
            }
            return membershipUsers;
        }
        private MembershipUser GetMembershipUser(scr_user user, bool userIsOnline)
        {
            if (userIsOnline && user != null)
                user.date_last_activity = DateTime.UtcNow;
            return CreateMembershipUser(user);
        }
        private Base64String EncryptString(string Source, Base64String Salt, int Format)
        {
            if ((MembershipPasswordFormat)Format == MembershipPasswordFormat.Clear)
            {
                return Source;
            }
            else
            {
                byte[] clearBuffer = Encoding.UTF8.GetBytes(Source);
                byte[] saltBuffer = Convert.FromBase64String(Salt);
                byte[] encryptedBuffer = null;

                if ((MembershipPasswordFormat)Format == MembershipPasswordFormat.Hashed)
                {
                    HashAlgorithm hashAlgorithm = HashAlgorithm.Create("SHA1");
                    encryptedBuffer = hashAlgorithm.ComputeHash(clearBuffer.Concat(saltBuffer).ToArray());
                }
                else
                {
                    encryptedBuffer = base.EncryptPassword(clearBuffer.Concat(saltBuffer).ToArray());
                }
                return Convert.ToBase64String(encryptedBuffer);
            }
        }
        private string DecryptString(Base64String Source, int Format)
        {
            switch ((MembershipPasswordFormat)Format)
            {
                case MembershipPasswordFormat.Clear:
                    return Source;
                case MembershipPasswordFormat.Encrypted:
                    byte[] encryptedBuffer = Convert.FromBase64String(Source);
                    byte[] decryptedBuffer = base.DecryptPassword(encryptedBuffer);
                    string result = null;
                    if (decryptedBuffer != null)
                    {
                        result = Encoding.UTF8.GetString(decryptedBuffer, 0, decryptedBuffer.Length - CPasswordSaltLength);
                    }
                    return result;
                default:
                    return null;
            }
        }
        private Base64String RandomString(byte ByteCount)
        {
            byte[] randomBuffer = new byte[ByteCount];
            RandomNumberGenerator randomGen = RandomNumberGenerator.Create();

            randomGen.GetBytes(randomBuffer);

            return Convert.ToBase64String(randomBuffer);
        }

        internal override scr_principal GetPrincipalByUserName(SecurityContext context, string username)
        {
            return context.GetPrincipalByPrincipalName(username, SecurityContext.PrincipalType.User);
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            base.Initialize(name, config);

            enablePasswordReset = false;
            enablePasswordRetrieval = false;
            maxInvalidPasswordAttempts = CMaxInvalidPasswordAttempts;
            minRequiredNonAlphanumericCharacters = CMinRequiredNonAlphanumericCharacters;
            minRequiredPasswordLength = CMinRequiredPasswordLength;
            passwordAttemptWindow = CPasswordAttemptWindow;
            passwordFormat = CPasswordFormat;
            passwordStrengthRegularExpression = null;
            requiresQuestionAndAnswer = false;

            foreach (string key in config.Keys)
            {
                switch (key.ToLower())
                {
                    case "enablepasswordreset":
                        enablePasswordReset = bool.Parse(config[key]);
                        break;
                    case "enablepasswordretrieval":
                        enablePasswordRetrieval = bool.Parse(config[key]);
                        break;
                    case "maxinvalidpasswordattempts":
                        maxInvalidPasswordAttempts = int.Parse(config[key]);
                        break;
                    case "minrequirednonalphanumericcharacters":
                        minRequiredNonAlphanumericCharacters = int.Parse(config[key]);
                        break;
                    case "minrequiredpasswordlength":
                        minRequiredPasswordLength = int.Parse(config[key]);
                        break;
                    case "passwordattemptwindow":
                        passwordAttemptWindow = int.Parse(config[key]);
                        break;
                    case "passwordformat":
                        passwordFormat = (MembershipPasswordFormat)Enum.Parse(typeof(MembershipPasswordFormat), config[key], true);
                        break;
                    case "passwordstrengthregularexpression":
                        passwordStrengthRegularExpression = config[key];
                        break;
                    case "requiresquestionandanswer":
                        requiresQuestionAndAnswer = bool.Parse(config[key]);
                        break;
                }
            }
        }
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_user user = store.GetUserByUserName(username);
                if (!ValidateUserPasswordInternal(user, oldPassword))
                    throw new InvalidNameOrPasswordException();

                MembershipCreateStatus membershipStatus = MembershipCreateStatus.Success;
                if (!CheckPassword(newPassword, ref membershipStatus))
                    return false;

                user.password = EncryptString(newPassword, user.password_salt, user.password_format);
                store.UpdateUser(user);
                store.SaveChanges();
            }
            return true;
        }
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_user user = store.GetUserByUserName(username);
                if (user == null || !ValidateUserPasswordInternal(user, password))
                    throw new InvalidNameOrPasswordException();

                MembershipCreateStatus membershipStatus = MembershipCreateStatus.Success;
                if (!CheckPasswordQuestion(newPasswordQuestion, ref membershipStatus) || !CheckPasswordAnswer(newPasswordAnswer, ref membershipStatus))
                    return false;

                user.password_question = newPasswordQuestion;
                user.password_answer = this.EncryptString(newPasswordAnswer, user.password_salt, user.password_format);
                store.UpdateUser(user);
                store.SaveChanges();
            }
            return true;
        }
        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            status = MembershipCreateStatus.Success;

            if (
                !CheckUserName(username, ref status) ||
                !CheckEmail(username, email, ref status) ||
                !CheckPassword(password, ref status) ||
                !CheckPasswordQuestion(passwordQuestion, ref status) ||
                !CheckPasswordAnswer(passwordAnswer, ref status)
                )
            {
                return null;
            }

            Base64String passwordSalt = this.RandomString(CPasswordSaltLength);
            passwordQuestion = RequiresQuestionAndAnswer ? passwordQuestion.Trim() : null;
            passwordAnswer = RequiresQuestionAndAnswer ? EncryptString(passwordAnswer.Trim(), passwordSalt, (int)PasswordFormat) : null;
            password = EncryptString(password, passwordSalt, (int)PasswordFormat);

            scr_user user = new scr_user()
            {
                user_name = username,
                email = email,
                password = password,
                password_format = (int)PasswordFormat,
                password_salt = passwordSalt,
                password_question = passwordQuestion,
                password_answer = passwordAnswer,
                password_invalid_count = 0,
                date_creation = DateTime.UtcNow,
                date_last_login = base.MinDate,
                date_last_activity = base.MinDate,
                date_lock = base.MinDate,
                date_last_password_invalid = base.MinDate,
                is_locked = false,
                is_approved = isApproved,
            };

            using (SecurityContext store = this.CreateContext())
            {
                store.InsertUser(user);
                store.SaveChanges();
            }
            return CreateMembershipUser(user);
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_user user = store.GetUserByUserName(username);
                store.DeleteUser(user);
                store.SaveChanges();
            }
            return true;
        }
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            using (SecurityContext store = this.CreateContext())
            {
                IEnumerable<scr_user> usersAll = store.scr_user.Where(us => us.email != null && us.email.Contains(emailToMatch));
                IEnumerable<scr_user> users = usersAll.Where((us, ix) => ix >= pageIndex * pageSize && ix < (pageIndex + 1) * pageSize);
                totalRecords = usersAll.Count();
                return CreateMembershipUserCollection(users);
            }
        }
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            using (SecurityContext store = this.CreateContext())
            {
                IEnumerable<scr_user> usersAll = store.scr_user.Where(us => us.user_name.Contains(usernameToMatch));
                IEnumerable<scr_user> users = usersAll.Where((us, ix) => ix >= pageIndex * pageSize && ix < (pageIndex + 1) * pageSize);
                totalRecords = usersAll.Count();
                return CreateMembershipUserCollection(users);
            }
        }
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            using (SecurityContext store = this.CreateContext())
            {
                IEnumerable<scr_user> usersAll = store.scr_user;
                IEnumerable<scr_user> users = usersAll.Where((us, ix) => ix >= pageIndex * pageSize && ix < (pageIndex + 1) * pageSize);
                totalRecords = usersAll.Count();
                return CreateMembershipUserCollection(users);
            }
        }
        public override int GetNumberOfUsersOnline()
        {
            using (SecurityContext store = this.CreateContext())
            {
                return store.scr_user.Count(
                    us => us.date_last_activity.AddMinutes(System.Web.Security.Membership.UserIsOnlineTimeWindow) >= DateTime.UtcNow);
            }
        }
        public override string GetPassword(string username, string answer)
        {
            if (!EnablePasswordRetrieval)
                throw new PasswordRetrievalDisabledException();

            using (SecurityContext store = this.CreateContext())
            {
                scr_user user = store.GetUserByUserName(username);
                if (!ValidateUserAnswerInternal(user, answer))
                    throw new InvalidNameOrAnswerException();

                string result = DecryptString(user.password, user.password_format);
                if (result == null)
                    throw new PasswordRetrievalFailException();
                return result;
            }
        }
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            using (SecurityContext store = this.CreateContext())
            {
                return GetMembershipUser(store.GetUserByUserId((int)providerUserKey), userIsOnline);
            }
        }
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            using (SecurityContext store = this.CreateContext())
            {
                return GetMembershipUser(store.GetUserByUserName(username), userIsOnline);
            }
        }
        public override string GetUserNameByEmail(string email)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_user user = store.GetUsersByEmail(email).FirstOrDefault();
                return user != null ? user.user_name : null;
            }
        }
        public override string ResetPassword(string username, string answer)
        {
            if (!EnablePasswordReset)
                throw new PasswordResetDisabledException();

            using (SecurityContext store = this.CreateContext())
            {
                scr_user user = store.GetUserByUserName(username);
                if (RequiresQuestionAndAnswer && !ValidateUserAnswerInternal(user, answer))
                    throw new InvalidNameOrAnswerException();

                // не учитывает PasswordStrengthRegularExpression
                string result = System.Web.Security.Membership.GeneratePassword(this.MinRequiredPasswordLength, this.MinRequiredNonAlphanumericCharacters);
                user.password = this.EncryptString(result, user.password_salt, user.password_format);
                store.UpdateUser(user);
                store.SaveChanges();
                return result;
            }
        }
        public override bool UnlockUser(string userName)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_user user = store.GetUserByUserName(userName);
                if (user == null)
                    throw new UserNotFoundException(userName);

                user.is_locked = false;
                user.password_invalid_count = 0;
                user.date_last_password_invalid = base.MinDate;
                user.date_lock = base.MinDate;
                store.UpdateUser(user);
                store.SaveChanges();
            }
            return true;
        }
        public override void UpdateUser(MembershipUser membershipUser)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_user user = store.GetUserByUserName(membershipUser.UserName);
                if (user == null)
                    throw new UserNotFoundException(membershipUser.UserName);

                MembershipCreateStatus membershipStatus = MembershipCreateStatus.Success;
                if (!CheckEmail(membershipUser.UserName, membershipUser.Email, ref membershipStatus))
                {
                    switch (membershipStatus)
                    {
                        case MembershipCreateStatus.InvalidEmail:
                            throw new InvalidEmailException();
                        case MembershipCreateStatus.DuplicateEmail:
                            throw new DuplicateEmailException(membershipUser.Email);
                        default:
                            return;
                    }
                }

                user.email = membershipUser.Email;
                user.date_last_activity = membershipUser.LastActivityDate;
                user.date_last_login = membershipUser.LastLoginDate;

                store.UpdateUser(user);
                store.SaveChanges();
            }
        }
        public override bool ValidateUser(string username, string password)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_user user = store.GetUserByUserName(username);
                bool result = user != null && user.is_approved && !user.is_locked && ValidateUserPasswordInternal(user, password);
                if (result)
                {
                    user.date_last_activity = DateTime.UtcNow;
                    user.date_last_login = DateTime.UtcNow;
                    store.UpdateUser(user);
                    store.SaveChanges();
                }
                return result;
            }
        }
    }

    public class WindowsMembershipProvider : SuperMembershipProvider
    {
        private const int CMinRequiredPasswordLength = 0;

        protected override string DefaultProviderName
        {
            get
            {
                return "WindowsMembershipProvider";
            }
        }
        public override bool EnablePasswordReset
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public override bool EnablePasswordRetrieval
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public override int MaxInvalidPasswordAttempts
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public override int MinRequiredNonAlphanumericCharacters
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public override int MinRequiredPasswordLength
        {
            get
            {
                //throw new NotImplementedException();
                return CMinRequiredPasswordLength;
            }
        }
        public override int PasswordAttemptWindow
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public override MembershipPasswordFormat PasswordFormat
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public override string PasswordStrengthRegularExpression
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public override bool RequiresQuestionAndAnswer
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private bool CheckUserName(string UserName, ref MembershipCreateStatus status)
        {
            bool result = !String.IsNullOrWhiteSpace(UserName);
            if (!result)
            {
                status = MembershipCreateStatus.InvalidUserName;
                return result;
            }

            using (SecurityContext store = this.CreateContext())
            {
                result = !store.scr_win_user.Any(us => String.Compare(UserName, us.user_name, true) == 0);
            }
            if (!result)
            {
                status = MembershipCreateStatus.DuplicateUserName;
            }
            return result;
        }
        private bool CheckEmail(string Email, ref MembershipCreateStatus status)
        {
            bool result = !this.RequiresUniqueEmail || !String.IsNullOrWhiteSpace(Email);
            if (!result)
            {
                status = MembershipCreateStatus.InvalidEmail;
                return result;
            }

            using (SecurityContext store = this.CreateContext())
            {
                result = !this.RequiresUniqueEmail || !store.scr_win_user.Any(us => String.Compare(Email, us.email, true) == 0);
            }
            if (!result)
            {
                status = MembershipCreateStatus.DuplicateEmail;
            }
            return result;
        }
        private MembershipUser CreateMembershipUser(scr_win_user user)
        {
            return user != null ?
                new MembershipUser(
                    base.Name,
                    user.user_name,
                    user.user_id,
                    user.email,
                        String.Empty,
                    string.Empty,
                    true,
                    user.is_locked,
                    user.date_creation,
                        base.MinDate,
                    user.date_last_activity,
                    base.MinDate,
                    user.date_lock) :
                null;
        }
        private MembershipUserCollection CreateMembershipUserCollection(IEnumerable<scr_win_user> users)
        {
            MembershipUserCollection membershipUsers = new MembershipUserCollection();
            foreach (scr_win_user user in users)
            {
                membershipUsers.Add(CreateMembershipUser(user));
            }
            return membershipUsers;
        }
        private MembershipUser GetMembershipUser(scr_win_user user, bool userIsOnline)
        {
            if (userIsOnline && user != null)
            {
                user.date_last_activity = DateTime.UtcNow;
            }
            return CreateMembershipUser(user);
        }

        internal override scr_principal GetPrincipalByUserName(SecurityContext context, string username)
        {
            return context.GetPrincipalByPrincipalName(username, SecurityContext.PrincipalType.WinUser);
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            base.Initialize(name, config);
        }
        public override bool ChangePassword(Base64String username, Base64String oldPassword, Base64String newPassword)
        {
            throw new InvalidOperationException();
        }
        public override bool ChangePasswordQuestionAndAnswer(Base64String username, Base64String password, Base64String newPasswordQuestion, Base64String newPasswordAnswer)
        {
            throw new InvalidOperationException();
        }
        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            //throw new InvalidOperationException();
            return this.CreateUser(username, email, isApproved, out status);
        }
        public MembershipUser CreateUser(string username, string email, bool isApproved, out MembershipCreateStatus status)
        {
            status = MembershipCreateStatus.Success;

            if (!CheckUserName(username, ref status) || !CheckEmail(email, ref status))
            {
                return null;
            }

            scr_win_user user = new scr_win_user()
            {
                user_name = username,
                email = email,
                date_creation = DateTime.UtcNow,
                date_last_activity = base.MinDate,
                date_lock = base.MinDate,
                is_locked = false
            };

            using (SecurityContext store = this.CreateContext())
            {
                store.InsertUser(user);
                store.SaveChanges();
            }
            return CreateMembershipUser(user);
        }
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_win_user user = store.GetWinUserByUserName(username);
                store.DeleteUser(user);
                store.SaveChanges();
            }
            return true;
        }
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            using (SecurityContext store = this.CreateContext())
            {
                IEnumerable<scr_win_user> usersAll = store.scr_win_user.Where(us => us.email != null && us.email.Contains(emailToMatch));
                IEnumerable<scr_win_user> users = usersAll.Where((us, ix) => ix >= pageIndex * pageSize && ix < (pageIndex + 1) * pageSize);
                totalRecords = usersAll.Count();
                return CreateMembershipUserCollection(users);
            }
        }
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            using (SecurityContext store = this.CreateContext())
            {
                IEnumerable<scr_win_user> usersAll = store.scr_win_user.Where(us => us.user_name.Contains(usernameToMatch));
                IEnumerable<scr_win_user> users = usersAll.Where((us, ix) => ix >= pageIndex * pageSize && ix < (pageIndex + 1) * pageSize);
                totalRecords = usersAll.Count();
                return CreateMembershipUserCollection(users);
            }
        }
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            using (SecurityContext store = this.CreateContext())
            {
                IEnumerable<scr_win_user> usersAll = store.scr_win_user;
                IEnumerable<scr_win_user> users = usersAll.Where((us, ix) => ix >= pageIndex * pageSize && ix < (pageIndex + 1) * pageSize);
                totalRecords = usersAll.Count();
                return CreateMembershipUserCollection(users);
            }
        }
        public override int GetNumberOfUsersOnline()
        {
            using (SecurityContext store = this.CreateContext())
            {
                return store.scr_win_user.Count(us => us.date_last_activity.AddMinutes(System.Web.Security.Membership.UserIsOnlineTimeWindow) >= DateTime.UtcNow);
            }
        }
        public override string GetPassword(string username, string answer)
        {
            throw new InvalidOperationException();
        }
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_win_user user = store.GetWinUserByUserId((int)providerUserKey);
                if (user == null)
                {
                    throw new UserNotFoundException(providerUserKey);
                }
                if (user.is_locked)
                {
                    throw new UserIsLockedException();
                }

                return this.GetMembershipUser(user, userIsOnline);
            }
        }
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_win_user user = store.GetWinUserByUserName(username);
                if (user == null)
                {
                    throw new UserNotFoundException(username);
                }
                if (user.is_locked)
                {
                    throw new UserIsLockedException();
                }

                return this.GetMembershipUser(user, userIsOnline);
            }
        }
        public override string GetUserNameByEmail(string email)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_win_user user = store.GetWinUsersByEmail(email).FirstOrDefault();
                return user != null ? user.user_name : null;
            }
        }
        public override string ResetPassword(string username, string answer)
        {
            throw new InvalidOperationException();
        }
        public override bool UnlockUser(string userName)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_win_user user = store.GetWinUserByUserName(userName);
                if (user == null)
                {
                    throw new UserNotFoundException(userName);
                }

                user.is_locked = false;
                user.date_lock = base.MinDate;
                store.UpdateUser(user);
                store.SaveChanges();
            }
            return true;
        }
        public override void UpdateUser(MembershipUser membershipUser)
        {
            using (SecurityContext store = this.CreateContext())
            {
                scr_win_user user = store.GetWinUserByUserName(membershipUser.UserName);
                if (user == null)
                {
                    throw new UserNotFoundException(membershipUser.UserName);
                }

                MembershipCreateStatus membershipStatus = MembershipCreateStatus.Success;
                if (!CheckEmail(membershipUser.Email, ref membershipStatus))
                {
                    switch (membershipStatus)
                    {
                        case MembershipCreateStatus.InvalidEmail:
                            throw new InvalidEmailException();
                        case MembershipCreateStatus.DuplicateEmail:
                            throw new DuplicateEmailException(membershipUser.Email);
                        default:
                            return;
                    }
                }

                user.email = membershipUser.Email;
                user.date_last_activity = membershipUser.LastActivityDate;

                store.UpdateUser(user);
                store.SaveChanges();
            }
        }
        public override bool ValidateUser(string username, string password)
        {
            throw new InvalidOperationException();
        }
    }
}
