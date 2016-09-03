using System;
using System.Linq;
using System.Reflection;

namespace Infrastructure.Extension
{
    public static class ReflectionExtension
    {
        public static bool HasAttribute(this PropertyInfo propertyInfo, Type attributeType)
        {
            if (propertyInfo == null || attributeType == null)
            {
                return false;
            }

            return propertyInfo.GetCustomAttributes(attributeType, true).Any();
        }

        public static T GetFirstCustomAttribute<T>(this PropertyInfo propertyInfo) where T : class
        {
            var type = typeof(T);

            if (propertyInfo == null)
            {
                return null;
            }

            var attribute = propertyInfo.GetCustomAttributes(type, true).FirstOrDefault(p => p.GetType() == type) as T;
            return attribute;
        }

        public static Type GetPropertyType(this PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                return null;
            }

            var propertyType = propertyInfo.PropertyType;

            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return Nullable.GetUnderlyingType(propertyType);
            }

            return propertyType;
        }
    }
}
