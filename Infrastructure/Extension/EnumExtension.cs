using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Infrastructure.Extension
{
    public static class EnumExtension
    {
        private static readonly object _locker = new object();

        private static readonly Dictionary<Type, Dictionary<string, string>> _typeDict = new Dictionary<Type, Dictionary<string, string>>();

        public static string GetDescription(this Enum value)
        {
            if (value == null)
            {
                return String.Empty;
            }

            var dict = GetEnumDescriptionDictionary(value.GetType());
            var key = value.ToString();

            if (dict == null || !dict.ContainsKey(key))
            {
                return String.Empty;
            }

            return dict[key];
        }

        private static Dictionary<string, string> GetEnumDescriptionDictionary(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (_typeDict.ContainsKey(type))
            {
                return _typeDict[type];
            }

            lock (_locker)
            {
                if (_typeDict.ContainsKey(type))
                {
                    return _typeDict[type];
                }

                var tempDict = new Dictionary<string, string>();
                foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public))
                {
                    var attribute = field.GetCustomAttribute<DescriptionAttribute>();
                    if (attribute != null)
                    {
                        tempDict.Add(field.Name, attribute.Description);
                    }
                }

                _typeDict[type] = tempDict;
                return tempDict;
            }
        }
    }
}
