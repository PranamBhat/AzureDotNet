using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pranam.Restme.Utils;

//using Microsoft.Extensions.Logging;

namespace Pranam
{
    public class StringUtils
    {
        //private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Validate an input whether it is null or empty string
        /// </summary>
        /// <param name="stringValue">string value to validate</param>
        /// <returns>return the original string if it passes</returns>
        /// <exception cref="PranamException" > throws when the string is invalid </exception>
        public static string CanNotBeNullOrEmpty(string stringValue, bool trim = true)
        {
            if (string.IsNullOrEmpty(stringValue) || (trim && string.IsNullOrEmpty(stringValue.Trim())))
            {
                throw new PranamException(
                    "The string value requested is null or empty which is not allowed for the current process");
            }

            return stringValue.Trim();
        }

        public static string CanNotBeNullOrEmpty(object stringObj, bool trim = true)
        {
            return CanNotBeNullOrEmpty(GetStringValueOrEmpty(stringObj, trim));
        }

        /// <summary>
        /// A simple subString method to format a string cut
        /// </summary>
        /// <param name="original"></param>
        /// <param name="length"></param>
        /// <param name="appendix"></param>
        /// <returns></returns>
        public static string SubString(string original, int length, string appendix)
        {
            var newString = "";
            if (original.Length <= length)
            {
                newString = original;
            }
            else
            {
                newString = original.Substring(0, length) + appendix;
            }

            return newString;
        }

        public static string GetFullClassNameFromObject(object objectValue)
        {
            return GetFullClassNameFromType(objectValue.GetType());
        }

        public static string GetFullClassNameFromType(Type type)
        {
            return type.Name;
        }

        public static string GetStringFromStream(Stream stream)
        {
            return GetStringFromStream(stream, Encoding.UTF8);
        }

        public static string GetStringFromStream(Stream stream, Encoding encoding)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            try
            {
                stream.Position = 0;
                using (var reader = new StreamReader(stream, encoding))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                RestmeLogger.LogDebug("Failed Converting Stream value into String value: " + ex.Message, ex);
                return string.Empty;
            }
        }

        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static string GetStringValueOrEmpty(object objectValue, bool trim = true)
        {
            if (objectValue == null) return string.Empty;
            try
            {
                var result = Convert.ToString(objectValue);
                if (result == objectValue.GetType().ToString()) return string.Empty;
                return trim ? result.Trim() : result;
            }
            catch (Exception ex)
            {
                RestmeLogger.LogDebug(
                    string.Format("Failed Converting object value [typeof: {0} ] into String value: {1}",
                        objectValue.GetType(), ex.Message), ex);
                return string.Empty;
            }
        }

        public static bool StringContains(string stringToValidate, IEnumerable<string> stringsToAttempt,
            string[] splitters,
            int minimumIncludes)
        {
            if (string.IsNullOrEmpty(stringToValidate)) return true;
            var values = stringToValidate.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
            return StringContains(values, stringsToAttempt, minimumIncludes);
        }

        public static bool StringContains(string[] strings, IEnumerable<string> stringsToAttempt, int minimumIncludes)
        {
            var counter = 0;
            if (strings == null || strings.Length <= 0) return false;
            foreach (var role in stringsToAttempt)
            {
                if (strings.Contains(role)) counter++;
                if (counter >= minimumIncludes) return true;
            }

            return false;
        }

        public static bool StringContains(string stringToValidate, int[] idsToAttempt, string[] splitters,
            int minimumIncludes)
        {
            if (string.IsNullOrEmpty(stringToValidate)) return false;
            var values = NumericUtils.GetIntegerArrayFromString(stringToValidate, splitters);
            return NumericUtils.ArrayContainsAny(values, idsToAttempt, minimumIncludes);
        }

        /// <summary>
        /// The Id is very unique however may still be possible to duplication
        /// </summary>
        /// <returns></returns>
        public static string GenerateRandomId()
        {
            var i = Guid.NewGuid().ToByteArray().Aggregate<byte, long>(1, (current, b) => current * (b + 1));
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }

        /// <summary>
        /// Random declaration must be done outside the method to actually generate random numbers
        /// </summary>
        private static readonly Random Random = new Random();

        /// <summary>
        /// Generate passwords
        /// </summary>
        /// <param name="passwordLength"></param>
        /// <param name="strongPassword"> </param>
        /// <returns></returns>
        public static string PasswordGenerator(int passwordLength, bool strongPassword)
        {
            int seed = Random.Next(1, int.MaxValue);
            //const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            const string specialCharacters = @"!#$%&'()*+,-./:;<=>?@[\]_";

            var chars = new char[passwordLength];
            var rd = new Random(seed);

            for (var i = 0; i < passwordLength; i++)
            {
                // If we are to use special characters
                if (strongPassword && i % Random.Next(3, passwordLength) == 0)
                {
                    chars[i] = specialCharacters[rd.Next(0, specialCharacters.Length)];
                }
                else
                {
                    chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
                }
            }

            return new string(chars);
        }

        #region Web Related

        public static string ToSeoFriendly(string title, int maxLength = -1)
        {
            var match = Regex.Match(title.ToLower(), "[\\w]+");
            var result = new StringBuilder("");
            var maxLengthHit = false;
            while (match.Success && !maxLengthHit)
            {
                if (result.Length + match.Value.Length <= maxLength || maxLength <= 0)
                {
                    result.Append(match.Value + "-");
                }
                else
                {
                    maxLengthHit = true;
                    // Handle a situation where there is only one word and it is greater than the max length.
                    if (result.Length == 0) result.Append(match.Value.Substring(0, maxLength));
                }

                match = match.NextMatch();
            }

            // Remove trailing '-'
            if (result[result.Length - 1] == '-') result.Remove(result.Length - 1, 1);
            return result.ToString();
        }

        public static string HtmlToText(string html, int maxLength = -1)
        {
            if (string.IsNullOrEmpty(html)) return "";
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var result = HttpUtility.HtmlDecode(doc.DocumentNode.InnerText);
            if (maxLength <= 0) return result;
            if (result?.Length - 3 > maxLength)
            {
                return result.Substring(0, maxLength) + "...";
            }

            return result?.Substring(0, maxLength);
        }

        #endregion

        public static string JsonSerialize(object value,
            JsonSerializerSettings serializerSettings = null)
        {
            if (value == null)
                return string.Empty;
            try
            {
                //return ServiceStack.Text.JsonSerializer.SerializeToString(value, value.GetType());
                return JsonConvert.SerializeObject(value, serializerSettings
                                                          ??
                                                          new JsonSerializerSettings
                                                          {
                                                              ContractResolver =
                                                                  new PranamJsonResolver(),
                                                              NullValueHandling = NullValueHandling.Ignore,
                                                              MissingMemberHandling = MissingMemberHandling.Ignore
                                                          });
            }
            catch (Exception ex)
            {
                RestmeLogger.LogDebug(
                    "JSON serializing an object type of " +
                    (value != null ? value.GetType().Name : "null :" + ex.Message), ex);
                return string.Empty;
            }
        }

        public static T JsonDeserialize<T>(string value,
            JsonSerializerSettings jsonSerializerSettings = null)
        {
            try
            {
                if (!value.IsNotNullOrEmpty()) return default(T);
                if (typeof(T).IsPrimitiveType())
                    return (T) Convert.ChangeType(value, typeof(T));

                return JsonConvert.DeserializeObject<T>(value,
                    jsonSerializerSettings ??
                    new JsonSerializerSettings
                    {
                        ContractResolver = new PranamJsonResolver(),
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    });
            }
            catch (Exception ex)
            {
                try
                {
                    //in case it is JSON encoded as a JSON string
                    var token = JToken.Parse(value);
                    return JObject.Parse((string) token)
                        .ToObject<T>(new JsonSerializer() {ContractResolver = new PranamJsonResolver()});
                }
                catch (Exception ex2)
                {
                    // ignored
                }

                RestmeLogger.LogDebug(
                    $"Deserializing an object to type {typeof(T).FullName} has failed. The original value was: \n {value}",
                    ex);
            }

            return default(T);
        }

        /// <summary>
        /// Converts a string of characters representing hexadecimal values into an array of bytes
        /// </summary>
        /// <param name="strInput">A hexadecimal string of characters to convert to binary.</param>
        /// <returns>A byte array</returns>
        public static byte[] HEXStringToBytes(string strInput)
        {
            int numBytes = (strInput.Length / 2);
            byte[] bytes = new byte[numBytes];

            for (int x = 0; x <= numBytes - 1; x++)
            {
                bytes[x] = System.Convert.ToByte(strInput.Substring(x * 2, 2), 16);
            }

            return bytes;
        }

        /// <summary>
        /// Converts an array of bytes into a hexadecimal string representation.
        /// </summary>
        /// <param name="ba">Array of bytes to convert</param>
        /// <returns>String of hexadecimal values.</returns>
        public static string ByteArrayToHexString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }


        public static class ByXPath
        {
            public const string AnchorTag = "a";
            public const string AnyNode = "*";

            public enum Comparison
            {
                Presence,
                Equality,
                Contains,
                StartsWith,
                EndsWith
            }

            public static string XPath(string xpath)
            {
                return xpath;
            }

            public static string Tag(string tagName)
            {
                return string.Format(@"//{0}", tagName);
            }

            public static string Id(string id)
            {
                return XPath(Comparison.Equality, "id", id);
            }

            public static string ClassName(string name, Comparison type = Comparison.Equality)
            {
                return XPath(type, "class", name);
            }

            public static string Link(string linkText, Comparison comparisonType = Comparison.Equality)
            {
                return XPath(comparisonType, "href", linkText, AnchorTag);
            }

            public static string PartialLink(string linkText)
            {
                return Link(linkText, Comparison.Contains);
            }

            public static string Attribute(string attribute, string name,
                Comparison comparisonType = Comparison.Equality, string node = null)
            {
                return XPath(comparisonType, attribute, name, node);
            }

            public static string XPath(Comparison compareBy, string attribute, string attributeValue = "",
                string node = null)
            {
                switch (compareBy)
                {
                    case Comparison.Equality:
                        return string.Format(@"//{0}[@{1}]", node ?? AnyNode, attribute);

                    case Comparison.Presence:
                        return string.Format(@"//{0}[@{1}=""{2}""]", node ?? AnyNode, attribute, attributeValue);

                    case Comparison.Contains:
                        return XpathFunction(node, "contains", attribute, attributeValue);

                    case Comparison.StartsWith:
                        return XpathFunction(node, "starts-with", attribute, attributeValue);

                    case Comparison.EndsWith:
                        return XpathFunction(node, "ends-with", attribute, attributeValue);

                    default:
                        return string.Empty;
                }
            }

            private static string XpathFunction(string node, string func, string attr, string name)
            {
                return string.Format(@"//{0}[{1}(@{2}=""{3}"")]", node ?? AnyNode, func, attr, name);
            }
        }

        public static string ToUpperFirstLetter(string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            // convert to char array of the string
            char[] letters = source.ToCharArray();
            // upper case the first char
            letters[0] = char.ToUpper(letters[0]);
            // return the array made of the new char array
            return new string(letters);
        }

        public static string ByteSizeInString(long byteSize)
        {
            var culture = CultureInfo.CurrentUICulture;
            const string format = "#,0.0";

            if (byteSize < 1024)
                return byteSize.ToString("#,0", culture);
            byteSize /= 1024;
            if (byteSize < 1024)
                return byteSize.ToString(format, culture) + " KB";
            byteSize /= 1024;
            if (byteSize < 1024)
                return byteSize.ToString(format, culture) + " MB";
            byteSize /= 1024;
            if (byteSize < 1024)
                return byteSize.ToString(format, culture) + " GB";
            byteSize /= 1024;
            return byteSize.ToString(format, culture) + " TB";
        }

        public class Base32Encoding
        {
            public static byte[] ToBytes(string input)
            {
                if (string.IsNullOrEmpty(input))
                {
                    throw new ArgumentNullException("input");
                }

                input = input.TrimEnd('='); //remove padding characters
                int byteCount = input.Length * 5 / 8; //this must be TRUNCATED
                byte[] returnArray = new byte[byteCount];

                byte curByte = 0, bitsRemaining = 8;
                int mask = 0, arrayIndex = 0;

                foreach (char c in input)
                {
                    int cValue = CharToValue(c);

                    if (bitsRemaining > 5)
                    {
                        mask = cValue << (bitsRemaining - 5);
                        curByte = (byte) (curByte | mask);
                        bitsRemaining -= 5;
                    }
                    else
                    {
                        mask = cValue >> (5 - bitsRemaining);
                        curByte = (byte) (curByte | mask);
                        returnArray[arrayIndex++] = curByte;
                        curByte = (byte) (cValue << (3 + bitsRemaining));
                        bitsRemaining += 3;
                    }
                }

                //if we didn't end with a full byte
                if (arrayIndex != byteCount)
                {
                    returnArray[arrayIndex] = curByte;
                }

                return returnArray;
            }

            public static string ToString(byte[] input)
            {
                if (input == null || input.Length == 0)
                {
                    throw new ArgumentNullException("input");
                }

                int charCount = (int) Math.Ceiling(input.Length / 5d) * 8;
                char[] returnArray = new char[charCount];

                byte nextChar = 0, bitsRemaining = 5;
                int arrayIndex = 0;

                foreach (byte b in input)
                {
                    nextChar = (byte) (nextChar | (b >> (8 - bitsRemaining)));
                    returnArray[arrayIndex++] = ValueToChar(nextChar);

                    if (bitsRemaining < 4)
                    {
                        nextChar = (byte) ((b >> (3 - bitsRemaining)) & 31);
                        returnArray[arrayIndex++] = ValueToChar(nextChar);
                        bitsRemaining += 5;
                    }

                    bitsRemaining -= 3;
                    nextChar = (byte) ((b << bitsRemaining) & 31);
                }

                //if we didn't end with a full char
                if (arrayIndex != charCount)
                {
                    returnArray[arrayIndex++] = ValueToChar(nextChar);
                    while (arrayIndex != charCount) returnArray[arrayIndex++] = '='; //padding
                }

                return new string(returnArray);
            }

            private static int CharToValue(char c)
            {
                int value = (int) c;

                //65-90 == uppercase letters
                if (value < 91 && value > 64)
                {
                    return value - 65;
                }

                //50-55 == numbers 2-7
                if (value < 56 && value > 49)
                {
                    return value - 24;
                }

                //97-122 == lowercase letters
                if (value < 123 && value > 96)
                {
                    return value - 97;
                }

                throw new ArgumentException("Character is not a Base32 character.", "c");
            }

            private static char ValueToChar(byte b)
            {
                if (b < 26)
                {
                    return (char) (b + 65);
                }

                if (b < 32)
                {
                    return (char) (b + 24);
                }

                throw new ArgumentException("Byte is not a Base32 value.", "b");
            }
        }
    }
}