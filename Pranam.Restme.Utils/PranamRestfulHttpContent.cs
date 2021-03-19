using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Pranam
{
    public class PranamRestfulHttpContent : ByteArrayContent
    {
        public PranamRestfulHttpContent(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
            : base(GetContentByteArray(nameValueCollection))
        {
            Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        }

        private static byte[] GetContentByteArray(IEnumerable<KeyValuePair<string, string>> nameValueCollection,
            Encoding encoding = null)
        {
            if (nameValueCollection == null)
            {
                throw new ArgumentNullException("nameValueCollection");
            }

            var stringBuilder = new StringBuilder();
            foreach (var current in nameValueCollection)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append('&');
                }

                stringBuilder.Append(Encode(current.Key));
                stringBuilder.Append('=');
                stringBuilder.Append(Encode(current.Value));
            }

            encoding = encoding ?? Encoding.UTF8;
            return encoding.GetBytes(stringBuilder.ToString());
        }

        private static string Encode(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }

            return System.Net.WebUtility.UrlEncode(data).Replace("%20", "+");
        }
    }
}