using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Infrastructure.Descriptor
{
    public class EntityDescriptor
    {
        private readonly IDictionary<string, PropertyDescriptor> _propertyDict;

        public IEnumerable<PropertyDescriptor> PropertyDescriptors { get; protected set; }

        public EntityDescriptor(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            _propertyDict = new Dictionary<string, PropertyDescriptor>(properties.Count());

            foreach (var property in properties)
            {
                _propertyDict.Add(property.Name, new PropertyDescriptor(type, property));
            }

            PropertyDescriptors = _propertyDict.Values;
        }

        public PropertyDescriptor GetPropertyDescriptor(string propertyName)
        {
            if (!_propertyDict.ContainsKey(propertyName))
            {
                return null;
            }

            return _propertyDict[propertyName];
        }

        public class PropertyDescriptor : IPropertyDescriptor
        {
            private readonly Delegate _getDelegate;

            private readonly Delegate _setDelegate;

            public PropertyInfo PropertyInfo { get; protected set; }

            public string PropertyName { get; protected set; }

            public IEnumerable<Attribute> Attributes { get; protected set; }

            public object GetValue(object entity)
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }

                if (entity.GetType() != this.PropertyInfo.DeclaringType)
                {
                    throw new ArgumentException();
                }

                if (this._getDelegate == null)
                {
                    return null;
                }

                return this._getDelegate.DynamicInvoke(entity);
            }

            public void SetValue(object entity, object value)
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }

                if (entity.GetType() != this.PropertyInfo.DeclaringType)
                {
                    throw new ArgumentException();
                }

                if (this._setDelegate == null)
                {
                    return;
                }

                this._setDelegate.DynamicInvoke(entity, value);
            }

            public PropertyDescriptor(Type type, PropertyInfo propertyInfo)
            {
                if (type == null)
                {
                    throw new ArgumentNullException("type");
                }

                if (propertyInfo == null)
                {
                    throw new ArgumentNullException("propertyInfo");
                }

                this.PropertyInfo = propertyInfo;
                this.PropertyName = propertyInfo.Name;
                this.Attributes = propertyInfo.GetCustomAttributes(true).OfType<Attribute>();

                if (propertyInfo.GetGetMethod() != null)
                {
                    // p => (object)p.XXX;
                    var getParameter = Expression.Parameter(type);
                    var getBody = Expression.Convert(Expression.Property(getParameter, propertyInfo.Name), typeof(object));
                    var delegateType = typeof(Func<,>).MakeGenericType(type, typeof(object));
                    this._getDelegate = Expression.Lambda(delegateType, getBody, getParameter).Compile();
                }

                if (propertyInfo.GetSetMethod() != null)
                {
                    // p => p.XXX = (realType)value;
                    var setParameter1 = Expression.Parameter(type);
                    var setParameter2 = Expression.Parameter(typeof(object));
                    var setBody = Expression.Assign(Expression.Property(setParameter1, propertyInfo), Expression.Convert(setParameter2, propertyInfo.PropertyType));
                    var delegateType = typeof(Action<,>).MakeGenericType(type, typeof(object));
                    this._setDelegate = Expression.Lambda(delegateType, setBody, setParameter1, setParameter2).Compile();
                }
            }

            public IEnumerable<TAttraibute> GetAttributes<TAttraibute>() where TAttraibute : Attribute
            {
                return this.Attributes.OfType<TAttraibute>();
            }

            public TAttraibute GetFirstAttribute<TAttraibute>() where TAttraibute : Attribute
            {
                return this.GetAttributes<TAttraibute>().FirstOrDefault();
            }

            public bool HasAttribute<TAttraibute>() where TAttraibute : Attribute
            {
                return this.GetAttributes<TAttraibute>().Any();
            }
        }
    }

    public sealed class EntityDescriptor<TEntity> where TEntity : class
    {
        private readonly IDictionary<string, PropertyDescriptor> _propertyDict;

        public IEnumerable<PropertyDescriptor> PropertyDescriptors { get; protected set; }

        public static EntityDescriptor<TEntity> Instance = new EntityDescriptor<TEntity>();

        private EntityDescriptor()
        {
            var type = typeof(TEntity);
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            _propertyDict = new Dictionary<string, PropertyDescriptor>(properties.Count());

            foreach (var property in properties)
            {
                _propertyDict.Add(property.Name, new PropertyDescriptor(property));
            }

            PropertyDescriptors = _propertyDict.Values;
        }

        public PropertyDescriptor GetPropertyDescriptor<TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            var memberExpression = expression.Body as MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException("Expression is not MemberExpression");
            }

            return this.GetPropertyDescriptor(memberExpression.Member.Name);
        }

        public PropertyDescriptor GetPropertyDescriptor(string propertyName)
        {
            if (!_propertyDict.ContainsKey(propertyName))
            {
                return null;
            }

            return _propertyDict[propertyName];
        }

        public class PropertyDescriptor : IPropertyDescriptor
        {
            private readonly IDictionary<Type, IEnumerable<Attribute>> _attributeDict;

            private readonly Func<TEntity, object> _getDelegate;

            private readonly Action<TEntity, object> _setDelegate;

            public PropertyInfo PropertyInfo { get; protected set; }

            public string PropertyName { get; protected set; }

            public IEnumerable<Attribute> Attributes { get; protected set; }

            public object GetValue(TEntity entity)
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }

                if (this._getDelegate == null)
                {
                    return null;
                }

                return this._getDelegate(entity);
            }

            public void SetValue(TEntity entity, object value)
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }

                if (this._setDelegate == null)
                {
                    return;
                }

                this._setDelegate(entity, value);
            }

            public PropertyDescriptor(PropertyInfo propertyInfo)
            {
                if (propertyInfo == null)
                {
                    throw new ArgumentNullException("propertyInfo");
                }

                this.PropertyInfo = propertyInfo;
                this.PropertyName = propertyInfo.Name;
                Attributes = propertyInfo.GetCustomAttributes(true).OfType<Attribute>();

                if (propertyInfo.GetGetMethod() != null)
                {
                    // p => (object)p.XXX;
                    var getParameter = Expression.Parameter(typeof(TEntity));
                    var getBody = Expression.Convert(Expression.Property(getParameter, propertyInfo.Name), typeof(object));
                    this._getDelegate = Expression.Lambda<Func<TEntity, object>>(getBody, getParameter).Compile();
                }

                if (propertyInfo.GetSetMethod() != null)
                {
                    // p => p.XXX = (realtype)value;
                    var setParameter1 = Expression.Parameter(typeof(TEntity));
                    var setParameter2 = Expression.Parameter(typeof(object));
                    var setBody = Expression.Assign(Expression.Property(setParameter1, propertyInfo), Expression.Convert(setParameter2, propertyInfo.PropertyType));
                    this._setDelegate = Expression.Lambda<Action<TEntity, object>>(setBody, setParameter1, setParameter2).Compile();
                }
            }

            public IEnumerable<TAttraibute> GetAttributes<TAttraibute>() where TAttraibute : Attribute
            {
                return this.Attributes.OfType<TAttraibute>();
            }

            public TAttraibute GetFirstAttribute<TAttraibute>() where TAttraibute : Attribute
            {
                return this.GetAttributes<TAttraibute>().FirstOrDefault();
            }

            public bool HasAttribute<TAttraibute>() where TAttraibute : Attribute
            {
                return this.GetAttributes<TAttraibute>().Any();
            }

            public object GetValue(object entity)
            {
                return this.GetValue(entity as TEntity);
            }

            public void SetValue(object entity, object value)
            {
                this.SetValue(entity as TEntity, value);
            }
        }
    }
}
