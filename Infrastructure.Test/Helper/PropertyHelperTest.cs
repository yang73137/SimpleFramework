using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infrastructure.Test.Helper
{
    [TestClass]
    public class PropertyHelperTest
    {
        private class Person
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        [TestMethod]
        public void TestGetSet()
        {
            var person = new Person {Id = 1, Name = "张三"};
            Assert.AreEqual(person.Id, PropertyHelper<Person>.GetProperty(person, "Id"));
            const int newId = 2;
            PropertyHelper<Person>.SetProperty(person, "Id", newId);
            Assert.AreEqual(newId, PropertyHelper<Person>.GetProperty(person, "Id"));
        }

        [TestMethod]
        public void TestFillModel()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof (int));
            dataTable.Columns.Add("Name", typeof(string));
            var person = new Person { Id = 1, Name = "张三" };
            var dataRow = dataTable.NewRow();
            PropertyHelper<Person>.FillDataRow(person, ref dataRow);
            Assert.AreEqual(person.Id.ToString(), dataRow["Id"].ToString());
            Assert.AreEqual(person.Name.ToString(), dataRow["Name"].ToString());

            var person2 = new Person { Id = 2, Name = null };
            var dataRow2 = dataTable.NewRow();
            PropertyHelper<Person>.FillDataRow(person2, ref dataRow2);
            Assert.AreEqual(person2.Id.ToString(), dataRow2["Id"].ToString());
            Assert.AreEqual(DBNull.Value, dataRow2["Name"]);
        }

        [TestMethod]
        public void TestFillEntity()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(int));
            dataTable.Columns.Add("Name", typeof(string));
            var dataRow = dataTable.NewRow();
            dataRow["Id"] = 1;
            dataRow["Name"] = "李四";

            var entity = PropertyHelper<Person>.FillEntity(dataRow);
            Assert.AreEqual(dataRow["Id"], entity.Id);
            Assert.AreEqual(dataRow["Name"], entity.Name);
        }
    }
}
