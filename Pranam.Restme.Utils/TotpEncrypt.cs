using System;
using System.Security.Cryptography;
using System.Web;

namespace Pranam.Restme.Utils
{
    public class TotpEncryptor
    {
        public byte[] Key { get; set; }

        public TotpEncryptor()
        {
            GenerateKey();
        }

        public void GenerateKey()
        {
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
            {
                /*    Keys SHOULD be of the length of the HMAC output to facilitate
                      interoperability.*/
                Key = new byte[HMACSHA1.Create().HashSize / 8];
                rng.GetBytes(Key);
            }
        }

        public int HOTP(UInt64 C, int digits = 6)
        {
            var hmac = HMACSHA1.Create();
            hmac.Key = Key;
            hmac.ComputeHash(BitConverter.GetBytes(C));
            return Truncate(hmac.Hash, digits);
        }

        public UInt64 CounterNow(int T1 = 30)
        {
            var secondsSinceEpoch =
                (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            return (UInt64) Math.Floor(secondsSinceEpoch / T1);
        }

        private int DT(byte[] hmac_result)
        {
            int offset = hmac_result[19] & 0xf;
            int bin_code = (hmac_result[offset] & 0x7f) << 24
                           | (hmac_result[offset + 1] & 0xff) << 16
                           | (hmac_result[offset + 2] & 0xff) << 8
                           | (hmac_result[offset + 3] & 0xff);
            return bin_code;
        }

        private int Truncate(byte[] hmac_result, int digits)
        {
            var Snum = DT(hmac_result);
            return Snum % (int) Math.Pow(10, digits);
        }

        public static string GenerateQrCodeUri(string label, string issuer, string user, string secret, int digits = 6,
            int period = 30)
        {
            var v =
                $"otpauth://totp/{Uri.EscapeUriString(label)}:{Uri.EscapeUriString(user)}?secret={secret}&issuer={Uri.EscapeUriString(issuer)}";
            if (digits != 6 && digits > 0)
            {
                v += $"&digits={digits}";
            }

            if (period != 30 && period > 0)
            {
                v += $"&period={period}";
            }

            return v;
        }
    }
}