using Infrastructure.Descriptor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace Infrastructure.Test.Descriptor
{
    [TestClass()]
    public class EntityDescriptorTests
    {
        public class TestEntity
        {
            [DescriptionAttribute("我是ID")]
            public long ID { get; set; }

            [DescriptionAttribute("Age")]
            public int? Age { get; set; }

            public string Name { get; set; }
        }

        [TestMethod()]
        public void EntityDescriptorTest()
        {
            var descriptor = EntityDescriptor<TestEntity>.Instance;
            Assert.IsNotNull(descriptor);

            var idDescriptor = descriptor.GetPropertyDescriptor(p => p.ID);
            Assert.IsNotNull(idDescriptor);
            Assert.AreEqual(idDescriptor.PropertyName, "ID");
            var idDescription = idDescriptor.GetFirstAttribute<DescriptionAttribute>();
            Assert.IsNotNull(idDescription);
            Assert.AreEqual(idDescription.Description, "我是ID");

            var ageDescriptor = descriptor.GetPropertyDescriptor(p => p.Age);
            Assert.IsNotNull(ageDescriptor);
            Assert.AreEqual(ageDescriptor.PropertyName, "Age");
            var ageDescription = ageDescriptor.GetFirstAttribute<DescriptionAttribute>();
            Assert.IsNotNull(ageDescription);
            Assert.AreEqual(ageDescription.Description, "Age");

            var entity = new TestEntity();
            Assert.AreEqual(default(long), idDescriptor.GetValue(entity));
            idDescriptor.SetValue(entity, 1L);
            Assert.AreEqual(1, entity.ID);
            Assert.AreEqual(1L, idDescriptor.GetValue(entity));

            Assert.AreEqual(default(int?), ageDescriptor.GetValue(entity));
            ageDescriptor.SetValue(entity, 2);
            Assert.AreEqual(2, entity.Age);
            Assert.AreEqual(2, ageDescriptor.GetValue(entity));
        }

        [TestMethod()]
        public void EntityDescriptorTest2()
        {
            var descriptor = new EntityDescriptor(typeof(TestEntity));
            Assert.IsNotNull(descriptor);

            var idDescriptor = descriptor.GetPropertyDescriptor("ID");
            Assert.IsNotNull(idDescriptor);
            Assert.AreEqual(idDescriptor.PropertyName, "ID");
            var idDescription = idDescriptor.GetFirstAttribute<DescriptionAttribute>();
            Assert.IsNotNull(idDescription);
            Assert.AreEqual(idDescription.Description, "我是ID");

            var ageDescriptor = descriptor.GetPropertyDescriptor("Age");
            Assert.IsNotNull(ageDescriptor);
            Assert.AreEqual(ageDescriptor.PropertyName, "Age");
            var ageDescription = ageDescriptor.GetFirstAttribute<DescriptionAttribute>();
            Assert.IsNotNull(ageDescription);
            Assert.AreEqual(ageDescription.Description, "Age");

            var entity = new TestEntity();
            Assert.AreEqual(default(long), idDescriptor.GetValue(entity));
            idDescriptor.SetValue(entity, 1L);
            Assert.AreEqual(1, entity.ID);
            Assert.AreEqual(1L, idDescriptor.GetValue(entity));

            Assert.AreEqual(default(int?), ageDescriptor.GetValue(entity));
            ageDescriptor.SetValue(entity, 2);
            Assert.AreEqual(2, entity.Age);
            Assert.AreEqual(2, ageDescriptor.GetValue(entity));
        }
    }
}
