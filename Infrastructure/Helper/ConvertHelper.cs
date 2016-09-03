using System;

namespace Infrastructure.Helper
{
    public class ConvertHelper
    {
        public static object ChangeType(object value, Type conversionType)
        {
            if (value == null || conversionType == null)
            {
                return null;
            }

            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                conversionType = Nullable.GetUnderlyingType(conversionType);
            }

            return Convert.ChangeType(value, conversionType);
        }
    }
}
