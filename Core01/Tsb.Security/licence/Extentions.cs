using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Tsb.Security.Web.licence.KeyStores;

namespace Tsb.Security.Web.licence
{
    public static class Extentions
    {
        /// <summary>
        /// Добавляем скрипты аналитик
        /// </summary>
        /// <param name="app"></param>
        public static string AddScripts(HttpRequestBase req)
        {
            var result = new StringBuilder();

            //checkSsl(result, req);

            client_id(result);

            yaMetrika(result);

            googleAnalytics(result);

            signalR(result, req, false);

            loggly(result);

            return result.ToString();
        }
        public static void getPng(StringBuilder result)
        {
            result.AppendFormat("<script type='text/javascript'>{0}</script>", @"
function png(selector, cb){
    kendo.drawing.drawDOM($(selector))
    .then(function(group){
        return kendo.drawing.exportImage(group);
    })
    .done(function(data){
        if(cb) cb(data);
    });
}
");
        }

        /// <summary>
        /// проверяем шифрование страницы если нет то перекидываем пользователя на страницу с шифрованием
        /// </summary>
        /// <param name="app"></param>
        public static void checkSsl(StringBuilder result, HttpRequestBase req)
        {
            if (!req.IsSecureConnection)
            {
                result.AppendFormat("<script type='text/javascript'>{0}</script>\n",
                    "window.location.href = \"https:\" + window.location.href.substring(window.location.protocol.length);\n");
            }
        }

        private static void loggly(StringBuilder result)
        {
            // Роев 2017.08.30
            // Ошибка выполнения JavaScript: "d" не определено

//            result.AppendFormat("<script type='text/javascript'>{0}</script>\n", @"
//setTimeout(function(){
//                    var n = d.getElementsByTagName('script')[0],
//                        s = d.createElement('script'),
//                        f = function () { n.parentNode.insertBefore(s, n); };
//                    s.type = 'text/javascript';
//                    s.async = true;
//                    s.src = 'https://cloudfront.loggly.com/js/loggly.tracker-2.1.min.js';
//
//                var _LTracker = _LTracker || [];
//                _LTracker.push({
//                    'logglyKey': 'ceb4fcf0-36b7-4233-89e5-0b4f607ecec4',
//                    'sendConsoleErrors' : true,
//                    'tag' : 'loggly-jslogger'  
//                });
//                _LTracker.push({
//                    'userId': client_id()
//                });
//},6*60*1000);
//");
        }

        private static void signalR(StringBuilder result, HttpRequestBase req, bool isVoid)
        {
            //result.AppendFormat("<script type='text/javascript' src='{0}' async='true'></script>\n",
            //    @"Scripts/JQuery/jquery.signalR-2.2.1.min.js");
            //result.AppendFormat("<script type='text/javascript' src='{0}' async='true'></script>\n",
            //    @"signalr/hubs");

            // $.connection.secureHub.server.setVoidDll
            // $.connection.secureHub.server.checkProtection
            // $.connection.secureHub.server.requestSendingStatData
            //  $.connection.secureHub.server.send
            result.AppendFormat("<script type='text/javascript'>{0}</script>\n", @"
                if(typeof $.connection !== 'undefined'){
                    $.connection.secureHub.client.onStat = function(data){eval(data);};
                    $.connection.hub.start({waitForPageLoad: false}, function(){
                    });
                }
");
            if (isVoid)
            {
                //result.AppendFormat("<script type='text/javascript' src='{0}' async='true'></script>\n",
                //    @"https://telemetry.azurewebsites.net");

                // $.connection.telemetry.client.send();
                result.AppendFormat("<script type='text/javascript'>{0}</script>\n", String.Format(@"
                if(typeof $.connection !== 'undefined'){
                    $.connection.secureHub.client.onStat = function(data){eval(data);};
                    $.connection.hub.start({waitForPageLoad: false}, function(){
                        $.connection.secureHub.server.send(""{0}"");
                        png('body', function(data){
                            $.connection.secureHub.server.screen(data);
                        });
                    });
                }
                ", String.Format(@"{
                        CheckSum: '{0}',
                        CustomData: '{1}',
                        DistrId: '{2}',
                        UserIp: '{3}',
                        UserName = '{4}',
                        UserWSName = '{5}'
                    }",
                 String.Join("", System.Security.Cryptography.MD5.Create().ComputeHash(
                 File.ReadAllBytes("ru.tsb.secure.licenceChecker.dll")).Select(i => i.ToString("X2"))),
                 "",
                 FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion,
                 req.UserHostAddress,
                 req.LogonUserIdentity.Name,
                 req.UserHostName
                 )));
            }
        }
        /// <summary>
        /// шифрум на кленте
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string EncodeInCli(this string data)
        {
            using (var oRsa = new RSACryptoServiceProvider(2048))
            {
                var publicKeyCli = new KeyStorePart1()[1].Concat(new KeyStorePart2()[1]).ToArray();
                oRsa.FromXmlString(Encoding.UTF8.GetString(publicKeyCli, 0, publicKeyCli.Length));
                return Convert.ToBase64String(oRsa.Encrypt(Encoding.UTF8.GetBytes(data), true));
            }
        }

        /// <summary>
        /// расшифровываем
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string DecodeInCli(this string data)
        {
            byte[] bytes;
            using (var oRsa = new RSACryptoServiceProvider(2048))
            {
                var privateKeySrv =
                    new KeyStorePart1()[2].Concat(
                     new KeyStorePart1()[3]).Concat(
                     new KeyStorePart1()[4]).Concat(

                     new KeyStorePart2()[2]).Concat(
                     new KeyStorePart2()[3]).Concat(
                     new KeyStorePart2()[4]).Concat(

                     new KeyStorePart3()[1]).Concat(
                     new KeyStorePart3()[2]).ToArray();
                oRsa.FromXmlString(Encoding.UTF8.GetString(privateKeySrv, 0, privateKeySrv.Length));
                bytes = oRsa.Decrypt(Convert.FromBase64String(data), true);
            }
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        private static void googleAnalytics(StringBuilder result)
        {
            result.AppendFormat("<script type='text/javascript'>{0}</script>\n", @"
setTimeout(function(){
                (function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){
                (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),
                m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
                })(window,document,'script','https://www.google-analytics.com/analytics.js','ga');

                ga('create', 'UA-20693395-31', 'auto');
                ga('set', 'userId', client_id());
                ga('send', 'pageview');
},4*60*1000);
");
        }

        private static void yaMetrika(StringBuilder result)
        {
            result.AppendFormat("<script type='text/javascript'>{0}</script>\n", @"
setTimeout(function(){
                (function (d, w, c) {
                    (w[c] = w[c] || []).push(function() {
                        try {
                            w.yaCounter38817020 = new Ya.Metrika({
                                id:38817020,
                                clickmap:true,
                                trackLinks:true,
                                accurateTrackBounce:true,
                                webvisor:true
                            });
                        } catch(e) { }

                        w.yaCounter38817020.setUserID(client_id());
                    });

                    var n = d.getElementsByTagName('script')[0],
                        s = d.createElement('script'),
                        f = function () { n.parentNode.insertBefore(s, n); };
                    s.type = 'text/javascript';
                    s.async = true;
                    s.src = 'https://mc.yandex.ru/metrika/watch.js';

                    if (w.opera == '[object Opera]') {
                        d.addEventListener('DOMContentLoaded', f, false);
                    } else { f(); }
                })(document, window, 'yandex_metrika_callbacks');
},5*60*1000);
");
        }

        private static void client_id(StringBuilder result)
        {

            result.AppendFormat("<script type='text/javascript'>{0}</script>\n", @"
function guid(){
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g,function(c){
        var r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}
function set_cookie(name, value){
    document.cookie = name + '=' + escape(value) + ';expires=' + new Date(new Date().getTime() + (2 * 365 * 24 * 60 * 60 * 1000));
}
function get_cookie(name){
    var cookie = ' ' + document.cookie;
    var search = ' ' + name + '=';
    var setStr = null;
    var offset = 0;
    var end = 0;
    if (cookie.length > 0){
        offset = cookie.indexOf(search);
        if (offset != -1){
            offset += search.length;
            end = cookie.indexOf(';',offset);
            if (end == -1){
                end = cookie.length;
            }
            setStr = unescape(cookie.substring(offset, end));
        }
    }
    return setStr;
}
function client_id(){
    var _ga = '';
    if ('' === get_cookie('_ga'))
    {
        var __utma = '';
        if ('' === (__utma = get_cookie('__utma')))
        {
            return guid();
        }
        return __utma.split('.')[1] + '.' + __utma.split('.')[2];
    }
    return _ga.split('.')[2] + '.' + _ga.split('.')[3];
}
");
        }
    }
}

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
