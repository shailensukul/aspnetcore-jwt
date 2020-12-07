using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace api.core.Security
{
    public static class SecurityExtensions
    {
        private static int saltLengthLimit = 32;

        public static byte[] GetSalt(this string s)
        {
            return s.GetSalt(saltLengthLimit);
        }

        public static byte[] GetSalt(this string s, int maximumSaltLength)
        {
            var salt = new byte[maximumSaltLength];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }

            return salt;
        }
    }
}
