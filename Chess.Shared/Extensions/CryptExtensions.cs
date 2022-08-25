using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Shared.Extensions
{
    public static class CryptExtensions
    {
        public static string ToSha256(this string Text)
        {
            string crypt = "";
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] arr = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(Text));
                crypt = arr.ToHexString().ToLower();
            }
            return crypt;
        }

        private static string ToHexString(this byte[] arr)
        {
            StringBuilder hex = new StringBuilder(arr.Length * 2);
            foreach (byte b in arr)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }


        public static string ToBase64(this string text)
        {
            var byteArr = Encoding.UTF8.GetBytes(text);
            string crypt = Convert.ToBase64String(byteArr);
            return crypt;
        }

        public static string FromBase64(this string crypt)
        {
            var jsonBytes = Convert.FromBase64String(crypt);
            var text = Encoding.UTF8.GetString(jsonBytes);
            return text;
        }
    }
}
