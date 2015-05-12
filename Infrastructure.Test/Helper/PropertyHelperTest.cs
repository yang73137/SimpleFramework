﻿using System;
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
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            Console.WriteLine("AssemblyInitialize");
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Console.WriteLine("ClassInitialize");
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Console.WriteLine("TestInitialize");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Console.WriteLine("TestCleanup");
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Console.WriteLine("ClassCleanup");
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            Console.WriteLine("AssemblyCleanup");
        }


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
            Assert.AreEqual(person.Id, dataRow["Id"]);
            Assert.AreEqual(person.Name, dataRow["Name"].ToString());
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
