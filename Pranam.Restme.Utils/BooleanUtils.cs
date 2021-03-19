using System;
using Pranam.Restme.Utils;

namespace Pranam
{
    public class BooleanUtils
    {
        public static bool GetBooleanValueFromObject(object objectValue, bool boolReturnedIfFailed = false)
        {
            if (objectValue != null)
            {
                try
                {
                    if (objectValue.GetType() == typeof(string) &&
                        StringUtils.GetStringValueOrEmpty(objectValue) == "1")
                        return true;
                    return Convert.ToBoolean(objectValue);
                }
                catch (Exception ex)
                {
                    RestmeLogger.LogDebug(ex.Message, ex);
                    return boolReturnedIfFailed;
                }
            }

            return boolReturnedIfFailed;
        }
    }
}