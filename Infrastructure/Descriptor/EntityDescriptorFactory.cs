using System;
using System.Collections.Concurrent;

namespace Infrastructure.Descriptor
{
    public sealed class EntityDescriptorFactory
    {
        private static readonly ConcurrentDictionary<Type, Descriptor.EntityDescriptor> _dict = new ConcurrentDictionary<Type, Descriptor.EntityDescriptor>();

        public EntityDescriptorFactory Instance = new EntityDescriptorFactory();

        private EntityDescriptorFactory()
        {
        }

        public static Descriptor.EntityDescriptor GetEntityDescriptor(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return _dict.GetOrAdd(type, t => new Descriptor.EntityDescriptor(t));
        }
    }
}
