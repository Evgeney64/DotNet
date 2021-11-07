using System.Configuration.Provider;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Tsb.Extensions.Providers
{
    public abstract class SmsProvider : ProviderBase
    {
        public virtual bool AllowMultipleTargets
        {
            get
            {
                return true;
            }
        }

        public abstract bool SendMessage(string to, string message, string from = null);
        public abstract Task<bool> SendMessageAsync(string to, string message, string from = null);
    }

    public abstract class EmailProvider : ProviderBase
    {
        public abstract bool SendMessage(MailMessage mailMessage);
        public abstract Task<bool> SendMessageAsync(MailMessage mailMessage);
    }
}