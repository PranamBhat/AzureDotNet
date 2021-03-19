using System;
using System.Linq;

namespace Pranam
{
    public static class PrimitiveTypes
    {
        public static readonly Type[] List = new[]
                      {
                              typeof (Enum),
                              typeof (String),
                              typeof (Char),

                              typeof (Boolean),
                              typeof (Byte),
                              typeof (Int16),
                              typeof (Int32),
                              typeof (Int64),
                              typeof (Single),
                              typeof (Double),
                              typeof (Decimal),

                              typeof (SByte),
                              typeof (UInt16),
                              typeof (UInt32),
                              typeof (UInt64),

                              typeof (DateTime),
                              typeof (DateTimeOffset),
                              typeof (TimeSpan)
                          };

        public static bool IsPrimitiveType(this Type type)
        {
            if (List.Any(x => x == type))
                return true;

            var nut = Nullable.GetUnderlyingType(type);
            return List.Any(x => x == nut);
        }
    }
}
