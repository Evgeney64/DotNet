using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Web.Security;

namespace Tsb.Security
{
    public static class Cryptography
    {
        // https://msdn.microsoft.com/ru-ru/library/system.security.cryptography.aes(v=vs.110).aspx

        public static void Test()
        {
            string original = "Here is some data to encrypt!";

            // Create a new instance of the Aes
            // class.  This generates a new key and initialization 
            // vector (IV).
            using (Aes myAes = Aes.Create())
            {

                // Encrypt the string to an array of bytes.
                byte[] encrypted = EncryptStringToBytes_Aes(original, myAes.Key, myAes.IV);

                // Decrypt the bytes to a string.
                string roundtrip = DecryptStringFromBytes_Aes(encrypted, myAes.Key, myAes.IV);

                //Display the original data and the decrypted data.
                Console.WriteLine("Original:   {0}", original);
                Console.WriteLine("Round Trip: {0}", roundtrip);
            }
        }

        public static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }
        public static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;
        }
    }
}

namespace Tsb.Security.Web.Membership
{
    public class MembershipPrincipalFactory : PrincipalFactory
    {
        public override IGroupPrincipal Create(IPrincipal principal)
        {
            RolePrincipal rolePrincipal = principal as RolePrincipal;
            if (rolePrincipal == null)
            {
                throw new ArgumentException();
            }

            IGroupPrincipal groupPrincipal = new GroupRolePrincipal(rolePrincipal);
            return groupPrincipal;
        }
        public override IGroupPrincipal CreateForThread(IPrincipal principal)
        {
            RolePrincipal rolePrincipal = principal as RolePrincipal;
            if (rolePrincipal == null)
            {
                throw new ArgumentException();
            }

            IGroupPrincipal threadPrincipal = new GroupRolePrincipal(rolePrincipal, new ThreadIdentity(rolePrincipal.Identity));
            return threadPrincipal;
        }
    }
    
    
    public interface IGroupPrincipal : IPrincipal
    {
        int? Id { get; }
        int[] GetUserGroups();
        IGroups GetGroupsForRole(string roleName);
    }

    public class ThreadIdentity : IIdentity
    {
        private string authenticationType;
        private bool isAuthenticated;
        private string name;
        
        public string AuthenticationType
        {
            get
            {
                return this.authenticationType;
            }
        }
        public bool IsAuthenticated
        {
            get
            {
                return this.isAuthenticated;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }
        
        public ThreadIdentity(IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }

            this.authenticationType = identity.AuthenticationType;
            this.isAuthenticated = identity.IsAuthenticated;
            this.name = identity.Name;
        }
    }

    [Serializable]
    public class GroupRolePrincipal : RolePrincipal, IGroupPrincipal
    {
        public int? Id { get; private set; }
        
        public GroupRolePrincipal(RolePrincipal rolePrincipal)
            : this(rolePrincipal, rolePrincipal.Identity)
        {
        }
        public GroupRolePrincipal(RolePrincipal rolePrincipal, IIdentity identity)
            : base(rolePrincipal.ProviderName, identity, rolePrincipal.ToEncryptedTicket() ?? String.Empty)
        {
            if (rolePrincipal.Identity.IsAuthenticated)
            {
                MembershipUser membershipUser = System.Web.Security.Membership.GetUser(false);
                if (membershipUser != null)
                {
                    this.Id = membershipUser.ProviderUserKey as int?;
                }
            }
        }

        public int[] GetUserGroups()
        {
            if (!(System.Web.Security.Membership.Provider is IGroupMembershipProvider))
            {
                throw new NotSupportedException();
            }

            return ((IGroupMembershipProvider)System.Web.Security.Membership.Provider).GetGroupsForUser(this.Identity.Name);
        }
        public IGroups GetGroupsForRole(string roleName)
        {
            if (!(Roles.Providers[this.ProviderName] is IGroupRoleProvider))
            {
                throw new NotSupportedException();
            }
            if (!this.IsInRole(roleName))
            {
                throw new UnauthorizedAccessException();
            }
            return ((IGroupRoleProvider)Roles.Providers[this.ProviderName]).GetGroupsForUserInRole(this.Identity.Name, roleName);
        }
    }
}