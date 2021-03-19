using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Pranam
{
    public class PranamFormUrlEncodedContent : ByteArrayContent
    {
        public PranamFormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
            : base(GetContentByteArray(nameValueCollection))
        {
            Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
        }

        private static byte[] GetContentByteArray(IEnumerable<KeyValuePair<string, string>> nameValueCollection,
            Encoding encoding = null)
        {
            if (nameValueCollection == null)
            {
                throw new ArgumentNullException("nameValueCollection");
            }

            var stringBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> current in nameValueCollection)
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
            return string.IsNullOrEmpty(data)
                ? string.Empty
                : System.Net.WebUtility.UrlEncode(data).Replace("%20", "+");
        }
    }
}