using System;
using Pranam.Restme.Utils;

namespace Pranam
{
    public static class GuidUtils
    {
        public static Guid GetGuidOrEmpty(object value)
        {
            if (value == null) return Guid.Empty;
            try
            {
                if (value is string)
                    return new Guid(StringUtils.GetStringValueOrEmpty(value));
                return (Guid) value;
            }
            catch (Exception ex)
            {
                RestmeLogger.LogDebug(ex.Message, ex);
                return Guid.Empty;
            }

            return Guid.Empty;
        }

        public static bool IsNullOrEmpty(this Guid value)
        {
            return value == Guid.Empty;
        }

        public static bool IsNotNullOrEmpty(this Guid value)
        {
            return !IsNullOrEmpty(value);
        }

        public static bool IsNotNullOrEmpty(this Guid? value)
        {
            return !IsNullOrEmpty(value);
        }

        public static bool IsNullOrEmpty(this Guid? value)
        {
            return value == null || value == Guid.Empty;
        }
    }
}