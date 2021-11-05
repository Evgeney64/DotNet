using System.Collections;
using System.Text;

namespace Tsb.Security.Web.licence.KeyStores
{
    public class KeyStorePart3
    {
        readonly Hashtable _parts = new Hashtable
        {
            {1, Encoding.UTF8.GetBytes("xAHJPVUvxBEUH3h3WfOaT4h3F9gxeQu45TKhdf6VXeGhIhtWqPp/4/yY+f4xfDlC1cHs0qQ0mpmB57SFmg+dY4w8=</InverseQ><D>oThsTCV8cpReybEQz17DrNkYCjf9oOb+qO5wPyPSCMuvQpuisAADJvcI52qKtVZ7wjVjbb0WhDYnesKwnr95vR2s+") }, 
            {2, Encoding.UTF8.GetBytes("lblt2nHsfD2uf9a7O1K8yDa0pm4vA0l+LABlICjvkcfZj79B9Gv9mazubY/SMzaqnSTuU5qKSBE/aFD8eQsD4vDXGM31iyLdB+FP6nPuaeIVQa8vw5kABhTuFqtptmeminM6eq5lXEdA7+JNTf0y4haWAVzC4UygN9v6xW1gSMz19zMqIbMg32Fljdi6X+hIpK2SIO9MZNv3XjH/2ZUdTym40PidFURFBgyPosdF4A5sjW/Qpkg664VAtV5IQ==</D></RSAKeyValue>") }, 
        };

        public byte[] this[int key]
        {
            get { return (byte[])_parts[key]; }
        }
    }
}