using System.Collections;
using System.Text;

namespace Tsb.Security.Web.licence.KeyStores
{
    public class KeyStorePart1
    {
        readonly Hashtable _parts = new Hashtable
        {
            { 1, Encoding.UTF8.GetBytes("<RSAKeyValue><Modulus>QsrdROl5xUumr1nIdnMr934jyMqZZqe60y07fFF2P3Xc0bl4yffv1AK0XP6VRRA41jNfDtNqvuzv2wjSGtTkkElwnXY9IQ/suBKZOwBdVy1LBmHNEs8yET1XRGrK6d/8G6DIRb5BJoKNLJJnDeHsbhj3L1rK/iHXsUmgVLGRrzPYPWPmIx9PMIBMXB+Ivy2z1ooXVnsu5Ir0kkk6/") }, 
            { 2, Encoding.UTF8.GetBytes("<RSAKeyValue><Modulus>r7Y4C6fTSBaNb7GkazbLwLSqpY7beeXOpiiaRjYxIcJawiUuJchCgv94tqLGL3LMa+bjjrlUdA6PA9/3JqPBP/nplvJB9sSdiTUDxqvp6hE7p7PnoP8T3RTEZ9XhINMhDJAOokgMJ1219Rh64m29jEmDRnJTiUgQuIE1pmtBsfH5OUvw/") }, 
            {3, Encoding.UTF8.GetBytes("nB3SrBxbJ6KZAvz0fs3AxqQU2g91wMTG90tLKsSqf86eonBbXH4hHex2FCRuNOawezmimyEQAXRcHrLeuI51w1nQivJvuCeMUzOz6yFRK7dfMhA51v8Gg1E2vgqxCPSGoDKMj5cX6oifgjqdfTmRbHq9Z1GFKAhKjpGGw==</Modulus><Exponent>AAEAAQ==</Exponent><P>/") }, 
            {4,Encoding.UTF8.GetBytes("VN4Rwy3DRoz/6eiHlfL6I6C0EFLXgNamgwMR2YJmHibeC/vBrXWgFMkvCZpYSjkxcTRk+eRtd8UdjsywQbNASpqNFoMMxiAgdJysWDmJTBNin5uXrCIsG2cHiiXmTGfrMUpe+0+Ppv7C8dnbmRTYsF50SdKKLbaHVQhyiwN2+c=</P><Q>sZEFqo6ATENer/")},
        };

        public byte[] this[int key]
        {
            get { return (byte[])_parts[key]; }
        }
    }
}