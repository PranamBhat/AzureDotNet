using System.Collections.Generic;
using System.Net;

namespace Pranam
{
    public static class QueryUtils
    {
        public static Dictionary<string, string> IdentifyQueryParams(this string value, bool noQuestionMark = false)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (!noQuestionMark)
            {
                var paramIndex = value?.IndexOf('?');
                if (paramIndex < 0) return result;
                value = value.Substring(paramIndex.GetValueOrDefault() + 1);
            }

            var paramPairs = value.Split('&');
            foreach (var pair in paramPairs)
            {
                var pairArray = pair.Split('=');
                if (pairArray?.Length != 2) continue;
                var kKey = pairArray[0].Trim();
                var kValue = WebUtility.UrlDecode(pairArray[1].Trim());

                //note: BUG Identified: when multiple parameters with same name appears this will throw an exception
                result.Add(kKey, kValue);
            }

            return result;
        }

        public static string ParseIntoQueryString(this Dictionary<string, string> values,
            bool includeQuestionMark = true, bool encode = true)
        {
            string result = null;
            if (values?.Count > 0)
            {
                var index = 0;
                foreach (var k in values.Keys)
                {
                    result = index == 0
                        ? $"{k}={(encode ? WebUtility.UrlEncode(values[k]) : values[k])}"
                        : result + $"&{k}={(encode ? WebUtility.UrlEncode(values[k]) : values[k])}";
                    index++;
                }
            }

            if (includeQuestionMark && result.IsNotNullOrEmpty())
                result = $"?{result}";

            return result;
        }
    }
}