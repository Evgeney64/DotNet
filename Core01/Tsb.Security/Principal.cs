using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Tsb.Security.Web.Membership;

namespace Tsb.Security.Web
{
    public abstract class PrincipalFactory
    {
        private static PrincipalFactory current;

        public static PrincipalFactory Current
        {
            get
            {
                if (current == null)
                {
                    current = new Tsb.Security.Web.Identity.IdentityPrincipalFactory();
                }
                return current;
            }
        }
        
        protected PrincipalFactory()
        {
        }

        public abstract IGroupPrincipal Create(IPrincipal principal, params string[] defaultAuthenticationTypes);
    }
}

namespace Tsb.Security.Web.Membership
{
    public interface IGroups
    {
        int[] Grant { get; }
        int[] Deny { get; }
    }

    public interface IConnectionIdentity
    {
        string ConnectionName { get; }
        int ClientId { get; }
        int ModuleId { get; }
    }

    public interface IBitrix24Identity
    {
        string Email { get; }
        string AccessToken { get; }
        string RefreshToken { get; }
    }

    public interface IACSaleIdentity
    {
        string AccessToken { get; }
        string RefreshToken { get; }
        DateTime AccessTokenExpires { get; }

        int Id { get; }
        long? PartnerId { get; }
    }

    public interface IGroupPrincipal : IPrincipal
    {
        int ApplicationType { get; }
        string Name { get; }
        int Id { get; }
        int ScreenSize { get; }
        string Email { get; }
        string PhoneNumber { get; }
        long? PartnerId { get; }
        string ConnectionName { get; }
        int ClientId { get; }
        int ModuleId { get; }
        bool DeveloperMode { get; }
        bool Debug { get; }
        int[] UserGroups { get; }
        IBitrix24Identity Bitrix24Identity { get; }
        IACSaleIdentity ACSaleIdentity { get; }

        IGroups GetGroupsForRole(string roleName);
    }

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
}
