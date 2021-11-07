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
using Tsb.Security.Web.Membership;

namespace Tsb.Security.Web.Identity
{
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
            //public IBitrix24Identity Bitrix24Identity
            //{
            //    get
            //    {
            //        if (this.bitrix24Identity == null)
            //        {
            //            foreach (ClaimsIdentity identity in this.Identities)
            //            {
            //                if (identity.AuthenticationType == "Bitrix24")
            //                {
            //                    this.bitrix24Identity = new Bitrix24Identity(identity);
            //                    break;
            //                }
            //            }
            //        }
            //        return this.bitrix24Identity;
            //    }
            //}
            //public IACSaleIdentity ACSaleIdentity
            //{
            //    get
            //    {
            //        if (this.acSaleIdentity == null)
            //        {
            //            foreach (ClaimsIdentity identity in this.Identities)
            //            {
            //                if (identity.AuthenticationType == "BillBerry")
            //                {
            //                    this.acSaleIdentity = new ACSaleIdentity(identity);
            //                    break;
            //                }
            //            }
            //        }
            //        return this.acSaleIdentity;
            //    }
            //}

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
        //public IBitrix24Identity Bitrix24Identity
        //{
        //    get
        //    {
        //        if (this.bitrix24Identity == null)
        //        {
        //            foreach (ClaimsIdentity identity in this.Identities)
        //            {
        //                if (identity.AuthenticationType == "Bitrix24")
        //                {
        //                    this.bitrix24Identity = new Bitrix24Identity(identity);
        //                    break;
        //                }
        //            }
        //        }
        //        return this.bitrix24Identity;
        //    }
        //}
        //public IACSaleIdentity ACSaleIdentity
        //{
        //    get
        //    {
        //        if (this.acSaleIdentity == null)
        //        {
        //            foreach (ClaimsIdentity identity in this.Identities)
        //            {
        //                if (identity.AuthenticationType == "BillBerry")
        //                {
        //                    this.acSaleIdentity = new ACSaleIdentity(identity);
        //                    break;
        //                }
        //            }
        //        }
        //        return this.acSaleIdentity;
        //    }
        //}

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

}
