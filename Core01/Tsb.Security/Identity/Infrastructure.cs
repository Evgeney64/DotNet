//using Microsoft.AspNet.Identity.Owin;
//using Microsoft.Owin;
//using Microsoft.Owin.Security;
//using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
//using Tsb.Security.Web.Membership;

namespace Tsb.Security.Web.Identity
{
    public enum ApplicationType
    {
        Corporate = 1,
        Personal = 2,
    }

    public static class Application
    {
        public static ApplicationType Type
        {
            get
            {
#if LIK
                return ApplicationType.Personal;
#else
                return ApplicationType.Corporate;
#endif
            }
        }
        public static ApplicationType GetValidType(int appType)
        {
            if (!Enum.IsDefined(typeof(ApplicationType), appType))
            {
                return Application.Type;
            }
            return (ApplicationType)appType;
        }
    }

    public class IdentityPrincipalFactory : PrincipalFactory
    {
        public override IGroupPrincipal Create(IPrincipal principal, params string[] defaultAuthenticationTypes)
        {
            ClaimsPrincipal claimsPrincipal = principal as ClaimsPrincipal;
            if (claimsPrincipal == null)
            {
                throw new ArgumentException();
            }

            IGroupPrincipal groupPrincipal = new Tsb.Security.Web.Identity.GroupRolePrincipal(claimsPrincipal, defaultAuthenticationTypes);
            return groupPrincipal;
        }
    }

    public class Bitrix24Identity : ClaimsIdentity, IBitrix24Identity
    {
        private string email;
        private string accessToken;
        private string refreshToken;

        public string Email
        {
            get
            {
                if (email == null)
                {
                    var emailClaim = this.FindFirst(System.Security.Claims.ClaimTypes.Email);
                    if (emailClaim != null)
                    {
                        email = emailClaim.Value;
                    }
                }
                return email;
            }
        }
        public string AccessToken
        {
            get
            {
                if (accessToken == null)
                {
                    var accessTokenClaims = this.FindFirst(ClaimTypes.AccessToken);
                    if (accessTokenClaims != null)
                    {
                        accessToken = accessTokenClaims.Value;
                    }
                }
                return accessToken;
            }
        }
        public string RefreshToken
        {
            get
            {
                if (refreshToken == null)
                {
                    var refreshTokenClaims = this.FindFirst(ClaimTypes.RefreshToken);
                    if (refreshTokenClaims != null)
                    {
                        refreshToken = refreshTokenClaims.Value;
                    }
                }
                return refreshToken;
            }
        }
        
        public Bitrix24Identity(ClaimsIdentity identity)
            : base(identity)
        {
        }
    }

    public class ACSaleIdentity : ClaimsIdentity, IACSaleIdentity
    {
        private string accessToken;
        private string refreshToken;
        private DateTime? accessTokenExpires;
        private int? id;
        private long? partnerId;
        
        public string AccessToken
        {
            get
            {
                if (accessToken == null)
                {
                    var accessTokenClaims = this.FindFirst(ClaimTypes.AccessToken);
                    if (accessTokenClaims != null)
                    {
                        accessToken = accessTokenClaims.Value;
                    }
                }
                return accessToken;
            }
        }
        public string RefreshToken
        {
            get
            {
                if (refreshToken == null)
                {
                    var refreshTokenClaims = this.FindFirst(ClaimTypes.RefreshToken);
                    if (refreshTokenClaims != null)
                    {
                        refreshToken = refreshTokenClaims.Value;
                    }
                }
                return refreshToken;
            }
        }
        public DateTime AccessTokenExpires
        {
            get
            {
                if (accessTokenExpires == null)
                {
                    DateTime result;
                    var claim = this.FindFirst(ClaimTypes.AccessTokenExpires);
                    // todo: сделать Nullable
                    accessTokenExpires = claim != null && DateTime.TryParse(claim.Value, out result) ? result : DateTime.MaxValue;
                }
                return accessTokenExpires.Value;
            }
        }
        public int Id
        {
            get
            {
                if (id == null)
                {
                    int result;
                    var idClaims = this.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                    id = idClaims != null && int.TryParse(idClaims.Value, out result) ? result : 0;
                }
                return id.Value;
            }
        }
        public long? PartnerId
        {
            get
            {
                if (partnerId == null)
                {
                    long result;
                    var partnerIdClaims = this.FindFirst(ClaimTypes.PartnerId);
                    if (partnerIdClaims != null && long.TryParse(partnerIdClaims.Value, out result))
                    {
                        partnerId = result;
                    }
                }
                return partnerId;
            }
        }

        public ACSaleIdentity(ClaimsIdentity identity)
            : base(identity)
        {
        }
    }

    public class GroupRolePrincipal : ClaimsPrincipal, IGroupPrincipal
    {
        private string[] defaultAuthenticationTypes;
        private int? applicationType;
        private string name;
        private int? id;
        private int? screenSize;
        private string email;
        private string phoneNumber;
        private long? partnerId;
        private string connectionName;
        private int? clientId;
        private int? moduleId;
        private bool? developerMode;
        private bool? debug;
        private int[] userGroups;
        private readonly IIdentity genericIdentity;
        private IIdentity identity;
        private IBitrix24Identity bitrix24Identity;
        private IACSaleIdentity acSaleIdentity;

        public int ApplicationType
        {
            get
            {
                if (applicationType == null)
                {
                    int result;
                    var claim = ((ClaimsIdentity)this.Identity).FindFirst(ClaimTypes.ApplicationType);
                    applicationType = claim != null && int.TryParse(claim.Value, out result) ? result : 0;
                }
                return applicationType.Value;
            }
        }
        public string Name
        {
            get
            {
                if (name == null)
                {
                    // bad
                    var nameClaim = this.FindFirst(c => c.Subject.AuthenticationType != "Bitrix24" && c.Type == System.Security.Claims.ClaimTypes.Name);
                    if (nameClaim != null)
                    {
                        name = nameClaim.Value;
                    }
                }
                return name;
            }
        }
        public int Id
        {
            get
            {
                if (id == null)
                {
                    int result;
                    var idClaims = ((ClaimsIdentity)this.Identity).FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                    id = idClaims != null && int.TryParse(idClaims.Value, out result) ? result : 0;
                }
                return id.Value;
            }
        }
        public int ScreenSize
        {
            get
            {
                if (screenSize == null)
                {
                    int result;
                    var screenSizeClaims = this.FindFirst(ClaimTypes.ScreenSize);
                    screenSize = screenSizeClaims != null && int.TryParse(screenSizeClaims.Value, out result) ? result : 0;
                }
                return screenSize.Value;
            }
        }
        public string Email
        {
            get
            {
                if (email == null)
                {
                    var emailClaim = ((ClaimsIdentity)this.Identity).FindFirst(System.Security.Claims.ClaimTypes.Email);
                    if (emailClaim != null)
                    {
                        email = emailClaim.Value;
                    }
                }
                return email;
            }
        }
        public string PhoneNumber
        {
            get
            {
                if (phoneNumber == null)
                {
                    var phoneNumberClaim = ((ClaimsIdentity)this.Identity).FindFirst(System.Security.Claims.ClaimTypes.MobilePhone);
                    if (phoneNumberClaim != null)
                    {
                        phoneNumber = phoneNumberClaim.Value;
                    }
                }
                return phoneNumber;
            }
        }
        public long? PartnerId
        {
            get
            {
                if (partnerId == null)
                {
                    long result;
                    var partnerIdClaims = ((ClaimsIdentity)this.Identity).FindFirst(ClaimTypes.PartnerId);
                    if (partnerIdClaims != null && long.TryParse(partnerIdClaims.Value, out result))
                    {
                        partnerId = result;
                    }
                }
                return partnerId;
            }
        }
        public string ConnectionName
        {
            get
            {
                if (connectionName == null)
                {
                    var connectionNameClaims = this.FindFirst(ClaimTypes.ConnectionName);
                    if (connectionNameClaims != null)
                    {
                        connectionName = connectionNameClaims.Value;
                    }
                }
                return connectionName;
            }
        }
        public int ClientId
        {
            get
            {
                if (clientId == null)
                {
                    int result;
                    var clientIdClaims = this.FindFirst(ClaimTypes.ClientId);
                    clientId = clientIdClaims != null && int.TryParse(clientIdClaims.Value, out result) ? result : 0;
                }
                return clientId.Value;
            }
        }
        public int ModuleId
        {
            get
            {
                if (moduleId == null)
                {
                    int result;
                    var moduleIdClaims = this.FindFirst(ClaimTypes.ModuleId);
                    moduleId = moduleIdClaims != null && int.TryParse(moduleIdClaims.Value, out result) ? result : 0;
                }
                return moduleId.Value;
            }
        }
        public bool DeveloperMode
        {
            get
            {
                if (developerMode == null)
                {
                    bool result;
                    var developerModeClaims = this.FindFirst(ClaimTypes.DeveloperMode);
                    developerMode = developerModeClaims != null && bool.TryParse(developerModeClaims.Value, out result) ? result : false;
                }
                return developerMode.Value;
            }
        }
        public bool Debug
        {
            get
            {
                if (debug == null)
                {
                    bool result;
                    var claim = this.FindFirst(ClaimTypes.Debug);
                    debug = claim != null && bool.TryParse(claim.Value, out result) ? result : true;
                }
                return debug.Value;
            }
        }
        public int[] UserGroups
        {
            get
            {
                if (userGroups == null)
                {
                    Dictionary<int, int> groupSet = new Dictionary<int, int>();
                    var userGroupClaims = this.Claims.Where(c => c.Type == ClaimTypes.UserGroup);
                    foreach (var userGroupClaim in userGroupClaims)
                    {
                        // format: order;groupId
                        int order, groupId;
                        string[] userGroup = userGroupClaim.Value.Split(';');
                        if (userGroup.Length != 2 || !int.TryParse(userGroup[0], out order) || !int.TryParse(userGroup[1], out groupId))
                        {
                            throw new FormatException();
                        }
                        groupSet.Add(order, groupId);
                    }
                    userGroups = groupSet.OrderBy(it => it.Key).Select(it => it.Value).ToArray();
                }
                return userGroups;
            }
        }
        public override IIdentity Identity
        {
            get
            {
                if (this.identity == null)
                {
                    foreach (ClaimsIdentity identity in this.Identities)
                    {
                        if (defaultAuthenticationTypes.Contains(identity.AuthenticationType))
                        {
                            this.identity = identity;
                            break;
                        }
                    }
                }
                if (this.identity == null)
                {
                    return this.genericIdentity;
                }
                return this.identity;
            }
        }
        public IBitrix24Identity Bitrix24Identity
        {
            get
            {
                if (this.bitrix24Identity == null)
                {
                    foreach (ClaimsIdentity identity in this.Identities)
                    {
                        if (identity.AuthenticationType == "Bitrix24")
                        {
                            this.bitrix24Identity = new Bitrix24Identity(identity);
                            break;
                        }
                    }
                }
                return this.bitrix24Identity;
            }
        }
        public IACSaleIdentity ACSaleIdentity
        {
            get
            {
                if (this.acSaleIdentity == null)
                {
                    foreach (ClaimsIdentity identity in this.Identities)
                    {
                        if (identity.AuthenticationType == "BillBerry")
                        {
                            this.acSaleIdentity = new ACSaleIdentity(identity);
                            break;
                        }
                    }
                }
                return this.acSaleIdentity;
            }
        }

        public GroupRolePrincipal(ClaimsPrincipal principal, params string[] defaultAuthenticationTypes)
            : base(principal)
        {
            this.defaultAuthenticationTypes = defaultAuthenticationTypes;
            this.genericIdentity = new GenericIdentity("");
        }
        
        public IGroups GetGroupsForRole(string roleName)
        {
            ICollection<int> grantSet = new List<int>();
            ICollection<int> denySet = new List<int>();
            var roleGroupClaims = this.Claims.Where(c => c.Type == ClaimTypes.RoleAndGroup);
            foreach (var roleGroupClaim in roleGroupClaims)
            {
                // format: groupId;roleName
                int groupId;
                string[] roleGroup = roleGroupClaim.Value.Split(';');
                if (roleGroup.Length != 2 || !int.TryParse(roleGroup[0], out groupId))
                {
                    throw new FormatException();
                }
                if (roleGroup[1] == roleName)
                {
                    if (groupId > 0)
                    {
                        grantSet.Add(groupId);
                    }
                    else
                    {
                        denySet.Add(-groupId);
                    }
                }
            }
            return new GroupsForRole(grantSet, denySet);
        }
    }

    /// <summary>
    /// Затычка
    /// </summary>
    public class AppContext
    {
        public delegate object AppContextFunc(AppContext context, int appType);

        private readonly IDictionary<string, AppContextFunc> store = new Dictionary<string, AppContextFunc>();
        private static readonly AppContext instance = new AppContext();

        public static AppContext Current
        {
            get
            {
                return instance;
            }
        }

        private static string GetFactoryKey(Type t)
        {
            string res = "Tsb.Security.Web.Identity[" + t.AssemblyQualifiedName + "]Factory";
            return res;
        }
        public T Get<T>()
        {
            return Get<T>(0);
        }
        public T Get<T>(int appType)
        {
            AppContextFunc func;
            if (store.TryGetValue(AppContext.GetFactoryKey(typeof(T)), out func))
            {
                return (T)func(this, appType);
            }
            return default(T);
        }
        public void Set<T>(Func<T> func)
        {
            Set((ctx, p) => func());
        }
        public void Set<T>(Func<AppContext, int, T> func)
        {
            store[AppContext.GetFactoryKey(typeof(T))] = (ctx, p) => func(ctx, p);
        }
    }

    public static class OwinExtensions
    {
        public static string GetFactoryKey(Type t)
        {
            string res = "Tsb.Security.Web.Identity[" + t.AssemblyQualifiedName + "]Factory";
            return res;
        }
        public static T GetManager<T>(this IOwinContext context, int appType)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var factory = context.Get<Func<IOwinContext, int, T>>(OwinExtensions.GetFactoryKey(typeof(T)));
            if (factory != null)
            {
                return factory(context, appType);
            }
            return default(T);
        }
        public static void SetManagerFactory<T>(this IOwinContext context, Func<IOwinContext, int, T> factory)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.Set(OwinExtensions.GetFactoryKey(typeof(T)), factory);
        }
    }

    public static class IdentityExtensions
    {
        public static void RemoveClaims(this ClaimsIdentity identity, string type)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

            var claims = identity.FindAll(type);
            foreach (var claim in claims)
            {
                identity.RemoveClaim(claim);
            }
        }
    }
}

namespace Tsb.Security.Web.Identity
{
    internal sealed class SecurityToken
    {
        private readonly byte[] _data;

        public SecurityToken(byte[] data)
        {
            this._data = (byte[])data.Clone();
        }

        internal byte[] GetDataNoClone()
        {
            return this._data;
        }
    }

    internal static class Rfc6238AuthenticationService
    {
        private readonly static DateTime _unixEpoch;

        private readonly static TimeSpan _timestep;

        private readonly static Encoding _encoding;

        static Rfc6238AuthenticationService()
        {
            Rfc6238AuthenticationService._unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            Rfc6238AuthenticationService._timestep = TimeSpan.FromMinutes(3);
            Rfc6238AuthenticationService._encoding = new UTF8Encoding(false, true);
        }

        private static byte[] ApplyModifier(byte[] input, string modifier)
        {
            if (string.IsNullOrEmpty(modifier))
            {
                return input;
            }
            byte[] bytes = Rfc6238AuthenticationService._encoding.GetBytes(modifier);
            byte[] numArray = new byte[checked((int)input.Length + (int)bytes.Length)];
            Buffer.BlockCopy(input, 0, numArray, 0, (int)input.Length);
            Buffer.BlockCopy(bytes, 0, numArray, (int)input.Length, (int)bytes.Length);
            return numArray;
        }

        private static int ComputeTotp(HashAlgorithm hashAlgorithm, ulong timestepNumber, string modifier)
        {
            byte[] bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((long)timestepNumber));
            byte[] numArray = hashAlgorithm.ComputeHash(Rfc6238AuthenticationService.ApplyModifier(bytes, modifier));
            int num = numArray[(int)numArray.Length - 1] & 15;
            int num1 = (numArray[num] & 127) << 24 | (numArray[num + 1] & 255) << 16 | (numArray[num + 2] & 255) << 8 | numArray[num + 3] & 255;
            return num1 % 1000000;
        }

        public static int GenerateCode(SecurityToken securityToken, string modifier = null)
        {
            int num;
            if (securityToken == null)
            {
                throw new ArgumentNullException("securityToken");
            }
            ulong currentTimeStepNumber = Rfc6238AuthenticationService.GetCurrentTimeStepNumber();
            using (HMACSHA1 hMACSHA1 = new HMACSHA1(securityToken.GetDataNoClone()))
            {
                num = Rfc6238AuthenticationService.ComputeTotp(hMACSHA1, currentTimeStepNumber, modifier);
            }
            return num;
        }

        private static ulong GetCurrentTimeStepNumber()
        {
            TimeSpan utcNow = DateTime.UtcNow - Rfc6238AuthenticationService._unixEpoch;
            return (ulong)(utcNow.Ticks / Rfc6238AuthenticationService._timestep.Ticks);
        }

        public static bool ValidateCode(SecurityToken securityToken, int code, string modifier = null)
        {
            bool flag;
            if (securityToken == null)
            {
                throw new ArgumentNullException("securityToken");
            }
            ulong currentTimeStepNumber = Rfc6238AuthenticationService.GetCurrentTimeStepNumber();
            using (HMACSHA1 hMACSHA1 = new HMACSHA1(securityToken.GetDataNoClone()))
            {
                int num = -2;
                while (num <= 2)
                {
                    if (Rfc6238AuthenticationService.ComputeTotp(hMACSHA1, (ulong)((long)currentTimeStepNumber + (long)num), modifier) != code)
                    {
                        num++;
                    }
                    else
                    {
                        flag = true;
                        return flag;
                    }
                }
                return false;
            }
            return flag;
        }
    }

    internal class DataProtectionService
    {
        private readonly static Encoding DefaultEncoding;

        public IDataProtector Protector { get; private set; }
        public TimeSpan TokenLifespan { get; set; }

        static DataProtectionService()
        {
            DefaultEncoding = new UTF8Encoding(false, true);
        }

        public DataProtectionService(IDataProtector protector)
        {
            if (protector == null)
            {
                throw new ArgumentNullException("protector");
            }
            this.Protector = protector;
            this.TokenLifespan = TimeSpan.FromDays(1);
        }

        public string GenerateToken(string stamp, string modifier)
        {
            if (stamp == null)
            {
                throw new ArgumentNullException("stamp");
            }

            MemoryStream memoryStream = new MemoryStream();
            using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream, DefaultEncoding, true))
            {
                binaryWriter.Write(DateTimeOffset.UtcNow.UtcTicks);
                binaryWriter.Write(modifier ?? "");
                binaryWriter.Write(stamp);
            }
            byte[] numArray = this.Protector.Protect(memoryStream.ToArray());
            return Convert.ToBase64String(numArray);
        }
        public bool ValidateToken(string stamp, string modifier, string token)
        {
            if (stamp == null)
            {
                throw new ArgumentNullException("stamp");
            }

            try
            {
                byte[] numArray = this.Protector.Unprotect(Convert.FromBase64String(token));
                using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(numArray), DefaultEncoding, true))
                {
                    DateTimeOffset dateTimeOffset = new DateTimeOffset(binaryReader.ReadInt64(), TimeSpan.Zero);
                    if ((dateTimeOffset + this.TokenLifespan) >= DateTimeOffset.UtcNow)
                    {
                        string str = binaryReader.ReadString();
                        if (string.Equals(str, modifier ?? ""))
                        {
                            string str1 = binaryReader.ReadString();
                            if (binaryReader.PeekChar() == -1)
                            {
                                return string.Equals(str1, stamp);
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            return false;
        }
    }
}

namespace Tsb.Security.Web.Identity
{
    public static class OwinSecurityExtensions
    {
        public static DateTimeOffset? GetValidatedUtc(this AuthenticationProperties properties)
        {
            string str;
            DateTimeOffset dateTimeOffset;
            if (properties.Dictionary.TryGetValue(".validated", out str) && DateTimeOffset.TryParseExact(str, "r", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind, out dateTimeOffset))
            {
                return dateTimeOffset;
            }
            return null;
        }
        public static void SetValidatedUtc(this AuthenticationProperties properties, DateTimeOffset? dateTimeOffset)
        {
            if (!dateTimeOffset.HasValue)
            {
                if (properties.Dictionary.ContainsKey(".validated"))
                {
                    properties.Dictionary.Remove(".validated");
                }
                return;
            }
            properties.Dictionary[".validated"] = dateTimeOffset.Value.ToString("r", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}


