using System;
using System.Collections.Generic;
using System.Reflection;

namespace Infrastructure.Descriptor
{
    public interface IPropertyDescriptor
    {
        PropertyInfo PropertyInfo { get; }

        string PropertyName { get; }

        IEnumerable<Attribute> Attributes { get; }

        object GetValue(object entity);

        void SetValue(object entity, object value);

        IEnumerable<TAttraibute> GetAttributes<TAttraibute>() where TAttraibute : Attribute;

        TAttraibute GetFirstAttribute<TAttraibute>() where TAttraibute : Attribute;

        bool HasAttribute<TAttraibute>() where TAttraibute : Attribute;
    }
}
